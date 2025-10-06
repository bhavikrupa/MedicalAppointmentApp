using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Interfaces
{
    public interface IPatientService
    {
        Task<ApiResponse<PatientResponseDto>> CreatePatientAsync(CreatePatientDto patientDto);
        Task<ApiResponse<List<PatientResponseDto>>> GetPatientsAsync();
    }
}
