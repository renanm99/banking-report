using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace banking_report.Application.Domain.Models
{
    public class ItemCardExpenses
    {
        public int ExpenseId { get; set; }
        public int IndividualReportId { get; set; }
        public int PersonId { get; set; }
        public string Description { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public string? Installment { get; set; }
    }
}
