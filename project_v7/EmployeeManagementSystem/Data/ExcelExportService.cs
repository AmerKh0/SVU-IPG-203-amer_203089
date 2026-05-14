using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ClosedXML.Excel;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data
{
    /// <summary>
    /// خدمة تصدير بيانات الموظفين والبيعات إلى ملف Excel
    /// </summary>
    public static class ExcelExportService
    {
        public static void Export(IReadOnlyList<Employee> employees, string filePath)
        {
            using var wb = new XLWorkbook();
            wb.RightToLeft = true;

            BuildEmployeesSheet(wb, employees);
            BuildSalesSheet(wb, employees);

            wb.SaveAs(filePath);
        }

        // ── ورقة الموظفين ─────────────────────────────────────────
        private static void BuildEmployeesSheet(XLWorkbook wb, IReadOnlyList<Employee> employees)
        {
            var ws = wb.Worksheets.Add("الموظفون");
            ws.RightToLeft = true;

            // عنوان الورقة
            ws.Cell(1, 1).Value = "كشف رواتب الموظفين";
            var titleRange = ws.Range(1, 1, 1, 11);
            titleRange.Merge();
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = 14;
            titleRange.Style.Font.FontName = "Arial";
            titleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(30, 64, 175);
            titleRange.Style.Font.FontColor = XLColor.White;
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Row(1).Height = 30;

            // تاريخ التصدير
            ws.Cell(2, 1).Value = $"تاريخ التصدير: {DateTime.Now:dd/MM/yyyy  HH:mm}";
            var dateRange = ws.Range(2, 1, 2, 11);
            dateRange.Merge();
            dateRange.Style.Font.Italic = true;
            dateRange.Style.Font.FontColor = XLColor.FromArgb(100, 116, 139);
            dateRange.Style.Font.FontName = "Arial";
            dateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Row(2).Height = 20;

            // رؤوس الأعمدة
            string[] headers = { "الرقم", "الاسم", "القسم", "النوع", "الأساسي ($)",
                                  "مكافأة ($)", "خصم ($)", "الصافي ($)", "عدد البيعات",
                                  "إجمالي البيعات ($)", "تاريخ التوظيف" };

            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(3, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontName = "Arial";
                cell.Style.Font.FontSize = 10;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(219, 234, 254);
                cell.Style.Font.FontColor = XLColor.FromArgb(30, 64, 175);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                cell.Style.Border.BottomBorderColor = XLColor.FromArgb(30, 64, 175);
            }
            ws.Row(3).Height = 28;

            // بيانات الموظفين
            int row = 4;
            foreach (var emp in employees)
            {
                string typeLabel = emp switch
                {
                    FullTimeEmployee fte => $"كامل الوقت",
                    PartTimeEmployee     => "دوام جزئي",
                    Contractor c         => $"متعاقد - {c.ContractType}",
                    _                    => "—"
                };

                double baseSal  = emp.CalculateSalary();
                double final    = emp.GetFinalSalary();
                double salesAmt = emp.Sales.Sum(s => s.Amount);

                ws.Cell(row, 1).Value  = emp.Id;
                ws.Cell(row, 2).Value  = emp.Name;
                ws.Cell(row, 3).Value  = emp.Department;
                ws.Cell(row, 4).Value  = typeLabel;
                ws.Cell(row, 5).Value  = baseSal;
                ws.Cell(row, 6).Value  = emp.CompanyBonus;
                ws.Cell(row, 7).Value  = emp.Deduction;
                ws.Cell(row, 8).Value  = final;
                ws.Cell(row, 9).Value  = emp.Sales.Count;
                ws.Cell(row, 10).Value = salesAmt;
                ws.Cell(row, 11).Value = emp.HireDate.ToString("dd/MM/yyyy");

                // تنسيق الأرقام
                string numFmt = "#,##0.00";
                foreach (int c in new[] { 5, 6, 7, 8, 10 })
                    ws.Cell(row, c).Style.NumberFormat.Format = numFmt;

                // لون خلفية متناوب
                if (row % 2 == 0)
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);

                // تمييز الخصم بالأحمر
                if (emp.Deduction > 0)
                    ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromArgb(185, 28, 28);

                // تمييز المكافأة بالأخضر
                if (emp.CompanyBonus > 0)
                    ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromArgb(22, 101, 52);

                // تمييز الراتب المرتفع
                if (final > 5000)
                    ws.Cell(row, 8).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 249, 195);

                // تنسيق الخط لكل الصف
                ws.Row(row).Style.Font.FontName = "Arial";
                ws.Row(row).Style.Font.FontSize = 9;
                ws.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(row).Height = 22;

                row++;
            }

            // صف الإجماليات
            if (employees.Count > 0)
            {
                ws.Cell(row, 1).Value = "الإجمالي";
                ws.Cell(row, 5).FormulaA1 = $"=SUM(E4:E{row - 1})";
                ws.Cell(row, 6).FormulaA1 = $"=SUM(F4:F{row - 1})";
                ws.Cell(row, 7).FormulaA1 = $"=SUM(G4:G{row - 1})";
                ws.Cell(row, 8).FormulaA1 = $"=SUM(H4:H{row - 1})";
                ws.Cell(row, 10).FormulaA1 = $"=SUM(J4:J{row - 1})";

                var totRow = ws.Row(row);
                totRow.Style.Font.Bold = true;
                totRow.Style.Font.FontName = "Arial";
                totRow.Style.Fill.BackgroundColor = XLColor.FromArgb(23, 37, 84);
                totRow.Style.Font.FontColor = XLColor.White;
                totRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                totRow.Height = 26;
                foreach (int c in new[] { 5, 6, 7, 8, 10 })
                    ws.Cell(row, c).Style.NumberFormat.Format = "#,##0.00";
            }

            // ضبط عرض الأعمدة
            int[] widths = { 12, 22, 18, 16, 14, 13, 12, 14, 12, 18, 16 };
            for (int c = 0; c < widths.Length; c++)
                ws.Column(c + 1).Width = widths[c];

            // تجميد الرؤوس
            ws.SheetView.FreezeRows(3);
        }

        // ── ورقة البيعات ──────────────────────────────────────────
        private static void BuildSalesSheet(XLWorkbook wb, IReadOnlyList<Employee> employees)
        {
            var ws = wb.Worksheets.Add("سجل البيعات");
            ws.RightToLeft = true;

            // عنوان
            ws.Cell(1, 1).Value = "سجل بيعات جميع الموظفين";
            var titleRange = ws.Range(1, 1, 1, 8);
            titleRange.Merge();
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = 14;
            titleRange.Style.Font.FontName = "Arial";
            titleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(6, 148, 162);
            titleRange.Style.Font.FontColor = XLColor.White;
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Row(1).Height = 30;

            ws.Cell(2, 1).Value = $"تاريخ التصدير: {DateTime.Now:dd/MM/yyyy  HH:mm}";
            var dateRange = ws.Range(2, 1, 2, 8);
            dateRange.Merge();
            dateRange.Style.Font.Italic = true;
            dateRange.Style.Font.FontColor = XLColor.FromArgb(100, 116, 139);
            dateRange.Style.Font.FontName = "Arial";
            dateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Row(2).Height = 20;

            // رؤوس الأعمدة
            string[] headers = { "#", "اسم الموظف", "رقم الموظف", "اسم المشتري",
                                  "رقم الهاتف", "مبلغ البيعة ($)", "مكافأة ($)", "التاريخ" };
            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(3, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontName = "Arial";
                cell.Style.Font.FontSize = 10;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(207, 250, 254);
                cell.Style.Font.FontColor = XLColor.FromArgb(6, 148, 162);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                cell.Style.Border.BottomBorderColor = XLColor.FromArgb(6, 148, 162);
            }
            ws.Row(3).Height = 28;

            int row = 4;
            int saleNum = 1;
            foreach (var emp in employees)
            {
                foreach (var s in emp.Sales.OrderByDescending(x => x.Date))
                {
                    ws.Cell(row, 1).Value = saleNum++;
                    ws.Cell(row, 2).Value = emp.Name;
                    ws.Cell(row, 3).Value = emp.Id;
                    ws.Cell(row, 4).Value = s.BuyerName;
                    ws.Cell(row, 5).Value = s.BuyerPhone;
                    ws.Cell(row, 6).Value = s.Amount;
                    ws.Cell(row, 7).Value = s.Bonus > 0 ? s.Bonus : 0;
                    ws.Cell(row, 8).Value = s.Date.ToString("dd/MM/yyyy  HH:mm");

                    ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                    if (s.Bonus > 0)
                        ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromArgb(22, 101, 52);

                    if (row % 2 == 0)
                        ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);

                    ws.Row(row).Style.Font.FontName = "Arial";
                    ws.Row(row).Style.Font.FontSize = 9;
                    ws.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(row).Height = 22;
                    row++;
                }
            }

            // إجماليات
            if (row > 4)
            {
                ws.Cell(row, 1).Value = "الإجمالي";
                ws.Cell(row, 6).FormulaA1 = $"=SUM(F4:F{row - 1})";
                ws.Cell(row, 7).FormulaA1 = $"=SUM(G4:G{row - 1})";
                var totRow = ws.Row(row);
                totRow.Style.Font.Bold = true;
                totRow.Style.Font.FontName = "Arial";
                totRow.Style.Fill.BackgroundColor = XLColor.FromArgb(6, 148, 162);
                totRow.Style.Font.FontColor = XLColor.White;
                totRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                totRow.Height = 26;
                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            }
            else
            {
                ws.Cell(4, 1).Value = "لا توجد بيعات مسجّلة";
                ws.Range(4, 1, 4, 8).Merge();
                ws.Cell(4, 1).Style.Font.Italic = true;
                ws.Cell(4, 1).Style.Font.FontColor = XLColor.Gray;
                ws.Cell(4, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int[] widths = { 6, 22, 12, 22, 16, 16, 12, 18 };
            for (int c = 0; c < widths.Length; c++)
                ws.Column(c + 1).Width = widths[c];

            ws.SheetView.FreezeRows(3);
        }
    }
}
