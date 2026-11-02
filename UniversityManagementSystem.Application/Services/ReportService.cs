using UniversityManagementSystem.Application.DTOs.Reports;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IExamRepository _examRepo;
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly ICourseRepository _courseRepo;

        // Safety net: EPPlus 8+ requires the license to be set before use.
        // This ensures exports work even when ReportService is resolved outside the web host.
        static ReportService()
        {
            ExcelPackage.License.SetNonCommercialPersonal("University Management System");
        }

        public ReportService(
            IStudentRepository studentRepo,
            IInstructorRepository instructorRepo,
            IAttendanceRepository attendanceRepo,
            IExamRepository examRepo,
            IAssignmentRepository assignmentRepo,
            IGradeRepository gradeRepo,
            ICourseRepository courseRepo)
        {
            _studentRepo = studentRepo;
            _instructorRepo = instructorRepo;
            _attendanceRepo = attendanceRepo;
            _examRepo = examRepo;
            _assignmentRepo = assignmentRepo;
            _gradeRepo = gradeRepo;
            _courseRepo = courseRepo;
        }

        public async Task<byte[]> GenerateReportAsync(string reportType, string format, ExportFilterDto? filter = null)
        {
            // Fetch data based on reportType
            var data = await GetReportDataAsync(reportType, filter);
            if (format?.ToLower() == "excel")
                return ExportToExcel(data);
            else if (format?.ToLower() == "pdf")
                return ExportToPdf(data);
            else
                throw new NotSupportedException($"Format '{format}' not supported.");
        }

        private async Task<List<Dictionary<string, object>>> GetReportDataAsync(string reportType, ExportFilterDto? filter)
        {
            var result = new List<Dictionary<string, object>>();

            switch (reportType?.ToLower())
            {
                case "students":
                    var students = await _studentRepo.GetAllStudentsWithDetailsAsync();
                    foreach (var s in students)
                    {
                        result.Add(new Dictionary<string, object>
                        {
                            ["Student ID"] = s.StudentId,
                            ["Name"] = $"{s.FirstName} {s.LastName}",
                            ["Email"] = s.Email,
                            ["Phone"] = s.Phone,
                            ["Department"] = s.Department?.Name ?? "",
                            ["Enrollment Date"] = s.EnrollmentDate.ToString("yyyy-MM-dd")
                        });
                    }
                    break;

                case "courses":
                    var courses = await _courseRepo.GetAllCoursesWithDetailsAsync();
                    foreach (var c in courses)
                    {
                        result.Add(new Dictionary<string, object>
                        {
                            ["Code"] = c.Code,
                            ["Title"] = c.Title,
                            ["Credits"] = c.Credits.ToString(),
                            ["Department"] = c.Department?.Name ?? "",
                            ["Instructor"] = c.Instructor != null ? $"{c.Instructor.FirstName} {c.Instructor.LastName}" : "",
                            ["Start Date"] = c.StartDate.ToString("yyyy-MM-dd"),
                            ["End Date"] = c.EndDate.ToString("yyyy-MM-dd")
                        });
                    }
                    break;

                case "instructors":
                    var instructors = await _instructorRepo.GetAllInstructorsWithDetailsAsync();
                    foreach (var t in instructors)
                    {
                        result.Add(new Dictionary<string, object>
                        {
                            ["Name"] = $"{t.FirstName} {t.LastName}",
                            ["Email"] = t.Email,
                            ["Phone"] = t.Phone,
                            ["Qualification"] = t.Qualification,
                            ["Department"] = t.Department?.Name ?? "",
                            ["Hire Date"] = t.HireDate.ToString("yyyy-MM-dd")
                        });
                    }
                    break;

                case "attendance":
                    var allAtt = (await _attendanceRepo.GetAllAsync()).ToList();
                    foreach (var a in allAtt)
                    {
                        result.Add(new Dictionary<string, object>
                        {
                            ["Student"] = $"{a.Student?.FirstName} {a.Student?.LastName}",
                            ["Course"] = a.Course?.Title ?? "",
                            ["Date"] = a.Date.ToString("yyyy-MM-dd"),
                            ["Status"] = a.Status.ToString(),
                            ["Remarks"] = a.Remarks ?? ""
                        });
                    }
                    break;



                case "exam-results":
                case "examresults":
                    var examId = filter?.CourseId ?? 0; // Use CourseId as examId for filtering
                    if (examId > 0)
                    {
                        var examResults = await _examRepo.GetResultsByExamIdAsync(examId);
                        foreach (var r in examResults)
                        {
                            result.Add(new Dictionary<string, object>
                            {
                                ["Exam"] = r.Exam?.Title ?? "",
                                ["Student"] = $"{r.Student?.FirstName} {r.Student?.LastName}",
                                ["Score"] = r.TotalScore.ToString(),
                                ["Grade"] = r.Grade ?? "",
                                ["Result"] = r.IsPass ? "Pass" : "Fail"
                            });
                        }
                    }
                    break;

                case "assignments":
                    var assignments = (await _assignmentRepo.GetAllAsync()).ToList();
                    foreach (var a in assignments)
                    {
                        result.Add(new Dictionary<string, object>
                        {
                            ["Title"] = a.Title,
                            ["Course"] = a.Course?.Title ?? "",
                            ["Deadline"] = a.Deadline.ToString("yyyy-MM-dd"),
                            ["Total Marks"] = a.TotalMarks.ToString(),
                            ["Published"] = a.IsPublished ? "Yes" : "No"
                        });
                    }
                    break;

                case "grades":
                    var grades = (await _gradeRepo.GetAllAsync()).ToList();
                    foreach (var g in grades)
                    {
                        result.Add(new Dictionary<string, object>
                        {
                            ["Student"] = $"{g.Student?.FirstName} {g.Student?.LastName}",
                            ["Course"] = g.Course?.Title ?? "",
                            ["Score"] = $"{g.Score}/{g.MaxScore}",
                            ["Letter Grade"] = g.LetterGrade ?? "",
                            ["Type"] = g.Type.ToString(),
                            ["Remarks"] = g.Remarks ?? ""
                        });
                    }
                    break;

                default:
                    // Unknown report type, return empty
                    break;
            }

            return result;
        }

        private byte[] ExportToExcel(List<Dictionary<string, object>> data)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Report");
            if (data.Any())
            {
                var headers = data.First().Keys.ToArray();
                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cells[1, i + 1].Value = headers[i];
                for (int row = 0; row < data.Count; row++)
                    for (int col = 0; col < headers.Length; col++)
                        worksheet.Cells[row + 2, col + 1].Value = data[row][headers[col]]?.ToString();
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            }
            return package.GetAsByteArray();
        }

        private byte[] ExportToPdf(List<Dictionary<string, object>> data)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(document, ms);
            document.Open();
            if (data.Any())
            {
                var table = new PdfPTable(data.First().Keys.Count);
                var headers = data.First().Keys.ToArray();
                foreach (var h in headers)
                    table.AddCell(new Phrase(h));
                foreach (var row in data)
                    foreach (var key in headers)
                        table.AddCell(row[key]?.ToString() ?? "");
                document.Add(table);
            }
            document.Close();
            return ms.ToArray();
        }
    }
}