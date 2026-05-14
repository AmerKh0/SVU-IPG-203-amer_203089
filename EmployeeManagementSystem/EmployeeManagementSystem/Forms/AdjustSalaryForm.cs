using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Forms
{
    /// <summary>
    /// نافذة تعديل الراتب: إضافة مكافأة أو خصم لموظف محدد
    /// </summary>
    public class AdjustSalaryForm : Form
    {
        private readonly Employee _employee;

        private NumericUpDown nudBonus     = null!;
        private NumericUpDown nudDeduction = null!;
        private Label         lblPreview   = null!;

        public AdjustSalaryForm(Employee employee)
        {
            _employee = employee;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text            = "تعديل الراتب — مكافأة / خصم";
            this.Size            = new Size(400, 370);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Tahoma", 9f);

            // Header
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(30, 64, 175) };
            pnlHeader.Controls.Add(new Label
            {
                Text      = "✏️  تعديل الراتب",
                ForeColor = Color.White,
                Font      = new Font("Tahoma", 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock      = DockStyle.Fill,
                Padding   = new Padding(15, 0, 0, 0)
            });

            // معلومات الموظف
            int y = 70;
            AddInfoRow("الموظف:", _employee.Name, ref y);

            string empType = _employee switch
            {
                FullTimeEmployee => "دوام كامل",
                PartTimeEmployee => "دوام جزئي",
                Contractor       => "متعاقد",
                _                => "—"
            };
            AddInfoRow("النوع:", empType, ref y);
            AddInfoRow("الراتب الأساسي:", $"{_employee.CalculateSalary():F2} $", ref y);

            // فاصل
            var sep = new Panel { Top = y + 5, Left = 20, Width = 340, Height = 1, BackColor = Color.FromArgb(226, 232, 240) };
            this.Controls.Add(sep);
            y += 20;

            // مكافأة الشركة
            var lblBonus = new Label { Text = "مكافأة الشركة ($):", Top = y, Left = 20, Width = 160, Font = new Font("Tahoma", 9f, FontStyle.Bold) };
            nudBonus = new NumericUpDown
            {
                Top = y - 3, Left = 185, Width = 170,
                Minimum = 0, Maximum = 99999, DecimalPlaces = 2,
                Value = (decimal)Math.Max(_employee.CompanyBonus, 0)
            };
            nudBonus.ValueChanged += UpdatePreview;
            this.Controls.AddRange(new Control[] { lblBonus, nudBonus });
            y += 35;

            // خصم
            var lblDed = new Label { Text = "خصم ($):", Top = y, Left = 20, Width = 160, Font = new Font("Tahoma", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(185, 28, 28) };
            nudDeduction = new NumericUpDown
            {
                Top = y - 3, Left = 185, Width = 170,
                Minimum = 0, Maximum = 99999, DecimalPlaces = 2,
                Value = (decimal)Math.Max(_employee.Deduction, 0)
            };
            nudDeduction.ValueChanged += UpdatePreview;
            this.Controls.AddRange(new Control[] { lblDed, nudDeduction });
            y += 40;

            // معاينة الراتب النهائي
            lblPreview = new Label
            {
                Top = y, Left = 20, Width = 340, Height = 40,
                Font      = new Font("Tahoma", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(240, 253, 244),
                ForeColor = Color.FromArgb(22, 101, 52)
            };
            this.Controls.Add(lblPreview);
            UpdatePreview(null, EventArgs.Empty);
            y += 55;

            // أزرار
            var btnSave = new Button
            {
                Text      = "✔  حفظ",
                Top = y, Left = 185, Width = 90, Height = 35,
                BackColor = Color.FromArgb(30, 64, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Tahoma", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Text         = "إلغاء",
                Top = y, Left = 285, Width = 75, Height = 35,
                FlatStyle    = FlatStyle.Flat,
                BackColor    = Color.FromArgb(241, 245, 249),
                DialogResult = DialogResult.Cancel,
                Cursor       = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.LightGray;

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
            this.Controls.Add(pnlHeader);
            this.CancelButton = btnCancel;
        }

        private void AddInfoRow(string label, string value, ref int y)
        {
            this.Controls.Add(new Label { Text = label, Top = y, Left = 20,  Width = 160, ForeColor = Color.Gray });
            this.Controls.Add(new Label { Text = value, Top = y, Left = 185, Width = 175, Font = new Font("Tahoma", 9f, FontStyle.Bold) });
            y += 26;
        }

        private void UpdatePreview(object? sender, EventArgs e)
        {
            double baseSal = _employee.CalculateSalary();
            double bonus   = (double)nudBonus.Value;
            double ded     = (double)nudDeduction.Value;
            double final   = baseSal + bonus - ded;

            lblPreview.Text      = $"الراتب النهائي: {final:F2} $";
            lblPreview.ForeColor = final >= baseSal ? Color.FromArgb(22, 101, 52) : Color.FromArgb(185, 28, 28);
            lblPreview.BackColor = final >= baseSal ? Color.FromArgb(240, 253, 244) : Color.FromArgb(254, 242, 242);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            _employee.CompanyBonus = (double)nudBonus.Value;
            _employee.Deduction    = (double)nudDeduction.Value;
            this.DialogResult      = DialogResult.OK;
            this.Close();
        }
    }
}
