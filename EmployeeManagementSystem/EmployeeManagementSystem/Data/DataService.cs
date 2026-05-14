using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data
{
    /// <summary>
    /// فئة ساكنة لحفظ وتحميل بيانات الموظفين من ملف JSON
    /// Static Class — Data Persistence
    /// الملف يُحفظ في: %AppData%\HRSystem\employees.json
    /// </summary>
    public static class DataService
    {
        private static readonly string _folder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HRSystem");

        private static readonly string _filePath =
            Path.Combine(_folder, "employees.json");

        private static readonly JsonSerializerOptions _jsonOpts =
            new JsonSerializerOptions { WriteIndented = true };

        // ============================================================
        // حفظ قائمة الموظفين
        // ============================================================
        public static void Save(IReadOnlyList<Employee> employees)
        {
            Directory.CreateDirectory(_folder);
            var dtos = employees.Select(ToDto).ToList();
            string json = JsonSerializer.Serialize(dtos, _jsonOpts);
            File.WriteAllText(_filePath, json);
        }

        // ============================================================
        // تحميل قائمة الموظفين (ترجع قائمة فارغة إذا لا يوجد ملف)
        // ============================================================
        public static List<Employee> Load()
        {
            if (!File.Exists(_filePath))
                return new List<Employee>();

            try
            {
                string json = File.ReadAllText(_filePath);
                var dtos = JsonSerializer.Deserialize<List<EmployeeDto>>(json);
                if (dtos == null) return new List<Employee>();

                return dtos
                    .Select(FromDto)
                    .Where(e => e != null)
                    .Select(e => e!)
                    .ToList();
            }
            catch
            {
                return new List<Employee>(); // إذا الملف تالف، ابدأ من جديد
            }
        }

        // ============================================================
        // تحويل Employee → EmployeeDto
        // ============================================================
        private static EmployeeDto ToDto(Employee emp)
        {
            var dto = new EmployeeDto
            {
                Id          = emp.Id,
                Name        = emp.Name,
                Department  = emp.Department,
                HireDate    = emp.HireDate,
                CompanyBonus = emp.CompanyBonus,
                Deduction   = emp.Deduction,
                WorkDays    = emp.WorkDays,
                DailyHours  = emp.DailyHours,
                Sales       = emp.Sales
            };

            switch (emp)
            {
                case FullTimeEmployee fte:
                    dto.Type       = "FullTime";
                    dto.BaseSalary = fte.BaseSalary;
                    dto.Bonus      = fte.Bonus;
                    break;

                case PartTimeEmployee pte:
                    dto.Type       = "PartTime";
                    dto.HoursWorked = pte.HoursWorked;
                    dto.HourlyRate  = pte.HourlyRate;
                    break;

                case Contractor c:
                    dto.Type            = "Contractor";
                    dto.ContractAmount  = c.ContractAmount;
                    dto.ContractType    = c.ContractType;
                    dto.ContractEndDate = c.ContractEndDate;
                    break;
            }

            return dto;
        }

        // ============================================================
        // تحويل EmployeeDto → Employee (بالبيانات المحفوظة)
        // ============================================================
        private static Employee? FromDto(EmployeeDto dto)
        {
            Employee? emp = dto.Type switch
            {
                "FullTime"   => new FullTimeEmployee(dto.Id, dto.Name, dto.Department, dto.HireDate,
                                    dto.BaseSalary, dto.Bonus),

                "PartTime"   => new PartTimeEmployee(dto.Id, dto.Name, dto.Department, dto.HireDate,
                                    dto.HoursWorked, dto.HourlyRate),

                "Contractor" => new Contractor(dto.Id, dto.Name, dto.Department, dto.HireDate,
                                    dto.ContractAmount, dto.ContractType, dto.ContractEndDate),
                _            => null
            };

            if (emp != null)
            {
                emp.CompanyBonus = dto.CompanyBonus;
                emp.Deduction    = dto.Deduction;
                emp.WorkDays     = dto.WorkDays > 0 ? dto.WorkDays : 22;
                emp.DailyHours   = dto.DailyHours > 0 ? dto.DailyHours : 8;
                emp.Sales        = dto.Sales ?? new System.Collections.Generic.List<EmployeeManagementSystem.Models.SaleRecord>();
            }

            return emp;
        }
    }
}
