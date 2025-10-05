-- Insert Demo Data for Medical Appointment Application
-- Execute this after running schema.sql and stored_procedures.sql

-- 1. Insert sample doctors (if not already present)
INSERT INTO doctors (first_name, last_name, email, phone, specialization, license_number) 
VALUES
    ('John', 'Smith', 'john.smith@clinic.com', '(555) 123-4567', 'General Practice', 'MD001'),
    ('Sarah', 'Johnson', 'sarah.johnson@clinic.com', '(555) 234-5678', 'Pediatrics', 'MD002'),
    ('Michael', 'Brown', 'michael.brown@clinic.com', '(555) 345-6789', 'Cardiology', 'MD003')
ON CONFLICT (email) DO NOTHING;

-- 2. Get doctor IDs for reference
DO $$
DECLARE
    dr_smith_id UUID;
    dr_johnson_id UUID;
    dr_brown_id UUID;
    patient1_id UUID;
    patient2_id UUID;
    patient3_id UUID;
BEGIN
    -- Get doctor IDs
    SELECT id INTO dr_smith_id FROM doctors WHERE email = 'john.smith@clinic.com';
    SELECT id INTO dr_johnson_id FROM doctors WHERE email = 'sarah.johnson@clinic.com';
    SELECT id INTO dr_brown_id FROM doctors WHERE email = 'michael.brown@clinic.com';

    -- 3. Insert sample patients using stored procedure
    SELECT patient_id INTO patient1_id FROM create_patient(
        'Alice', 'Williams', 'alice.williams@email.com', '(555) 111-1111',
        '1985-03-15'::date, '123 Oak Street, Springfield', 
        'Bob Williams', '(555) 111-2222',
        'Blue Cross', 'BC123456'
    );

    SELECT patient_id INTO patient2_id FROM create_patient(
        'Bob', 'Davis', 'bob.davis@email.com', '(555) 222-2222',
        '1990-07-22'::date, '456 Maple Avenue, Springfield',
        'Jane Davis', '(555) 222-3333',
        'Aetna', 'AE789012'
    );

    SELECT patient_id INTO patient3_id FROM create_patient(
        'Carol', 'Martinez', 'carol.martinez@email.com', '(555) 333-3333',
        '1978-11-08'::date, '789 Pine Road, Springfield',
        'David Martinez', '(555) 333-4444',
        'United Healthcare', 'UH345678'
    );

    -- 4. Insert doctor schedules (Monday to Friday, 9 AM to 5 PM)
    INSERT INTO doctor_schedules (doctor_id, day_of_week, start_time, end_time) 
    SELECT 
        id,
        day_num,
        '09:00'::time,
        '17:00'::time
    FROM doctors
    CROSS JOIN generate_series(1, 5) AS day_num
    ON CONFLICT (doctor_id, day_of_week, start_time) DO NOTHING;

    -- 5. Insert sample appointments using stored procedure
    PERFORM schedule_appointment(
        patient1_id,
        dr_smith_id,
        CURRENT_DATE + INTERVAL '1 day',
        '10:00:00'::time,
        30,
        'Annual checkup'
    );

    PERFORM schedule_appointment(
        patient2_id,
        dr_johnson_id,
        CURRENT_DATE + INTERVAL '2 days',
        '14:00:00'::time,
        45,
        'Pediatric consultation'
    );

    PERFORM schedule_appointment(
        patient3_id,
        dr_brown_id,
        CURRENT_DATE + INTERVAL '3 days',
        '11:30:00'::time,
        60,
        'Cardiac evaluation'
    );

    -- Add a past appointment
    PERFORM schedule_appointment(
        patient1_id,
        dr_smith_id,
        CURRENT_DATE - INTERVAL '5 days',
        '09:00:00'::time,
        30,
        'Follow-up visit'
    );

    RAISE NOTICE 'Demo data inserted successfully!';
    RAISE NOTICE 'Patient IDs: %, %, %', patient1_id, patient2_id, patient3_id;
    RAISE NOTICE 'Doctor IDs: %, %, %', dr_smith_id, dr_johnson_id, dr_brown_id;
END $$;

-- 6. Verify data insertion
SELECT 'Doctors' as table_name, COUNT(*) as count FROM doctors WHERE is_active = true
UNION ALL
SELECT 'Patients', COUNT(*) FROM patients WHERE is_active = true
UNION ALL
SELECT 'Doctor Schedules', COUNT(*) FROM doctor_schedules WHERE is_available = true
UNION ALL
SELECT 'Appointments', COUNT(*) FROM appointments
UNION ALL
SELECT 'Services', COUNT(*) FROM services WHERE is_active = true;

-- 7. View sample data
SELECT 'Recent Patients' as info;
SELECT id, first_name, last_name, email, phone FROM patients ORDER BY created_at DESC LIMIT 5;

SELECT 'Active Doctors' as info;
SELECT id, first_name, last_name, specialization, email FROM doctors WHERE is_active = true;

SELECT 'Upcoming Appointments' as info;
SELECT 
    a.appointment_date,
    a.appointment_time,
    p.first_name || ' ' || p.last_name as patient,
    d.first_name || ' ' || d.last_name as doctor,
    a.status
FROM appointments a
JOIN patients p ON a.patient_id = p.id
JOIN doctors d ON a.doctor_id = d.id
WHERE a.appointment_date >= CURRENT_DATE
ORDER BY a.appointment_date, a.appointment_time;