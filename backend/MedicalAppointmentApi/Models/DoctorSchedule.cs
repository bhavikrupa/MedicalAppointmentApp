using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MedicalAppointmentApi.Models
{
    [Table("doctor_schedules")]
    public class DoctorSchedule : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        
        [Required]
        [Column("doctor_id")]
        public Guid DoctorId { get; set; }
        
        [Required]
        [Range(0, 6)] // 0 = Sunday, 6 = Saturday
        [Column("day_of_week")]
        public int DayOfWeek { get; set; }
        
        [Required]
        [Column("start_time")]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }
        
        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual Doctor Doctor { get; set; } = null!;
    }
}