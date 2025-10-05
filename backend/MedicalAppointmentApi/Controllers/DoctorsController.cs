using Microsoft.AspNetCore.Mvc;
using MedicalAppointmentApi.Services;
using MedicalAppointmentApi.DTOs;
using MedicalAppointmentApi.Models;

namespace MedicalAppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(ISupabaseService supabaseService, ILogger<DoctorsController> logger)
        {
            _supabaseService = supabaseService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active doctors
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Doctor>>>> GetDoctors()
        {
            try
            {
                var result = await _supabaseService.GetDoctorsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors");
                return StatusCode(500, new ApiResponse<List<Doctor>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving doctors"
                });
            }
        }

        /// <summary>
        /// Get doctor schedule for a date range
        /// </summary>
        [HttpGet("{doctorId}/schedule")]
        public async Task<ActionResult<ApiResponse<List<DoctorScheduleResponseDto>>>> GetDoctorSchedule(
            Guid doctorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new ApiResponse<List<DoctorScheduleResponseDto>>
                    {
                        Success = false,
                        Message = "Start date cannot be after end date"
                    });
                }

                var result = await _supabaseService.GetDoctorScheduleAsync(doctorId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor schedule");
                return StatusCode(500, new ApiResponse<List<DoctorScheduleResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the doctor schedule"
                });
            }
        }

        /// <summary>
        /// Get available time slots for a specific doctor and date
        /// </summary>
        [HttpGet("{doctorId}/available-slots")]
        public async Task<ActionResult<ApiResponse<List<TimeSlotDto>>>> GetAvailableTimeSlots(
            Guid doctorId,
            [FromQuery] DateTime appointmentDate,
            [FromQuery] int durationMinutes = 30)
        {
            try
            {
                if (appointmentDate.Date < DateTime.Today)
                {
                    return BadRequest(new ApiResponse<List<TimeSlotDto>>
                    {
                        Success = false,
                        Message = "Appointment date cannot be in the past"
                    });
                }

                var result = await _supabaseService.GetAvailableTimeSlotsAsync(doctorId, appointmentDate, durationMinutes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available time slots");
                return StatusCode(500, new ApiResponse<List<TimeSlotDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving available time slots"
                });
            }
        }
    }
}