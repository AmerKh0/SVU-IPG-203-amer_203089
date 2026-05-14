using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Forms
{
    /// <summary>
    /// نافذة عرض سجل بيعات الموظف مع إمكانية التعديل والحذف
    /// </summary>
    public class SalesHistoryForm : Form
    {
        private readonly Employee _employee;
        private readonly Action?  _onChanged;

        private DataGridView dgvSales = null!;
        private Label        lblTotal = null!;
        private Label        lblBonus = null!;

        public SalesHistoryForm(Employee employee, Action? onChanged = null)
        {
            _employee  = employee;
            _onChanged = onChanged;
            InitializeComponent();
            LoadSales();
        }

        private void InitializeComponent()
        {
            this.Text            = $"سجل البيعات — {_employee.Name}";
            this.Size            = new Size(900, 480);
            this.MinimumSize     = new Size(900, 480);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Color.FromArgb(245, 247, 250);
            this.Font            = new Font("Tahoma", 9f);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // ── HEADER ──────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 55,
                BackColor = Color.FromArgb(30, 64, 175)
            };
            pnlHeader.Controls.Add(new Label
            {
                Text      = $"📋  سجل البيعات — {_employee.Name}  [{_employee.Id}]",
                ForeColor = Color.White,
                Font      = new Font("Tahoma", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Fill
            });

            // ── GRID ─────────────────────────────────────────────────
            dgvSales = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                BackgroundColor       = Color.White,
                BorderStyle           = BorderStyle.None,
                RowHeadersVisible     = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                ReadOnly              = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor             = Color.FromArgb(229, 231, 235),
                CellBorderStyle       = DataGridViewCellBorderStyle.SingleHorizontal,
                RowTemplate           = { Height = 36 }
            };

            dgvSales.EnableHeadersVisualStyles                 = false;
            dgvSales.ColumnHeadersHeight                       = 40;
            dgvSales.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(30, 64, 175);
            dgvSales.ColumnHeadersDefaultCellStyle.ForeColor   = Color.White;
            dgvSales.ColumnHeadersDefaultCellStyle.Font        = new Font("Tahoma", 9f, FontStyle.Bold);
            dgvSales.ColumnHeadersDefaultCellStyle.Alignment   = DataGridViewContentAlignment.MiddleCenter;
            dgvSales.DefaultCellStyle.Alignment                = DataGridViewContentAlignment.MiddleCenter;
            dgvSales.DefaultCellStyle.SelectionBackColor       = Color.FromArgb(219, 234, 254);
            dgvSales.DefaultCellStyle.SelectionForeColor       = Color.Black;
            dgvSales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);

            // عمود مخفي لمعرّف البيعة
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSaleId", Visible = false });

            // أعمدة النص — قراءة فقط
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNum",   HeaderText = "#",               FillWeight = 30,  ReadOnly = true });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate",  HeaderText = "التاريخ",         FillWeight = 95,  ReadOnly = true });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBuyer", HeaderText = "اسم المشتري",     FillWeight = 140, ReadOnly = true });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPhone", HeaderText = "رقم الهاتف",      FillWeight = 110, ReadOnly = true });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAmt",   HeaderText = "مبلغ البيعة ($)", FillWeight = 90,  ReadOnly = true });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBonus", HeaderText = "مكافأة ($)",      FillWeight = 80,  ReadOnly = true });

            // عمود زر التعديل
            var editCol = new DataGridViewButtonColumn
            {
                Name                        = "colEdit",
                HeaderText                  = "",
                Text                        = "✏️ تعديل",
                UseColumnTextForButtonValue = true,
                FillWeight                  = 65,
                FlatStyle                   = FlatStyle.Flat
            };
            editCol.DefaultCellStyle.BackColor = Color.FromArgb(219, 234, 254);
            editCol.DefaultCellStyle.ForeColor = Color.FromArgb(30, 64, 175);
            editCol.DefaultCellStyle.Font      = new Font("Tahoma", 8.5f, FontStyle.Bold);
            editCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSales.Columns.Add(editCol);

            // عمود زر الحذف
            var deleteCol = new DataGridViewButtonColumn
            {
                Name                        = "colDelete",
                HeaderText                  = "إجراء",
                Text                        = "🗑 حذف",
                UseColumnTextForButtonValue = true,
                FillWeight                  = 65,
                FlatStyle                   = FlatStyle.Flat
            };
            deleteCol.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
            deleteCol.DefaultCellStyle.ForeColor = Color.FromArgb(185, 28, 28);
            deleteCol.DefaultCellStyle.Font      = new Font("Tahoma", 8.5f, FontStyle.Bold);
            deleteCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSales.Columns.Add(deleteCol);

            dgvSales.CellContentClick += DgvSales_CellContentClick;

            // ── FOOTER STATS ──────────────────────────────────────────
            var pnlFooter = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 55,
                BackColor = Color.FromArgb(23, 37, 84),
                Padding   = new Padding(20, 0, 20, 0)
            };

            lblTotal = new Label
            {
                Text      = "إجمالي المبيعات: —",
                ForeColor = Color.White,
                Font      = new Font("Tahoma", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock      = DockStyle.Left,
                Width     = 310
            };
            lblBonus = new Label
            {
                Text      = "إجمالي المكافآت: —",
                ForeColor = Color.FromArgb(250, 204, 21),
                Font      = new Font("Tahoma", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Dock      = DockStyle.Right,
                Width     = 280
            };

            pnlFooter.Controls.Add(lblTotal);
            pnlFooter.Controls.Add(lblBonus);

            this.Controls.Add(dgvSales);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFooter);
        }

        // ── منطق التعديل والحذف ───────────────────────────────────
        private void DgvSales_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string saleId    = dgvSales.Rows[e.RowIndex].Cells["colSaleId"].Value?.ToString() ?? "";
            string buyerName = dgvSales.Rows[e.RowIndex].Cells["colBuyer"].Value?.ToString() ?? "";
            string amount    = dgvSales.Rows[e.RowIndex].Cells["colAmt"].Value?.ToString() ?? "";
            string bonus     = dgvSales.Rows[e.RowIndex].Cells["colBonus"].Value?.ToString() ?? "";

            int editIdx   = dgvSales.Columns["colEdit"].Index;
            int deleteIdx = dgvSales.Columns["colDelete"].Index;

            // ── تعديل ───────────────────────────────────────────────
            if (e.ColumnIndex == editIdx)
            {
                var sale = _employee.Sales.FirstOrDefault(s => s.Id == saleId);
                if (sale == null) return;

                using var editForm = new AddSaleForm(_employee, sale);
                if (editForm.ShowDialog(this) == DialogResult.OK && editForm.CreatedSale != null)
                {
                    bool updated = _employee.UpdateSale(saleId, editForm.CreatedSale);
                    if (!updated) return;

                    _onChanged?.Invoke();
                    LoadSales();
                    MessageBox.Show("تم تعديل البيعة بنجاح.", "تم التعديل",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            // ── حذف ─────────────────────────────────────────────────
            if (e.ColumnIndex == deleteIdx)
            {
                string bonusWarn = (bonus != "—")
                    ? $"\n\n⚠  ستُطرح المكافأة {bonus}$ من راتب الموظف."
                    : "";

                var result = MessageBox.Show(
                    $"هل تريد حذف هذه البيعة؟\n\n" +
                    $"المشتري : {buyerName}\n" +
                    $"المبلغ  : {amount} $" +
                    bonusWarn,
                    "تأكيد حذف البيعة",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result != DialogResult.Yes) return;

                bool removed = _employee.RemoveSale(saleId);
                if (!removed) return;

                _onChanged?.Invoke();
                LoadSales();

                string successMsg = "تم حذف البيعة بنجاح.";
                if (bonus != "—") successMsg += $"\nتم طرح المكافأة {bonus}$ من راتب الموظف.";
                MessageBox.Show(successMsg, "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ── تحميل البيانات في الجدول ─────────────────────────────
        private void LoadSales()
        {
            dgvSales.Rows.Clear();

            if (_employee.Sales.Count == 0)
            {
                lblTotal.Text = "لا توجد مبيعات مسجّلة.";
                lblBonus.Text = "";
                return;
            }

            int i = 1;
            foreach (var s in _employee.Sales.OrderByDescending(x => x.Date))
            {
                int row = dgvSales.Rows.Add(
                    s.Id,
                    i++,
                    s.Date.ToString("dd/MM/yyyy  HH:mm"),
                    s.BuyerName,
                    s.BuyerPhone,
                    $"{s.Amount:F2}",
                    s.Bonus > 0 ? $"+{s.Bonus:F2}" : "—"
                );

                if (s.Bonus > 0)
                    dgvSales.Rows[row].Cells["colBonus"].Style.ForeColor = Color.FromArgb(22, 101, 52);
            }

            double totalAmt   = _employee.Sales.Sum(x => x.Amount);
            double totalBonus = _employee.Sales.Sum(x => x.Bonus);

            lblTotal.Text = $"إجمالي المبيعات: {totalAmt:F2} $  ({_employee.Sales.Count} بيعة)";
            lblBonus.Text = $"إجمالي المكافآت: +{totalBonus:F2} $";
        }
    }
}
