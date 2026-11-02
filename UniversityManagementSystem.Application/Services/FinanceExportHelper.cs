using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniversityManagementSystem.Application.Services
{
    /// <summary>
    /// Shared Excel/PDF export helpers for finance data. Mirrors the conventions
    /// of ReportService (EPPlus license set before use, auto-fit columns,
    /// landscape A4 PDF with title header).
    /// </summary>
    internal static class FinanceExportHelper
    {
        internal static byte[] ExportToExcel(List<Dictionary<string, object>> data)
        {
            // Safety net: EPPlus 8+ requires the license to be set before use.
            ExcelPackage.License.SetNonCommercialPersonal("University Management System");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Finance");
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

        internal static byte[] ExportToPdf(List<Dictionary<string, object>> data, string title)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            document.Add(new Paragraph(title, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
            document.Add(new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", FontFactory.GetFont(FontFactory.HELVETICA, 9)));
            document.Add(new Paragraph(" "));

            if (data.Any())
            {
                var table = new PdfPTable(data.First().Keys.Count) { WidthPercentage = 100 };
                var headers = data.First().Keys.ToArray();
                foreach (var h in headers)
                    table.AddCell(new Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)));
                foreach (var row in data)
                    foreach (var key in headers)
                        table.AddCell(new Phrase(row[key]?.ToString() ?? "", FontFactory.GetFont(FontFactory.HELVETICA, 8)));
                document.Add(table);
            }
            else
            {
                document.Add(new Paragraph("No data for the selected filters."));
            }

            document.Close();
            return ms.ToArray();
        }
    }
}
