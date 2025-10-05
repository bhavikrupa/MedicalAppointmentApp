import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.interface';

export const API_BASE_URL = 'http://localhost:5236/api'; // Export for use in other services

@Injectable({
  providedIn: 'root'
})
export class Api {
  private readonly baseUrl = API_BASE_URL;
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) { }

  get<T>(endpoint: string): Observable<ApiResponse<T>> {
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, this.httpOptions);
  }

  post<T>(endpoint: string, data: any): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, data, this.httpOptions);
  }

  put<T>(endpoint: string, data: any): Observable<ApiResponse<T>> {
    return this.http.put<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, data, this.httpOptions);
  }

  delete<T>(endpoint: string): Observable<ApiResponse<T>> {
    return this.http.delete<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, this.httpOptions);
  }
}
