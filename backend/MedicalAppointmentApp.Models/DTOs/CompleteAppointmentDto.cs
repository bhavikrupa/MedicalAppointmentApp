using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.Models.DTOs
{
    public class CompleteAppointmentDto
    {
        [Required]
        public Guid AppointmentId { get; set; }
        
        [Required]
        public List<InvoiceServiceDto> Services { get; set; } = new List<InvoiceServiceDto>();
        
        public decimal TaxRate { get; set; } = 0.10m;
        
        public string? PaymentMethod { get; set; }
    }
}
