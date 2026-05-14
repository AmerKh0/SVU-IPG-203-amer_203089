using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Forms
{
    /// <summary>
    /// نموذج تسجيل بيعة جديدة أو تعديل بيعة موجودة
    /// </summary>
    public class AddSaleForm : Form
    {
        private readonly Employee    _employee;
        private readonly SaleRecord? _existingSale;   // غير null في وضع التعديل
        private readonly bool        _isEditing;

        public SaleRecord? CreatedSale { get; private set; }

        private TextBox txtBuyerName  = null!;
        private TextBox txtBuyerPhone = null!;
        private TextBox txtAmount     = null!;
        private TextBox txtBonus      = null!;
        private Label   lblBonusInfo  = null!;
        private Button  btnOk         = null!;
        private Button  btnCancel     = null!;

        // وضع الإضافة
        public AddSaleForm(Employee employee)
        {
            _employee   = employee;
            _isEditing  = false;
            InitializeComponent();
        }

        // وضع التعديل
        public AddSaleForm(Employee employee, SaleRecord existingSale)
        {
            _employee     = employee;
            _existingSale = existingSale;
            _isEditing    = true;
            InitializeComponent();
            PopulateFromExisting();
        }

        private void PopulateFromExisting()
        {
            txtBuyerName.Text  = _existingSale!.BuyerName;
            txtBuyerPhone.Text = _existingSale!.BuyerPhone;
            txtAmount.Text     = _existingSale!.Amount.ToString("F2");
            txtBonus.Text      = _existingSale!.Bonus > 0
                                    ? _existingSale!.Bonus.ToString("F2")
                                    : "";
        }

        private void InitializeComponent()
        {
            string title = _isEditing
                ? $"✏️  تعديل البيعة — {_employee.Name}"
                : $"تسجيل بيعة — {_employee.Name}";

            this.Text            = title;
            this.Size            = new Size(420, 420);
            this.MinimumSize     = new Size(420, 420);
            this.MaximumSize     = new Size(420, 420);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Color.FromArgb(245, 247, 250);
            this.Font            = new Font("Tahoma", 9f);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // ── HEADER ──────────────────────────────────────────────
            Color headerColor = _isEditing
                ? Color.FromArgb(8, 112, 184)   // أزرق للتعديل
                : Color.FromArgb(30, 64, 175);   // أزرق داكن للإضافة

            string headerText = _isEditing
                ? "✏️  تعديل بيانات البيعة"
                : "💼  تسجيل بيعة جديدة";

            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = headerColor
            };
            pnlHeader.Controls.Add(new Label
            {
                Text      = headerText,
                ForeColor = Color.White,
                Font      = new Font("Tahoma", 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Fill
            });

            // ── FORM BODY ────────────────────────────────────────────
            var pnlBody = new Panel
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(25, 15, 25, 10)
            };

            var lblName   = MakeLabel("اسم المشتري *");
            txtBuyerName  = MakeTextBox("أدخل اسم المشتري");

            var lblPhone  = MakeLabel("رقم الهاتف *");
            txtBuyerPhone = MakeTextBox("مثال: 0599123456");
            txtBuyerPhone.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != '+')
                    e.Handled = true;
            };

            var lblAmount = MakeLabel("مبلغ البيعة ($) *");
            txtAmount     = MakeTextBox("0.00");
            txtAmount.KeyPress += NumericOnly;

            var lblBonus  = MakeLabel("مكافأة على البيعة ($)  — اختياري");
            txtBonus      = MakeTextBox("0.00  (اتركه فارغاً إذا لا توجد مكافأة)");
            txtBonus.KeyPress += NumericOnly;
            txtBonus.GotFocus += (s, e) => { if (txtBonus.Text.Contains("اتركه")) txtBonus.Clear(); };

            lblBonusInfo = new Label
            {
                Text      = _isEditing
                              ? "⚡ سيتم تحديث راتب الموظف بناءً على المكافأة الجديدة"
                              : "⚡ ستُضاف المكافأة تلقائياً إلى راتب الموظف",
                ForeColor = Color.FromArgb(22, 101, 52),
                Font      = new Font("Tahoma", 8f, FontStyle.Italic),
                AutoSize  = false,
                Size      = new Size(360, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ── OK / CANCEL ──────────────────────────────────────────
            var pnlButtons = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 50,
                FlowDirection = FlowDirection.RightToLeft,
                Padding       = new Padding(10, 8, 10, 8),
                BackColor     = Color.White
            };

            btnCancel = new Button
            {
                Text         = "إلغاء",
                Width        = 90,
                Height       = 34,
                FlatStyle    = FlatStyle.Flat,
                BackColor    = Color.FromArgb(241, 245, 249),
                ForeColor    = Color.Gray,
                Cursor       = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderColor = Color.LightGray;

            string okText = _isEditing ? "✔  حفظ التعديل" : "✔  حفظ البيعة";
            Color  okColor = _isEditing
                ? Color.FromArgb(8, 112, 184)
                : Color.FromArgb(22, 163, 74);

            btnOk = new Button
            {
                Text      = okText,
                Width     = 120,
                Height    = 34,
                FlatStyle = FlatStyle.Flat,
                BackColor = okColor,
                ForeColor = Color.White,
                Font      = new Font("Tahoma", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += BtnOk_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnCancel, btnOk });

            // ── LAYOUT inside pnlBody ────────────────────────────────
            int y = 10;
            void AddRow(Control lbl, Control input, int gap = 12)
            {
                lbl.Location   = new Point(0, y);
                input.Location = new Point(0, y + 22);
                pnlBody.Controls.Add(lbl);
                pnlBody.Controls.Add(input);
                y += 22 + input.Height + gap;
            }

            AddRow(lblName,   txtBuyerName);
            AddRow(lblPhone,  txtBuyerPhone);
            AddRow(lblAmount, txtAmount);
            AddRow(lblBonus,  txtBonus, 4);
            lblBonusInfo.Location = new Point(0, y);
            pnlBody.Controls.Add(lblBonusInfo);

            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        // ── Helpers ──────────────────────────────────────────────────
        private static Label MakeLabel(string text) => new Label
        {
            Text      = text,
            AutoSize  = false,
            Size      = new Size(365, 20),
            ForeColor = Color.FromArgb(51, 65, 85),
            Font      = new Font("Tahoma", 8.5f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        private static TextBox MakeTextBox(string placeholder) => new TextBox
        {
            Size            = new Size(365, 26),
            PlaceholderText = placeholder,
            BorderStyle     = BorderStyle.FixedSingle,
            Font            = new Font("Tahoma", 9f)
        };

        private static void NumericOnly(object? s, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)8)
                e.Handled = true;
        }

        // ── Validation & Save ─────────────────────────────────────────
        private void BtnOk_Click(object? s, EventArgs e)
        {
            string name  = txtBuyerName.Text.Trim();
            string phone = txtBuyerPhone.Text.Trim();
            string amtTx = txtAmount.Text.Trim();
            string bonTx = txtBonus.Text.Trim();

            if (string.IsNullOrEmpty(name))
            { MessageBox.Show("الرجاء إدخال اسم المشتري.", "تحقق", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtBuyerName.Focus(); return; }

            if (name.Length < 2)
            { MessageBox.Show("الاسم يجب أن يكون حرفين على الأقل.", "تحقق", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtBuyerName.Focus(); return; }

            if (string.IsNullOrEmpty(phone))
            { MessageBox.Show("الرجاء إدخال رقم هاتف المشتري.", "تحقق", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtBuyerPhone.Focus(); return; }

            if (!double.TryParse(amtTx, out double amount) || amount <= 0)
            { MessageBox.Show("الرجاء إدخال مبلغ البيعة بشكل صحيح.", "تحقق", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtAmount.Focus(); return; }

            double bonus = 0;
            if (!string.IsNullOrEmpty(bonTx) && !bonTx.Contains("اتركه"))
            {
                if (!double.TryParse(bonTx, out bonus) || bonus < 0)
                { MessageBox.Show("قيمة المكافأة غير صحيحة.", "تحقق", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtBonus.Focus(); return; }
            }

            CreatedSale  = new SaleRecord(name, phone, amount, bonus);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
