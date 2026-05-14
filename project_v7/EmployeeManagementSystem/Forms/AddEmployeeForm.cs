using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.Forms
{
    /// <summary>
    /// نافذة إضافة موظف جديد — تدعم الأنواع الثلاثة
    /// </summary>
    public class AddEmployeeForm : Form
    {
        /// <summary>الموظف الذي تم إنشاؤه (يُقرأ بعد إغلاق النافذة بنتيجة OK)</summary>
        public Employee? CreatedEmployee { get; private set; }

        // نوع الموظف
        private RadioButton rbFullTime = null!;
        private RadioButton rbPartTime = null!;
        private RadioButton rbContractor = null!;

        // حقول مشتركة
        private TextBox txtName = null!;
        private TextBox txtDepartment = null!;

        // حقول الدوام الكامل
        private Panel pnlFullTime = null!;
        private NumericUpDown nudBaseSalary = null!;
        private NumericUpDown nudBonus = null!;

        // حقول الدوام الجزئي
        private Panel pnlPartTime = null!;
        private NumericUpDown nudHours = null!;
        private NumericUpDown nudHourlyRate = null!;

        // حقول المتعاقد
        private Panel pnlContractor = null!;
        private NumericUpDown nudContractAmount = null!;
        private TextBox txtContractType = null!;
        private NumericUpDown nudDuration = null!;

        private Label lblError = null!;
        private Button btnConfirm = null!;

        public AddEmployeeForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "إضافة موظف جديد";
            this.Size = new Size(430, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Tahoma", 9f);

            // ── HEADER ──────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.FromArgb(30, 64, 175)
            };
            var lblTitle = new Label
            {
                Text = "➕  إضافة موظف جديد",
                ForeColor = Color.White,
                Font = new Font("Tahoma", 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 0, 0, 0)
            };
            pnlHeader.Controls.Add(lblTitle);

            // ── BODY ─────────────────────────────────────────────────
            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 15, 20, 10) };

            // نوع الموظف
            var grpType = new GroupBox
            {
                Text = "نوع الموظف",
                Top = 10, Left = 0, Width = 370, Height = 55,
                Font = new Font("Tahoma", 9f, FontStyle.Bold)
            };
            rbFullTime   = new RadioButton { Text = "دوام كامل",  Left = 10,  Top = 22, Checked = true, Width = 110 };
            rbPartTime   = new RadioButton { Text = "دوام جزئي",  Left = 130, Top = 22, Width = 110 };
            rbContractor = new RadioButton { Text = "متعاقد",     Left = 250, Top = 22, Width = 100 };
            grpType.Controls.AddRange(new Control[] { rbFullTime, rbPartTime, rbContractor });

            // الاسم والقسم
            MakeLabel(body, "الاسم:", 80);
            txtName       = MakeTextBox(body, 80);
            MakeLabel(body, "القسم:", 112);
            txtDepartment = MakeTextBox(body, 112);

            // ── لوحة الدوام الكامل ───────────────────────────────────
            pnlFullTime = MakePanel(145);
            MakeFieldLabel(pnlFullTime, "الراتب الأساسي ($):", 0);
            nudBaseSalary = MakeNumeric(pnlFullTime, 0, 0, 999999, 3000);
            MakeFieldLabel(pnlFullTime, "العلاوة الشهرية ($):", 32);
            nudBonus = MakeNumeric(pnlFullTime, 32, 0, 99999, 500);

            // ── لوحة الدوام الجزئي ───────────────────────────────────
            pnlPartTime = MakePanel(145, false);
            MakeFieldLabel(pnlPartTime, "عدد ساعات العمل:", 0);
            nudHours = MakeNumericInt(pnlPartTime, 0, 1, 744, 80);
            MakeFieldLabel(pnlPartTime, "سعر الساعة ($):", 32);
            nudHourlyRate = MakeNumeric(pnlPartTime, 32, 0, 9999, 15);

            // ── لوحة المتعاقد ────────────────────────────────────────
            pnlContractor = MakePanel(145, false);
            MakeFieldLabel(pnlContractor, "قيمة العقد الشهرية ($):", 0);
            nudContractAmount = MakeNumeric(pnlContractor, 0, 0, 999999, 5000);
            MakeFieldLabel(pnlContractor, "نوع العقد:", 32);
            txtContractType = new TextBox { Top = 30, Left = 155, Width = 210, Text = "سنوي" };
            pnlContractor.Controls.Add(txtContractType);
            MakeFieldLabel(pnlContractor, "مدة العقد (أشهر):", 62);
            nudDuration = MakeNumericInt(pnlContractor, 62, 1, 120, 12);

            // رسالة الخطأ
            lblError = new Label
            {
                Top = 240, Left = 0, Width = 370, Height = 30,
                ForeColor = Color.FromArgb(185, 28, 28),
                Font = new Font("Tahoma", 8.5f)
            };

            // أزرار
            btnConfirm = new Button
            {
                Text = "✔  إضافة",
                Top = 275, Left = 200, Width = 90, Height = 36,
                BackColor = Color.FromArgb(30, 64, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += BtnConfirm_Click;

            var btnCancel = new Button
            {
                Text = "إلغاء",
                Top = 275, Left = 300, Width = 70, Height = 36,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                DialogResult = DialogResult.Cancel,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.LightGray;

            body.Controls.AddRange(new Control[]
            {
                grpType, pnlFullTime, pnlPartTime, pnlContractor,
                lblError, btnConfirm, btnCancel
            });

            this.Controls.Add(body);
            this.Controls.Add(pnlHeader);
            this.CancelButton = btnCancel;

            // ربط أحداث تغيير النوع
            rbFullTime.CheckedChanged   += TypeChanged;
            rbPartTime.CheckedChanged   += TypeChanged;
            rbContractor.CheckedChanged += TypeChanged;
        }

        // ============================================================
        // مساعدات بناء الواجهة
        // ============================================================
        private void MakeLabel(Panel parent, string text, int top)
        {
            parent.Controls.Add(new Label { Text = text, Top = top + 3, Left = 0, Width = 120 });
        }
        private TextBox MakeTextBox(Panel parent, int top)
        {
            var tb = new TextBox { Top = top, Left = 130, Width = 240 };
            parent.Controls.Add(tb);
            return tb;
        }
        private Panel MakePanel(int top, bool visible = true)
        {
            return new Panel { Top = top, Left = 0, Width = 375, Height = 100, Visible = visible };
        }
        private void MakeFieldLabel(Panel p, string text, int top)
        {
            p.Controls.Add(new Label { Text = text, Top = top + 3, Left = 0, Width = 150 });
        }
        private NumericUpDown MakeNumeric(Panel p, int top, decimal min, decimal max, decimal val)
        {
            var n = new NumericUpDown { Top = top, Left = 155, Width = 215, Minimum = min, Maximum = max, Value = val, DecimalPlaces = 2 };
            p.Controls.Add(n);
            return n;
        }
        private NumericUpDown MakeNumericInt(Panel p, int top, decimal min, decimal max, decimal val)
        {
            var n = new NumericUpDown { Top = top, Left = 155, Width = 215, Minimum = min, Maximum = max, Value = val, DecimalPlaces = 0 };
            p.Controls.Add(n);
            return n;
        }

        // ============================================================
        // تغيير نوع الموظف
        // ============================================================
        private void TypeChanged(object? sender, EventArgs e)
        {
            pnlFullTime.Visible   = rbFullTime.Checked;
            pnlPartTime.Visible   = rbPartTime.Checked;
            pnlContractor.Visible = rbContractor.Checked;
        }

        // ============================================================
        // تأكيد الإضافة مع التحقق من البيانات
        // ============================================================
        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";

            string name = txtName.Text.Trim();
            string dept = txtDepartment.Text.Trim();

            // التحقق من صحة البيانات باستخدام Validator (Static Class)
            if (!Validator.IsValidName(name))
            {
                lblError.Text = "❌  " + Validator.GetNameError();
                txtName.Focus();
                return;
            }
            if (!Validator.IsValidDepartment(dept))
            {
                lblError.Text = "❌  اسم القسم يجب أن يكون غير فارغ.";
                txtDepartment.Focus();
                return;
            }

            try
            {
                // إنشاء الكائن المناسب بحسب النوع (Polymorphism)
                if (rbFullTime.Checked)
                {
                    CreatedEmployee = new FullTimeEmployee(
                        name, dept,
                        (double)nudBaseSalary.Value,
                        (double)nudBonus.Value);
                }
                else if (rbPartTime.Checked)
                {
                    CreatedEmployee = new PartTimeEmployee(
                        name, dept,
                        (int)nudHours.Value,
                        (double)nudHourlyRate.Value);
                }
                else
                {
                    CreatedEmployee = new Contractor(
                        name, dept,
                        (double)nudContractAmount.Value,
                        string.IsNullOrWhiteSpace(txtContractType.Text) ? "غير محدد" : txtContractType.Text,
                        (int)nudDuration.Value);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "❌  " + ex.Message;
            }
        }
    }
}
