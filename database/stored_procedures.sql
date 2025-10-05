-- Medical Appointment Application - Stored Procedures
-- PostgreSQL/Supabase Stored Procedures with Transaction Management

-- 1. Create Patient Procedure
CREATE OR REPLACE FUNCTION create_patient(
    p_first_name VARCHAR(100),
    p_last_name VARCHAR(100),
    p_email VARCHAR(255),
    p_phone VARCHAR(20),
    p_date_of_birth DATE,
    p_address TEXT DEFAULT NULL,
    p_emergency_contact_name VARCHAR(100) DEFAULT NULL,
    p_emergency_contact_phone VARCHAR(20) DEFAULT NULL,
    p_insurance_provider VARCHAR(100) DEFAULT NULL,
    p_insurance_policy_number VARCHAR(50) DEFAULT NULL
)
RETURNS TABLE(
    patient_id UUID,
    success BOOLEAN,
    message TEXT
) AS $$
DECLARE
    new_patient_id UUID;
BEGIN
    -- Start transaction
    BEGIN
        -- Insert new patient
        INSERT INTO patients (
            first_name, last_name, email, phone, date_of_birth, 
            address, emergency_contact_name, emergency_contact_phone,
            insurance_provider, insurance_policy_number
        )
        VALUES (
            p_first_name, p_last_name, p_email, p_phone, p_date_of_birth,
            p_address, p_emergency_contact_name, p_emergency_contact_phone,
            p_insurance_provider, p_insurance_policy_number
        )
        RETURNING id INTO new_patient_id;
        
        -- Return success
        RETURN QUERY SELECT new_patient_id, TRUE, 'Patient created successfully'::TEXT;
        
    EXCEPTION
        WHEN unique_violation THEN
            -- Handle duplicate email
            RETURN QUERY SELECT NULL::UUID, FALSE, 'Email already exists'::TEXT;
        WHEN OTHERS THEN
            -- Handle other errors
            RETURN QUERY SELECT NULL::UUID, FALSE, SQLERRM::TEXT;
    END;
END;
$$ LANGUAGE plpgsql;

-- 2. Schedule Appointment with Validation
CREATE OR REPLACE FUNCTION schedule_appointment(
    p_patient_id UUID,
    p_doctor_id UUID,
    p_appointment_date DATE,
    p_appointment_time TIME,
    p_duration_minutes INTEGER DEFAULT 30,
    p_notes TEXT DEFAULT NULL
)
RETURNS TABLE(
    appointment_id UUID,
    success BOOLEAN,
    message TEXT
) AS $$
DECLARE
    new_appointment_id UUID;
    doctor_available BOOLEAN := FALSE;
    appointment_day_of_week INTEGER;
