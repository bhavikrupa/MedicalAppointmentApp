import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { PatientManagement } from './components/patient-management/patient-management';
import { AppointmentScheduling } from './components/appointment-scheduling/appointment-scheduling';
import { DoctorSchedule } from './components/doctor-schedule/doctor-schedule';
import { Billing } from './components/billing/billing';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'patients', component: PatientManagement },
  { path: 'appointments', component: AppointmentScheduling },
  { path: 'doctor-schedules', component: DoctorSchedule },
  { path: 'billing', component: Billing },
  { path: '**', redirectTo: '/dashboard' }
];
