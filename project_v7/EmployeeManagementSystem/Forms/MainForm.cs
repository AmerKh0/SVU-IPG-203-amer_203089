using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Forms
{
    public class MainForm : Form
    {
        private Department _department;

        private DataGridView dgvEmployees   = null!;
        private Label        lblTotalCount  = null!;
        private Label        lblTotalSalary = null!;
        private RichTextBox  rtbAlerts      = null!;
        private Button       btnAdd         = null!;
        private Button       btnEdit        = null!;
        private Button       btnRemove      = null!;
        private Button       btnAdjust      = null!;
        private Button       btnCalc        = null!;
        private Button       btnPolyDemo    = null!;
        private Button       btnAddSale     = null!;
        private Button       btnViewSales   = null!;
        private Button       btnAbsence     = null!;
        private Button       btnExport      = null!;

        public MainForm()
        {
            _department = new Department("الإدارة العامة");
            _department.OnHighSalaryDetected += HandleHighSalaryAlert;

            InitializeComponent();

            // تحميل البيانات المحفوظة (أو بدء فارغ)
            foreach (var emp in DataService.Load())
                _department.AddEmployee(emp);

            RefreshGrid();
        }

        // ============================================================
        private void InitializeComponent()
        {
            this.Text        = "نظام إدارة الموظفين — SVU TIC IPG 203";
            this.Size        = new Size(1100, 720);
            this.MinimumSize = new Size(950, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor   = Color.FromArgb(245, 247, 250);
            this.Font        = new Font("Tahoma", 9f);

            // ── SIDEBAR ──────────────────────────────────────────────
            var sidebar = new Panel { Width = 210, Dock = DockStyle.Left, BackColor = Color.FromArgb(15, 23, 42) };

            var pnlLogo = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(30, 64, 175) };
            pnlLogo.Controls.Add(new Label { Text = "👔 HR System", ForeColor = Color.White, Font = new Font("Tahoma", 14f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });

            // Stats panel — مثبّت في الأسفل دائماً بغض النظر عن حجم النافذة
            var pnlStats = new Panel { Dock = DockStyle.Bottom, Height = 140, BackColor = Color.FromArgb(23, 37, 84), Padding = new Padding(10) };
            pnlStats.Controls.Add(lblTotalSalary = new Label { Text = "---",            ForeColor = Color.FromArgb(250, 204, 21), Font = new Font("Tahoma", 13f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 35 });
            pnlStats.Controls.Add(new Label { Text = "إجمالي الرواتب ($)", ForeColor = Color.FromArgb(148, 163, 184), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 22 });
            pnlStats.Controls.Add(lblTotalCount = new Label { Text = "0",              ForeColor = Color.White, Font = new Font("Tahoma", 24f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 45 });
            pnlStats.Controls.Add(new Label { Text = "إجمالي الموظفين",   ForeColor = Color.FromArgb(148, 163, 184), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 22 });

            // Buttons area — يملأ المساحة ويدعم التمرير تلقائياً
            var pnlButtonsWrap = new Panel { Dock = DockStyle.Fill };
            var pnlButtons = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding       = new Padding(15, 15, 5, 10),
                WrapContents  = false,
                AutoScroll    = true
            };

            btnAdd      = MakeBtn("➕  إضافة موظف",        Color.FromArgb(34, 197, 94));
            btnEdit     = MakeBtn("✏️   تعديل الموظف",      Color.FromArgb(59, 130, 246));
            btnRemove   = MakeBtn("🗑   حذف الموظف",        Color.FromArgb(239, 68, 68));
            btnAdjust   = MakeBtn("💸  مكافأة / خصم",      Color.FromArgb(249, 115, 22));
            btnCalc     = MakeBtn("💰  احتساب الرواتب",     Color.FromArgb(234, 179, 8));
            btnPolyDemo = MakeBtn("📄  الرواتب التفصيلية",  Color.FromArgb(139, 92, 246));

            // فاصل مرئي
            var sep = new Panel { Width = 178, Height = 1, BackColor = Color.FromArgb(51, 65, 85) };

            btnAddSale   = MakeBtn("📝  تسجيل بيعة",       Color.FromArgb(6, 148, 162));
            btnViewSales = MakeBtn("📋  سجل البيعات",       Color.FromArgb(8, 112, 184));

            // فاصل ثانٍ
            var sep2 = new Panel { Width = 178, Height = 1, BackColor = Color.FromArgb(51, 65, 85) };

            btnAbsence = MakeBtn("⏱   خصم الغياب",         Color.FromArgb(185, 28, 28));

            // فاصل ثالث
            var sep3 = new Panel { Width = 178, Height = 1, BackColor = Color.FromArgb(51, 65, 85) };

            btnExport  = MakeBtn("📊  تصدير Excel",         Color.FromArgb(21, 128, 61));

            pnlButtons.Controls.AddRange(new Control[] {
                btnAdd, btnEdit, btnRemove, btnAdjust, btnCalc, btnPolyDemo,
                sep, btnAddSale, btnViewSales,
                sep2, btnAbsence,
                sep3, btnExport
            });

            pnlButtonsWrap.Controls.Add(pnlButtons);

            sidebar.Controls.Add(pnlButtonsWrap);
            sidebar.Controls.Add(pnlStats);
            sidebar.Controls.Add(pnlLogo);

            // ── MAIN CONTENT ─────────────────────────────────────────
            var mainContent = new Panel { Dock = DockStyle.Fill };

            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White, Padding = new Padding(15, 0, 15, 0) };
            pnlHeader.Controls.Add(new Label { Text = "قائمة الموظفين", Font = new Font("Tahoma", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 64, 175), TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill });

            // DataGridView
            dgvEmployees = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.FromArgb(229, 231, 235),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                RowTemplate = { Height = 36 }
            };
            dgvEmployees.EnableHeadersVisualStyles = false;
            dgvEmployees.ColumnHeadersHeight = 40;
            dgvEmployees.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 175);
            dgvEmployees.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEmployees.ColumnHeadersDefaultCellStyle.Font      = new Font("Tahoma", 9f, FontStyle.Bold);
            dgvEmployees.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvEmployees.DefaultCellStyle.Alignment             = DataGridViewContentAlignment.MiddleCenter;
            dgvEmployees.DefaultCellStyle.Font                  = new Font("Tahoma", 9f);
            dgvEmployees.DefaultCellStyle.SelectionBackColor    = Color.FromArgb(219, 234, 254);
            dgvEmployees.DefaultCellStyle.SelectionForeColor    = Color.Black;
            dgvEmployees.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);

            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId",       HeaderText = "الرقم",          FillWeight = 70  });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName",     HeaderText = "الاسم",          FillWeight = 130 });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDept",     HeaderText = "القسم",          FillWeight = 100 });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colType",     HeaderText = "النوع",          FillWeight = 95  });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBase",     HeaderText = "الأساسي ($)",    FillWeight = 80  });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBonus",    HeaderText = "مكافأة ($)",     FillWeight = 70  });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDed",      HeaderText = "خصم ($)",        FillWeight = 65  });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFinal",    HeaderText = "الصافي ($)",     FillWeight = 85  });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSales",    HeaderText = "بيعات",          FillWeight = 50  });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate",     HeaderText = "تاريخ التوظيف", FillWeight = 90  });

            // Alerts
            var pnlAlerts = new Panel { Dock = DockStyle.Bottom, Height = 130, BackColor = Color.White, Padding = new Padding(10, 5, 10, 8) };
            var pnlAH = new Panel { Dock = DockStyle.Top, Height = 30 };
            pnlAH.Controls.Add(new Label { Text = "📢  سجل التنبيهات (Delegates & Events)", Font = new Font("Tahoma", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 64, 175), TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill });
            var btnClear = new Button { Text = "مسح", Dock = DockStyle.Right, Width = 55, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(241, 245, 249), ForeColor = Color.Gray, Cursor = Cursors.Hand };
            btnClear.FlatAppearance.BorderColor = Color.LightGray;
            btnClear.Click += (s, e) => rtbAlerts.Clear();
            pnlAH.Controls.Add(btnClear);
            rtbAlerts = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.FromArgb(254, 252, 232), Font = new Font("Tahoma", 8.5f), BorderStyle = BorderStyle.FixedSingle, ScrollBars = RichTextBoxScrollBars.Vertical };
            pnlAlerts.Controls.Add(rtbAlerts);
            pnlAlerts.Controls.Add(pnlAH);

            mainContent.Controls.Add(dgvEmployees);
            mainContent.Controls.Add(pnlHeader);
            mainContent.Controls.Add(pnlAlerts);
            mainContent.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.FromArgb(226, 232, 240) });

            this.Controls.Add(mainContent);
            this.Controls.Add(sidebar);

            btnAdd.Click      += BtnAdd_Click;
            btnEdit.Click     += BtnEdit_Click;
            btnRemove.Click   += BtnRemove_Click;
            btnAdjust.Click   += BtnAdjust_Click;
            btnCalc.Click     += BtnCalc_Click;
            btnPolyDemo.Click += BtnPolyDemo_Click;
            btnAddSale.Click  += BtnAddSale_Click;
            btnViewSales.Click += BtnViewSales_Click;
            btnAbsence.Click  += BtnAbsence_Click;
            btnExport.Click   += BtnExport_Click;
        }

        private Button MakeBtn(string text, Color color)
        {
            var b = new Button { Text = text, Width = 178, Height = 42, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 9f, FontStyle.Bold), Cursor = Cursors.Hand, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        // ── Event handler ────────────────────────────────────────
        private void HandleHighSalaryAlert(string name, double salary)
        {
            rtbAlerts.SelectionColor = Color.FromArgb(185, 28, 28);
            rtbAlerts.SelectionFont  = new Font("Tahoma", 8.5f, FontStyle.Bold);
            rtbAlerts.AppendText($"[{DateTime.Now:HH:mm:ss}]  ⚠️  {name} — الراتب الصافي: {salary:F2}$ تجاوز الحد ({_department.SalaryThreshold:F0}$)\n");
            rtbAlerts.ScrollToCaret();
        }

        // ── Buttons ───────────────────────────────────────────────
        private void BtnAdd_Click(object? s, EventArgs e)
        {
            using var form = new AddEmployeeForm();
            if (form.ShowDialog(this) == DialogResult.OK && form.CreatedEmployee != null)
            {
                _department.AddEmployee(form.CreatedEmployee);
                DataService.Save(_department.Employees); // حفظ تلقائي
                RefreshGrid();
            }
        }

        private void BtnEdit_Click(object? s, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            { MessageBox.Show("الرجاء تحديد موظف أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string id  = dgvEmployees.SelectedRows[0].Cells["colId"].Value?.ToString() ?? "";
            var    emp = _department.Employees.FirstOrDefault(x => x.Id == id);
            if (emp == null) return;

            using var form = new EditEmployeeForm(emp);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                DataService.Save(_department.Employees);
                RefreshGrid();

                rtbAlerts.SelectionColor = Color.FromArgb(59, 130, 246);
                rtbAlerts.SelectionFont  = new Font("Tahoma", 8.5f, FontStyle.Bold);
                rtbAlerts.AppendText($"[{DateTime.Now:HH:mm:ss}]  ✏️  تم تعديل بيانات الموظف: {emp.Name} (ID: {emp.Id})\n");
                rtbAlerts.ScrollToCaret();
            }
        }

        private void BtnRemove_Click(object? s, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            { MessageBox.Show("الرجاء تحديد موظف.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string id   = dgvEmployees.SelectedRows[0].Cells["colId"].Value?.ToString() ?? "";
            string name = dgvEmployees.SelectedRows[0].Cells["colName"].Value?.ToString() ?? "";

            if (MessageBox.Show($"هل تريد حذف \"{name}\"؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _department.RemoveEmployee(id);
                DataService.Save(_department.Employees); // حفظ تلقائي
                RefreshGrid();
            }
        }

        private void BtnAdjust_Click(object? s, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            { MessageBox.Show("الرجاء تحديد موظف أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string id  = dgvEmployees.SelectedRows[0].Cells["colId"].Value?.ToString() ?? "";
            var    emp = _department.Employees.FirstOrDefault(x => x.Id == id);
            if (emp == null) return;

            using var form = new AdjustSalaryForm(emp);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                DataService.Save(_department.Employees); // حفظ تلقائي
                RefreshGrid();
            }
        }

        private void BtnCalc_Click(object? s, EventArgs e)
        {
            rtbAlerts.Clear();
            double total = _department.GetTotalSalaries(); // يُطلق الأحداث
            lblTotalSalary.Text = $"{total:F2}";
            MessageBox.Show(
                $"✅  إجمالي الرواتب الصافية:\n\n   {total:F2} $\n\n" +
                $"إجمالي الموظفين المُنشئين (Static): {Employee.TotalCreated}\n\n" +
                $"راجع سجل التنبيهات للرواتب المرتفعة.",
                "احتساب الرواتب", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPolyDemo_Click(object? s, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("══════════════════════════════════════════════");
            sb.AppendLine("         الرواتب التفصيلية لجميع الموظفين      ");
            sb.AppendLine("══════════════════════════════════════════════\n");

            if (_department.Employees.Count == 0)
            {
                MessageBox.Show("لا يوجد موظفون حالياً.", "الرواتب التفصيلية",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var emp in _department.Employees)
            {
                string kind = emp switch
                {
                    FullTimeEmployee fte => $"دوام كامل  ({fte.BaseSalary:F0}$ + علاوة {fte.Bonus:F0}$)",
                    PartTimeEmployee pte => $"دوام جزئي  ({pte.HoursWorked} ساعة × {pte.HourlyRate:F0}$)",
                    Contractor c         => $"متعاقد  ({c.ContractType})",
                    _                    => "موظف"
                };
                sb.AppendLine($"▸  {emp.Name}  [{emp.Id}]");
                sb.AppendLine($"   النوع        : {kind}");
                sb.AppendLine($"   القسم        : {emp.Department}");
                sb.AppendLine($"   الراتب الأساسي: {emp.CalculateSalary():F2} $");
                sb.AppendLine($"   المكافأة      : +{emp.CompanyBonus:F2} $");
                sb.AppendLine($"   الخصم         : -{emp.Deduction:F2} $");
                sb.AppendLine($"   الصافي النهائي: {emp.GetFinalSalary():F2} $");
                sb.AppendLine($"   البيعات       : {emp.Sales.Count} بيعة");
                sb.AppendLine(new string('─', 46));
            }

            double total = _department.Employees.Sum(e => e.GetFinalSalary());
            sb.AppendLine($"\n   إجمالي الرواتب الصافية: {total:F2} $");
            sb.AppendLine($"   إجمالي الموظفين: {_department.Employees.Count}");

            MessageBox.Show(sb.ToString(), "الرواتب التفصيلية", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAbsence_Click(object? s, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            { MessageBox.Show("الرجاء تحديد موظف أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string id  = dgvEmployees.SelectedRows[0].Cells["colId"].Value?.ToString() ?? "";
            var    emp = _department.Employees.FirstOrDefault(x => x.Id == id);
            if (emp == null) return;

            using var form = new AbsenceDeductionForm(emp);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                DataService.Save(_department.Employees);
                RefreshGrid();

                // تنبيه في سجل التنبيهات
                if (emp.Deduction > 0)
                {
                    rtbAlerts.SelectionColor = Color.FromArgb(185, 28, 28);
                    rtbAlerts.SelectionFont  = new Font("Tahoma", 8.5f, FontStyle.Bold);
                    rtbAlerts.AppendText(
                        $"[{DateTime.Now:HH:mm:ss}]  ⏱  {emp.Name} — تم تسجيل خصم غياب، إجمالي الخصم: {emp.Deduction:F2}$\n");
                    rtbAlerts.ScrollToCaret();
                }
            }
        }

        private void BtnAddSale_Click(object? s, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            { MessageBox.Show("الرجاء تحديد موظف أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string id  = dgvEmployees.SelectedRows[0].Cells["colId"].Value?.ToString() ?? "";
            var    emp = _department.Employees.FirstOrDefault(x => x.Id == id);
            if (emp == null) return;

            using var form = new AddSaleForm(emp);
            if (form.ShowDialog(this) == DialogResult.OK && form.CreatedSale != null)
            {
                emp.AddSale(form.CreatedSale);
                DataService.Save(_department.Employees);
                RefreshGrid();

                // تنبيه في سجل التنبيهات
                double bonus = form.CreatedSale.Bonus;
                rtbAlerts.SelectionColor = Color.FromArgb(6, 95, 70);
                rtbAlerts.SelectionFont  = new Font("Tahoma", 8.5f, FontStyle.Bold);
                string bonusMsg = bonus > 0 ? $"  (+{bonus:F2}$ مكافأة أُضيفت للراتب)" : "";
                rtbAlerts.AppendText(
                    $"[{DateTime.Now:HH:mm:ss}]  💼  {emp.Name} — بيعة جديدة بقيمة {form.CreatedSale.Amount:F2}$ للمشتري \"{form.CreatedSale.BuyerName}\"{bonusMsg}\n");
                rtbAlerts.ScrollToCaret();
            }
        }

        private void BtnViewSales_Click(object? s, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            { MessageBox.Show("الرجاء تحديد موظف أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string id  = dgvEmployees.SelectedRows[0].Cells["colId"].Value?.ToString() ?? "";
            var    emp = _department.Employees.FirstOrDefault(x => x.Id == id);
            if (emp == null) return;

            // نمرّر callback: يُحفظ JSON ويُجدَّد جدول الموظفين بعد كل حذف أو تعديل
            using var hist = new SalesHistoryForm(emp, onChanged: () =>
            {
                DataService.Save(_department.Employees);
                RefreshGrid();

                // تنبيه في سجل الأحداث
                rtbAlerts.SelectionColor = Color.FromArgb(8, 112, 184);
                rtbAlerts.SelectionFont  = new Font("Tahoma", 8.5f, FontStyle.Bold);
                rtbAlerts.AppendText(
                    $"[{DateTime.Now:HH:mm:ss}]  ✏️  {emp.Name} — تم تعديل/حذف بيعة (مكافأة: {emp.CompanyBonus:F2}$)\\n");
                rtbAlerts.ScrollToCaret();
            });
            hist.ShowDialog(this);
        }

        private void BtnExport_Click(object? s, EventArgs e)
        {
            if (_department.Employees.Count == 0)
            {
                MessageBox.Show("لا يوجد موظفون لتصديرهم.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Title            = "تصدير بيانات الموظفين إلى Excel",
                Filter           = "ملف Excel (*.xlsx)|*.xlsx",
                FileName         = $"كشف_الرواتب_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Data.ExcelExportService.Export(_department.Employees, dlg.FileName);
                MessageBox.Show(
                    $"✅  تم تصدير البيانات بنجاح!\n\nالملف: {dlg.FileName}",
                    "تم التصدير", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // فتح الملف مباشرة
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName  = dlg.FileName,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء التصدير:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Grid Refresh ──────────────────────────────────────────
        private void RefreshGrid()
        {
            dgvEmployees.Rows.Clear();

            foreach (var emp in _department.Employees)
            {
                string typeLabel = emp switch
                {
                    FullTimeEmployee fte => $"كامل ({fte.BaseSalary:F0}$+{fte.Bonus:F0}$)",
                    PartTimeEmployee pte => $"جزئي ({pte.HoursWorked}س×{pte.HourlyRate:F0}$)",
                    Contractor c         => $"متعاقد-{c.ContractType}",
                    _                    => "—"
                };

                double baseSal  = emp.CalculateSalary();
                double final    = emp.GetFinalSalary();

                int row = dgvEmployees.Rows.Add(
                    emp.Id, emp.Name, emp.Department, typeLabel,
                    $"{baseSal:F2}",
                    emp.CompanyBonus > 0 ? $"+{emp.CompanyBonus:F2}" : "—",
                    emp.Deduction    > 0 ? $"-{emp.Deduction:F2}"    : "—",
                    $"{final:F2}",
                    emp.Sales.Count > 0 ? $"🛒 {emp.Sales.Count}" : "—",
                    emp.HireDate.ToString("dd/MM/yyyy")
                );

                // تلوين الراتب النهائي المرتفع
                if (final > _department.SalaryThreshold)
                    dgvEmployees.Rows[row].Cells["colFinal"].Style.BackColor = Color.FromArgb(254, 249, 195);

                // تلوين عمود المكافأة أخضر والخصم أحمر
                if (emp.CompanyBonus > 0)
                    dgvEmployees.Rows[row].Cells["colBonus"].Style.ForeColor = Color.FromArgb(22, 101, 52);
                if (emp.Deduction > 0)
                    dgvEmployees.Rows[row].Cells["colDed"].Style.ForeColor = Color.FromArgb(185, 28, 28);
            }

            lblTotalCount.Text  = _department.Employees.Count.ToString();
            lblTotalSalary.Text = "---";
        }
    }
}
