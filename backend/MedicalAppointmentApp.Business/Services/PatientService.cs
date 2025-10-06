using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<ApiResponse<PatientResponseDto>> CreatePatientAsync(CreatePatientDto patientDto)
        {
            // Add any business logic here (validation, transformation, etc.)
            return await _patientRepository.CreatePatientAsync(patientDto);
        }

        public async Task<ApiResponse<List<PatientResponseDto>>> GetPatientsAsync()
        {
            return await _patientRepository.GetPatientsAsync();
        }
    }
}
