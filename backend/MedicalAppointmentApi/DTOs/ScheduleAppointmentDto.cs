using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApi.DTOs
{
    public class ScheduleAppointmentDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public Guid DoctorId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        public TimeSpan AppointmentTime { get; set; }
        
        public int DurationMinutes { get; set; } = 30;
        
        public string? Notes { get; set; }
    }
}