export interface Service {
  id: string;
  name: string;
  description?: string;
  price: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface Invoice {
  id: string;
  patientId: string;
  patientName: string;
  appointmentId?: string;
  invoiceNumber: string;
  invoiceDate: Date;
  subtotal: number;
  taxAmount: number;
  totalAmount: number;
  status: string;
  paymentMethod?: string;
  paymentDate?: Date;
  notes?: string;
  items: InvoiceItem[];
}

export interface InvoiceItem {
  id: string;
  serviceId: string;
  serviceName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface CreateInvoiceDto {
  appointmentId: string;
  items: InvoiceItemDto[];
  notes?: string;
}

export interface InvoiceItemDto {
  serviceId: string;
  quantity: number;
  unitPrice: number;
  discount?: number;
}