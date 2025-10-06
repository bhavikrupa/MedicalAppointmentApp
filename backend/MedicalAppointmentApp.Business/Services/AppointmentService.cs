using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<ApiResponse<AppointmentResponseDto>> ScheduleAppointmentAsync(ScheduleAppointmentDto appointmentDto)
        {
            // Add any business logic here (validation, transformation, etc.)
            return await _appointmentRepository.ScheduleAppointmentAsync(appointmentDto);
        }

        public async Task<ApiResponse<List<AppointmentResponseDto>>> GetAppointmentsAsync()
        {
            return await _appointmentRepository.GetAppointmentsAsync();
        }

        public async Task<ApiResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime appointmentDate, int durationMinutes = 30)
        {
            return await _appointmentRepository.GetAvailableTimeSlotsAsync(doctorId, appointmentDate, durationMinutes);
        }

        public async Task<ApiResponse<dynamic>> CompleteAppointmentWithBillingAsync(CompleteAppointmentDto completeDto)
        {
            // Add any business logic here (validation, transformation, etc.)
            return await _appointmentRepository.CompleteAppointmentWithBillingAsync(completeDto);
        }
    }
}
