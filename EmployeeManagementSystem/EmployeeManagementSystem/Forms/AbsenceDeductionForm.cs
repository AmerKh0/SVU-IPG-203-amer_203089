using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Forms
{
    /// <summary>
    /// نافذة خصم الغياب — تدعم خصم ساعات وخصم أيام في آنٍ واحد
    ///
    /// خصم الساعات:  قيمة الساعة = الراتب ÷ أيام الدوام ÷ ساعات اليوم
    /// خصم الأيام:   قيمة اليوم  = الراتب ÷ أيام الدوام
    /// الخصم الإجمالي = خصم الساعات + خصم الأيام
    /// </summary>
    public class AbsenceDeductionForm : Form
    {
        private readonly Employee _employee;

        private NumericUpDown nudWorkDays     = null!;
        private NumericUpDown nudDailyHours   = null!;
        private NumericUpDown nudAbsenceHours = null!;
        private NumericUpDown nudAbsenceDays  = null!;
        private Label         lblHourValue    = null!;
        private Label         lblDayValue     = null!;
        private Label         lblHourDed      = null!;
        private Label         lblDayDed       = null!;
        private Label         lblTotalDed     = null!;
        private Label         lblPreview      = null!;

        public AbsenceDeductionForm(Employee employee)
        {
            _employee = employee;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text            = $"خصم الغياب — {_employee.Name}";
            this.Size            = new Size(440, 730);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Tahoma", 9f);

            // ── HEADER ──────────────────────────────────────────────
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(185, 28, 28) };
            pnlHeader.Controls.Add(new Label
            {
                Text      = "⏱  خصم الغياب (ساعات وأيام)",
                ForeColor = Color.White,
                Font      = new Font("Tahoma", 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock      = DockStyle.Fill,
                Padding   = new Padding(15, 0, 0, 0)
            });
            this.Controls.Add(pnlHeader);

            // ── بيانات الموظف ────────────────────────────────────────
            int y = 70;
            AddInfoRow("الموظف:",          _employee.Name,                        ref y);
            AddInfoRow("الراتب الأساسي:",  $"{_employee.CalculateSalary():F2} $", ref y);
            AddInfoRow("الخصم الحالي:",    $"{_employee.Deduction:F2} $",         ref y);

            AddSep(ref y);

            // ── إعدادات الدوام ───────────────────────────────────────
            AddLabel("أيام الدوام الشهرية:", y, bold: true);
            nudWorkDays = new NumericUpDown
            {
                Top = y - 3, Left = 230, Width = 170,
                Minimum = 1, Maximum = 31, DecimalPlaces = 0,
                Value = _employee.WorkDays
            };
            nudWorkDays.ValueChanged += Recalculate;
            this.Controls.Add(nudWorkDays);
            y += 38;

            AddLabel("ساعات الدوام اليومية:", y, bold: true);
            nudDailyHours = new NumericUpDown
            {
                Top = y - 3, Left = 230, Width = 170,
                Minimum = 1, Maximum = 24, DecimalPlaces = 0,
                Value = _employee.DailyHours
            };
            nudDailyHours.ValueChanged += Recalculate;
            this.Controls.Add(nudDailyHours);
            y += 38;

            // قيمة الساعة / اليوم
            AddLabel("قيمة الساعة الواحدة:", y, color: Color.FromArgb(71, 85, 105));
            lblHourValue = MakeValueLabel(y, Color.FromArgb(30, 64, 175));
            y += 25;

            AddLabel("قيمة اليوم الواحد:", y, color: Color.FromArgb(71, 85, 105));
            lblDayValue = MakeValueLabel(y, Color.FromArgb(30, 64, 175));
            y += 28;

            AddSep(ref y);

            // ── قسم خصم الساعات ──────────────────────────────────────
            var lblSecHours = new Label
            {
                Text = "📌  خصم ساعات الغياب",
                Top = y, Left = 20, Width = 380,
                Font = new Font("Tahoma", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(185, 28, 28),
                BackColor = Color.FromArgb(254, 242, 242),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0),
                Height = 24
            };
            this.Controls.Add(lblSecHours);
            y += 30;

            AddLabel("عدد ساعات الغياب:", y, bold: true, color: Color.FromArgb(185, 28, 28));
            nudAbsenceHours = new NumericUpDown
            {
                Top = y - 3, Left = 230, Width = 170,
                Minimum = 0, Maximum = 999, DecimalPlaces = 1,
                Value = 0
            };
            nudAbsenceHours.ValueChanged += Recalculate;
            this.Controls.Add(nudAbsenceHours);
            y += 36;

            AddLabel("خصم الساعات:", y, color: Color.FromArgb(71, 85, 105));
            lblHourDed = MakeValueLabel(y, Color.FromArgb(185, 28, 28));
            y += 28;

            AddSep(ref y);

            // ── قسم خصم الأيام ───────────────────────────────────────
            var lblSecDays = new Label
            {
                Text = "📌  خصم أيام الغياب",
                Top = y, Left = 20, Width = 380,
                Font = new Font("Tahoma", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(146, 64, 14),
                BackColor = Color.FromArgb(255, 247, 237),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0),
                Height = 24
            };
            this.Controls.Add(lblSecDays);
            y += 30;

            AddLabel("عدد أيام الغياب:", y, bold: true, color: Color.FromArgb(146, 64, 14));
            nudAbsenceDays = new NumericUpDown
            {
                Top = y - 3, Left = 230, Width = 170,
                Minimum = 0, Maximum = 31, DecimalPlaces = 0,
                Value = 0
            };
            nudAbsenceDays.ValueChanged += Recalculate;
            this.Controls.Add(nudAbsenceDays);
            y += 36;

            AddLabel("خصم الأيام:", y, color: Color.FromArgb(71, 85, 105));
            lblDayDed = MakeValueLabel(y, Color.FromArgb(146, 64, 14));
            y += 28;

            AddSep(ref y);

            // ── الخصم الإجمالي ───────────────────────────────────────
            AddLabel("إجمالي الخصم الجديد:", y, bold: true);
            lblTotalDed = MakeValueLabel(y, Color.FromArgb(185, 28, 28));
            lblTotalDed.Font = new Font("Tahoma", 10f, FontStyle.Bold);
            y += 32;

            // ── معاينة الراتب النهائي ────────────────────────────────
            lblPreview = new Label
            {
                Top = y, Left = 20, Width = 385, Height = 42,
                Font      = new Font("Tahoma", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(254, 242, 242),
                ForeColor = Color.FromArgb(185, 28, 28)
            };
            this.Controls.Add(lblPreview);
            y += 52;

            // ── الأزرار ──────────────────────────────────────────────
            var btnApply = new Button
            {
                Text      = "✔  تطبيق الخصم",
                Top = y, Left = 20, Width = 170, Height = 36,
                BackColor = Color.FromArgb(185, 28, 28),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Tahoma", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += BtnApply_Click;

            var btnCancel = new Button
            {
                Text         = "إلغاء",
                Top = y, Left = 200, Width = 90, Height = 36,
                FlatStyle    = FlatStyle.Flat,
                BackColor    = Color.FromArgb(241, 245, 249),
                DialogResult = DialogResult.Cancel,
                Cursor       = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.LightGray;

            var btnReset = new Button
            {
                Text      = "🔄 إعادة ضبط",
                Top = y, Left = 300, Width = 110, Height = 36,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(100, 116, 139),
                Cursor    = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderColor = Color.LightGray;
            btnReset.Click += (s, e) =>
            {
                if (MessageBox.Show("هل تريد إعادة ضبط الخصم الكلي للموظف إلى 0؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _employee.Deduction = 0;
                    this.DialogResult   = DialogResult.OK;
                    this.Close();
                }
            };

            this.Controls.AddRange(new Control[] { btnApply, btnCancel, btnReset });
            this.CancelButton = btnCancel;

            Recalculate(null, EventArgs.Empty);
        }

        // ── مساعدات البناء ───────────────────────────────────────────
        private void AddInfoRow(string label, string value, ref int y)
        {
            this.Controls.Add(new Label { Text = label, Top = y, Left = 20, Width = 200, ForeColor = Color.Gray });
            this.Controls.Add(new Label { Text = value, Top = y, Left = 225, Width = 180, Font = new Font("Tahoma", 9f, FontStyle.Bold) });
            y += 26;
        }

        private void AddSep(ref int y)
        {
            this.Controls.Add(new Panel { Top = y + 3, Left = 20, Width = 385, Height = 1, BackColor = Color.FromArgb(226, 232, 240) });
            y += 18;
        }

        private void AddLabel(string text, int y, bool bold = false, Color color = default)
        {
            this.Controls.Add(new Label
            {
                Text      = text,
                Top = y, Left = 20, Width = 205,
                Font      = new Font("Tahoma", 9f, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color == default ? Color.FromArgb(51, 65, 85) : color
            });
        }

        private Label MakeValueLabel(int y, Color color)
        {
            var lbl = new Label
            {
                Top = y, Left = 230, Width = 170,
                Font      = new Font("Tahoma", 9f, FontStyle.Bold),
                ForeColor = color
            };
            this.Controls.Add(lbl);
            return lbl;
        }

        // ── الحساب التلقائي ──────────────────────────────────────────
        private void Recalculate(object? sender, EventArgs e)
        {
            double salary     = _employee.CalculateSalary();
            int    workDays   = (int)nudWorkDays.Value;
            int    dailyHours = (int)nudDailyHours.Value;
            double absHours   = (double)nudAbsenceHours.Value;
            int    absDays    = (int)nudAbsenceDays.Value;

            double hourValue  = salary / workDays / dailyHours;
            double dayValue   = salary / workDays;

            double hoursDed   = hourValue * absHours;
            double daysDed    = dayValue  * absDays;
            double totalDed   = hoursDed + daysDed;

            double newFinal   = salary + _employee.CompanyBonus - _employee.Deduction - totalDed;

            lblHourValue.Text = $"{hourValue:F2} $";
            lblDayValue.Text  = $"{dayValue:F2} $";
            lblHourDed.Text   = absHours > 0 ? $"- {hoursDed:F2} $" : "0.00 $";
            lblDayDed.Text    = absDays  > 0 ? $"- {daysDed:F2} $"  : "0.00 $";
            lblTotalDed.Text  = totalDed > 0 ? $"- {totalDed:F2} $" : "0.00 $";

            bool hasDeduction = totalDed > 0;
            lblPreview.Text      = hasDeduction
                ? $"الراتب النهائي بعد الخصم: {newFinal:F2} $"
                : $"لا يوجد غياب — الراتب الحالي: {salary + _employee.CompanyBonus - _employee.Deduction:F2} $";
            lblPreview.BackColor = hasDeduction ? Color.FromArgb(254, 242, 242) : Color.FromArgb(240, 253, 244);
            lblPreview.ForeColor = hasDeduction ? Color.FromArgb(185, 28, 28)   : Color.FromArgb(22, 101, 52);
        }

        // ── حفظ ──────────────────────────────────────────────────────
        private void BtnApply_Click(object? sender, EventArgs e)
        {
            double absHours = (double)nudAbsenceHours.Value;
            int    absDays  = (int)nudAbsenceDays.Value;

            if (absHours <= 0 && absDays <= 0)
            {
                MessageBox.Show("الرجاء إدخال عدد ساعات أو أيام الغياب.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double salary     = _employee.CalculateSalary();
            int    workDays   = (int)nudWorkDays.Value;
            int    dailyHours = (int)nudDailyHours.Value;

            double hourValue = salary / workDays / dailyHours;
            double dayValue  = salary / workDays;
            double totalDed  = (hourValue * absHours) + (dayValue * absDays);

            // حفظ إعدادات الدوام
            _employee.WorkDays   = workDays;
            _employee.DailyHours = dailyHours;

            // إضافة الخصم فوق الخصم الموجود
            _employee.Deduction += totalDed;

            // رسالة تفصيلية
            string details = "";
            if (absHours > 0) details += $"خصم {absHours} ساعة: {hourValue * absHours:F2} $\n";
            if (absDays  > 0) details += $"خصم {absDays} يوم: {dayValue * absDays:F2} $\n";
            details += $"الإجمالي المضاف: {totalDed:F2} $";

            MessageBox.Show($"✅  تم تطبيق الخصم بنجاح:\n\n{details}",
                "تم الخصم", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
