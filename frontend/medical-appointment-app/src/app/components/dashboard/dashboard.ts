import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
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

  ngOnInit() {
    // TODO: Load actual stats from API
    this.loadDashboardStats();
  }

  private loadDashboardStats() {
    // Mock data for now
    this.stats = {
      totalPatients: 245,
      totalAppointments: 1200,
      todaysAppointments: 8,
      pendingBills: 15
    };
  }
}
