CREATE TABLE appointment_audit(
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

CREATE OR REPLACE FUNCTION appointment_audit_trigger() RETURNS TRIGGER AS $appointment_audit_trigger$
BEGIN
    INSERT INTO appointment_audit(operation, timestamp, user_name, id, active, client_id, room_id, consultant_id)
    VALUES (TG_OP, now(), current_user, NEW.id, NEW.active, NEW.client_id, NEW.room_id, NEW.consultant_id);
    RETURN NEW;
END;
$appointment_audit_trigger$ LANGUAGE plpgsql;

CREATE TRIGGER appointment_audit_trigger
    AFTER INSERT OR UPDATE OR DELETE ON appointment
    FOR EACH ROW EXECUTE PROCEDURE appointment_audit_trigger();
