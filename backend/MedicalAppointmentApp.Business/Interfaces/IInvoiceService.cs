using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Interfaces
{
    public interface IInvoiceService
    {
        Task<ApiResponse<InvoiceResponseDto>> CreateInvoiceAsync(CreateInvoiceDto invoiceDto);
        Task<ApiResponse<List<InvoiceResponseDto>>> GetInvoicesAsync();
    }
}
