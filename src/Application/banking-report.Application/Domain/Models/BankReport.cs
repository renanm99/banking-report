using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace banking_report.Application.Domain.Models
{
    public class BankReport
    {
        public string ReportId { get; set; }
        public string EntitledPerson { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime DueDate { get; set; }
        public IEnumerable<IndividualReport>? IndividualReports { get; set; }
        public Decimal? IndividualAmount { get; set; }
        public decimal TotalExpenseAmount { get; set; }

    }
}
