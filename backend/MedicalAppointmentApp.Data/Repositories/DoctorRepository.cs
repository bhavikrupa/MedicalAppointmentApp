using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DoctorRepository> _logger;

        public DoctorRepository(IConfiguration configuration, ILogger<DoctorRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse<List<DoctorResponseDto>>> GetDoctorsAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(@"
                    SELECT id, first_name, last_name, email, phone, specialization, 
                           license_number, is_active, created_at, updated_at
                    FROM doctors 
                    WHERE is_active = true
                    ORDER BY last_name ASC", connection);

                command.CommandTimeout = 30;

                var doctors = new List<DoctorResponseDto>();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    doctors.Add(new DoctorResponseDto
                    {
                        Id = reader.GetGuid(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Email = reader.GetString(3),
                        Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Specialization = reader.IsDBNull(5) ? null : reader.GetString(5),
                        LicenseNumber = reader.IsDBNull(6) ? null : reader.GetString(6),
                        IsActive = reader.GetBoolean(7),
                        CreatedAt = reader.GetDateTime(8),
                        UpdatedAt = reader.GetDateTime(9)
                    });
                }

                return new ApiResponse<List<DoctorResponseDto>>
                {
                    Success = true,
                    Message = doctors.Count > 0 ? "Doctors retrieved successfully" : "No doctors found",
                    Data = doctors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors");
                return new ApiResponse<List<DoctorResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving doctors"
                };
            }
        }

        public async Task<ApiResponse<List<DoctorScheduleResponseDto>>> GetDoctorScheduleAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand("SELECT * FROM get_doctor_schedule($1, $2, $3)", connection);
                command.Parameters.AddWithValue(doctorId);
                command.Parameters.AddWithValue(startDate.Date);
                command.Parameters.AddWithValue(endDate.Date);

                var schedules = new List<DoctorScheduleResponseDto>();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    schedules.Add(new DoctorScheduleResponseDto
                    {
                        Date = reader.GetDateTime(0),
                        DayName = reader.GetString(1).Trim(),
                        StartTime = reader.GetTimeSpan(2),
                        EndTime = reader.GetTimeSpan(3),
                        IsAvailable = reader.GetBoolean(4)
                    });
                }

                return new ApiResponse<List<DoctorScheduleResponseDto>>
                {
                    Success = true,
                    Message = "Doctor schedule retrieved successfully",
                    Data = schedules
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor schedule");
                return new ApiResponse<List<DoctorScheduleResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the doctor schedule"
                };
            }
        }
    }
}
