using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.Forms
{
    /// <summary>
    /// نافذة تعديل بيانات الموظف الكاملة:
    /// الاسم — الرقم التعريفي — تاريخ التوظيف — بيانات الراتب
    /// </summary>
    public class EditEmployeeForm : Form
    {
        private readonly Employee _emp;

        // حقول مشتركة
        private Label            lblIdDisplay  = null!;
        private TextBox          txtName       = null!;
        private TextBox          txtDepartment = null!;
        private DateTimePicker   dtpHireDate   = null!;

        // حقول الدوام الكامل
        private Panel            pnlFullTime   = null!;
        private NumericUpDown    nudBaseSalary = null!;
        private NumericUpDown    nudBonus      = null!;

        // حقول الدوام الجزئي
        private Panel            pnlPartTime   = null!;
        private NumericUpDown    nudHours      = null!;
        private NumericUpDown    nudHourlyRate = null!;

        // حقول المتعاقد
        private Panel            pnlContractor    = null!;
        private NumericUpDown    nudContractAmount = null!;
        private TextBox          txtContractType   = null!;

        private Label            lblError      = null!;

        public EditEmployeeForm(Employee emp)
        {
            _emp = emp;
            InitializeComponent();
            LoadEmployeeData();
        }

        // ============================================================
        private void InitializeComponent()
        {
            this.Text            = "تعديل بيانات الموظف";
            this.Size            = new Size(450, 580);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Tahoma", 9f);

            // ── HEADER ──────────────────────────────────────────────
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(30, 64, 175) };
            pnlHeader.Controls.Add(new Label
            {
                Text      = "✏️  تعديل بيانات الموظف",
                ForeColor = Color.White,
                Font      = new Font("Tahoma", 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock      = DockStyle.Fill,
                Padding   = new Padding(15, 0, 0, 0)
            });

            // ── BODY ─────────────────────────────────────────────────
            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 15, 20, 10) };

            // --- رقم تعريفي (للعرض فقط - غير قابل للتعديل) ---
            AddLabel(body, "الرقم التعريفي:", 10);
            lblIdDisplay = new Label
            {
                Top       = 13,
                Left      = 145,
                Width     = 250,
                Font      = new Font("Tahoma", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175)
            };
            body.Controls.Add(lblIdDisplay);

            // --- الاسم ---
            AddLabel(body, "اسم الموظف:", 45);
            txtName = AddTextBox(body, 45);

            // --- القسم ---
            AddLabel(body, "القسم:", 80);
            txtDepartment = AddTextBox(body, 80);

            // --- تاريخ التوظيف ---
            AddLabel(body, "تاريخ التوظيف:", 115);
            dtpHireDate = new DateTimePicker
            {
                Top    = 115,
                Left   = 145,
                Width  = 250,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Now
            };
            body.Controls.Add(dtpHireDate);

            // --- فاصل ---
            var sep = new Panel { Top = 155, Left = 0, Width = 390, Height = 1, BackColor = Color.FromArgb(226, 232, 240) };
            body.Controls.Add(sep);

            // --- عنوان قسم الراتب ---
            body.Controls.Add(new Label
            {
                Text      = "بيانات الراتب",
                Top       = 163,
                Left      = 0,
                Width     = 390,
                Font      = new Font("Tahoma", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175)
            });

            // ── لوحة الدوام الكامل ───────────────────────────────────
            pnlFullTime = new Panel { Top = 188, Left = 0, Width = 390, Height = 70, Visible = false };
            AddFieldLabel(pnlFullTime, "الراتب الأساسي ($):", 0);
            nudBaseSalary = AddNumeric(pnlFullTime, 0, 0, 999999, 3000, 2);
            AddFieldLabel(pnlFullTime, "العلاوة الشهرية ($):", 35);
            nudBonus = AddNumeric(pnlFullTime, 35, 0, 99999, 0, 2);

            // ── لوحة الدوام الجزئي ───────────────────────────────────
            pnlPartTime = new Panel { Top = 188, Left = 0, Width = 390, Height = 70, Visible = false };
            AddFieldLabel(pnlPartTime, "عدد ساعات العمل:", 0);
            nudHours = AddNumeric(pnlPartTime, 0, 1, 744, 80, 0);
            AddFieldLabel(pnlPartTime, "سعر الساعة ($):", 35);
            nudHourlyRate = AddNumeric(pnlPartTime, 35, 0, 9999, 15, 2);

            // ── لوحة المتعاقد ────────────────────────────────────────
            pnlContractor = new Panel { Top = 188, Left = 0, Width = 390, Height = 70, Visible = false };
            AddFieldLabel(pnlContractor, "قيمة العقد الشهرية ($):", 0);
            nudContractAmount = AddNumeric(pnlContractor, 0, 0, 999999, 5000, 2);
            AddFieldLabel(pnlContractor, "نوع العقد:", 35);
            txtContractType = new TextBox { Top = 33, Left = 160, Width = 225, Text = "سنوي" };
            pnlContractor.Controls.Add(txtContractType);

            // رسالة الخطأ
            lblError = new Label
            {
                Top       = 268,
                Left      = 0,
                Width     = 390,
                Height    = 35,
                ForeColor = Color.FromArgb(185, 28, 28),
                Font      = new Font("Tahoma", 8.5f)
            };

            // ── أزرار ──────────────────────────────────────────────
            var btnSave = new Button
            {
                Text      = "💾  حفظ التعديلات",
                Top       = 308,
                Left      = 145,
                Width     = 135,
                Height    = 38,
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
                Top          = 308,
                Left         = 290,
                Width        = 80,
                Height       = 38,
                FlatStyle    = FlatStyle.Flat,
                BackColor    = Color.FromArgb(241, 245, 249),
                DialogResult = DialogResult.Cancel,
                Cursor       = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.LightGray;

            body.Controls.AddRange(new Control[]
            {
                pnlFullTime, pnlPartTime, pnlContractor,
                lblError, btnSave, btnCancel
            });

            this.Controls.Add(body);
            this.Controls.Add(pnlHeader);
            this.CancelButton = btnCancel;
        }

        // ============================================================
        // تحميل بيانات الموظف الحالية
        // ============================================================
        private void LoadEmployeeData()
        {
            lblIdDisplay.Text  = _emp.Id;
            txtName.Text       = _emp.Name;
            txtDepartment.Text = _emp.Department;
            dtpHireDate.Value  = _emp.HireDate;

            switch (_emp)
            {
                case FullTimeEmployee fte:
                    pnlFullTime.Visible   = true;
                    nudBaseSalary.Value   = (decimal)fte.BaseSalary;
                    nudBonus.Value        = (decimal)fte.Bonus;
                    break;

                case PartTimeEmployee pte:
                    pnlPartTime.Visible   = true;
                    nudHours.Value        = pte.HoursWorked;
                    nudHourlyRate.Value   = (decimal)pte.HourlyRate;
                    break;

                case Contractor c:
                    pnlContractor.Visible    = true;
                    nudContractAmount.Value  = (decimal)c.ContractAmount;
                    txtContractType.Text     = c.ContractType;
                    break;
            }
        }

        // ============================================================
        // حفظ التعديلات مع التحقق
        // ============================================================
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";

            string newName = txtName.Text.Trim();
            string newDept = txtDepartment.Text.Trim();

            if (!Validator.IsValidName(newName))
            {
                lblError.Text = "❌  " + Validator.GetNameError();
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(newDept))
            {
                lblError.Text = "❌  اسم القسم لا يمكن أن يكون فارغاً.";
                txtDepartment.Focus();
                return;
            }
            if (dtpHireDate.Value.Date > DateTime.Now.Date)
            {
                lblError.Text = "❌  تاريخ التوظيف لا يمكن أن يكون في المستقبل.";
                return;
            }

            try
            {
                // تعديل البيانات المشتركة
                _emp.Name       = newName;
                _emp.Department = newDept;
                _emp.SetHireDate(dtpHireDate.Value);

                // تعديل بيانات الراتب حسب النوع
                switch (_emp)
                {
                    case FullTimeEmployee fte:
                        fte.BaseSalary = (double)nudBaseSalary.Value;
                        fte.Bonus      = (double)nudBonus.Value;
                        break;

                    case PartTimeEmployee pte:
                        pte.HoursWorked = (int)nudHours.Value;
                        pte.HourlyRate  = (double)nudHourlyRate.Value;
                        break;

                    case Contractor c:
                        c.ContractAmount = (double)nudContractAmount.Value;
                        c.ContractType   = string.IsNullOrWhiteSpace(txtContractType.Text)
                                           ? "غير محدد" : txtContractType.Text;
                        break;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "❌  " + ex.Message;
            }
        }

        // ── مساعدات بناء الواجهة ─────────────────────────────────
        private void AddLabel(Panel parent, string text, int top)
        {
            parent.Controls.Add(new Label { Text = text, Top = top + 3, Left = 0, Width = 140 });
        }

        private TextBox AddTextBox(Panel parent, int top)
        {
            var tb = new TextBox { Top = top, Left = 145, Width = 250 };
            parent.Controls.Add(tb);
            return tb;
        }

        private void AddFieldLabel(Panel p, string text, int top)
        {
            p.Controls.Add(new Label { Text = text, Top = top + 3, Left = 0, Width = 155 });
        }

        private NumericUpDown AddNumeric(Panel p, int top, decimal min, decimal max, decimal val, int dec)
        {
            var n = new NumericUpDown
            {
                Top           = top,
                Left          = 160,
                Width         = 225,
                Minimum       = min,
                Maximum       = max,
                Value         = val,
                DecimalPlaces = dec
            };
            p.Controls.Add(n);
            return n;
        }
    }
}
