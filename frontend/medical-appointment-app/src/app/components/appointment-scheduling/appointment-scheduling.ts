import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Appointment } from '../../services/appointment';
import { Patient } from '../../services/patient';
import { Doctor } from '../../services/doctor';
import { Appointment as AppointmentModel, ScheduleAppointmentDto } from '../../models/appointment.interface';
import { Patient as PatientModel } from '../../models/patient.interface';
import { Doctor as DoctorModel, TimeSlot } from '../../models/doctor.interface';

@Component({
  selector: 'app-appointment-scheduling',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './appointment-scheduling.html',
  styleUrl: './appointment-scheduling.scss'
})
export class AppointmentScheduling implements OnInit {
  appointments: AppointmentModel[] = [];
  patients: PatientModel[] = [];
  doctors: DoctorModel[] = [];
  availableTimeSlots: TimeSlot[] = [];
  appointmentForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showForm = false;
  loadingTimeSlots = false;

  constructor(
    private appointmentService: Appointment,
    private patientService: Patient,
    private doctorService: Doctor,
    private fb: FormBuilder
  ) {
    this.appointmentForm = this.fb.group({
      patientId: ['', [Validators.required]],
      doctorId: ['', [Validators.required]],
      appointmentDate: ['', [Validators.required]],
      appointmentTime: ['', [Validators.required]],
      durationMinutes: [30, [Validators.required, Validators.min(15)]],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadAppointments();
    this.loadPatients();
    this.loadDoctors();
    this.setupDateAndDoctorWatcher();
  }

  getCurrentDate(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  setupDateAndDoctorWatcher(): void {
    this.appointmentForm.get('doctorId')?.valueChanges.subscribe(() => {
      this.loadAvailableTimeSlots();
    });

    this.appointmentForm.get('appointmentDate')?.valueChanges.subscribe(() => {
      this.loadAvailableTimeSlots();
    });

    this.appointmentForm.get('durationMinutes')?.valueChanges.subscribe(() => {
      this.loadAvailableTimeSlots();
    });
  }

  loadAppointments(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.appointmentService.getAppointments().subscribe({
      next: (response) => {
        console.log('Appointment API Response:', response); // Debug log
        if (response.success) {
          this.appointments = response.data || [];
          console.log('Loaded appointments:', this.appointments); // Debug log
          
          // Format appointment time if it's a TimeSpan object
          this.appointments = this.appointments.map(apt => ({
            ...apt,
            appointmentTime: this.formatAppointmentTime(apt.appointmentTime)
          }));
          
          if (this.appointments.length === 0) {
            console.log('No appointments found. Please schedule appointments.');
          }
        } else {
          this.errorMessage = response.message || 'Failed to load appointments';
          console.error('API returned unsuccessful:', response);
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
        this.errorMessage = 'Error loading appointments: ' + (error.error?.message || error.message || 'Unknown error');
        this.appointments = [];
        this.isLoading = false;
      }
    });
  }

  formatAppointmentTime(time: any): string {
    if (typeof time === 'string') {
      // If it's already a string like "10:00:00", format it
      const parts = time.split(':');
      if (parts.length >= 2) {
        const hours = parseInt(parts[0]);
        const minutes = parts[1];
        const period = hours >= 12 ? 'PM' : 'AM';
        const displayHours = hours % 12 || 12;
        return `${displayHours}:${minutes} ${period}`;
      }
      return time;
    }
    return time.toString();
  }

  loadPatients(): void {
    this.patientService.getPatients().subscribe({
      next: (response) => {
        if (response.success) {
          this.patients = (response.data || []).filter(p => p.isActive);
          if (this.patients.length === 0) {
            console.warn('No active patients found. Please add patients first.');
          }
        } else {
          console.error('Failed to load patients:', response.message);
          this.patients = [];
        }
      },
      error: (error) => {
        console.error('Error loading patients:', error);
        this.patients = [];
      }
    });
  }

  loadDoctors(): void {
    this.doctorService.getDoctors().subscribe({
      next: (response) => {
        if (response.success) {
          this.doctors = (response.data || []).filter(d => d.isActive);
          if (this.doctors.length === 0) {
            console.warn('No active doctors found. Please add doctors first.');
          }
        } else {
          console.error('Failed to load doctors:', response.message);
          this.doctors = [];
        }
      },
      error: (error) => {
        console.error('Error loading doctors:', error);
        this.doctors = [];
      }
    });
  }

  loadAvailableTimeSlots(): void {
    const doctorId = this.appointmentForm.get('doctorId')?.value;
    const appointmentDate = this.appointmentForm.get('appointmentDate')?.value;
    const durationMinutes = this.appointmentForm.get('durationMinutes')?.value || 30;

    if (doctorId && appointmentDate) {
      this.loadingTimeSlots = true;
      this.availableTimeSlots = [];

      this.doctorService.getAvailableTimeSlots(
        doctorId,
        appointmentDate,
        durationMinutes
      ).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.availableTimeSlots = response.data.filter(slot => slot.isAvailable);
          }
          this.loadingTimeSlots = false;
        },
        error: (error) => {
          console.error('Error loading time slots:', error);
          this.loadingTimeSlots = false;
        }
      });
    }
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.appointmentForm.reset({ durationMinutes: 30 });
      this.errorMessage = '';
      this.successMessage = '';
      this.availableTimeSlots = [];
    }
  }

  onSubmit(): void {
    if (this.appointmentForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const appointmentData: ScheduleAppointmentDto = {
        ...this.appointmentForm.value,
        appointmentDate: new Date(this.appointmentForm.value.appointmentDate)
      };

      this.appointmentService.scheduleAppointment(appointmentData).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Appointment scheduled successfully!';
            this.appointmentForm.reset({ durationMinutes: 30 });
            this.loadAppointments();
            setTimeout(() => {
              this.showForm = false;
              this.successMessage = '';
            }, 2000);
          } else {
            this.errorMessage = response.message || 'Failed to schedule appointment';
          }
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Error scheduling appointment: ' + error.message;
          this.isLoading = false;
        }
      });
    } else {
      this.markFormGroupTouched(this.appointmentForm);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  getPatientName(patientId: string): string {
    const patient = this.patients.find(p => p.id === patientId);
    return patient ? `${patient.firstName} ${patient.lastName}` : 'Unknown';
  }

  getDoctorName(doctorId: string): string {
    const doctor = this.doctors.find(d => d.id === doctorId);
    return doctor ? `Dr. ${doctor.firstName} ${doctor.lastName}` : 'Unknown';
  }

  formatTimeSlot(timeSlot: TimeSlot): string {
    const timeValue = typeof timeSlot.timeSlot === 'string' ? this.parseTimeToSeconds(timeSlot.timeSlot) : timeSlot.timeSlot;
    const hours = Math.floor(timeValue / 3600);
    const minutes = Math.floor((timeValue % 3600) / 60);
    const period = hours >= 12 ? 'PM' : 'AM';
    const displayHours = hours % 12 || 12;
    return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
  }

  parseTimeToSeconds(timeStr: string): number {
    const [hours, minutes] = timeStr.split(':').map(Number);
    return hours * 3600 + minutes * 60;
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'scheduled':
        return 'status-scheduled';
      case 'completed':
        return 'status-completed';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return '';
    }
  }
}
