using Microsoft.AspNetCore.Mvc;
using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active patients
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PatientResponseDto>>>> GetPatients()
        {
            try
            {
                var result = await _patientService.GetPatientsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients");
                return StatusCode(500, new ApiResponse<List<PatientResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving patients"
                });
            }
        }

        /// <summary>
        /// Create a new patient record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PatientResponseDto>>> CreatePatient([FromBody] CreatePatientDto patientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<PatientResponseDto>
                    {
                        Success = false,
                        Message = "Invalid input data"
                    });
                }

                var result = await _patientService.CreatePatientAsync(patientDto);
                
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetPatients), new { id = result.Data?.Id }, result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, new ApiResponse<PatientResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the patient"
                });
            }
        }
    }
}