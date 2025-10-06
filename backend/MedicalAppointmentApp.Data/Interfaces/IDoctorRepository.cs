using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Interfaces
{
    public interface IDoctorRepository
    {
        Task<ApiResponse<List<DoctorResponseDto>>> GetDoctorsAsync();
        Task<ApiResponse<List<DoctorScheduleResponseDto>>> GetDoctorScheduleAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    }
}
