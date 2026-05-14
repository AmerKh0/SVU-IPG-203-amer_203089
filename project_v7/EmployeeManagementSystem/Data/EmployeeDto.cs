using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data
{
    /// <summary>
    /// كائن نقل البيانات لحفظ الموظفين في JSON
    /// </summary>
    public class EmployeeDto
    {
        public string Type       { get; set; } = "";   // "FullTime" | "PartTime" | "Contractor"
        public string Id         { get; set; } = "";
        public string Name       { get; set; } = "";
        public string Department { get; set; } = "";
        public DateTime HireDate { get; set; }

        // مكافأة وخصم الشركة
        public double CompanyBonus { get; set; }
        public double Deduction    { get; set; }

        // بيانات الدوام
        public int WorkDays   { get; set; } = 22;
        public int DailyHours { get; set; } = 8;

        // حقول الدوام الكامل
        public double BaseSalary { get; set; }
        public double Bonus      { get; set; }

        // حقول الدوام الجزئي
        public int    HoursWorked { get; set; }
        public double HourlyRate  { get; set; }

        // حقول المتعاقد
        public double   ContractAmount  { get; set; }
        public string   ContractType    { get; set; } = "";
        public DateTime ContractEndDate { get; set; }

        // سجل البيعات
        public List<SaleRecord> Sales { get; set; } = new List<SaleRecord>();
    }
}
