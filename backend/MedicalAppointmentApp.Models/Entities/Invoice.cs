using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MedicalAppointmentApp.Models.Entities
{
    [Table("invoices")]
    public class Invoice : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(20)]
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; } = string.Empty;
        
        [Required]
        [Column("patient_id")]
        public Guid PatientId { get; set; }
        
        [Column("appointment_id")]
        public Guid? AppointmentId { get; set; }
        
        [Required]
        [Column("invoice_date")]
        public DateTime InvoiceDate { get; set; }
        
        [Column("due_date")]
        public DateTime? DueDate { get; set; }
        
        [Column("subtotal")]
        public decimal Subtotal { get; set; }
        
        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }
        
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "pending";
        
        [Column("payment_method")]
        public string? PaymentMethod { get; set; }
        
        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }
        
        [Column("notes")]
        public string? Notes { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
