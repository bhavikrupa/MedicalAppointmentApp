using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResponse<List<ServiceResponseDto>>> GetServicesAsync();
    }
}
