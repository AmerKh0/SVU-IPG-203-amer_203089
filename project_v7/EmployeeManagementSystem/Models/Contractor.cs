using System;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.Models
{
    public class Contractor : Employee
    {
        private double   _contractAmount;
        private string   _contractType;
        private DateTime _contractEndDate;

        public double   ContractAmount  { get => _contractAmount; set { if (!Validator.IsValidAmount(value)) throw new ArgumentException("قيمة العقد يجب أن تكون موجبة."); _contractAmount = value; } }
        public string   ContractType    { get => _contractType;   set => _contractType = string.IsNullOrWhiteSpace(value) ? "غير محدد" : value; }
        public DateTime ContractEndDate => _contractEndDate;   // قراءة فقط من الخارج
        public bool     IsContractActive => DateTime.Now <= _contractEndDate;
        public string   EmploymentType  => "متعاقد";

        // Constructor: متعاقد جديد
        public Contractor(string name, string department, double contractAmount,
                          string contractType, int durationMonths = 12)
            : base(name, department)
        {
            ContractAmount   = contractAmount;
            ContractType     = contractType;
            _contractEndDate = DateTime.Now.AddMonths(durationMonths);
        }

        // Constructor: استعادة من JSON (بتاريخ انتهاء العقد الأصلي)
        public Contractor(string id, string name, string department, DateTime hireDate,
                          double contractAmount, string contractType, DateTime contractEndDate)
            : base(id, name, department, hireDate)
        {
            ContractAmount   = contractAmount;
            ContractType     = contractType;
            _contractEndDate = contractEndDate;
        }

        public override double CalculateSalary() => _contractAmount;

        public override string GetInfo()
        {
            string status = IsContractActive ? "✓ نشط" : "✗ منتهي";
            return base.GetInfo() + $" | متعاقد ({_contractType}) | {_contractAmount:F2}$ | {status}";
        }

        public override void PerformTask()
            => Console.WriteLine($"{Name}: عقد ({_contractType}) — {_contractAmount:F2}$.");
    }
}
