using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using MedicalAppointmentApp.Data.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApp.Data.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceRepository> _logger;

        public ServiceRepository(IConfiguration configuration, ILogger<ServiceRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
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

                command.CommandTimeout = 30;

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
    }
}
