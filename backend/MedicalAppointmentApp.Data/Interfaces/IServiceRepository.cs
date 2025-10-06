using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Interfaces
{
    public interface IServiceRepository
    {
        Task<ApiResponse<List<ServiceResponseDto>>> GetServicesAsync();
    }
}
