using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MedicalAppointmentApi.Models
{
    [Table("services")]
    public class Service : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        [Column("description")]
        public string? Description { get; set; }
        
        [Column("price")]
        public decimal Price { get; set; }
        
        [Column("duration_minutes")]
        public int DurationMinutes { get; set; } = 30;
        
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}