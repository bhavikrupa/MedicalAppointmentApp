using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MedicalAppointmentApi.Models
{
    [Table("appointments")]
    public class Appointment : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        
        [Required]
        [Column("patient_id")]
        public Guid PatientId { get; set; }
        
        [Required]
        [Column("doctor_id")]
        public Guid DoctorId { get; set; }
        
        [Required]
        [Column("appointment_date")]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [Column("appointment_time")]
        public TimeSpan AppointmentTime { get; set; }
        
        [Column("duration_minutes")]
        public int DurationMinutes { get; set; } = 30;
        
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "scheduled";
        
        [Column("notes")]
        public string? Notes { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}