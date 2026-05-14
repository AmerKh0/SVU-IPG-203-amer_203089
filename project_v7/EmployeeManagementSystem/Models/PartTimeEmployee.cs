using System;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.Models
{
    public class PartTimeEmployee : Employee
    {
        private int    _hoursWorked;
        private double _hourlyRate;

        public int HoursWorked
        {
            get => _hoursWorked;
            set
            {
                if (!Validator.IsValidHours(value))
                    throw new ArgumentException("عدد الساعات يجب أن يكون بين 1 و 744.");
                _hoursWorked = value;
            }
        }

        public double HourlyRate
        {
            get => _hourlyRate;
            set
            {
                if (!Validator.IsValidAmount(value))
                    throw new ArgumentException("سعر الساعة يجب أن يكون قيمة موجبة.");
                _hourlyRate = value;
            }
        }

        public string EmploymentType => "دوام جزئي";

        // Constructor: موظف جديد
        public PartTimeEmployee(string name, string department, int hoursWorked, double hourlyRate)
            : base(name, department)
        {
            HoursWorked = hoursWorked;
            HourlyRate  = hourlyRate;
        }

        // Constructor: استعادة من JSON
        public PartTimeEmployee(string id, string name, string department, DateTime hireDate,
                                int hoursWorked, double hourlyRate)
            : base(id, name, department, hireDate)
        {
            HoursWorked = hoursWorked;
            HourlyRate  = hourlyRate;
        }

        public override double CalculateSalary() => _hoursWorked * _hourlyRate;

        public override string GetInfo()
            => base.GetInfo() + $" | دوام جزئي | {_hoursWorked}س × {_hourlyRate:F2}$";

        public override void PerformTask()
            => Console.WriteLine($"{Name}: {_hoursWorked} ساعة شهرياً.");
    }
}
