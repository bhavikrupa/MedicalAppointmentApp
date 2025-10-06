import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { Patient } from '../../services/patient';
import { Appointment } from '../../services/appointment';
import { Invoice } from '../../services/invoice';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  stats = {
    totalPatients: 0,
    totalAppointments: 0,
    todaysAppointments: 0,
    pendingBills: 0
  };

  isLoading = false;

  constructor(
    private router: Router,
    private patientService: Patient,
    private appointmentService: Appointment,
    private invoiceService: Invoice
  ) {}

  ngOnInit() {
    this.loadDashboardStats();
  }

  private loadDashboardStats() {
    this.isLoading = true;

    // Load patients
    this.patientService.getPatients().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.stats.totalPatients = response.data.length;
        }
      },
      error: (error) => console.error('Error loading patients:', error)
    });

    // Load appointments
    this.appointmentService.getAppointments().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.stats.totalAppointments = response.data.length;
          
          // Count today's appointments
          const today = new Date().toDateString();
          this.stats.todaysAppointments = response.data.filter(apt => 
            new Date(apt.appointmentDate).toDateString() === today
          ).length;
        }
      },
      error: (error) => console.error('Error loading appointments:', error)
    });

    // Load invoices
    this.invoiceService.getInvoices().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // Count unpaid invoices
          this.stats.pendingBills = response.data.filter(
            inv => inv.status?.toLowerCase() === 'pending'
          ).length;
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading invoices:', error);
        this.isLoading = false;
      }
    });
  }

  navigateToPatients(): void {
    this.router.navigate(['/patients']);
  }

  navigateToAppointments(): void {
    this.router.navigate(['/appointments']);
  }

  navigateToBilling(): void {
    this.router.navigate(['/billing']);
  }
}
