import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Api } from './api';
import { Appointment as AppointmentModel, ScheduleAppointmentDto, CompleteAppointmentDto } from '../models/appointment.interface';
import { ApiResponse } from '../models/api-response.interface';

@Injectable({
  providedIn: 'root'
})
export class Appointment {

  constructor(private apiService: Api) { }

  getAppointments(): Observable<ApiResponse<AppointmentModel[]>> {
    return this.apiService.get<AppointmentModel[]>('appointments');
  }

  scheduleAppointment(appointment: ScheduleAppointmentDto): Observable<ApiResponse<AppointmentModel>> {
    return this.apiService.post<AppointmentModel>('appointments', appointment);
  }

  completeAppointment(appointmentId: string, completeData: CompleteAppointmentDto): Observable<ApiResponse<any>> {
    return this.apiService.post<any>(`appointments/${appointmentId}/complete`, completeData);
  }
}
