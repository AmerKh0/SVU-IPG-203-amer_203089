namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// واجهة IWorker - تمثّل عمليات الموظف المجردة الأساسية
    /// Interface demonstrating Abstraction principle
    /// </summary>
    public interface IWorker
    {
        /// <summary>حساب الراتب الصافي للموظف</summary>
        double CalculateSalary();

        /// <summary>إرجاع معلومات الموظف كنص</summary>
        string GetInfo();

        /// <summary>تنفيذ مهمة خاصة بنوع الموظف</summary>
        void PerformTask();
    }
}
