using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<ApiResponse<InvoiceResponseDto>> CreateInvoiceAsync(CreateInvoiceDto invoiceDto)
        {
            // Add any business logic here (validation, transformation, etc.)
            return await _invoiceRepository.CreateInvoiceAsync(invoiceDto);
        }

        public async Task<ApiResponse<List<InvoiceResponseDto>>> GetInvoicesAsync()
        {
            return await _invoiceRepository.GetInvoicesAsync();
        }
    }
}
