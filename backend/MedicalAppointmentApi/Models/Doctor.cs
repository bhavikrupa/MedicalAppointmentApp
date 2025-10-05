using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MedicalAppointmentApi.Models
{
    [Table("doctors")]
    public class Doctor : BaseModel
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
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        [Column("specialization")]
        public string? Specialization { get; set; }
        
        [StringLength(50)]
        [Column("license_number")]
        public string? LicenseNumber { get; set; }
        
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}