using banking_report.Application.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace banking_report.Application.Domain.Interfaces
{
    public interface IBankReportService
    {
        BankReport GetBankReport(int personId);
        void PostBankReport();
    }
}
