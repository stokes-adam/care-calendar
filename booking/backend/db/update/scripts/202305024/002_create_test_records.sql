DO $$
DECLARE guid UUID;
DECLARE firm_id UUID;
DECLARE client_id UUID;
DECLARE room_id UUID;
DECLARE consultant_id UUID;
BEGIN
    -- Create a firm and owner
    INSERT INTO persons (deleted) VALUES (false) RETURNING id INTO guid;
    INSERT INTO person_details (person_id, first_name, last_name, email, phone)
    VALUES (guid, 'John', 'Doe', 'john@doe.com', '123456789');
    INSERT INTO firms (owner_person_id, name) VALUES (guid, 'Adam''s Test Firm') RETURNING id INTO firm_id;
    
    -- Create a firm user / receptionist
    INSERT INTO persons (deleted) VALUES (false) RETURNING id INTO guid;
    INSERT INTO person_details (person_id, first_name, last_name, email, phone)
    VALUES (guid, 'Jane', 'Doe', 'jane@doe.com', '987654321');
    INSERT INTO users (person_id, firm_id) VALUES (guid, firm_id);
        
    
    -- Create a client
    INSERT INTO persons (deleted) VALUES (false) RETURNING id INTO guid;
    INSERT INTO person_details (person_id, first_name, last_name, email, phone)
    VALUES (guid, 'Ron', 'Burgundy', 'ron@bur.com"', '2468101214');
    INSERT INTO clients (person_id, firm_id) VALUES (guid, firm_id) RETURNING id INTO client_id;

    -- Create a consultant
    INSERT INTO persons (deleted) VALUES (false) RETURNING id INTO guid;
    INSERT INTO person_details (person_id, first_name, last_name, email, phone)
    VALUES (guid, 'Alex', 'Smith', 'alex@example.com', '13579111315');
    INSERT INTO consultants (person_id, firm_id) VALUES (guid, firm_id) RETURNING id INTO consultant_id;

    -- Create a room and appointment
    INSERT INTO rooms (deleted, firm_id) VALUES (false, firm_id) RETURNING id INTO room_id;

    INSERT INTO appointments (deleted, client_id, room_id, consultant_id, start_time, end_time)
    VALUES (false, client_id, room_id, consultant_id, '2023-05-27 10:00:00', '2023-05-27 11:00:00');
END $$;