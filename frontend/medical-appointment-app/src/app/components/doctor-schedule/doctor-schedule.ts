import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Doctor } from '../../services/doctor';
import { Appointment } from '../../services/appointment';
import { Doctor as DoctorModel, DoctorSchedule as ScheduleModel } from '../../models/doctor.interface';
import { Appointment as AppointmentModel } from '../../models/appointment.interface';

@Component({
  selector: 'app-doctor-schedule',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './doctor-schedule.html',
  styleUrl: './doctor-schedule.scss'
})
export class DoctorSchedule implements OnInit {
  doctors: DoctorModel[] = [];
  schedules: ScheduleModel[] = [];
  appointments: AppointmentModel[] = [];
  selectedDoctorId: string | null = null;
  selectedDate: string = new Date().toISOString().split('T')[0];
  filterForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  daysOfWeek = [
    { value: 0, label: 'Sunday' },
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' }
  ];

  constructor(
    private doctorService: Doctor,
    private appointmentService: Appointment,
    private fb: FormBuilder
  ) {
    this.filterForm = this.fb.group({
      doctorId: ['', [Validators.required]],
      date: [this.selectedDate, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadDoctors();
    this.setupFilterWatcher();
  }

  setupFilterWatcher(): void {
    this.filterForm.get('doctorId')?.valueChanges.subscribe((doctorId) => {
      this.selectedDoctorId = doctorId;
      this.loadDoctorSchedule();
      this.loadAppointments();
    });

    this.filterForm.get('date')?.valueChanges.subscribe((date) => {
      this.selectedDate = date;
      this.loadAppointments();
    });
  }

  loadDoctors(): void {
    this.doctorService.getDoctors().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.doctors = response.data.filter(d => d.isActive);
          if (this.doctors.length > 0) {
            this.filterForm.patchValue({ doctorId: this.doctors[0].id });
          }
        }
      },
      error: (error) => {
        this.errorMessage = 'Error loading doctors: ' + error.message;
      }
    });
  }

  loadDoctorSchedule(): void {
    if (!this.selectedDoctorId) return;

    this.isLoading = true;
    
    // Get the start of the week and end of the week
    const today = new Date();
    const startOfWeek = new Date(today.setDate(today.getDate() - today.getDay()));
    const endOfWeek = new Date(today.setDate(today.getDate() + 6));
    
    this.doctorService.getDoctorSchedule(
      this.selectedDoctorId,
      startOfWeek.toISOString().split('T')[0],
      endOfWeek.toISOString().split('T')[0]
    ).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.schedules = response.data;
        } else {
          this.errorMessage = response.message || 'Failed to load schedule';
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Error loading schedule: ' + error.message;
        this.isLoading = false;
      }
    });
  }

  loadAppointments(): void {
    if (!this.selectedDoctorId || !this.selectedDate) return;

    this.appointmentService.getAppointments().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.appointments = response.data.filter(a => 
            a.doctorId === this.selectedDoctorId &&
            this.isSameDate(new Date(a.appointmentDate), new Date(this.selectedDate))
          );
        }
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
      }
    });
  }

  isSameDate(date1: Date, date2: Date): boolean {
    return date1.getFullYear() === date2.getFullYear() &&
           date1.getMonth() === date2.getMonth() &&
           date1.getDate() === date2.getDate();
  }

  getDayOfWeekLabel(dayOfWeek: number): string {
    const day = this.daysOfWeek.find(d => d.value === dayOfWeek);
    return day ? day.label : 'Unknown';
  }

  formatTime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const period = hours >= 12 ? 'PM' : 'AM';
    const displayHours = hours % 12 || 12;
    return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
  }

  formatTimeRange(startTime: number, endTime: number): string {
    return `${this.formatTime(startTime)} - ${this.formatTime(endTime)}`;
  }

  getScheduleForDate(date: Date): ScheduleModel | undefined {
    const dayOfWeek = date.getDay();
    return this.schedules.find(s => s.dayOfWeek === dayOfWeek && s.isActive);
  }

  getAppointmentsForTime(startTime: number): AppointmentModel[] {
    return this.appointments.filter(a => {
      const appointmentTime = this.parseTimeToSeconds(a.appointmentTime);
      return appointmentTime === startTime;
    });
  }

  parseTimeToSeconds(timeStr: string): number {
    const [hours, minutes] = timeStr.split(':').map(Number);
    return hours * 3600 + minutes * 60;
  }

  getTimeSlots(): number[] {
    const schedule = this.getScheduleForDate(new Date(this.selectedDate));
    if (!schedule) return [];

    const slots: number[] = [];
    const slotDuration = 30 * 60; // 30 minutes in seconds
    
    for (let time = schedule.startTime; time < schedule.endTime; time += slotDuration) {
      slots.push(time);
    }
    
    return slots;
  }

  isSlotAvailable(startTime: number): boolean {
    const appointmentsAtTime = this.getAppointmentsForTime(startTime);
    return appointmentsAtTime.length === 0;
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

  getSelectedDoctor(): DoctorModel | undefined {
    return this.doctors.find(d => d.id === this.selectedDoctorId);
  }

  hasScheduleForSelectedDate(): boolean {
    if (!this.selectedDate) return false;
    const date = new Date(this.selectedDate);
    return this.getScheduleForDate(date) !== undefined;
  }

  getSelectedDateAsDate(): Date {
    return new Date(this.selectedDate);
  }
}
