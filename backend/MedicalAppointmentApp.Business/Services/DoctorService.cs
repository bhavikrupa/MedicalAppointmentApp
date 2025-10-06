using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<ApiResponse<List<DoctorResponseDto>>> GetDoctorsAsync()
        {
            return await _doctorRepository.GetDoctorsAsync();
        }

        public async Task<ApiResponse<List<DoctorScheduleResponseDto>>> GetDoctorScheduleAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            return await _doctorRepository.GetDoctorScheduleAsync(doctorId, startDate, endDate);
        }
    }
}