BEGIN
    -- Start transaction
    BEGIN
        -- Get day of week (0 = Sunday, 6 = Saturday)
        appointment_day_of_week := EXTRACT(DOW FROM p_appointment_date);
        
        -- Check doctor availability for the day and time
        SELECT EXISTS(
            SELECT 1 FROM doctor_schedules ds
            WHERE ds.doctor_id = p_doctor_id
            AND ds.day_of_week = appointment_day_of_week
            AND ds.start_time <= p_appointment_time
            AND ds.end_time >= (p_appointment_time + (p_duration_minutes || ' minutes')::INTERVAL)::TIME
            AND ds.is_available = TRUE
        ) INTO doctor_available;
        
        IF NOT doctor_available THEN
            RETURN QUERY SELECT NULL::UUID, FALSE, 'Doctor not available at the requested time'::TEXT;
            RETURN;
        END IF;
        
        -- Check for conflicting appointments
        IF EXISTS(
            SELECT 1 FROM appointments a
            WHERE a.doctor_id = p_doctor_id
            AND a.appointment_date = p_appointment_date
            AND a.status NOT IN ('cancelled')
            AND (
                (a.appointment_time <= p_appointment_time AND 
                 (a.appointment_time + (a.duration_minutes || ' minutes')::INTERVAL)::TIME > p_appointment_time)
                OR
                (p_appointment_time <= a.appointment_time AND 
                 (p_appointment_time + (p_duration_minutes || ' minutes')::INTERVAL)::TIME > a.appointment_time)
            )
        ) THEN
            RETURN QUERY SELECT NULL::UUID, FALSE, 'Time slot already booked'::TEXT;
            RETURN;
        END IF;
        
        -- Insert new appointment
        INSERT INTO appointments (
            patient_id, doctor_id, appointment_date, appointment_time,
            duration_minutes, notes
        )
        VALUES (
            p_patient_id, p_doctor_id, p_appointment_date, p_appointment_time,
            p_duration_minutes, p_notes
        )
        RETURNING id INTO new_appointment_id;
        
        -- Return success
        RETURN QUERY SELECT new_appointment_id, TRUE, 'Appointment scheduled successfully'::TEXT;
        
    EXCEPTION
        WHEN foreign_key_violation THEN
            RETURN QUERY SELECT NULL::UUID, FALSE, 'Invalid patient or doctor ID'::TEXT;
        WHEN OTHERS THEN
            RETURN QUERY SELECT NULL::UUID, FALSE, SQLERRM::TEXT;
    END;
END;
$$ LANGUAGE plpgsql;

-- 3. Create Invoice with Transaction Management
CREATE OR REPLACE FUNCTION create_invoice_with_services(
    p_patient_id UUID,
    p_appointment_id UUID,
    p_service_ids UUID[],
    p_quantities INTEGER[],
    p_tax_rate DECIMAL DEFAULT 0.10
)
RETURNS TABLE(
    invoice_id UUID,
    invoice_number VARCHAR(50),
    total_amount DECIMAL(10,2),
    success BOOLEAN,
    message TEXT
) AS $$
DECLARE
    new_invoice_id UUID;
    new_invoice_number VARCHAR(50);
    subtotal DECIMAL(10,2) := 0.00;
    tax_amount DECIMAL(10,2) := 0.00;
    total DECIMAL(10,2) := 0.00;
    service_id UUID;
    service_price DECIMAL(10,2);
    quantity INTEGER;
    item_total DECIMAL(10,2);
    i INTEGER;
