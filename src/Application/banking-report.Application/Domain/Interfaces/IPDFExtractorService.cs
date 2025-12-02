namespace banking_report.Application.Domain.Interfaces
{
    public interface IPDFExtractorService
    {
        Task ProcessPDFAsync(Stream pdfStream);
    }
}
