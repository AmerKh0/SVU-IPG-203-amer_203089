using System;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.Models
{
    public class FullTimeEmployee : Employee
    {
        private double _baseSalary;
        private double _bonus;

        public double BaseSalary
        {
            get => _baseSalary;
            set
            {
                if (!Validator.IsValidAmount(value))
                    throw new ArgumentException("الراتب الأساسي يجب أن يكون قيمة موجبة.");
                _baseSalary = value;
            }
        }

        public double Bonus
        {
            get => _bonus;
            set => _bonus = value >= 0 ? value : 0;
        }

        public string EmploymentType => "دوام كامل";

        // Constructor: موظف جديد
        public FullTimeEmployee(string name, string department, double baseSalary, double bonus)
            : base(name, department)
        {
            BaseSalary = baseSalary;
            Bonus      = bonus;
        }

        // Constructor: استعادة من JSON
        public FullTimeEmployee(string id, string name, string department, DateTime hireDate,
                                double baseSalary, double bonus)
            : base(id, name, department, hireDate)
        {
            BaseSalary = baseSalary;
            Bonus      = bonus;
        }

        public override double CalculateSalary() => _baseSalary + _bonus;

        public override string GetInfo()
            => base.GetInfo() + $" | دوام كامل | {_baseSalary:F2}$ + {_bonus:F2}$";

        public override void PerformTask()
            => Console.WriteLine($"{Name}: دوام كامل 8 ساعات يومياً.");
    }
}
