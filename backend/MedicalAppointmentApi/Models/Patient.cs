using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MedicalAppointmentApi.Models
{
    [Table("patients")]
    public class Patient : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(255)]
        [EmailAddress]
        [Column("email")]
        public string? Email { get; set; }
        
        [StringLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }
        
        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        
        [Column("address")]
        public string? Address { get; set; }
        
        [StringLength(100)]
        [Column("emergency_contact_name")]
        public string? EmergencyContactName { get; set; }
        
        [StringLength(20)]
        [Column("emergency_contact_phone")]
        public string? EmergencyContactPhone { get; set; }
        
        [StringLength(100)]
        [Column("insurance_provider")]
        public string? InsuranceProvider { get; set; }
        
        [StringLength(50)]
        [Column("insurance_policy_number")]
        public string? InsurancePolicyNumber { get; set; }
        
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}