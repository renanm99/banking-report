using banking_report.Application.Domain.Interfaces;
using banking_report.Application.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace banking_report.Application.Services
{
    public class BankReportService : IBankReportService
    {
        private readonly ILogger<BankReportService> _logger;

        private readonly string bankReportPath = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "BankReportData.json");
        private readonly string individualReportPath = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "IndividualReportData.json");
        private readonly string cardExpensesPath = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "ItemCardExpensesData.json");

        public BankReportService(ILogger<BankReportService> logger)
        {
            _logger = logger;
        }
        
        public BankReport GetBankReport(int personId)
        {
            // Leitura e deserialização dos arquivos
            var bankReports = JsonSerializer.Deserialize<List<BankReport>>(File.ReadAllText(bankReportPath)) ?? new();
            var individualReports = JsonSerializer.Deserialize<List<IndividualReport>>(File.ReadAllText(individualReportPath)) ?? new();
            var itemCardExpenses = JsonSerializer.Deserialize<List<ItemCardExpenses>>(File.ReadAllText(cardExpensesPath)) ?? new();

            // Filtra o relatório individual pelo personId
            var filteredIndividualReport = individualReports
                .Where(ir => ir.PersonId == personId).OrderByDescending(ir => ir.ReportId).ToList();

            if (filteredIndividualReport == null)
            {
                return new BankReport();
            }

            var filteredCardExpenses = new List<ItemCardExpenses>();
            decimal individualAmount = 0;
            foreach (var ir in filteredIndividualReport)
            {
                var cardExpenses = itemCardExpenses
                    .Where(ce => ce.PersonId == ir.PersonId && ce.IndividualReportId == ir.IndividualReportId)
                    .ToList();

                individualAmount += ir.CardExpenseAmount;

                filteredIndividualReport.Where(i => i.IndividualReportId == ir.IndividualReportId).Select(ir =>
                {
                    ir.CardExpenses = cardExpenses;
                    return ir;
                }).ToList();

                filteredCardExpenses.AddRange(cardExpenses);
            }

            var filteredBankReport = bankReports
                .Where(ir => ir.ReportId == filteredIndividualReport.First().ReportId).First();


            filteredBankReport.IndividualReports = filteredIndividualReport;
            filteredBankReport.IndividualAmount = individualAmount;


            return filteredBankReport;
        }


        public void PostBankReport()
        {

        }
    }
}