BEGIN
    -- Start transaction
    BEGIN
        -- Validate arrays have same length
        IF array_length(p_service_ids, 1) != array_length(p_quantities, 1) THEN
            RETURN QUERY SELECT NULL::UUID, NULL::VARCHAR(50), 0.00::DECIMAL(10,2), FALSE, 'Service IDs and quantities arrays must have same length'::TEXT;
            RETURN;
        END IF;
        
        -- Generate invoice number
        new_invoice_number := 'INV-' || TO_CHAR(NOW(), 'YYYYMMDD') || '-' || LPAD(NEXTVAL('invoice_sequence')::TEXT, 4, '0');
        
        -- Create sequence if it doesn't exist
        BEGIN
            PERFORM NEXTVAL('invoice_sequence');
        EXCEPTION
            WHEN undefined_table THEN
                CREATE SEQUENCE invoice_sequence START 1000;
                new_invoice_number := 'INV-' || TO_CHAR(NOW(), 'YYYYMMDD') || '-' || LPAD(NEXTVAL('invoice_sequence')::TEXT, 4, '0');
        END;
        
        -- Calculate subtotal
        FOR i IN 1..array_length(p_service_ids, 1) LOOP
            service_id := p_service_ids[i];
            quantity := p_quantities[i];
            
            -- Get service price
            SELECT price INTO service_price
            FROM services
            WHERE id = service_id AND is_active = TRUE;
            
            IF service_price IS NULL THEN
                RETURN QUERY SELECT NULL::UUID, NULL::VARCHAR(50), 0.00::DECIMAL(10,2), FALSE, 'Invalid or inactive service ID: ' || service_id::TEXT;
                RETURN;
            END IF;
            
            item_total := service_price * quantity;
            subtotal := subtotal + item_total;
        END LOOP;
        
        -- Calculate tax and total
        tax_amount := subtotal * p_tax_rate;
        total := subtotal + tax_amount;
        
        -- Create invoice
        INSERT INTO invoices (
            patient_id, appointment_id, invoice_number, subtotal, tax_amount, total_amount
        )
        VALUES (
            p_patient_id, p_appointment_id, new_invoice_number, subtotal, tax_amount, total
        )
        RETURNING id INTO new_invoice_id;
        
        -- Create invoice items
        FOR i IN 1..array_length(p_service_ids, 1) LOOP
            service_id := p_service_ids[i];
            quantity := p_quantities[i];
            
            -- Get service price again
            SELECT price INTO service_price
            FROM services
            WHERE id = service_id;
            
            item_total := service_price * quantity;
            
            INSERT INTO invoice_items (
                invoice_id, service_id, quantity, unit_price, total_price
            )
            VALUES (
                new_invoice_id, service_id, quantity, service_price, item_total
            );
        END LOOP;
        
        -- Return success
        RETURN QUERY SELECT new_invoice_id, new_invoice_number, total, TRUE, 'Invoice created successfully'::TEXT;
        
    EXCEPTION
        WHEN foreign_key_violation THEN
            RETURN QUERY SELECT NULL::UUID, NULL::VARCHAR(50), 0.00::DECIMAL(10,2), FALSE, 'Invalid patient or appointment ID'::TEXT;
        WHEN OTHERS THEN
            RETURN QUERY SELECT NULL::UUID, NULL::VARCHAR(50), 0.00::DECIMAL(10,2), FALSE, SQLERRM::TEXT;
    END;
END;
$$ LANGUAGE plpgsql;

-- 4. Complete Appointment and Create Invoice (Atomic Operation)
CREATE OR REPLACE FUNCTION complete_appointment_with_billing(
    p_appointment_id UUID,
    p_service_ids UUID[],
    p_quantities INTEGER[],
    p_tax_rate DECIMAL DEFAULT 0.10,
    p_payment_method VARCHAR(50) DEFAULT NULL
)
RETURNS TABLE(
    appointment_id UUID,
    invoice_id UUID,
    invoice_number VARCHAR(50),
    total_amount DECIMAL(10,2),
    success BOOLEAN,
    message TEXT
) AS $$
DECLARE
    appointment_patient_id UUID;
    invoice_result RECORD;
BEGIN
    -- Start transaction
    BEGIN
        -- Get appointment details and update status
        SELECT patient_id INTO appointment_patient_id
        FROM appointments
        WHERE id = p_appointment_id AND status = 'scheduled';
        
        IF appointment_patient_id IS NULL THEN
            RETURN QUERY SELECT p_appointment_id, NULL::UUID, NULL::VARCHAR(50), 0.00::DECIMAL(10,2), FALSE, 'Appointment not found or already processed'::TEXT;
            RETURN;
        END IF;
        
        -- Update appointment status to completed
        UPDATE appointments
        SET status = 'completed', updated_at = NOW()
        WHERE id = p_appointment_id;
        
        -- Create invoice
        SELECT * INTO invoice_result
        FROM create_invoice_with_services(
            appointment_patient_id,
            p_appointment_id,
            p_service_ids,
            p_quantities,
            p_tax_rate
        );
        
        IF NOT invoice_result.success THEN
            -- Rollback appointment update
            RAISE EXCEPTION 'Failed to create invoice: %', invoice_result.message;
        END IF;
        
        -- Update payment method if provided
        IF p_payment_method IS NOT NULL THEN
            UPDATE invoices
            SET payment_method = p_payment_method,
                status = 'paid',
                payment_date = CURRENT_DATE
            WHERE id = invoice_result.invoice_id;
        END IF;
        
        -- Return success
        RETURN QUERY SELECT 
            p_appointment_id,
            invoice_result.invoice_id,
            invoice_result.invoice_number,
            invoice_result.total_amount,
            TRUE,
            'Appointment completed and invoice created successfully'::TEXT;
        
    EXCEPTION
        WHEN OTHERS THEN
            -- Auto-rollback on error
            RETURN QUERY SELECT 
                p_appointment_id,
                NULL::UUID,
                NULL::VARCHAR(50),
                0.00::DECIMAL(10,2),
                FALSE,
                SQLERRM::TEXT;
    END;
