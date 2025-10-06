using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(IConfiguration configuration, ILogger<InvoiceRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
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

                command.CommandTimeout = 60;

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
    }
}
