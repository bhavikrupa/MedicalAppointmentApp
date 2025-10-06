using Microsoft.AspNetCore.Mvc;
using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(IServiceService serviceService, ILogger<ServicesController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active services
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ServiceResponseDto>>>> GetServices()
        {
            try
            {
                var result = await _serviceService.GetServicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving services");
                return StatusCode(500, new ApiResponse<List<ServiceResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving services"
                });
            }
        }
    }
}