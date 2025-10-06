using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Interfaces
{
    public interface IAppointmentService
    {
        Task<ApiResponse<AppointmentResponseDto>> ScheduleAppointmentAsync(ScheduleAppointmentDto appointmentDto);
        Task<ApiResponse<List<AppointmentResponseDto>>> GetAppointmentsAsync();
        Task<ApiResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime appointmentDate, int durationMinutes = 30);
        Task<ApiResponse<dynamic>> CompleteAppointmentWithBillingAsync(CompleteAppointmentDto completeDto);
    }
}
