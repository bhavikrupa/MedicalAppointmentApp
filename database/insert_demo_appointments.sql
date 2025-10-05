-- Insert Demo Appointments
-- Run this after you have patients and doctors in your database

-- First, let's see what patients and doctors we have
SELECT 'Available Patients:' as info;
SELECT id, first_name, last_name, email FROM patients WHERE is_active = true LIMIT 5;

SELECT 'Available Doctors:' as info;
SELECT id, first_name, last_name, specialization FROM doctors WHERE is_active = true LIMIT 5;

-- ========================================
-- METHOD 1: Direct INSERT (Simple, no stored procedure)
-- ========================================
-- Replace the UUIDs below with actual IDs from your patients and doctors tables

-- Example: Insert appointments directly
-- First, get the IDs you need:
-- Copy a patient ID from the SELECT above and replace 'PATIENT_ID_HERE'
-- Copy a doctor ID from the SELECT above and replace 'DOCTOR_ID_HERE'

/*
-- Template for direct insert (uncomment and replace IDs):
INSERT INTO appointments (
    patient_id, 
    doctor_id, 
    appointment_date, 
    appointment_time, 
    duration_minutes, 
    status, 
    notes
) VALUES 
    ('PATIENT_ID_HERE'::uuid, 'DOCTOR_ID_HERE'::uuid, CURRENT_DATE + 1, '10:00:00', 30, 'Scheduled', 'Annual checkup'),
    ('PATIENT_ID_HERE'::uuid, 'DOCTOR_ID_HERE'::uuid, CURRENT_DATE + 2, '14:00:00', 45, 'Scheduled', 'Follow-up visit'),
    ('PATIENT_ID_HERE'::uuid, 'DOCTOR_ID_HERE'::uuid, CURRENT_DATE + 3, '11:00:00', 30, 'Scheduled', 'General consultation');
*/

-- ========================================
-- METHOD 2: Using variables (Easier)
-- ========================================
DO $$
DECLARE
    patient_id1 UUID;
    patient_id2 UUID;
    patient_id3 UUID;
    doctor_id1 UUID;
    doctor_id2 UUID;
    doctor_id3 UUID;
BEGIN
    -- Get the first 3 patients
    SELECT id INTO patient_id1 FROM patients WHERE is_active = true ORDER BY created_at LIMIT 1;
    SELECT id INTO patient_id2 FROM patients WHERE is_active = true ORDER BY created_at OFFSET 1 LIMIT 1;
    SELECT id INTO patient_id3 FROM patients WHERE is_active = true ORDER BY created_at OFFSET 2 LIMIT 1;
    
    -- Get the first 3 doctors
    SELECT id INTO doctor_id1 FROM doctors WHERE is_active = true ORDER BY created_at LIMIT 1;
    SELECT id INTO doctor_id2 FROM doctors WHERE is_active = true ORDER BY created_at OFFSET 1 LIMIT 1;
    SELECT id INTO doctor_id3 FROM doctors WHERE is_active = true ORDER BY created_at OFFSET 2 LIMIT 1;
    
    -- Check if we have patients and doctors
    IF patient_id1 IS NULL OR doctor_id1 IS NULL THEN
        RAISE EXCEPTION 'Please insert patients and doctors first before creating appointments';
    END IF; 
    
    -- Insert future appointments (upcoming)
    INSERT INTO appointments (patient_id, doctor_id, appointment_date, appointment_time, duration_minutes, status, notes)
    VALUES
        (patient_id1, doctor_id1, CURRENT_DATE + 1, '09:00:00'::time, 30, 'scheduled', 'Annual checkup'),
        (patient_id1, doctor_id1, CURRENT_DATE + 7, '10:00:00'::time, 30, 'confirmed', 'Follow-up blood work'),
        (patient_id2, doctor_id2, CURRENT_DATE + 2, '14:00:00'::time, 45, 'scheduled', 'Pediatric consultation'),
        (patient_id2, doctor_id1, CURRENT_DATE + 5, '15:30:00'::time, 30, 'confirmed', 'General checkup'),
        (patient_id3, doctor_id3, CURRENT_DATE + 3, '11:00:00'::time, 60, 'scheduled', 'Cardiac evaluation'),
        (patient_id3, doctor_id2, CURRENT_DATE + 4, '13:00:00'::time, 30, 'scheduled', 'Routine checkup');
    
    -- Insert past appointments (for history)
    INSERT INTO appointments (patient_id, doctor_id, appointment_date, appointment_time, duration_minutes, status, notes)
    VALUES
        (patient_id1, doctor_id1, CURRENT_DATE - 7, '09:00:00'::time, 30, 'completed', 'Initial consultation'),
        (patient_id2, doctor_id2, CURRENT_DATE - 5, '10:30:00'::time, 30, 'completed', 'Vaccination'),
        (patient_id3, doctor_id3, CURRENT_DATE - 3, '14:00:00'::time, 45, 'completed', 'Cardiac screening'),
        (patient_id1, doctor_id2, CURRENT_DATE - 1, '11:00:00'::time, 30, 'completed', 'Lab results review');
    
    RAISE NOTICE 'Successfully inserted 10 demo appointments!';
    RAISE NOTICE 'Patient IDs used: %, %, %', patient_id1, patient_id2, patient_id3;
    RAISE NOTICE 'Doctor IDs used: %, %, %', doctor_id1, doctor_id2, doctor_id3;