END;
$$ LANGUAGE plpgsql;

-- 5. Get Doctor Schedule
CREATE OR REPLACE FUNCTION get_doctor_schedule(
    p_doctor_id UUID,
    p_start_date DATE,
    p_end_date DATE
)
RETURNS TABLE(
    schedule_date DATE,
    day_name TEXT,
    start_time TIME,
    end_time TIME,
    is_available BOOLEAN,
    appointment_id UUID,
    appointment_time TIME,
    patient_name TEXT,
    appointment_status VARCHAR(20)
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        d.date_val as schedule_date,
        TO_CHAR(d.date_val, 'Day') as day_name,
        ds.start_time,
        ds.end_time,
        ds.is_available,
        a.id as appointment_id,
        a.appointment_time,
        COALESCE(p.first_name || ' ' || p.last_name, '') as patient_name,
        a.status as appointment_status
    FROM generate_series(p_start_date, p_end_date, '1 day'::interval) d(date_val)
    LEFT JOIN doctor_schedules ds ON ds.doctor_id = p_doctor_id 
        AND ds.day_of_week = EXTRACT(DOW FROM d.date_val)
    LEFT JOIN appointments a ON a.doctor_id = p_doctor_id 
        AND a.appointment_date = d.date_val 
        AND a.status NOT IN ('cancelled')
    LEFT JOIN patients p ON p.id = a.patient_id
    WHERE ds.id IS NOT NULL
    ORDER BY d.date_val, a.appointment_time;
END;
$$ LANGUAGE plpgsql;

-- 6. Get Available Time Slots
CREATE OR REPLACE FUNCTION get_available_time_slots(
    p_doctor_id UUID,
    p_appointment_date DATE,
    p_duration_minutes INTEGER DEFAULT 30
)
RETURNS TABLE(
    time_slot TIME,
    is_available BOOLEAN
) AS $$
DECLARE
    day_of_week INTEGER;
    schedule_start TIME;
    schedule_end TIME;
    slot_time TIME;
BEGIN
    -- Get day of week
    day_of_week := EXTRACT(DOW FROM p_appointment_date);
    
    -- Get doctor's schedule for the day
    SELECT ds.start_time, ds.end_time
    INTO schedule_start, schedule_end
    FROM doctor_schedules ds
    WHERE ds.doctor_id = p_doctor_id
    AND ds.day_of_week = day_of_week
    AND ds.is_available = TRUE;
    
    -- If no schedule found, return empty
    IF schedule_start IS NULL THEN
        RETURN;
    END IF;
    
    -- Generate time slots
    slot_time := schedule_start;
    WHILE slot_time < schedule_end LOOP
        RETURN QUERY
        SELECT 
            slot_time,
            NOT EXISTS(
                SELECT 1 FROM appointments a
                WHERE a.doctor_id = p_doctor_id
                AND a.appointment_date = p_appointment_date
                AND a.status NOT IN ('cancelled')
                AND (
                    (a.appointment_time <= slot_time AND 
                     (a.appointment_time + (a.duration_minutes || ' minutes')::INTERVAL)::TIME > slot_time)
                    OR
                    (slot_time <= a.appointment_time AND 
                     (slot_time + (p_duration_minutes || ' minutes')::INTERVAL)::TIME > a.appointment_time)
                )
            ) as is_available;
        
        slot_time := slot_time + (p_duration_minutes || ' minutes')::INTERVAL;
    END LOOP;
END;
$$ LANGUAGE plpgsql;