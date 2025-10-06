using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentApp.Models.DTOs
{
    public class CreatePatientDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string? Address { get; set; }
        
        [StringLength(100)]
        public string? EmergencyContactName { get; set; }
        
        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }
        
        [StringLength(100)]
        public string? InsuranceProvider { get; set; }
        
        [StringLength(50)]
        public string? InsurancePolicyNumber { get; set; }
    }
}
