using banking_report.Application.Domain.Interfaces;
using banking_report.Application.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace banking_report.WebaApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IBankReportService _bankReportService;

        public ReportController(ILogger<ReportController> logger, IBankReportService bankReportService)
        {
            _logger = logger;
            _bankReportService = bankReportService;
        }

        [HttpPost(Name = "PostBankReport")]
        public void Post()
        {
            _bankReportService.PostBankReport();
        }

        [HttpGet(Name = "GetBankReport")]
        public BankReport Get(int personId)
        {
            return _bankReportService.GetBankReport(personId);
        }
    }


}
