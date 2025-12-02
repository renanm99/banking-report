using banking_report.Application.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace banking_report.WebaApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PDFController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IPDFExtractorService _pdfExtractorService;

        public PDFController(ILogger<ReportController> logger, IPDFExtractorService pdfExtractorService)
        {
            _logger = logger;
            _pdfExtractorService = pdfExtractorService;
        }

        [HttpPost(Name = "PostPDF")]
        public async Task<IActionResult> Post([FromForm] PDFUploadDto upload)
        {
            if (upload.File == null || upload.File.Length == 0)
                return BadRequest("No file uploaded.");

            using var inputStream = upload.File.OpenReadStream();
            await _pdfExtractorService.ProcessPDFAsync(inputStream);

            return Ok();
        }

        public class PDFUploadDto
        {
            public IFormFile File { get; set; }
        }
    }


}
