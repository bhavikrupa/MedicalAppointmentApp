using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Interfaces
{
    public interface IDoctorService
    {
        Task<ApiResponse<List<DoctorResponseDto>>> GetDoctorsAsync();
        Task<ApiResponse<List<DoctorScheduleResponseDto>>> GetDoctorScheduleAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    }
}
