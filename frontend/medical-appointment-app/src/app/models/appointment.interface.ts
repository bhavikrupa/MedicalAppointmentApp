export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  doctorId: string;
  doctorName: string;
  appointmentDate: Date;
  appointmentTime: string;
  durationMinutes: number;
  status: string;
  notes?: string;
  createdAt: Date;
}

export interface ScheduleAppointmentDto {
  patientId: string;
  doctorId: string;
  appointmentDate: Date;
  appointmentTime: string;
  durationMinutes?: number;
  notes?: string;
}

export interface CompleteAppointmentDto {
  appointmentId: string;
  services: InvoiceServiceDto[];
  taxRate?: number;
  paymentMethod?: string;
}

export interface InvoiceServiceDto {
  serviceId: string;
  quantity: number;
}