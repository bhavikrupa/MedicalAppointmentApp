using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.Models.DTOs
{
    public class CreateInvoiceDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public Guid? AppointmentId { get; set; }
        
        [Required]
        public List<InvoiceServiceDto> Services { get; set; } = new List<InvoiceServiceDto>();
        
        public decimal TaxRate { get; set; } = 0.10m;
        
        public string? PaymentMethod { get; set; }
    }
    
    public class InvoiceServiceDto
    {
        [Required]
        public Guid ServiceId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}
