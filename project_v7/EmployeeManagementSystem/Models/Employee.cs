using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.Models
{
    public abstract class Employee : IWorker
    {
        private string _id;
        private string _name;
        private string _department;

        private static int _totalCreated = 0;
        public static int TotalCreated => _totalCreated;

        public string   Id       => _id;
        public DateTime HireDate { get; private set; }

        /// <summary>تعديل الرقم التعريفي للموظف</summary>
        public void SetId(string newId)
        {
            if (string.IsNullOrWhiteSpace(newId) || newId.Length < 3)
                throw new ArgumentException("الرقم التعريفي يجب أن يكون 3 أحرف على الأقل.");
            _id = newId.Trim().ToUpper();
        }

        /// <summary>تعديل تاريخ التوظيف</summary>
        public void SetHireDate(DateTime date)
        {
            if (date > DateTime.Now)
                throw new ArgumentException("تاريخ التوظيف لا يمكن أن يكون في المستقبل.");
            HireDate = date;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (!Validator.IsValidName(value))
                    throw new ArgumentException("الاسم يجب أن يكون بين 2 و 50 حرفاً.");
                _name = value;
            }
        }

        public string Department
        {
            get => _department;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("اسم القسم لا يمكن أن يكون فارغاً.");
                _department = value;
            }
        }

        // مكافأة الشركة والخصم — يُحفظان في ملف JSON
        public double CompanyBonus { get; set; } = 0;
        public double Deduction    { get; set; } = 0;

        // بيانات الدوام لحساب خصم الغياب
        public int WorkDays  { get; set; } = 22;   // أيام العمل الشهرية
        public int DailyHours { get; set; } = 8;   // ساعات العمل اليومية

        // سجل البيعات
        public List<SaleRecord> Sales { get; set; } = new List<SaleRecord>();

        /// <summary>
        /// تضيف بيعة جديدة وتضيف مكافأتها (إن وُجدت) إلى CompanyBonus
        /// </summary>
        public void AddSale(SaleRecord sale)
        {
            Sales.Add(sale);
            if (sale.Bonus > 0)
                CompanyBonus += sale.Bonus;
        }

        /// <summary>
        /// تحذف بيعة بواسطة معرّفها وتطرح مكافأتها من CompanyBonus إن وُجدت
        /// </summary>
        public bool RemoveSale(string saleId)
        {
            var sale = Sales.FirstOrDefault(s => s.Id == saleId);
            if (sale == null) return false;

            Sales.Remove(sale);

            // طرح المكافأة من راتب الموظف عند حذف البيعة
            if (sale.Bonus > 0)
                CompanyBonus = Math.Max(0, CompanyBonus - sale.Bonus);

            return true;
        }

        /// <summary>
        /// تعدّل بيعة موجودة وتُعدّل CompanyBonus بناءً على الفرق في المكافأة
        /// </summary>
        public bool UpdateSale(string saleId, SaleRecord updated)
        {
            var existing = Sales.FirstOrDefault(s => s.Id == saleId);
            if (existing == null) return false;

            // تعديل المكافأة: نطرح القديمة ونضيف الجديدة
            if (existing.Bonus > 0)
                CompanyBonus = Math.Max(0, CompanyBonus - existing.Bonus);
            if (updated.Bonus > 0)
                CompanyBonus += updated.Bonus;

            // تحديث بيانات البيعة (مع الإبقاء على نفس المعرّف والتاريخ الأصلي)
            existing.BuyerName  = updated.BuyerName;
            existing.BuyerPhone = updated.BuyerPhone;
            existing.Amount     = updated.Amount;
            existing.Bonus      = updated.Bonus;

            return true;
        }

        // ── Constructor: موظف جديد ────────────────────────────────
        protected Employee(string name, string department)
        {
            _id        = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            Name       = name;
            Department = department;
            HireDate   = DateTime.Now;
            _totalCreated++;
        }

        // ── Constructor: استعادة من ملف محفوظ ─────────────────────
        protected Employee(string existingId, string name, string department, DateTime hireDate)
        {
            _id        = existingId;
            Name       = name;
            Department = department;
            HireDate   = hireDate;
            _totalCreated++;
        }

        public abstract double CalculateSalary();

        /// <summary>الراتب النهائي = الأساسي + مكافأة الشركة - الخصم</summary>
        public double GetFinalSalary() => CalculateSalary() + CompanyBonus - Deduction;

        public virtual string GetInfo()
            => $"[{Id}] {Name} | {Department} | {HireDate:dd/MM/yyyy}";

        public virtual void PerformTask() { }
    }
}
