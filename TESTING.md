# Testing Guide

## 🧪 Testing Strategy

This document outlines the testing approach for the Medical Appointment Application, covering unit tests, integration tests, and end-to-end tests.

## 📋 Testing Checklist

### Backend API Testing

#### ✅ Patient Management
- [ ] Create new patient with valid data
- [ ] Create patient with missing required fields (should fail)
- [ ] Create patient with invalid email format (should fail)
- [ ] Create patient with invalid phone format (should fail)
- [ ] Get all patients
- [ ] Get patient by ID
- [ ] Update patient information
- [ ] Soft delete patient (set isActive = false)

#### ✅ Appointment Scheduling
- [ ] Schedule appointment with available time slot
- [ ] Schedule appointment with conflicting time (should fail)
- [ ] Schedule appointment outside doctor's working hours (should fail)
- [ ] Get all appointments
- [ ] Get appointments for specific doctor
- [ ] Get appointments for specific patient
- [ ] Complete appointment
- [ ] Cancel appointment

#### ✅ Doctor Schedule Management
- [ ] Get doctor schedule for specific day
- [ ] Get available time slots for doctor
- [ ] Verify time slot availability calculation
- [ ] Check schedule conflicts

#### ✅ Billing & Invoicing
- [ ] Create invoice for completed appointment
- [ ] Create invoice with multiple service items
- [ ] Calculate subtotal, tax, and total correctly
- [ ] Create invoice for non-completed appointment (should fail)
- [ ] Get all invoices
- [ ] Get invoice by ID
- [ ] Generate invoice number correctly

#### ✅ Services Management
- [ ] Get all services
- [ ] Create new service
- [ ] Update service pricing
- [ ] Deactivate service

### Frontend Component Testing

#### ✅ Patient Management Component
- [ ] Load patients on component init
- [ ] Display patient list correctly
- [ ] Open/close patient form
- [ ] Submit valid patient form
- [ ] Display validation errors for invalid form
- [ ] Required field validation (first name, last name, phone, DOB)
- [ ] Email format validation
- [ ] Phone format validation (XXX) XXX-XXXX
- [ ] Success message after creating patient
- [ ] Error message handling

#### ✅ Appointment Scheduling Component
- [ ] Load appointments, patients, and doctors on init
- [ ] Filter patients and doctors (only active)
- [ ] Load available time slots when doctor and date selected
- [ ] Display time slots correctly
- [ ] Select time slot
- [ ] Submit valid appointment form
- [ ] Display validation errors
- [ ] Required field validation
- [ ] Success message after scheduling
- [ ] Refresh appointment list after creation

#### ✅ Billing Component
- [ ] Load invoices and completed appointments
- [ ] Filter only completed appointments without invoices
- [ ] Add invoice items
- [ ] Remove invoice items
- [ ] Calculate item totals correctly
- [ ] Calculate invoice total correctly
- [ ] Auto-populate service price
- [ ] Apply discount calculation
- [ ] Submit valid invoice
- [ ] Validation for empty items array

#### ✅ Doctor Schedule Component
- [ ] Load doctors on init
- [ ] Select doctor from dropdown
- [ ] Display weekly schedule
- [ ] Display daily schedule for selected date
- [ ] Show available and booked time slots
- [ ] Format time display correctly (12-hour format)
- [ ] Color coding for availability

## 🔬 Manual Testing Procedures

### Test Scenario 1: Complete Patient Visit Workflow

**Objective**: Test the entire workflow from patient registration to billing

**Steps**:
1. Navigate to Patient Management
2. Click "Add New Patient"
3. Fill in patient information:
   - First Name: John
   - Last Name: Doe
   - Email: john.doe@email.com
   - Phone: (555) 123-4567
   - Date of Birth: 01/15/1980
   - Address: 123 Main St
4. Submit form
5. Verify patient appears in patient list
6. Navigate to Appointment Scheduling
7. Click "Schedule New Appointment"
8. Select patient: John Doe
9. Select doctor: Dr. Smith
10. Select date: Tomorrow's date
11. Select duration: 30 minutes
12. Select available time slot
13. Add notes: "Regular checkup"
14. Submit form
15. Verify appointment appears in appointment list
16. Mark appointment as "Completed" (via API or UI)
17. Navigate to Billing
18. Click "Create Invoice"
19. Select the completed appointment
20. Click "Add Item"
21. Select service: "General Consultation"
22. Set quantity: 1
23. Verify unit price auto-populated
24. Add discount: 10%
25. Verify total calculation
26. Submit invoice
27. Verify invoice appears in invoice list

**Expected Result**: Complete workflow executes successfully with proper data flow

### Test Scenario 2: Appointment Conflict Detection

**Objective**: Verify that double-booking is prevented

**Steps**:
1. Create first appointment:
   - Patient: Alice
   - Doctor: Dr. Smith
   - Date: Tomorrow
   - Time: 10:00 AM
   - Duration: 30 minutes
2. Attempt to create second appointment:
   - Patient: Bob
   - Doctor: Dr. Smith
   - Date: Tomorrow
   - Time: 10:15 AM (overlaps with first appointment)
   - Duration: 30 minutes
3. Submit form

**Expected Result**: Error message indicating time slot is not available

### Test Scenario 3: Form Validation

**Objective**: Verify all form validation rules

