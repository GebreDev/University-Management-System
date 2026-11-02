using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.DTOs.Finance;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class FinanceRepository : IFinanceRepository
{
	private readonly ApplicationDbContext _context;

	public FinanceRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<FeeStructure>> GetFeeStructuresAsync(int? academicYearId = null, int? departmentId = null)
	{
		IQueryable<FeeStructure> query = _context.FeeStructures.Include((FeeStructure f) => f.AcademicYear).Include((FeeStructure f) => f.Department).AsQueryable();
		if (academicYearId.HasValue)
		{
			query = query.Where((FeeStructure f) => f.AcademicYearId == ((int?)academicYearId).Value);
		}
		if (departmentId.HasValue)
		{
			query = query.Where((FeeStructure f) => f.DepartmentId == ((int?)departmentId).Value);
		}
		return await (from f in query
			orderby f.Department.Name, f.ItemName
			select f).ToListAsync();
	}

	public async Task<FeeStructure?> GetFeeStructureByIdAsync(int id)
	{
		return await _context.FeeStructures.Include((FeeStructure f) => f.AcademicYear).Include((FeeStructure f) => f.Department).FirstOrDefaultAsync((FeeStructure f) => f.Id == id);
	}

	public async Task<FeeStructure> AddFeeStructureAsync(FeeStructure entity)
	{
		_context.FeeStructures.Add(entity);
		await _context.SaveChangesAsync();
		return entity;
	}

	public async Task UpdateFeeStructureAsync(FeeStructure entity)
	{
		_context.FeeStructures.Update(entity);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteFeeStructureAsync(FeeStructure entity)
	{
		_context.FeeStructures.Remove(entity);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> IsFeeStructureInUseAsync(int feeStructureId)
	{
		return await _context.InvoiceLines.AnyAsync((InvoiceLine l) => l.FeeStructureId == (int?)feeStructureId);
	}

	public async Task<Invoice?> GetInvoiceByIdAsync(int id)
	{
		return await _context.Invoices.Include((Invoice i) => i.Student).Include((Invoice i) => i.Semester).ThenInclude((Semester s) => s.AcademicYear)
			.Include((Invoice i) => i.Lines)
			.Include((Invoice i) => i.Payments)
			.Include((Invoice i) => i.Waivers)
			.FirstOrDefaultAsync((Invoice i) => i.Id == id);
	}

	public async Task<Invoice?> GetInvoiceByStudentAndSemesterAsync(int studentId, int semesterId)
	{
		return await _context.Invoices.Include((Invoice i) => i.Lines).Include((Invoice i) => i.Payments).Include((Invoice i) => i.Waivers)
			.Include((Invoice i) => i.Semester)
			.ThenInclude((Semester s) => s.AcademicYear)
			.FirstOrDefaultAsync((Invoice i) => i.StudentId == studentId && i.SemesterId == semesterId);
	}

	public async Task<IEnumerable<Invoice>> GetInvoicesAsync(int? semesterId = null, InvoiceStatus? status = null, string? search = null, int take = 300)
	{
		IQueryable<Invoice> query = _context.Invoices.Include((Invoice i) => i.Student).Include((Invoice i) => i.Semester).Include((Invoice i) => i.Payments)
			.Include((Invoice i) => i.Waivers)
			.AsQueryable();
		if (semesterId.HasValue)
		{
			query = query.Where((Invoice i) => i.SemesterId == ((int?)semesterId).Value);
		}
		if (status.HasValue)
		{
			query = query.Where((Invoice i) => (int)i.Status == (int)((InvoiceStatus?)status).Value);
		}
		if (!string.IsNullOrWhiteSpace(search))
		{
			string term = search.Trim();
			query = query.Where((Invoice i) => i.InvoiceNumber.Contains(term) || (i.Student != null && string.Concat(i.Student.FirstName + " ", i.Student.LastName).Contains(term)) || (i.Student != null && i.Student.StudentId.Contains(term)));
		}
		return await query.OrderByDescending((Invoice i) => i.CreatedAt).Take(take).ToListAsync();
	}

	public async Task<List<Invoice>> GetInvoicesByStudentAsync(int studentId)
	{
		return await (from i in _context.Invoices.Include((Invoice i) => i.Lines).Include((Invoice i) => i.Payments).Include((Invoice i) => i.Waivers)
				.Include((Invoice i) => i.Semester)
				.ThenInclude((Semester s) => s.AcademicYear)
				.Include((Invoice i) => i.Student)
			where i.StudentId == studentId
			orderby i.CreatedAt descending
			select i).ToListAsync();
	}

	public async Task<int> CountInvoicesAsync(int? semesterId = null)
	{
		IQueryable<Invoice> query = _context.Invoices.AsQueryable();
		if (semesterId.HasValue)
		{
			query = query.Where((Invoice i) => i.SemesterId == ((int?)semesterId).Value);
		}
		return await query.CountAsync();
	}

	public async Task<int> CountInvoicesByNumberPrefixAsync(string prefix)
	{
		return await _context.Invoices.CountAsync((Invoice i) => i.InvoiceNumber.StartsWith(prefix));
	}

	public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
	{
		_context.Invoices.Add(invoice);
		await _context.SaveChangesAsync();
		return invoice;
	}

	public async Task<Invoice> AddInvoiceWithEntriesAsync(Invoice invoice, IEnumerable<LedgerEntry> entries)
	{
		_context.Invoices.Add(invoice);
		_context.LedgerEntries.AddRange(entries);
		await _context.SaveChangesAsync();
		return invoice;
	}

	public async Task UpdateInvoiceAsync(Invoice invoice)
	{
		_context.Invoices.Update(invoice);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> HasVerifiedPaymentsAsync(int invoiceId)
	{
		return await _context.Payments.AnyAsync((Payment p) => p.InvoiceId == invoiceId && (int)p.Status == 2);
	}

	public async Task<IEnumerable<Invoice>> GetInvoicesWithDetailsAsync(int? academicYearId)
	{
		IQueryable<Invoice> query = _context.Invoices.Include((Invoice i) => i.Student).ThenInclude((Student s) => s.Department).Include((Invoice i) => i.Semester)
			.ThenInclude((Semester s) => s.AcademicYear)
			.Include((Invoice i) => i.Payments)
			.Include((Invoice i) => i.Waivers)
			.AsQueryable();
		if (academicYearId.HasValue)
		{
			query = query.Where((Invoice i) => i.Semester.AcademicYearId == ((int?)academicYearId).Value);
		}
		return await query.ToListAsync();
	}

	public async Task<Payment?> GetPaymentByIdAsync(int id)
	{
		return await _context.Payments.Include((Payment p) => p.Invoice).ThenInclude((Invoice i) => i.Payments).Include((Payment p) => p.Invoice)
			.ThenInclude((Invoice i) => i.Waivers)
			.Include((Payment p) => p.Student)
			.FirstOrDefaultAsync((Payment p) => p.Id == id);
	}

	public async Task<Payment?> GetPaymentByReferenceAsync(string reference)
	{
		return await _context.Payments.Include((Payment p) => p.Invoice).ThenInclude((Invoice i) => i.Payments).Include((Payment p) => p.Invoice)
			.ThenInclude((Invoice i) => i.Waivers)
			.Include((Payment p) => p.Student)
			.FirstOrDefaultAsync((Payment p) => p.Reference == reference);
	}

	public async Task<IEnumerable<Payment>> GetPaymentsAsync(int? semesterId = null, PaymentStatus? status = null)
	{
		IQueryable<Payment> query = _context.Payments.Include((Payment p) => p.Invoice).Include((Payment p) => p.Student).AsQueryable();
		if (semesterId.HasValue)
		{
			query = query.Where((Payment p) => p.Invoice != null && p.Invoice.SemesterId == ((int?)semesterId).Value);
		}
		if (status.HasValue)
		{
			query = query.Where((Payment p) => (int)p.Status == (int)((PaymentStatus?)status).Value);
		}
		return await query.OrderByDescending((Payment p) => p.CreatedAt).ToListAsync();
	}

	public async Task<Payment> AddPaymentAsync(Payment payment)
	{
		_context.Payments.Add(payment);
		await _context.SaveChangesAsync();
		return payment;
	}

	public async Task UpdatePaymentAsync(Payment payment)
	{
		_context.Payments.Update(payment);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> IsSlipNumberUsedAsync(string slipNumber)
	{
		return await _context.Payments.AnyAsync((Payment p) => p.SlipNumber == slipNumber && (int)p.Status != 3 && (int)p.Status != 4);
	}

	public async Task ApplyVerifiedPaymentAsync(Payment payment, LedgerEntry ledgerEntry)
	{
		_context.Payments.Update(payment);
		_context.LedgerEntries.Add(ledgerEntry);
		if (payment.Invoice != null)
		{
			_context.Invoices.Update(payment.Invoice);
		}
		await _context.SaveChangesAsync();
	}

	public async Task ApplyRefundAsync(Payment payment, LedgerEntry reversalEntry)
	{
		_context.Payments.Update(payment);
		_context.LedgerEntries.Add(reversalEntry);
		if (payment.Invoice != null)
		{
			_context.Invoices.Update(payment.Invoice);
		}
		await _context.SaveChangesAsync();
	}

	public async Task<Waiver> AddWaiverAsync(Waiver waiver)
	{
		_context.Waivers.Add(waiver);
		await _context.SaveChangesAsync();
		return waiver;
	}

	public async Task ApplyWaiverAsync(Waiver waiver, LedgerEntry ledgerEntry)
	{
		_context.Waivers.Add(waiver);
		_context.LedgerEntries.Add(ledgerEntry);
		await _context.SaveChangesAsync();
	}

	public async Task AddLedgerEntryAsync(LedgerEntry entry)
	{
		_context.LedgerEntries.Add(entry);
		await _context.SaveChangesAsync();
	}

	public async Task AddLedgerEntriesAsync(IEnumerable<LedgerEntry> entries)
	{
		_context.LedgerEntries.AddRange(entries);
		await _context.SaveChangesAsync();
	}

	public async Task<decimal> GetBalanceAsync(int studentId)
	{
		return (await _context.LedgerEntries.Where((LedgerEntry e) => e.StudentId == studentId).SumAsync((Expression<Func<LedgerEntry, decimal?>>)((LedgerEntry e) => e.Amount), default(CancellationToken))).GetValueOrDefault();
	}

	public async Task<IEnumerable<LedgerEntry>> GetLedgerAsync(int studentId)
	{
		return await (from e in _context.LedgerEntries.Include((LedgerEntry e) => e.Invoice).Include((LedgerEntry e) => e.Payment)
			where e.StudentId == studentId
			orderby e.CreatedAt
			select e).ToListAsync();
	}

	public async Task ApplyInvoiceCancellationAsync(Invoice invoice, LedgerEntry reversalEntry)
	{
		_context.Invoices.Update(invoice);
		_context.LedgerEntries.Add(reversalEntry);
		await _context.SaveChangesAsync();
	}

	private IQueryable<Invoice> ActiveInvoices(int? semesterId, int? academicYearId)
	{
		IQueryable<Invoice> queryable = _context.Invoices.Where((Invoice i) => (int)i.Status != 4);
		if (semesterId.HasValue)
		{
			queryable = queryable.Where((Invoice i) => i.SemesterId == ((int?)semesterId).Value);
		}
		if (academicYearId.HasValue)
		{
			queryable = queryable.Where((Invoice i) => i.Semester.AcademicYearId == ((int?)academicYearId).Value);
		}
		return queryable;
	}

	public async Task<decimal> SumInvoiceTotalsAsync(int? semesterId = null, int? academicYearId = null)
	{
		return (await ActiveInvoices(semesterId, academicYearId).SumAsync((Expression<Func<Invoice, decimal?>>)((Invoice i) => i.TotalAmount), default(CancellationToken))).GetValueOrDefault();
	}

	public async Task<decimal> SumVerifiedPaymentsAsync(int? semesterId = null, int? academicYearId = null)
	{
		IQueryable<Payment> query = _context.Payments.Where((Payment p) => (int)p.Status == 2 && (int)p.Invoice.Status != 4);
		if (semesterId.HasValue)
		{
			query = query.Where((Payment p) => p.Invoice.SemesterId == ((int?)semesterId).Value);
		}
		if (academicYearId.HasValue)
		{
			query = query.Where((Payment p) => p.Invoice.Semester.AcademicYearId == ((int?)academicYearId).Value);
		}
		return (await query.SumAsync((Expression<Func<Payment, decimal?>>)((Payment p) => p.Amount), default(CancellationToken))).GetValueOrDefault();
	}

	public async Task<decimal> SumWaiversAsync(int? semesterId = null, int? academicYearId = null)
	{
		IQueryable<Waiver> query = _context.Waivers.Where((Waiver w) => (int)w.Invoice.Status != 4);
		if (semesterId.HasValue)
		{
			query = query.Where((Waiver w) => w.Invoice.SemesterId == ((int?)semesterId).Value);
		}
		if (academicYearId.HasValue)
		{
			query = query.Where((Waiver w) => w.Invoice.Semester.AcademicYearId == ((int?)academicYearId).Value);
		}
		return (await query.SumAsync((Expression<Func<Waiver, decimal?>>)((Waiver w) => w.Amount), default(CancellationToken))).GetValueOrDefault();
	}

	public async Task<int> CountPaymentsAsync(PaymentStatus status)
	{
		return await _context.Payments.CountAsync((Payment p) => (int)p.Status == (int)status);
	}

	public async Task<Dictionary<PaymentStatus, int>> GetPaymentStatusCountsAsync(int? academicYearId)
	{
		IQueryable<Payment> query = _context.Payments.AsQueryable();
		if (academicYearId.HasValue)
		{
			query = query.Where((Payment p) => p.Invoice.Semester.AcademicYearId == ((int?)academicYearId).Value);
		}
		return await (from p in query
			group p by p.Status into g
			select new
			{
				Key = g.Key,
				Count = g.Count()
			}).ToDictionaryAsync(x => x.Key, x => x.Count);
	}

	public async Task<List<SemesterCollectionDto>> GetSemesterCollectionsAsync(int? academicYearId)
	{
		IQueryable<Invoice> query = ActiveInvoices(null, academicYearId);
		return await (from i in query
			group i by new
			{
				SemesterId = i.SemesterId,
				SemesterName = i.Semester.Name,
				YearName = i.Semester.AcademicYear.Name
			} into g
			select new SemesterCollectionDto
			{
				SemesterId = g.Key.SemesterId,
				SemesterName = g.Key.SemesterName,
				AcademicYearName = g.Key.YearName,
				Expected = g.Sum((Invoice i) => i.TotalAmount),
				Collected = g.Sum((Invoice i) => i.Payments.Where((Payment p) => (int)p.Status == 2).Sum((Func<Payment, decimal?>)((Payment p) => p.Amount)) ?? 0m),
				Waived = g.Sum((Invoice i) => ((IEnumerable<Waiver>)i.Waivers).Sum((Func<Waiver, decimal?>)((Waiver w) => w.Amount)) ?? 0m)
			} into s
			orderby s.AcademicYearName, s.SemesterName
			select s).ToListAsync();
	}

	public async Task<List<DepartmentCollectionDto>> GetDepartmentCollectionsAsync(int? academicYearId)
	{
		IQueryable<Invoice> query = from i in ActiveInvoices(null, academicYearId)
			where i.Student != null
			select i;
		return await (from i in query
			group i by new
			{
				DepartmentId = i.Student.DepartmentId,
				DepartmentName = i.Student.Department.Name
			} into g
			select new DepartmentCollectionDto
			{
				DepartmentId = g.Key.DepartmentId,
				DepartmentName = g.Key.DepartmentName,
				StudentCount = g.Select((Invoice i) => i.StudentId).Distinct().Count(),
				Expected = g.Sum((Invoice i) => i.TotalAmount),
				Collected = g.Sum((Invoice i) => i.Payments.Where((Payment p) => (int)p.Status == 2).Sum((Func<Payment, decimal?>)((Payment p) => p.Amount)) ?? 0m)
			} into d
			orderby d.DepartmentName
			select d).ToListAsync();
	}
}
