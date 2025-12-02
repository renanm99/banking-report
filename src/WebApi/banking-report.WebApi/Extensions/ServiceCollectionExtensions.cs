using banking_report.Application.Domain.Interfaces;
using banking_report.Application.Services;

namespace banking_report.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBankReportService, BankReportService>();
            services.AddScoped<IPDFExtractorService, PDFExtractorService>();
            return services;
        }
    }
}
