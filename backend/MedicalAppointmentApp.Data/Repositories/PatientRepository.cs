using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PatientRepository> _logger;

        public PatientRepository(IConfiguration configuration, ILogger<PatientRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse<PatientResponseDto>> CreatePatientAsync(CreatePatientDto patientDto)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand("SELECT * FROM create_patient($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)", connection);
                command.Parameters.AddWithValue(patientDto.FirstName);
                command.Parameters.AddWithValue(patientDto.LastName);
                command.Parameters.AddWithValue(patientDto.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(patientDto.Phone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(patientDto.DateOfBirth ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(patientDto.Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(patientDto.EmergencyContactName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(patientDto.EmergencyContactPhone ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(patientDto.InsuranceProvider ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(patientDto.InsurancePolicyNumber ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var patientId = reader.IsDBNull(0) ? (Guid?)null : reader.GetGuid(0);
                    var success = reader.GetBoolean(1);
                    var message = reader.GetString(2);

                    if (success && patientId.HasValue)
                    {
                        var patientResponse = new PatientResponseDto
                        {
                            Id = patientId.Value,
                            FirstName = patientDto.FirstName,
                            LastName = patientDto.LastName,
                            Email = patientDto.Email,
                            Phone = patientDto.Phone,
                            DateOfBirth = patientDto.DateOfBirth,
                            Address = patientDto.Address,
                            EmergencyContactName = patientDto.EmergencyContactName,
                            EmergencyContactPhone = patientDto.EmergencyContactPhone,
                            InsuranceProvider = patientDto.InsuranceProvider,
                            InsurancePolicyNumber = patientDto.InsurancePolicyNumber,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        return new ApiResponse<PatientResponseDto>
                        {
                            Success = true,
                            Message = message,
                            Data = patientResponse
                        };
                    }
                    else
                    {
                        return new ApiResponse<PatientResponseDto>
                        {
                            Success = false,
                            Message = message
                        };
                    }
                }

                return new ApiResponse<PatientResponseDto>
                {
                    Success = false,
                    Message = "No response from database"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return new ApiResponse<PatientResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the patient"
                };
            }
        }

        public async Task<ApiResponse<List<PatientResponseDto>>> GetPatientsAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(@"
                    SELECT id, first_name, last_name, email, phone, date_of_birth, 
                           address, emergency_contact_name, emergency_contact_phone,
                           insurance_provider, insurance_policy_number, is_active, created_at
                    FROM patients 
                    WHERE is_active = true
                    ORDER BY created_at DESC", connection);

                command.CommandTimeout = 30;

                var patients = new List<PatientResponseDto>();

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    patients.Add(new PatientResponseDto
                    {
                        Id = reader.GetGuid(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                        DateOfBirth = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                        Address = reader.IsDBNull(6) ? null : reader.GetString(6),
                        EmergencyContactName = reader.IsDBNull(7) ? null : reader.GetString(7),
                        EmergencyContactPhone = reader.IsDBNull(8) ? null : reader.GetString(8),
                        InsuranceProvider = reader.IsDBNull(9) ? null : reader.GetString(9),
                        InsurancePolicyNumber = reader.IsDBNull(10) ? null : reader.GetString(10),
                        IsActive = reader.GetBoolean(11),
                        CreatedAt = reader.GetDateTime(12)
                    });
                }

                return new ApiResponse<List<PatientResponseDto>>
                {
                    Success = true,
                    Message = patients.Count > 0 ? "Patients retrieved successfully" : "No patients found",
                    Data = patients
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patients");
                return new ApiResponse<List<PatientResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving patients"
                };
            }
        }
    }
}
