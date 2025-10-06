using Microsoft.AspNetCore.Mvc;
using MedicalAppointmentApp.Business.Interfaces;
using MedicalAppointmentApp.Models.Common;
using MedicalAppointmentApp.Models.DTOs;

namespace MedicalAppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<InvoiceResponseDto>>>> GetInvoices()
        {
            try
            {
                var result = await _invoiceService.GetInvoicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices");
                return StatusCode(500, new ApiResponse<List<InvoiceResponseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving invoices"
                });
            }
        }

        /// <summary>
        /// Create a new invoice
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> CreateInvoice([FromBody] CreateInvoiceDto invoiceDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<InvoiceResponseDto>
                    {
                        Success = false,
                        Message = "Invalid input data"
                    });
                }

                if (invoiceDto.Services == null || !invoiceDto.Services.Any())
                {
                    return BadRequest(new ApiResponse<InvoiceResponseDto>
                    {
                        Success = false,
                        Message = "At least one service is required"
                    });
                }

                var result = await _invoiceService.CreateInvoiceAsync(invoiceDto);
                
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetInvoices), new { id = result.Data?.Id }, result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return StatusCode(500, new ApiResponse<InvoiceResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the invoice"
                });
            }
        }
    }
}