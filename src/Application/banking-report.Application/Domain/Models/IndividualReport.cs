namespace banking_report.Application.Domain.Models
{
    public class IndividualReport
    {
        public int IndividualReportId { get; set; }
        public string ReportId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int CardLastDigits { get; set; }
        public IEnumerable<ItemCardExpenses>? CardExpenses { get; set; }
        public decimal CardExpenseAmount { get; set; }
    }
}
