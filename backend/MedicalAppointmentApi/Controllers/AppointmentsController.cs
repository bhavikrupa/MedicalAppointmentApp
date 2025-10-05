using Microsoft.AspNetCore.Mvc;
using MedicalAppointmentApi.Services;
using MedicalAppointmentApi.DTOs;

namespace MedicalAppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(ISupabaseService supabaseService, ILogger<AppointmentsController> logger)
        {
            _supabaseService = supabaseService;
            _logger = logger;
        }

        /// <summary>
        /// Get all appointments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AppointmentResponseDto>>>> GetAppointments()
        {
            try
            {
                var result = await _supabaseService.GetAppointmentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, new ApiResponse<List<AppointmentResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving appointments"
                });
            }
        }

        /// <summary>
        /// Schedule a new appointment
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AppointmentResponseDto>>> ScheduleAppointment([FromBody] ScheduleAppointmentDto appointmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<AppointmentResponseDto>
                    {
                        Success = false,
                        Message = "Invalid input data"
                    });
                }

                var result = await _supabaseService.ScheduleAppointmentAsync(appointmentDto);
                
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetAppointments), new { id = result.Data?.Id }, result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling appointment");
                return StatusCode(500, new ApiResponse<AppointmentResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while scheduling the appointment"
                });
            }
        }

        /// <summary>
        /// Complete appointment and create invoice
        /// </summary>
        [HttpPost("{appointmentId}/complete")]
        public async Task<ActionResult<ApiResponse<dynamic>>> CompleteAppointment(
            Guid appointmentId, 
            [FromBody] CompleteAppointmentDto completeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = "Invalid input data"
                    });
                }

                // Ensure the appointment ID matches
                completeDto.AppointmentId = appointmentId;

                var result = await _supabaseService.CompleteAppointmentWithBillingAsync(completeDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing appointment");
                return StatusCode(500, new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "An error occurred while completing the appointment"
                });
            }
        }
    }
}