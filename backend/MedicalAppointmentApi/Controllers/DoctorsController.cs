using Microsoft.AspNetCore.Mvc;
using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, IAppointmentService appointmentService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active doctors
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<DoctorResponseDto>>>> GetDoctors()
        {
            try
            {
                var result = await _doctorService.GetDoctorsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors");
                return StatusCode(500, new ApiResponse<List<DoctorResponseDto>>
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

                var result = await _doctorService.GetDoctorScheduleAsync(doctorId, startDate, endDate);
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

                var result = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, appointmentDate, durationMinutes);
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