using UniversityManagementSystem.Application.DTOs.Finance;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    /// <summary>
    /// Fee assessment and balance management. The student's balance is always
    /// derived from append-only ledger entries: charges (+), payments (-),
    /// waivers (-), adjustments (+/-). Invoices are snapshotted at assessment
    /// time so later price changes never alter already-issued invoices.
    /// All financial mutations are atomic (single SaveChanges per operation).
    /// </summary>
    public class FeeService : IFeeService
    {
        private readonly IFinanceRepository _financeRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IAuditLogService _auditLogService;

        public FeeService(
            IFinanceRepository financeRepo,
            IStudentRepository studentRepo,
            IAuditLogService auditLogService)
        {
            _financeRepo = financeRepo;
            _studentRepo = studentRepo;
            _auditLogService = auditLogService;
        }

        // ================= Assessment =================

        public async Task<InvoiceDto> AssessSemesterAsync(int studentId, int semesterId)
        {
            var existing = await _financeRepo.GetInvoiceByStudentAndSemesterAsync(studentId, semesterId);
            if (existing != null) return MapInvoice(existing);

            var student = await _studentRepo.GetByIdAsync(studentId)
                ?? throw new InvalidOperationException("Student not found.");
            var semester = await _studentRepo.GetSemesterWithYearAsync(semesterId)
                ?? throw new InvalidOperationException("Semester not found.");

            // Active price book for the student's department + the semester's academic year.
            var feeStructures = (await _financeRepo.GetFeeStructuresAsync(semester.AcademicYearId, student.DepartmentId))
                .Where(f => f.IsActive)
                .ToList();

            if (feeStructures.Count == 0)
                throw new InvalidOperationException(
                    $"No fee structure is defined for the {semester.AcademicYear?.Name ?? "selected"} academic year yet. Please contact the finance office.");

            var invoice = new Invoice
            {
                InvoiceNumber = await GenerateInvoiceNumberAsync(),
                StudentId = studentId,
                SemesterId = semesterId,
                PaymentCategory = student.PaymentCategory,
                DueDate = semester.EndDate,
                Status = InvoiceStatus.Unpaid
            };

            decimal total = 0m;
            foreach (var fs in feeStructures)
            {
                // Self-sponsored pays the full amount; government-sponsored pays
                // only the items that explicitly apply to them (0 = fully covered).
                var amount = student.PaymentCategory == PaymentCategory.SelfSponsored
                    ? fs.Amount
                    : fs.AmountForGovernmentSponsored;

                if (amount <= 0) continue;

                invoice.Lines.Add(new InvoiceLine
                {
                    ItemName = fs.ItemName,
                    Amount = amount,
                    FeeStructureId = fs.Id
                });
                total += amount;
            }

            invoice.TotalAmount = total;

            // Post the charge to the ledger via navigation properties so the FKs
            // resolve correctly even though the invoice has not been saved yet.
            var chargeEntry = new LedgerEntry
            {
                StudentId = studentId,
                Invoice = invoice,
                EntryType = "Charge",
                Amount = total,
                Description = $"Semester fees - {semester.Name}"
            };
            var entries = new List<LedgerEntry> { chargeEntry };

            // Government sponsorship: auto-waiver documenting coverage.
            if (student.PaymentCategory == PaymentCategory.GovernmentSponsored && total > 0)
            {
                var waiver = new Waiver
                {
                    Invoice = invoice,
                    Amount = total,
                    Reason = "Government sponsorship (auto)"
                };
                invoice.Waivers.Add(waiver);
                entries.Add(new LedgerEntry
                {
                    StudentId = studentId,
                    Invoice = invoice,
                    Waiver = waiver,
                    EntryType = "Waiver",
                    Amount = -total,
                    Description = "Covered by government sponsorship"
                });
                invoice.Status = InvoiceStatus.Paid;
            }

            // One atomic save: invoice + lines + waiver + ledger entries together.
            invoice = await _financeRepo.AddInvoiceWithEntriesAsync(invoice, entries);

            return MapInvoice(invoice);
        }

        // ================= Clearance (registration gate) =================

        public async Task<FinanceClearanceResult> CheckRegistrationClearanceAsync(int studentId, int semesterId)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null)
                return new FinanceClearanceResult { IsCleared = false, Message = "Student not found." };

            var settings = await _studentRepo.GetFinanceSettingsAsync();

            // The Admin can switch the gate off entirely.
            if (!settings.RegistrationGateEnabled)
                return new FinanceClearanceResult { IsCleared = true, Message = "Finance clearance is currently disabled.", Category = student.PaymentCategory };

            // Government-sponsored students always pass (their invoice is auto-waived anyway).
            if (student.PaymentCategory == PaymentCategory.GovernmentSponsored)
                return new FinanceClearanceResult { IsCleared = true, Message = "Government-sponsored - no payment required.", Category = student.PaymentCategory };

            var invoice = await _financeRepo.GetInvoiceByStudentAndSemesterAsync(studentId, semesterId);

            if (invoice == null)
            {
                // Nothing assessed yet - assess now so the student sees what to pay.
                // A clearance CHECK must never throw for a configuration state
                // such as "the finance office has not published fees yet": return
                // a result the student portal can display instead.
                try
                {
                    var dto = await AssessSemesterAsync(studentId, semesterId);
                    return new FinanceClearanceResult
                    {
                        IsCleared = dto.Balance <= 0,
                        Message = dto.Balance > 0
                            ? $"Outstanding semester fees: {dto.Balance:N2} ETB. At least {settings.FirstInstallmentPercent:N0}% ({dto.TotalAmount * settings.FirstInstallmentPercent / 100m:N2} ETB) must be paid before registration."
                            : "Fees settled.",
                        Category = student.PaymentCategory,
                        Balance = dto.Balance,
                        RequiredAmount = dto.TotalAmount * settings.FirstInstallmentPercent / 100m
                    };
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("No fee structure"))
                {
                    return new FinanceClearanceResult
                    {
                        IsCleared = false,
                        Message = "Fees for this semester have not been published by the finance office yet. Please check back later or contact the finance office.",
                        Category = student.PaymentCategory
                    };
                }
            }

            var paid = invoice.Payments.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount);
            var waived = invoice.Waivers.Sum(w => w.Amount);
            var applied = paid + waived;
            var required = invoice.TotalAmount * settings.FirstInstallmentPercent / 100m;

            if (applied >= invoice.TotalAmount)
                return new FinanceClearanceResult { IsCleared = true, Message = "Fees fully settled.", Category = student.PaymentCategory, Balance = invoice.TotalAmount - applied };

            if (applied >= required)
                return new FinanceClearanceResult { IsCleared = true, Message = $"First installment ({settings.FirstInstallmentPercent:N0}%) met. Outstanding balance: {(invoice.TotalAmount - applied):N2} ETB.", Category = student.PaymentCategory, Balance = invoice.TotalAmount - applied, RequiredAmount = required };

            return new FinanceClearanceResult
            {
                IsCleared = false,
                Message = $"Payment required before registration: at least {required:N2} ETB of {invoice.TotalAmount:N2} ETB (you have {applied:N2} ETB applied).",
                Category = student.PaymentCategory,
                Balance = invoice.TotalAmount - applied,
                RequiredAmount = required
            };
        }

        // ================= Student view =================

        public async Task<StudentFinanceDto> GetStudentFinanceAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdAsync(studentId)
                ?? throw new InvalidOperationException("Student not found.");

            var invoices = await _financeRepo.GetInvoicesByStudentAsync(studentId);
            var ledger = await _financeRepo.GetLedgerAsync(studentId);

            return new StudentFinanceDto
            {
                StudentName = $"{student.FirstName} {student.LastName}",
                StudentNumber = student.StudentId,
                PaymentCategory = student.PaymentCategory,
                TotalCharges = ledger.Where(e => e.Amount > 0).Sum(e => e.Amount),
                TotalPaid = ledger.Where(e => e.EntryType == "Payment").Sum(e => -e.Amount),
                TotalWaived = ledger.Where(e => e.EntryType == "Waiver").Sum(e => -e.Amount),
                Balance = ledger.Sum(e => e.Amount),
                Invoices = invoices.Select(MapInvoice).ToList(),
                Ledger = ledger.Select(MapLedger).ToList()
            };
        }

        public async Task<InvoiceDto?> GetInvoiceAsync(int invoiceId)
        {
            var invoice = await _financeRepo.GetInvoiceByIdAsync(invoiceId);
            return invoice == null ? null : MapInvoice(invoice);
        }

        // ================= Admin =================

        public async Task<IEnumerable<FeeStructureDto>> GetFeeStructuresAsync(int? academicYearId = null, int? departmentId = null)
        {
            var list = await _financeRepo.GetFeeStructuresAsync(academicYearId, departmentId);
            return list.Select(MapFeeStructure);
        }

        public async Task<FeeStructureDto> CreateFeeStructureAsync(FeeStructureCreateDto dto, string adminUserId)
        {
            if (dto.Amount < 0 || dto.AmountForGovernmentSponsored < 0)
                throw new InvalidOperationException("Fee amounts cannot be negative.");
            if (string.IsNullOrWhiteSpace(dto.ItemName))
                throw new InvalidOperationException("Item name is required.");

            var entity = new FeeStructure
            {
                AcademicYearId = dto.AcademicYearId,
                DepartmentId = dto.DepartmentId,
                ItemName = dto.ItemName.Trim(),
                Amount = dto.Amount,
                AmountForGovernmentSponsored = dto.AmountForGovernmentSponsored,
                DueDate = dto.DueDate,
                IsActive = true
            };

            entity = await _financeRepo.AddFeeStructureAsync(entity);

            await _auditLogService.LogAsync(adminUserId, "", "Admin", "Create", "FeeStructure", entity.Id,
                $"Created fee '{entity.ItemName}' amount {entity.Amount:N2} ETB for department {entity.DepartmentId}, year {entity.AcademicYearId}.");

            return MapFeeStructure(entity);
        }

        public async Task UpdateFeeStructureAsync(int id, FeeStructureCreateDto dto, string adminUserId)
        {
            var entity = await _financeRepo.GetFeeStructureByIdAsync(id)
                ?? throw new InvalidOperationException("Fee structure not found.");

            if (dto.Amount < 0 || dto.AmountForGovernmentSponsored < 0)
                throw new InvalidOperationException("Fee amounts cannot be negative.");

            // Changing a price does NOT touch existing invoices - they are snapshots.
            entity.AcademicYearId = dto.AcademicYearId;
            entity.DepartmentId = dto.DepartmentId;
            entity.ItemName = dto.ItemName.Trim();
            entity.Amount = dto.Amount;
            entity.AmountForGovernmentSponsored = dto.AmountForGovernmentSponsored;
            entity.DueDate = dto.DueDate;
            await _financeRepo.UpdateFeeStructureAsync(entity);

            await _auditLogService.LogAsync(adminUserId, "", "Admin", "Update", "FeeStructure", entity.Id,
                $"Updated fee '{entity.ItemName}' to {entity.Amount:N2} ETB.");
        }

        public async Task DeleteFeeStructureAsync(int id, string adminUserId)
        {
            var entity = await _financeRepo.GetFeeStructureByIdAsync(id)
                ?? throw new InvalidOperationException("Fee structure not found.");

            if (await _financeRepo.IsFeeStructureInUseAsync(id))
            {
                entity.IsActive = false; // soft delete keeps historical references intact
                await _financeRepo.UpdateFeeStructureAsync(entity);
                await _auditLogService.LogAsync(adminUserId, "", "Admin", "Deactivate", "FeeStructure", id,
                    $"Deactivated fee '{entity.ItemName}' (in use by existing invoices).");
                return;
            }

            await _financeRepo.DeleteFeeStructureAsync(entity);
            await _auditLogService.LogAsync(adminUserId, "", "Admin", "Delete", "FeeStructure", id,
                $"Deleted fee '{entity.ItemName}'.");
        }

        public async Task GrantWaiverAsync(int invoiceId, decimal amount, string reason, string adminUserId)
        {
            if (amount <= 0) throw new InvalidOperationException("Waiver amount must be positive.");
            if (string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("A reason is required for waivers.");

            var invoice = await _financeRepo.GetInvoiceByIdAsync(invoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");
            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot waive a cancelled invoice.");

            var waived = invoice.Waivers.Sum(w => w.Amount);
            var paid = invoice.Payments.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount);
            if (waived + paid + amount > invoice.TotalAmount)
                throw new InvalidOperationException("Waiver exceeds the remaining invoice balance.");

            var waiver = new Waiver
            {
                InvoiceId = invoiceId,
                Amount = amount,
                Reason = reason.Trim(),
                GrantedByUserId = adminUserId
            };

            var entry = new LedgerEntry
            {
                StudentId = invoice.StudentId,
                InvoiceId = invoiceId,
                Waiver = waiver,
                EntryType = "Waiver",
                Amount = -amount,
                Description = $"Waiver: {reason.Trim()}"
            };

            invoice.Status = ComputeInvoiceStatus(invoice.TotalAmount, paid, waived + amount);

            // One atomic save: waiver + ledger entry + invoice status.
            await _financeRepo.ApplyWaiverAsync(waiver, entry);

            await _auditLogService.LogAsync(adminUserId, "", "Admin", "Waive", "Invoice", invoiceId,
                $"Granted waiver {amount:N2} ETB: {reason.Trim()}.");
        }

        public async Task CancelInvoiceAsync(int invoiceId, string adminUserId)
        {
            var invoice = await _financeRepo.GetInvoiceByIdAsync(invoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");

            if (invoice.Payments.Any(p => p.Status == PaymentStatus.Verified))
                throw new InvalidOperationException("Cannot cancel an invoice that has verified payments. Use a refund instead.");

            var outstanding = invoice.TotalAmount - invoice.Waivers.Sum(w => w.Amount);

            invoice.Status = InvoiceStatus.Cancelled;
            invoice.Remarks = string.IsNullOrWhiteSpace(invoice.Remarks) ? "Cancelled by admin" : invoice.Remarks;

            // Reverse the original charge (and any waivers) so the ledger stays
            // consistent: the student's balance must not keep the cancelled debt.
            var reversal = new LedgerEntry
            {
                StudentId = invoice.StudentId,
                InvoiceId = invoice.Id,
                EntryType = "Adjustment",
                Amount = -outstanding,
                Description = $"Reversal of cancelled invoice {invoice.InvoiceNumber}"
            };

            await _financeRepo.ApplyInvoiceCancellationAsync(invoice, reversal);

            await _auditLogService.LogAsync(adminUserId, "", "Admin", "Cancel", "Invoice", invoiceId,
                $"Cancelled invoice {invoice.InvoiceNumber} and reversed {outstanding:N2} ETB from the ledger.");
        }

        public async Task<FinanceDashboardDto> GetDashboardAsync(int? semesterId)
        {
            var expected = await _financeRepo.SumInvoiceTotalsAsync(semesterId);
            var collected = await _financeRepo.SumVerifiedPaymentsAsync(semesterId);
            var waived = await _financeRepo.SumWaiversAsync(semesterId);
            var pendingCount = await _financeRepo.CountPaymentsAsync(PaymentStatus.Pending);
            var invoiceCount = await _financeRepo.CountInvoicesAsync(semesterId);

            var recentInvoices = (await _financeRepo.GetInvoicesAsync(semesterId, take: 8)).ToList();
            var pendingPayments = (await _financeRepo.GetPaymentsAsync(status: PaymentStatus.Pending))
                .Take(10)
                .Select(MapPayment)
                .ToList();

            return new FinanceDashboardDto
            {
                ExpectedAmount = expected,
                CollectedAmount = collected,
                WaivedAmount = waived,
                OutstandingAmount = Math.Max(0, expected - collected - waived),
                InvoiceCount = invoiceCount,
                PendingPaymentCount = pendingCount,
                RecentInvoices = recentInvoices.Select(MapInvoice).ToList(),
                PendingPayments = pendingPayments
            };
        }

        // ================= Analytics (reports) =================

        public async Task<FinanceAnalyticsDto> GetFinanceAnalyticsAsync(int? academicYearId)
        {
            var result = new FinanceAnalyticsDto
            {
                AcademicYearId = academicYearId,
                TotalExpected = await _financeRepo.SumInvoiceTotalsAsync(academicYearId: academicYearId),
                TotalCollected = await _financeRepo.SumVerifiedPaymentsAsync(academicYearId: academicYearId),
                TotalWaived = await _financeRepo.SumWaiversAsync(academicYearId: academicYearId)
            };

            var statusCounts = await _financeRepo.GetPaymentStatusCountsAsync(academicYearId);
            result.VerifiedCount = statusCounts.GetValueOrDefault(PaymentStatus.Verified);
            result.PendingCount = statusCounts.GetValueOrDefault(PaymentStatus.Pending);
            result.FailedCount = statusCounts.GetValueOrDefault(PaymentStatus.Failed);
            result.RefundedCount = statusCounts.GetValueOrDefault(PaymentStatus.Refunded);

            result.SemesterCollections = await _financeRepo.GetSemesterCollectionsAsync(academicYearId);
            result.DepartmentCollections = await _financeRepo.GetDepartmentCollectionsAsync(academicYearId);

            return result;
        }

        // ================= Helpers =================

        private static InvoiceStatus ComputeInvoiceStatus(decimal total, decimal paid, decimal waived)
        {
            var applied = paid + waived;
            return applied >= total ? InvoiceStatus.Paid
                : applied > 0 ? InvoiceStatus.PartiallyPaid
                : InvoiceStatus.Unpaid;
        }

        internal static InvoiceDto MapInvoice(Invoice invoice)
        {
            var paid = invoice.Payments?.Where(p => p.Status == PaymentStatus.Verified).Sum(p => p.Amount) ?? 0m;
            var waived = invoice.Waivers?.Sum(w => w.Amount) ?? 0m;
            var applied = paid + waived;

            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                StudentId = invoice.StudentId,
                StudentName = invoice.Student != null ? $"{invoice.Student.FirstName} {invoice.Student.LastName}" : string.Empty,
                StudentNumber = invoice.Student?.StudentId ?? string.Empty,
                PaymentCategory = invoice.PaymentCategory,
                SemesterId = invoice.SemesterId,
                SemesterName = invoice.Semester?.Name ?? string.Empty,
                AcademicYearName = invoice.Semester?.AcademicYear?.Name ?? string.Empty,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = paid,
                WaivedAmount = waived,
                Balance = invoice.Status == InvoiceStatus.Cancelled ? 0 : invoice.TotalAmount - applied,
                DueDate = invoice.DueDate,
                Status = invoice.Status,
                Remarks = invoice.Remarks,
                CreatedAt = invoice.CreatedAt,
                Lines = invoice.Lines?.Select(l => new InvoiceLineDto { Id = l.Id, ItemName = l.ItemName, Amount = l.Amount }).ToList() ?? new(),
                Payments = invoice.Payments?.Select(MapPayment).ToList() ?? new(),
                Waivers = invoice.Waivers?.Select(w => new WaiverDto { Id = w.Id, Amount = w.Amount, Reason = w.Reason, GrantedAt = w.GrantedAt }).ToList() ?? new()
            };
        }

        internal static PaymentDto MapPayment(Payment p)
        {
            return new PaymentDto
            {
                Id = p.Id,
                Reference = p.Reference,
                InvoiceId = p.InvoiceId,
                StudentName = p.Student != null ? $"{p.Student.FirstName} {p.Student.LastName}" : string.Empty,
                StudentNumber = p.Student?.StudentId ?? string.Empty,
                Amount = p.Amount,
                Method = p.Method,
                Status = p.Status,
                SlipNumber = p.SlipNumber,
                PayerName = p.PayerName,
                Remarks = p.Remarks,
                VerifiedAt = p.VerifiedAt,
                CreatedAt = p.CreatedAt,
                CheckoutUrl = p.CheckoutUrl,
                InvoiceNumber = p.Invoice?.InvoiceNumber
            };
        }

        private static LedgerEntryDto MapLedger(LedgerEntry e)
        {
            return new LedgerEntryDto
            {
                CreatedAt = e.CreatedAt,
                EntryType = e.EntryType,
                Amount = e.Amount,
                Description = e.Description,
                InvoiceNumber = e.Invoice?.InvoiceNumber
            };
        }

        private static FeeStructureDto MapFeeStructure(FeeStructure f)
        {
            return new FeeStructureDto
            {
                Id = f.Id,
                AcademicYearId = f.AcademicYearId,
                AcademicYearName = f.AcademicYear?.Name ?? string.Empty,
                DepartmentId = f.DepartmentId,
                DepartmentName = f.Department?.Name ?? string.Empty,
                ItemName = f.ItemName,
                Amount = f.Amount,
                AmountForGovernmentSponsored = f.AmountForGovernmentSponsored,
                DueDate = f.DueDate,
                IsActive = f.IsActive
            };
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            // Counts only this year's invoices, so the sequence resets per year.
            // The unique index on InvoiceNumber is the final guard: on a rare
            // race the caller gets a DB error instead of a duplicate number.
            var year = DateTime.UtcNow.Year;
            var count = await _financeRepo.CountInvoicesByNumberPrefixAsync($"INV-{year}-");
            return $"INV-{year}-{(count + 1):D6}";
        }
    }
}
