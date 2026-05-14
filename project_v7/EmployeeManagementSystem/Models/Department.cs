using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementSystem.Models
{
    public class Department
    {
        // ── Delegate & Event ──────────────────────────────────────
        public delegate void SalaryAlertHandler(string employeeName, double salary);
        public event SalaryAlertHandler? OnHighSalaryDetected;

        // ── Fields ───────────────────────────────────────────────
        private string _name;
        private readonly List<Employee> _employees;

        public double SalaryThreshold { get; set; } = 5000.0;
        public string Name { get => _name; set => _name = string.IsNullOrWhiteSpace(value) ? "غير محدد" : value; }
        public IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

        public Department(string name)
        {
            _name      = name;
            _employees = new List<Employee>();
        }

        public void AddEmployee(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            _employees.Add(employee);
        }

        public bool RemoveEmployee(string id)
        {
            var emp = _employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return false;
            _employees.Remove(emp);
            return true;
        }

        /// <summary>
        /// احتساب إجمالي الرواتب النهائية (بعد المكافآت والخصومات)
        /// POLYMORPHISM: كل emp.GetFinalSalary() يستدعي CalculateSalary() الخاص بنوعه
        /// EVENT: يُطلق OnHighSalaryDetected إذا تجاوز الراتب الحد
        /// </summary>
        public double GetTotalSalaries()
        {
            double total = 0;
            foreach (var emp in _employees)
            {
                double salary = emp.GetFinalSalary(); // POLYMORPHISM
                total += salary;
                if (salary > SalaryThreshold)
                    OnHighSalaryDetected?.Invoke(emp.Name, salary); // EVENT
            }
            return total;
        }

        public List<string> GetAllEmployeesInfo()
            => _employees.Select(e => e.GetInfo()).ToList(); // Polymorphism

        public Employee? FindByName(string name)
            => _employees.FirstOrDefault(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }
}
