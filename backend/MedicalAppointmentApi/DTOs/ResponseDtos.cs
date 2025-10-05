namespace MedicalAppointmentApi.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
    
    public class PatientResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class AppointmentResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class InvoiceResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid? AppointmentId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Notes { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new List<InvoiceItemResponseDto>();
    }
    
    public class InvoiceItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    
    public class DoctorScheduleResponseDto
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public List<AppointmentSlotDto> Appointments { get; set; } = new List<AppointmentSlotDto>();
    }
    
    public class AppointmentSlotDto
    {
        public Guid? AppointmentId { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string? PatientName { get; set; }
        public string? Status { get; set; }
    }
    
    public class TimeSlotDto
    {
        public TimeSpan TimeSlot { get; set; }
        public bool IsAvailable { get; set; }
    }
}