using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(IConfiguration configuration, ILogger<AppointmentRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse<AppointmentResponseDto>> ScheduleAppointmentAsync(ScheduleAppointmentDto appointmentDto)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand("SELECT * FROM schedule_appointment($1, $2, $3, $4, $5, $6)", connection);
                command.Parameters.AddWithValue(appointmentDto.PatientId);
                command.Parameters.AddWithValue(appointmentDto.DoctorId);
                command.Parameters.AddWithValue(appointmentDto.AppointmentDate.Date);
                command.Parameters.AddWithValue(appointmentDto.AppointmentTime);
                command.Parameters.AddWithValue(appointmentDto.DurationMinutes);
                command.Parameters.AddWithValue(appointmentDto.Notes ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var appointmentId = reader.IsDBNull(0) ? (Guid?)null : reader.GetGuid(0);
                    var success = reader.GetBoolean(1);
                    var message = reader.GetString(2);

                    if (success && appointmentId.HasValue)
                    {
                        var appointmentResponse = new AppointmentResponseDto
                        {
                            Id = appointmentId.Value,
                            PatientId = appointmentDto.PatientId,
                            DoctorId = appointmentDto.DoctorId,
                            AppointmentDate = appointmentDto.AppointmentDate,
                            AppointmentTime = reader.GetTimeSpan(6).ToString(@"hh\:mm"),
                            DurationMinutes = appointmentDto.DurationMinutes,
                            Status = "scheduled",
                            Notes = appointmentDto.Notes,
                            CreatedAt = DateTime.UtcNow
                        };

                        return new ApiResponse<AppointmentResponseDto>
                        {
                            Success = true,
                            Message = message,
                            Data = appointmentResponse
                        };
                    }
                    else
                    {
                        return new ApiResponse<AppointmentResponseDto>
                        {
                            Success = false,
                            Message = message
                        };
                    }
                }

                return new ApiResponse<AppointmentResponseDto>
                {
                    Success = false,
                    Message = "No response from database"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling appointment");
                return new ApiResponse<AppointmentResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while scheduling the appointment"
                };
            }
        }

        public async Task<ApiResponse<List<AppointmentResponseDto>>> GetAppointmentsAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(@"
                    SELECT 
                        a.id, 
                        a.patient_id, 
                        CONCAT(p.first_name, ' ', p.last_name) as patient_name,
                        a.doctor_id, 
                        CONCAT('Dr. ', d.first_name, ' ', d.last_name) as doctor_name,
                        a.appointment_date, 
                        a.appointment_time::text as appointment_time,
                        a.duration_minutes, 
                        a.status, 
                        a.notes,
                        a.created_at
                    FROM appointments a
                    INNER JOIN patients p ON a.patient_id = p.id
                    INNER JOIN doctors d ON a.doctor_id = d.id
                    ORDER BY a.appointment_date DESC, a.appointment_time DESC", connection);

                command.CommandTimeout = 30;

                var appointments = new List<AppointmentResponseDto>();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    appointments.Add(new AppointmentResponseDto
                    {
                        Id = reader.GetGuid(0),
                        PatientId = reader.GetGuid(1),
                        PatientName = reader.GetString(2),
                        DoctorId = reader.GetGuid(3),
                        DoctorName = reader.GetString(4),
                        AppointmentDate = reader.GetDateTime(5),
                        AppointmentTime = reader.GetString(6),
                        DurationMinutes = reader.GetInt32(7),
                        Status = reader.GetString(8),
                        Notes = reader.IsDBNull(9) ? null : reader.GetString(9),
                        CreatedAt = reader.GetDateTime(10)
                    });
                }

                return new ApiResponse<List<AppointmentResponseDto>>
                {
                    Success = true,
                    Message = appointments.Count > 0 ? "Appointments retrieved successfully" : "No appointments found",
                    Data = appointments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return new ApiResponse<List<AppointmentResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving appointments"
                };
            }
        }

        public async Task<ApiResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime appointmentDate, int durationMinutes = 30)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand("SELECT * FROM get_available_time_slots($1, $2, $3)", connection);
                command.Parameters.AddWithValue(doctorId);
                command.Parameters.AddWithValue(appointmentDate.Date);
                command.Parameters.AddWithValue(durationMinutes);

                var timeSlots = new List<TimeSlotDto>();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    timeSlots.Add(new TimeSlotDto
                    {
                        TimeSlot = reader.GetTimeSpan(0),
                        IsAvailable = reader.GetBoolean(1)
                    });
                }

                return new ApiResponse<List<TimeSlotDto>>
                {
                    Success = true,
                    Message = "Available time slots retrieved successfully",
                    Data = timeSlots
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available time slots");
                return new ApiResponse<List<TimeSlotDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving available time slots"
                };
            }
        }

        public async Task<ApiResponse<dynamic>> CompleteAppointmentWithBillingAsync(CompleteAppointmentDto completeDto)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var serviceIds = completeDto.Services.Select(s => s.ServiceId).ToArray();
                var quantities = completeDto.Services.Select(s => s.Quantity).ToArray();

                await using var command = new NpgsqlCommand("SELECT * FROM complete_appointment_with_billing($1, $2, $3, $4, $5)", connection);
                command.Parameters.AddWithValue(completeDto.AppointmentId);
                command.Parameters.AddWithValue(serviceIds);
                command.Parameters.AddWithValue(quantities);
                command.Parameters.AddWithValue(completeDto.TaxRate);
                command.Parameters.AddWithValue(completeDto.PaymentMethod ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var appointmentId = reader.GetGuid(0);
                    var invoiceId = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1);
                    var invoiceNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                    var totalAmount = reader.IsDBNull(3) ? 0m : reader.GetDecimal(3);
                    var success = reader.GetBoolean(4);
                    var message = reader.GetString(5);

                    return new ApiResponse<dynamic>
                    {
                        Success = success,
                        Message = message,
                        Data = new
                        {
                            AppointmentId = appointmentId,
                            InvoiceId = invoiceId,
                            InvoiceNumber = invoiceNumber,
                            TotalAmount = totalAmount
                        }
                    };
                }

                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "No response from database"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing appointment with billing");
                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "An error occurred while completing the appointment and billing"
                };
            }
        }
    }
}
