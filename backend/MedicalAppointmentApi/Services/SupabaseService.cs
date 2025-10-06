using Supabase;
using Supabase.Postgrest;
using Npgsql;
using MedicalAppointmentApi.Models;
using MedicalAppointmentApi.DTOs;

namespace MedicalAppointmentApi.Services
{
    public interface ISupabaseService
    {
        Task<ApiResponse<PatientResponseDto>> CreatePatientAsync(CreatePatientDto patientDto);
        Task<ApiResponse<AppointmentResponseDto>> ScheduleAppointmentAsync(ScheduleAppointmentDto appointmentDto);
        Task<ApiResponse<InvoiceResponseDto>> CreateInvoiceAsync(CreateInvoiceDto invoiceDto);
        Task<ApiResponse<dynamic>> CompleteAppointmentWithBillingAsync(CompleteAppointmentDto completeDto);
        Task<ApiResponse<List<DoctorScheduleResponseDto>>> GetDoctorScheduleAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<ApiResponse<List<TimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime appointmentDate, int durationMinutes = 30);
        Task<ApiResponse<List<PatientResponseDto>>> GetPatientsAsync();
        Task<ApiResponse<List<DoctorResponseDto>>> GetDoctorsAsync();
        Task<ApiResponse<List<ServiceResponseDto>>> GetServicesAsync();
        Task<ApiResponse<List<InvoiceResponseDto>>> GetInvoicesAsync();
        Task<ApiResponse<List<AppointmentResponseDto>>> GetAppointmentsAsync();
    }

    public class SupabaseService : ISupabaseService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SupabaseService> _logger;

        public SupabaseService(Supabase.Client supabaseClient, IConfiguration configuration, ILogger<SupabaseService> logger)
        {
            _supabaseClient = supabaseClient;
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
                            AppointmentTime = reader.GetTimeSpan(6).ToString(@"hh\:mm"), // Convert TimeSpan to string in "hh:mm" format
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

        public async Task<ApiResponse<InvoiceResponseDto>> CreateInvoiceAsync(CreateInvoiceDto invoiceDto)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var serviceIds = invoiceDto.Services.Select(s => s.ServiceId).ToArray();
                var quantities = invoiceDto.Services.Select(s => s.Quantity).ToArray();

                await using var command = new NpgsqlCommand("SELECT * FROM create_invoice_with_services($1, $2, $3, $4, $5)", connection);
                command.Parameters.AddWithValue(invoiceDto.PatientId);
                command.Parameters.AddWithValue(invoiceDto.AppointmentId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(serviceIds);
                command.Parameters.AddWithValue(quantities);
                command.Parameters.AddWithValue(invoiceDto.TaxRate);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var invoiceId = reader.IsDBNull(0) ? (Guid?)null : reader.GetGuid(0);
                    var invoiceNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
                    var totalAmount = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2);
                    var success = reader.GetBoolean(3);
                    var message = reader.GetString(4);

                    if (success && invoiceId.HasValue)
                    {
                        var invoiceResponse = new InvoiceResponseDto
                        {
                            Id = invoiceId.Value,
                            PatientId = invoiceDto.PatientId,
                            AppointmentId = invoiceDto.AppointmentId,
                            InvoiceNumber = invoiceNumber ?? string.Empty,
                            InvoiceDate = DateTime.UtcNow.Date,
                            TotalAmount = totalAmount,
                            Status = string.IsNullOrEmpty(invoiceDto.PaymentMethod) ? "pending" : "paid",
                            PaymentMethod = invoiceDto.PaymentMethod
                        };

                        return new ApiResponse<InvoiceResponseDto>
                        {
                            Success = true,
                            Message = message,
                            Data = invoiceResponse
                        };
                    }
                    else
                    {
                        return new ApiResponse<InvoiceResponseDto>
                        {
                            Success = false,
                            Message = message
                        };
                    }
                }

                return new ApiResponse<InvoiceResponseDto>
                {
                    Success = false,
                    Message = "No response from database"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return new ApiResponse<InvoiceResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the invoice"
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

        // Additional methods for getting data
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

        // Simple read methods using Supabase client
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

                command.CommandTimeout = 30; // Set command timeout

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

                command.CommandTimeout = 30; // Set command timeout

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

        public async Task<ApiResponse<List<ServiceResponseDto>>> GetServicesAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(@"
                    SELECT id, name, description, price, duration_minutes, is_active, created_at, updated_at
                    FROM services 
                    WHERE is_active = true
                    ORDER BY name ASC", connection);

                command.CommandTimeout = 30; // Set command timeout

                var services = new List<ServiceResponseDto>();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    services.Add(new ServiceResponseDto
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DurationMinutes = reader.GetInt32(4),
                        IsActive = reader.GetBoolean(5),
                        CreatedAt = reader.GetDateTime(6),
                        UpdatedAt = reader.GetDateTime(7)
                    });
                }

                return new ApiResponse<List<ServiceResponseDto>>
                {
                    Success = true,
                    Message = services.Count > 0 ? "Services retrieved successfully" : "No services found",
                    Data = services
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting services");
                return new ApiResponse<List<ServiceResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving services"
                };
            }
        }

        public async Task<ApiResponse<List<InvoiceResponseDto>>> GetInvoicesAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Supabase");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(@"
                    SELECT 
                        i.id,
                        i.invoice_number,
                        i.patient_id,
                        CONCAT(p.first_name, ' ', p.last_name) as patient_name,
                        i.appointment_id,
                        i.invoice_date,
                        i.subtotal,
                        i.tax_amount,
                        i.total_amount,
                        i.status,
                        i.payment_method,
                        i.payment_date,
                        i.notes
                    FROM invoices i
                    INNER JOIN patients p ON i.patient_id = p.id
                    ORDER BY i.invoice_date DESC", connection);

                command.CommandTimeout = 60; // Set timeout to 60 seconds

                var invoices = new List<InvoiceResponseDto>();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    invoices.Add(new InvoiceResponseDto
                    {
                        Id = reader.GetGuid(0),
                        InvoiceNumber = reader.GetString(1),
                        PatientId = reader.GetGuid(2),
                        PatientName = reader.GetString(3),
                        AppointmentId = reader.IsDBNull(4) ? null : reader.GetGuid(4),
                        InvoiceDate = reader.GetDateTime(5),
                        Subtotal = reader.GetDecimal(6),
                        TaxAmount = reader.GetDecimal(7),
                        TotalAmount = reader.GetDecimal(8),
                        Status = reader.GetString(9),
                        PaymentMethod = reader.IsDBNull(10) ? null : reader.GetString(10),
                        PaymentDate = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                        Notes = reader.IsDBNull(12) ? null : reader.GetString(12),
                        Items = new List<InvoiceItemResponseDto>()
                    });
                }

                return new ApiResponse<List<InvoiceResponseDto>>
                {
                    Success = true,
                    Message = invoices.Count > 0 ? "Invoices retrieved successfully" : "No invoices found",
                    Data = invoices
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices");
                return new ApiResponse<List<InvoiceResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving invoices"
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

                command.CommandTimeout = 30; // Set command timeout

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
                        AppointmentTime = reader.GetString(6), // Now reading as string
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
    }
}