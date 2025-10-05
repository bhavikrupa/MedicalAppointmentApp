import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api';
import { ApiResponse } from '../models/api-response.interface';
import { Service as ServiceModel } from '../models/service.interface';

@Injectable({
  providedIn: 'root'
})
export class Service {
  private apiUrl = `${API_BASE_URL}/services`;

  constructor(private http: HttpClient) { }

  getServices(): Observable<ApiResponse<ServiceModel[]>> {
    return this.http.get<ApiResponse<ServiceModel[]>>(this.apiUrl);
  }

  getServiceById(id: string): Observable<ApiResponse<ServiceModel>> {
    return this.http.get<ApiResponse<ServiceModel>>(`${this.apiUrl}/${id}`);
  }

  createService(service: Partial<ServiceModel>): Observable<ApiResponse<ServiceModel>> {
    return this.http.post<ApiResponse<ServiceModel>>(this.apiUrl, service);
  }

  updateService(id: string, service: Partial<ServiceModel>): Observable<ApiResponse<ServiceModel>> {
    return this.http.put<ApiResponse<ServiceModel>>(`${this.apiUrl}/${id}`, service);
  }

  deleteService(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }
}
