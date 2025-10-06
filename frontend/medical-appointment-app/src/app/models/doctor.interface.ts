export interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  specialization?: string;
  licenseNumber?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface DoctorSchedule {
  date: string | Date;
  dayName: string;
  startTime: string | number;
  endTime: string | number;
  isAvailable: boolean;
  appointments?: AppointmentSlot[];
}

export interface DoctorScheduleDb {
  id: string;
  doctorId: string;
  dayOfWeek: number;
  startTime: number;
  endTime: number;
  maxPatientsPerDay: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface AppointmentSlot {
  appointmentId?: string;
  appointmentTime: string;
  patientName?: string;
  status?: string;
}

export interface TimeSlot {
  timeSlot: string | number;
  isAvailable: boolean;
}