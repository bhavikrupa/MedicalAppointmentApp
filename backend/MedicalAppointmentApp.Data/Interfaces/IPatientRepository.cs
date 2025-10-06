using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Interfaces
{
    public interface IPatientRepository
    {
        Task<ApiResponse<PatientResponseDto>> CreatePatientAsync(CreatePatientDto patientDto);
        Task<ApiResponse<List<PatientResponseDto>>> GetPatientsAsync();
    }
}
