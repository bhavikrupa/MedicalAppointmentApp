using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<ApiResponse<InvoiceResponseDto>> CreateInvoiceAsync(CreateInvoiceDto invoiceDto);
        Task<ApiResponse<List<InvoiceResponseDto>>> GetInvoicesAsync();
    }
}
