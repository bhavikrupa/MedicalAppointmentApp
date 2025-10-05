using Microsoft.AspNetCore.Mvc;
using MedicalAppointmentApi.Services;
using MedicalAppointmentApi.DTOs;
using MedicalAppointmentApi.Models;

namespace MedicalAppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(ISupabaseService supabaseService, ILogger<ServicesController> logger)
        {
            _supabaseService = supabaseService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active services
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Service>>>> GetServices()
        {
            try
            {
                var result = await _supabaseService.GetServicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving services");
                return StatusCode(500, new ApiResponse<List<Service>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving services"
                });
            }
        }
    }
}