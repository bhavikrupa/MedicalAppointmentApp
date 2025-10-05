import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Api } from './api';
import { Patient as PatientModel, CreatePatientDto } from '../models/patient.interface';
import { ApiResponse } from '../models/api-response.interface';

@Injectable({
  providedIn: 'root'
})
export class Patient {

  constructor(private apiService: Api) { }

  getPatients(): Observable<ApiResponse<PatientModel[]>> {
    return this.apiService.get<PatientModel[]>('patients');
  }

  createPatient(patient: CreatePatientDto): Observable<ApiResponse<PatientModel>> {
    return this.apiService.post<PatientModel>('patients', patient);
  }
}
