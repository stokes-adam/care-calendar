CREATE TABLE appointments_audit
(
    audit_id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    operation CHAR(1) NOT NULL,
    timestamp TIMESTAMP NOT NULL,
    user_name VARCHAR(50) NOT NULL,
    id UUID,
    active BOOLEAN,
    client_id UUID,
    room_id UUID,
    consultant_id UUID
);

CREATE OR REPLACE FUNCTION appointments_audit_trigger() RETURNS TRIGGER AS $appointments_audit_trigger$
BEGIN
    INSERT INTO appointments_audit(operation, timestamp, user_name, id, active, client_id, room_id, consultant_id)
    VALUES (substring(TG_OP FROM 1 FOR 1), now(), current_user, NEW.id, NEW.active, NEW.client_id, NEW.room_id, NEW.consultant_id);
    RETURN NEW;
END;
$appointments_audit_trigger$ LANGUAGE plpgsql;

CREATE TRIGGER appointments_audit_trigger
    AFTER INSERT OR UPDATE OR DELETE ON appointments
    FOR EACH ROW EXECUTE PROCEDURE appointments_audit_trigger();
