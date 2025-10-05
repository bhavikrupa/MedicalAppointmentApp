import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Api } from './api';
import { Invoice as InvoiceModel, CreateInvoiceDto, Service } from '../models/service.interface';
import { ApiResponse } from '../models/api-response.interface';

@Injectable({
  providedIn: 'root'
})
export class Invoice {

  constructor(private apiService: Api) { }

  getInvoices(): Observable<ApiResponse<InvoiceModel[]>> {
    return this.apiService.get<InvoiceModel[]>('invoices');
  }

  createInvoice(invoice: CreateInvoiceDto): Observable<ApiResponse<InvoiceModel>> {
    return this.apiService.post<InvoiceModel>('invoices', invoice);
  }

  getServices(): Observable<ApiResponse<Service[]>> {
    return this.apiService.get<Service[]>('services');
  }
}
