DO $$
DECLARE guid UUID;
DECLARE firm_id UUID;
DECLARE client_id UUID;
DECLARE room_id UUID;
DECLARE consultant_id UUID;
BEGIN
    INSERT INTO persons (active) VALUES (true) RETURNING id INTO guid;
    INSERT INTO person_details (person_id, first_name, last_name, email, phone)
    VALUES (guid, 'John', 'Doe', 'john@doe.com', '123456789');
    INSERT INTO firms (person_id) VALUES (guid) RETURNING id INTO firm_id;
    INSERT INTO clients (person_id, firm_id) VALUES (guid, firm_id) RETURNING id INTO client_id;

    INSERT INTO persons (active) VALUES (true) RETURNING id INTO guid;
    INSERT INTO person_details (person_id, first_name, last_name, email, phone)
    VALUES (guid, 'Alex', 'Smith', 'alex@example.com', '987654321');
    INSERT INTO consultants (person_id, firm_id) VALUES (guid, firm_id) RETURNING id INTO consultant_id;

    INSERT INTO rooms (active, firm_id) VALUES (true, firm_id) RETURNING id INTO room_id;

    INSERT INTO appointments (active, client_id, room_id, consultant_id)
    VALUES (true, client_id, room_id, consultant_id);
END $$;