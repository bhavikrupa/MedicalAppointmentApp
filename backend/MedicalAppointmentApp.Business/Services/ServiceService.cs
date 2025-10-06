using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Business.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<ApiResponse<List<ServiceResponseDto>>> GetServicesAsync()
        {
            return await _serviceRepository.GetServicesAsync();
        }
    }
}