END $$;

-- ========================================
-- METHOD 3: Using the stored procedure (If available)
-- ========================================
-- Note: This uses the schedule_appointment stored procedure
/*
DO $$
DECLARE
    patient_id1 UUID;
    doctor_id1 UUID;
    appt_id UUID;
BEGIN
    -- Get first patient and doctor
    SELECT id INTO patient_id1 FROM patients WHERE is_active = true LIMIT 1;
    SELECT id INTO doctor_id1 FROM doctors WHERE is_active = true LIMIT 1;
    
    -- Schedule using stored procedure
    SELECT appointment_id INTO appt_id FROM schedule_appointment(
        patient_id1,
        doctor_id1,
        CURRENT_DATE + 1,
        '10:00:00'::time,
        30,
        'Test appointment via stored procedure'
    );
    
    RAISE NOTICE 'Appointment created with ID: %', appt_id;
END $$;
*/

-- ========================================
-- Verify the inserted appointments
-- ========================================
SELECT 
    'Inserted Appointments' as info,
    COUNT(*) as total_count,
    COUNT(*) FILTER (WHERE status = 'Scheduled') as scheduled_count,
    COUNT(*) FILTER (WHERE status = 'Completed') as completed_count
FROM appointments;

-- View all appointments with patient and doctor names
SELECT 
    a.id,
    a.appointment_date,
    a.appointment_time,
    p.first_name || ' ' || p.last_name as patient_name,
    d.first_name || ' ' || d.last_name as doctor_name,
    d.specialization,
    a.duration_minutes,
    a.status,
    a.notes
FROM appointments a
JOIN patients p ON a.patient_id = p.id
JOIN doctors d ON a.doctor_id = d.id
ORDER BY a.appointment_date DESC, a.appointment_time DESC;

-- View upcoming appointments only
SELECT 
    'Upcoming Appointments' as info;
    
SELECT 
    a.appointment_date,
    a.appointment_time,
    p.first_name || ' ' || p.last_name as patient,
    d.first_name || ' ' || d.last_name as doctor,
    a.duration_minutes || ' mins' as duration,
    a.status
FROM appointments a
JOIN patients p ON a.patient_id = p.id
JOIN doctors d ON a.doctor_id = d.id
WHERE a.appointment_date >= CURRENT_DATE
ORDER BY a.appointment_date, a.appointment_time;
