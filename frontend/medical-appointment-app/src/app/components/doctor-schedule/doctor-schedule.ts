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
        console.log('Doctors API Response:', response); // Debug log
        if (response.success) {
          this.doctors = (response.data || []).filter(d => d.isActive);
          console.log('Loaded doctors:', this.doctors); // Debug log
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

  loadDoctorSchedule(): void {
    if (!this.selectedDoctorId) {
      console.warn('No doctor selected');
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    
    const date = new Date(this.selectedDate);
    const startOfWeek = new Date(date);
    startOfWeek.setDate(date.getDate() - date.getDay());
    const endOfWeek = new Date(startOfWeek);
    endOfWeek.setDate(startOfWeek.getDate() + 6);
    
    console.log('Loading schedule for:', {
      doctorId: this.selectedDoctorId,
      startDate: startOfWeek.toISOString().split('T')[0],
      endDate: endOfWeek.toISOString().split('T')[0]
    });
    
    this.doctorService.getDoctorSchedule(
      this.selectedDoctorId,
      startOfWeek.toISOString().split('T')[0],
      endOfWeek.toISOString().split('T')[0]
    ).subscribe({
      next: (response) => {
        console.log('Schedule API Response:', response); // Debug log
        if (response.success) {
          this.schedules = response.data || [];
          console.log('Loaded schedules:', this.schedules); // Debug log
          if (this.schedules.length === 0) {
            console.warn('No schedule found for selected doctor and date range');
          }
          this.loadAppointments();
        } else {
          this.errorMessage = response.message || 'Failed to load schedule';
          console.error('API returned unsuccessful:', response);
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading schedule:', error);
        this.errorMessage = 'Error loading schedule: ' + (error.error?.message || error.message || 'Unknown error');
        this.schedules = [];
        this.isLoading = false;
      }
    });
  }

  loadAppointments(): void {
    if (!this.selectedDoctorId || !this.selectedDate) {
      console.warn('Missing doctorId or selectedDate');
      return;
    }

    this.appointmentService.getAppointments().subscribe({
      next: (response) => {
        console.log('Appointments API Response:', response); // Debug log
        if (response.success) {
          const allAppointments = response.data || [];
          this.appointments = allAppointments.filter(a => {
            const matchesDoctor = a.doctorId === this.selectedDoctorId;
            const matchesDate = this.isSameDate(new Date(a.appointmentDate), new Date(this.selectedDate));
            return matchesDoctor && matchesDate;
          });
          console.log('Filtered appointments:', this.appointments);
        } else {
          console.error('Failed to load appointments:', response.message);
        }
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
      }
    });
  }

  getCurrentDate(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatTime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const period = hours >= 12 ? 'PM' : 'AM';
    const displayHours = hours % 12 || 12;
    return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
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

  formatTimeRange(startTime: string | number, endTime: string | number): string {
    const startSeconds = this.timeToSeconds(startTime);
    const endSeconds = this.timeToSeconds(endTime);
    return `${this.formatTime(startSeconds)} - ${this.formatTime(endSeconds)}`;
  }

  getScheduleForDate(date: Date): ScheduleModel | undefined {
    return this.schedules.find(s => 
      this.isSameDate(new Date(s.date), date)
    );
  }

  parseTimeToSeconds(timeStr: string): number {
    const [hours, minutes] = timeStr.split(':').map(Number);
    return hours * 3600 + minutes * 60;
  }

  getTimeSlots(): number[] {
    if (this.schedules.length === 0) {
      console.warn('No schedules available to generate time slots');
      return [];
    }

    const schedule = this.schedules.find(s => 
      this.isSameDate(new Date(s.date), new Date(this.selectedDate))
    );

    if (!schedule) {
      console.warn('No schedule found for selected date:', this.selectedDate);
      return [];
    }

    const slots: number[] = [];
    const startSeconds = this.timeToSeconds(schedule.startTime);
    const endSeconds = this.timeToSeconds(schedule.endTime);
    const slotDuration = 30 * 60; // 30 minutes in seconds

    for (let time = startSeconds; time < endSeconds; time += slotDuration) {
      slots.push(time);
    }

    console.log('Generated time slots:', slots);
    return slots;
  }

  timeToSeconds(time: any): number {
    if (typeof time === 'string') {
      const parts = time.split(':');
      return parseInt(parts[0]) * 3600 + parseInt(parts[1]) * 60 + (parseInt(parts[2]) || 0);
    }
    if (typeof time === 'number') {
      return time;
    }
    return 0;
  }

  isSlotAvailable(timeSlot: number): boolean {
    if (!this.appointments || this.appointments.length === 0) {
      return true;
    }

    return !this.appointments.some(apt => {
      const aptTime = this.timeToSeconds(apt.appointmentTime);
      return aptTime === timeSlot;
    });
  }

  getAppointmentsForTime(timeSlot: number): AppointmentModel[] {
    return this.appointments.filter(apt => {
      const aptTime = this.timeToSeconds(apt.appointmentTime);
      return aptTime === timeSlot;
    });
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
