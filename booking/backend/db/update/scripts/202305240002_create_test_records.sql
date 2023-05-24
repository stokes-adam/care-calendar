DO $$
DECLARE guid UUID;
    DECLARE client_id UUID;
    DECLARE room_id UUID;
    DECLARE consultant_id UUID;
BEGIN
    INSERT INTO person (active) VALUES (true) RETURNING id INTO guid;
    INSERT INTO client (person_id) VALUES (guid) RETURNING id INTO client_id;

    INSERT INTO person (active) VALUES (true) RETURNING id INTO guid;
    INSERT INTO consultant (person_id) VALUES (guid) RETURNING id INTO consultant_id;

    INSERT INTO room (active) VALUES (true) RETURNING id INTO room_id;

    INSERT INTO appointment (active, client_id, room_id, consultant_id)
    VALUES (true, client_id, room_id, consultant_id);
END $$;