**Patient Form Validation**:
- Leave first name empty → Error: "firstName is required"
- Leave last name empty → Error: "lastName is required"
- Enter invalid email "notanemail" → Error: "Invalid email format"
- Enter invalid phone "1234567" → Error: "Invalid format. Use (555) 123-4567"
- Leave DOB empty → Error: "dateOfBirth is required"

**Appointment Form Validation**:
- Don't select patient → Error: "Patient is required"
- Don't select doctor → Error: "Doctor is required"
- Don't select date → Error: "Date is required"
- Don't select time → Error: "Time is required"

**Invoice Form Validation**:
- Don't select appointment → Error: "Appointment is required"
- Try to submit without items → Error: "Please add at least one item"

## 🔧 API Testing with Thunder Client / Postman

### Setup
1. Install Thunder Client extension in VS Code
2. Create new collection: "Medical Appointment API"
3. Set base URL: `http://localhost:5236/api`

### Test Requests

#### 1. Get All Patients
```
GET {{baseUrl}}/patients
```

**Expected Response**: 200 OK
```json
{
  "success": true,
  "data": [...],
  "message": null
}
```

#### 2. Create Patient
```
POST {{baseUrl}}/patients
Content-Type: application/json

{
  "firstName": "Test",
  "lastName": "Patient",
  "email": "test@example.com",
  "phone": "(555) 987-6543",
  "dateOfBirth": "1990-05-15",
  "address": "456 Oak Ave",
  "insuranceProvider": "Blue Cross",
  "insurancePolicyNumber": "BC123456"
}
```

**Expected Response**: 200 OK with created patient data

#### 3. Schedule Appointment
```
POST {{baseUrl}}/appointments
Content-Type: application/json

{
  "patientId": "patient-uuid-here",
  "doctorId": "doctor-uuid-here",
  "appointmentDate": "2024-12-20",
  "appointmentTime": "10:00:00",
  "durationMinutes": 30,
  "notes": "Follow-up visit"
}
```

**Expected Response**: 200 OK with appointment details

#### 4. Get Available Time Slots
```
GET {{baseUrl}}/doctors/{doctorId}/available-slots?date=2024-12-20&duration=30
```

**Expected Response**: 200 OK with array of available time slots

#### 5. Create Invoice
```
POST {{baseUrl}}/invoices
Content-Type: application/json

{
  "appointmentId": "appointment-uuid-here",
  "items": [
    {
      "serviceId": "service-uuid-here",
      "quantity": 1,
      "unitPrice": 150.00,
      "discount": 10
    }
  ],
  "notes": "Payment due in 30 days"
}
```

**Expected Response**: 200 OK with invoice details including calculated totals

## 📊 Database Testing

### Stored Procedure Testing

Execute these SQL queries in Supabase SQL Editor:

#### 1. Test create_patient
```sql
SELECT create_patient(
  'Test',
  'User',
  'test@example.com',
  '(555) 111-2222',
  '1985-03-20',
  '789 Pine St',
  'Emergency Contact',
  '(555) 222-3333',
  'Insurance Co',
  'POL12345'
);
```

#### 2. Test schedule_appointment
```sql
SELECT schedule_appointment(
  'patient-uuid',
  'doctor-uuid',
  '2024-12-20'::date,
  '10:00:00'::time,
  30,
  'Test appointment'
);
```

#### 3. Test get_available_time_slots
```sql
SELECT * FROM get_available_time_slots(
  'doctor-uuid',
  '2024-12-20'::date,
  30
);
```

#### 4. Test create_invoice_with_services
```sql
SELECT create_invoice_with_services(
  'appointment-uuid',
  ARRAY['service-uuid-1', 'service-uuid-2']::uuid[],
  ARRAY[1, 2]::int[],
  ARRAY[150.00, 75.00]::decimal[],
  ARRAY[0, 10]::decimal[],
  'Test invoice'
);
```

## 🐛 Common Issues and Solutions

### Issue 1: "Cannot read property of undefined"
**Cause**: Data not loaded before component renders
**Solution**: Add null checks or use Angular's async pipe

### Issue 2: "Time slot not available" when it appears free
**Cause**: Time zone mismatch or incorrect time calculation
**Solution**: Verify date/time handling in both frontend and backend

### Issue 3: Invoice total calculation incorrect
**Cause**: Discount applied incorrectly or tax not calculated
**Solution**: Review calculation logic in billing component and stored procedure

### Issue 4: CORS error when calling API
**Cause**: Frontend origin not in AllowedOrigins
**Solution**: Add frontend URL to CORS configuration in Program.cs

## ✅ Test Coverage Goals

- **Backend**: 80% code coverage
- **Frontend Components**: 70% code coverage
- **Critical Paths**: 100% covered
- **Integration Tests**: All API endpoints
- **E2E Tests**: Main user workflows

## 🎯 Testing Best Practices

1. **Test Early, Test Often**: Run tests after each significant change
2. **Automate Where Possible**: Set up CI/CD pipeline for automated testing
3. **Test Edge Cases**: Don't just test happy paths
4. **Keep Tests Independent**: Each test should run in isolation
5. **Mock External Dependencies**: Use mocks for database and external APIs in unit tests
6. **Document Test Cases**: Clear descriptions help maintain tests
7. **Review Test Failures**: Don't ignore failing tests
8. **Test Performance**: Monitor response times and optimize slow queries

---

**Happy Testing! 🧪**
