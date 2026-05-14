using System;

namespace EmployeeManagementSystem.Utilities
{
    /// <summary>
    /// فئة ساكنة للتحقق من صحة البيانات
    /// Static Class demonstrating: Static Members, Utility Methods
    /// </summary>
    public static class Validator
    {
        // ============================================================
        // STATIC METHODS: دوال ساكنة للتحقق من صحة البيانات
        // ============================================================

        /// <summary>التحقق من صحة الاسم (2-50 حرفاً، غير فارغ)</summary>
        public static bool IsValidName(string? name)
        {
            return !string.IsNullOrWhiteSpace(name)
                && name.Trim().Length >= 2
                && name.Trim().Length <= 50;
        }

        /// <summary>التحقق من صحة قيمة مالية (موجبة وضمن الحد)</summary>
        public static bool IsValidAmount(double amount)
        {
            return amount >= 0 && amount <= 999_999;
        }

        /// <summary>التحقق من صحة عدد ساعات العمل (1-744 ساعة/شهر)</summary>
        public static bool IsValidHours(int hours)
        {
            return hours >= 1 && hours <= 744;
        }

        /// <summary>التحقق من صحة اسم القسم</summary>
        public static bool IsValidDepartment(string? department)
        {
            return !string.IsNullOrWhiteSpace(department) && department.Trim().Length >= 2;
        }

        /// <summary>التحقق من صحة عدد الأشهر (1-120 شهراً)</summary>
        public static bool IsValidDurationMonths(int months)
        {
            return months >= 1 && months <= 120;
        }

        /// <summary>رسالة خطأ منسّقة للاسم غير الصحيح</summary>
        public static string GetNameError() =>
            "الاسم يجب أن يكون بين 2 و 50 حرفاً وغير فارغ.";

        /// <summary>رسالة خطأ منسّقة للقيمة المالية غير الصحيحة</summary>
        public static string GetAmountError() =>
            "القيمة يجب أن تكون بين 0 و 999,999.";

        /// <summary>رسالة خطأ منسّقة لساعات العمل غير الصحيحة</summary>
        public static string GetHoursError() =>
            "عدد الساعات يجب أن يكون بين 1 و 744 ساعة.";
    }
}
