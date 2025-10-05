import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { Invoice } from '../../services/invoice';
import { Appointment } from '../../services/appointment';
import { Service } from '../../services/service';
import { Invoice as InvoiceModel, CreateInvoiceDto } from '../../models/service.interface';
import { Appointment as AppointmentModel } from '../../models/appointment.interface';
import { Service as ServiceModel } from '../../models/service.interface';

@Component({
  selector: 'app-billing',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './billing.html',
  styleUrl: './billing.scss'
})
export class Billing implements OnInit {
  invoices: InvoiceModel[] = [];
  completedAppointments: AppointmentModel[] = [];
  services: ServiceModel[] = [];
  invoiceForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showForm = false;
  selectedAppointment: AppointmentModel | null = null;

  constructor(
    private invoiceService: Invoice,
    private appointmentService: Appointment,
    private serviceService: Service,
    private fb: FormBuilder
  ) {
    this.invoiceForm = this.fb.group({
      appointmentId: ['', [Validators.required]],
      items: this.fb.array([]),
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadInvoices();
    this.loadCompletedAppointments();
    this.loadServices();
  }

  get items(): FormArray {
    return this.invoiceForm.get('items') as FormArray;
  }

  loadInvoices(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.invoiceService.getInvoices().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.invoices = response.data;
        } else {
          this.errorMessage = response.message || 'Failed to load invoices';
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Error loading invoices: ' + error.message;
        this.isLoading = false;
      }
    });
  }

  loadCompletedAppointments(): void {
    this.appointmentService.getAppointments().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.completedAppointments = response.data.filter(
            a => a.status.toLowerCase() === 'completed' && !this.hasInvoice(a.id)
          );
        }
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
      }
    });
  }

  loadServices(): void {
    this.serviceService.getServices().subscribe({
      next: (response: any) => {
        if (response.success && response.data) {
          this.services = response.data.filter((s: any) => s.isActive);
        }
      },
      error: (error: any) => {
        console.error('Error loading services:', error);
      }
    });
  }

  hasInvoice(appointmentId: string): boolean {
    return this.invoices.some(inv => inv.appointmentId === appointmentId);
  }

  onAppointmentSelect(): void {
    const appointmentId = this.invoiceForm.get('appointmentId')?.value;
    this.selectedAppointment = this.completedAppointments.find(a => a.id === appointmentId) || null;
  }

  addItem(): void {
    const itemGroup = this.fb.group({
      serviceId: ['', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      discount: [0, [Validators.min(0), Validators.max(100)]]
    });

    itemGroup.get('serviceId')?.valueChanges.subscribe((serviceId) => {
      const service = this.services.find(s => s.id === serviceId);
      if (service) {
        itemGroup.patchValue({ unitPrice: service.price }, { emitEvent: false });
      }
    });

    this.items.push(itemGroup);
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
  }

  getItemTotal(index: number): number {
    const item = this.items.at(index);
    const quantity = item.get('quantity')?.value || 0;
    const unitPrice = item.get('unitPrice')?.value || 0;
    const discount = item.get('discount')?.value || 0;
    const subtotal = quantity * unitPrice;
    return subtotal - (subtotal * discount / 100);
  }

  getInvoiceTotal(): number {
    let total = 0;
    for (let i = 0; i < this.items.length; i++) {
      total += this.getItemTotal(i);
    }
    return total;
  }

  getServiceName(serviceId: string): string {
    const service = this.services.find(s => s.id === serviceId);
    return service ? service.name : 'Unknown';
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.invoiceForm.reset();
      this.items.clear();
      this.errorMessage = '';
      this.successMessage = '';
      this.selectedAppointment = null;
    }
  }

  onSubmit(): void {
    if (this.invoiceForm.valid && this.items.length > 0) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const invoiceData: CreateInvoiceDto = {
        appointmentId: this.invoiceForm.value.appointmentId,
        items: this.items.value.map((item: any) => ({
          serviceId: item.serviceId,
          quantity: item.quantity,
          unitPrice: item.unitPrice,
          discount: item.discount || 0
        })),
        notes: this.invoiceForm.value.notes || null
      };

      this.invoiceService.createInvoice(invoiceData).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Invoice created successfully!';
            this.invoiceForm.reset();
            this.items.clear();
            this.loadInvoices();
            this.loadCompletedAppointments();
            setTimeout(() => {
              this.showForm = false;
              this.successMessage = '';
            }, 2000);
          } else {
            this.errorMessage = response.message || 'Failed to create invoice';
          }
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Error creating invoice: ' + error.message;
          this.isLoading = false;
        }
      });
    } else {
      this.errorMessage = 'Please fill in all required fields and add at least one item.';
      this.markFormGroupTouched(this.invoiceForm);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
      
      if (control instanceof FormArray) {
        control.controls.forEach(group => {
          if (group instanceof FormGroup) {
            this.markFormGroupTouched(group);
          }
        });
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'paid':
        return 'status-paid';
      case 'pending':
        return 'status-pending';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return '';
    }
  }
}
