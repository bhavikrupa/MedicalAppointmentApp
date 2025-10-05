using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MedicalAppointmentApi.Models
{
    [Table("invoice_items")]
    public class InvoiceItem : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        
        [Required]
        [Column("invoice_id")]
        public Guid InvoiceId { get; set; }
        
        [Required]
        [Column("service_id")]
        public Guid ServiceId { get; set; }
        
        [Column("quantity")]
        public int Quantity { get; set; } = 1;
        
        [Column("unit_price")]
        public decimal UnitPrice { get; set; }
        
        [Column("total_price")]
        public decimal TotalPrice { get; set; }
        
        [StringLength(500)]
        [Column("description")]
        public string? Description { get; set; }

        // Navigation properties
        public virtual Invoice Invoice { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}