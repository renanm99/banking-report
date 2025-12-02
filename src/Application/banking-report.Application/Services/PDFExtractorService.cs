using Azure;
using Azure.AI.DocumentIntelligence;
using banking_report.Application.Domain.Interfaces;
using banking_report.Application.Domain.Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace banking_report.Application.Services
{
    public class PDFExtractorService : IPDFExtractorService
    {
        public async Task ProcessPDFAsync(Stream pdfStream)
        {
            var password = "47612456893";
            PdfDocument inputDocument = PdfReader.Open(pdfStream, password, PdfDocumentOpenMode.Import);

            // Create a new PDF document
            PdfDocument outputDocument = new PdfDocument();

            // Copy all pages
            foreach (PdfPage page in inputDocument.Pages)
            {
                outputDocument.AddPage(page);
            }

            // Save the new document without encryption
            var outputStream = new MemoryStream();
            outputDocument.Save(outputStream, false);

            string endpoint = "<endpoint-here>";
            string key = "<key-here>";
            AzureKeyCredential credential = new AzureKeyCredential(key);
            DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(endpoint), credential);

            var options = new AnalyzeDocumentOptions("prebuilt-bankStatement.us",
            BinaryData.FromStream(outputStream));
            options.Features.Add(DocumentAnalysisFeature.QueryFields);
            options.QueryFields.Add("EntitledName");

            Operation<AnalyzeResult> operation = await
                client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                options
                );

            AnalyzeResult result = operation.Value;




            List<BankReport> listBankReport = new List<BankReport>();
            // After you get the AnalyzeResult result = operation.Value;
            foreach (var statement in result.Documents)
            {
                Console.WriteLine("--------Recognizing statement--------");

                var fields = statement.Fields;

                if (fields.TryGetValue("EntitledName", out var EntitledName))
                    Console.WriteLine($"EntitledName: {EntitledName.Content} (confidence: {EntitledName.Confidence})");

                if (fields.TryGetValue("StatementEndDate", out var statementEndDate))
                    Console.WriteLine($"Statement End Date: {statementEndDate.Content} (confidence: {statementEndDate.Confidence})");

                if (fields.TryGetValue("BankName", out var BankName))
                    Console.WriteLine($"BankName: {BankName.Content} (confidence: {BankName.Confidence})");

                if (fields.TryGetValue("TotalExpenseAmount", out var TotalExpenseAmount))
                    Console.WriteLine($"TotalExpenseAmount: {TotalExpenseAmount.Content} (confidence: {TotalExpenseAmount.Confidence})");

                var bankReport = new BankReport
                {
                    DueDate = DateTime.Parse(statementEndDate.Content),
                    EntitledPerson = EntitledName.Content,
                    ReportDate = DateTime.Now,
                    ReportId = "",
                    TotalExpenseAmount = Convert.ToDecimal(TotalExpenseAmount.Content),
                };

                var listIndividualReport = new List<IndividualReport>();
                if (fields.TryGetValue("Accounts", out var accounts) && accounts.FieldType == DocumentFieldType.List)
                {
                    int accountIdx = 1;
                    foreach (var account in fields["Accounts"].ValueList)
                    {
                        var accountObj = account.ValueDictionary;

                        if (accountObj.TryGetValue("AccountNumber", out var accountNumber))
                            Console.WriteLine($"......Account Number: {accountNumber.Content} (confidence: {accountNumber.Confidence})");

                        if (accountObj.TryGetValue("AccountType", out var accountType))
                            Console.WriteLine($"......Account Type: {accountType.Content} (confidence: {accountType.Confidence})");

                        var individualReport = new IndividualReport
                        {
                            CardLastDigits = Convert.ToInt32(accountNumber.Content.Substring(accountNumber.Content.Length - 4)),
                            PersonName = accountType.Content,
                            IndividualReportId = accountIdx,
                            ReportId = "",
                            CardExpenses = new List<ItemCardExpenses>()
                        };

                        accountIdx++;

                        var listItemCardExpenses = new List<ItemCardExpenses>();
                        if (accountObj.TryGetValue("Transactions", out var transactions) && transactions.FieldType == DocumentFieldType.List)
                        {
                            int transactionIdx = 1;
                            Console.WriteLine("......Transactions:");
                            foreach (var transaction in transactions.ValueList)
                            {

                                var transactionObj = transaction.ValueDictionary;

                                if (transactionObj.TryGetValue("WithdrawalAmount", out var withdrawalAmount))
                                    continue; //Ignorando estorno

                                if (transactionObj.TryGetValue("Date", out var transactionDate))
                                    Console.WriteLine($"............Date: {transactionDate.ValueDate} (confidence: {transactionDate.Confidence})");

                                if (transactionObj.TryGetValue("Description", out var description))
                                    Console.WriteLine($"............Description: {description.Content} (confidence: {description.Confidence})");

                                if (transactionObj.TryGetValue("DepositAmount", out var depositAmount))
                                    Console.WriteLine($"............Deposit Amount: {depositAmount.Content} (confidence: {depositAmount.Confidence})");

                                var itemCardExpenses = new ItemCardExpenses
                                {
                                    Amount = Convert.ToDecimal(depositAmount.Content),
                                    Description = description.Content,
                                    ExpenseDate = transactionDate.ValueDate != null ? transactionDate.ValueDate.Value.DateTime : default,
                                    ExpenseId = transactionIdx
                                };


                                individualReport.CardExpenses = listItemCardExpenses;

                                listItemCardExpenses.Add(itemCardExpenses);
                                transactionIdx++;
                            }
                        }

                        individualReport.CardExpenseAmount = listItemCardExpenses.Sum(x => x.Amount);

                        listIndividualReport.Add(individualReport);
                    }
                    bankReport.IndividualReports = listIndividualReport;
                    listBankReport.Add(bankReport);
                }
                Console.WriteLine("--------------------------------------");
            }
            await WriteDB(listBankReport);
            return;
        }

        public async Task WriteDB(IEnumerable<BankReport> bankReport) 
        {

        }
    }
}
