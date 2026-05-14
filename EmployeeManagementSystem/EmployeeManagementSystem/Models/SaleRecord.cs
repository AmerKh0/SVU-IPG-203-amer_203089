using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// يمثّل بيعة واحدة سجّلها الموظف
    /// </summary>
    public class SaleRecord
    {
        public string   Id          { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        public string   BuyerName   { get; set; } = "";
        public string   BuyerPhone  { get; set; } = "";
        public double   Amount      { get; set; }        // مبلغ البيعة
        public double   Bonus       { get; set; }        // مكافأة على هذه البيعة (اختياري)
        public DateTime Date        { get; set; } = DateTime.Now;

        public SaleRecord() { }

        public SaleRecord(string buyerName, string buyerPhone, double amount, double bonus)
        {
            BuyerName  = buyerName;
            BuyerPhone = buyerPhone;
            Amount     = amount;
            Bonus      = bonus;
        }
    }
}
