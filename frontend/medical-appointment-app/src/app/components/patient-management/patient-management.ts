import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Patient } from '../../services/patient';
import { Patient as PatientModel, CreatePatientDto } from '../../models/patient.interface';

@Component({
  selector: 'app-patient-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './patient-management.html',
  styleUrl: './patient-management.scss'
})
export class PatientManagement implements OnInit {
  patients: PatientModel[] = [];
  patientForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showForm = false;

  constructor(
    private patientService: Patient,
    private fb: FormBuilder
  ) {
    this.patientForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^\(\d{3}\)\s\d{3}-\d{4}$/)]],
      dateOfBirth: ['', [Validators.required]],
      address: [''],
      emergencyContactName: [''],
      emergencyContactPhone: ['', [Validators.pattern(/^\(\d{3}\)\s\d{3}-\d{4}$/)]],
      insuranceProvider: [''],
      insurancePolicyNumber: ['']
    });
  }

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.patientService.getPatients().subscribe({
      next: (response) => {
        console.log('Patient API Response:', response); // Debug log
        if (response.success) {
          this.patients = response.data || [];
          console.log('Loaded patients:', this.patients); // Debug log
          if (this.patients.length === 0) {
            console.warn('No patients found in response');
          }
        } else {
          this.errorMessage = response.message || 'Failed to load patients';
          console.error('API returned unsuccessful:', response);
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading patients:', error);
        this.errorMessage = 'Error loading patients: ' + (error.error?.message || error.message || 'Unknown error');
        this.patients = [];
        this.isLoading = false;
      }
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.patientForm.reset();
      this.errorMessage = '';
      this.successMessage = '';
    }
  }

  onSubmit(): void {
    if (this.patientForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const patientData: CreatePatientDto = this.patientForm.value;

      this.patientService.createPatient(patientData).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Patient created successfully!';
            this.patientForm.reset();
            this.loadPatients();
            setTimeout(() => {
              this.showForm = false;
              this.successMessage = '';
            }, 2000);
          } else {
            this.errorMessage = response.message || 'Failed to create patient';
          }
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Error creating patient: ' + error.message;
          this.isLoading = false;
        }
      });
    } else {
      this.markFormGroupTouched(this.patientForm);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  getErrorMessage(fieldName: string): string {
    const control = this.patientForm.get(fieldName);
    if (control?.hasError('required')) {
      return `${fieldName} is required`;
    }
    if (control?.hasError('email')) {
      return 'Invalid email format';
    }
    if (control?.hasError('minLength')) {
      return `Minimum length is ${control.errors?.['minLength'].requiredLength}`;
    }
    if (control?.hasError('pattern')) {
      return 'Invalid format. Use (555) 123-4567';
    }
    return '';
  }
}
