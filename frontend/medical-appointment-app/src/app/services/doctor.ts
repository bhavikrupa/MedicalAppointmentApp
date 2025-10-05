import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Api } from './api';
import { Doctor as DoctorModel, DoctorSchedule, TimeSlot } from '../models/doctor.interface';
import { ApiResponse } from '../models/api-response.interface';

@Injectable({
  providedIn: 'root'
})
export class Doctor {

  constructor(private apiService: Api) { }

  getDoctors(): Observable<ApiResponse<DoctorModel[]>> {
    return this.apiService.get<DoctorModel[]>('doctors');
  }

  getDoctorSchedule(doctorId: string, startDate: string, endDate: string): Observable<ApiResponse<DoctorSchedule[]>> {
    return this.apiService.get<DoctorSchedule[]>(`doctors/${doctorId}/schedule?startDate=${startDate}&endDate=${endDate}`);
  }

  getAvailableTimeSlots(doctorId: string, appointmentDate: string, durationMinutes: number = 30): Observable<ApiResponse<TimeSlot[]>> {
    return this.apiService.get<TimeSlot[]>(`doctors/${doctorId}/available-slots?appointmentDate=${appointmentDate}&durationMinutes=${durationMinutes}`);
  }
}
