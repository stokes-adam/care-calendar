CREATE EXTENSION "uuid-ossp";

CREATE TABLE persons (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP
);

CREATE TABLE person_details (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP,
    person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE UNIQUE,
    encrypted_first_name TEXT NOT NULL,
    encrypted_last_name TEXT NOT NULL,
    encrypted_email TEXT NOT NULL,
    encrypted_phone TEXT NOT NULL
);
CREATE INDEX idx_person_details_person_id ON person_details (person_id);


CREATE TABLE firms (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP,
    owner_person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL UNIQUE
);
CREATE INDEX idx_firms_owner_person_id ON firms (owner_person_id);


CREATE TABLE roles (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP,
    name VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE firm_roles (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP,
    firm_id UUID NOT NULL REFERENCES firms(id) ON DELETE CASCADE,
    person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    UNIQUE (firm_id, person_id, role_id)   
);
CREATE INDEX idx_firm_roles_firm_id ON firm_roles (firm_id);

CREATE TABLE rooms (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP,
    firm_id UUID NOT NULL REFERENCES firms(id) ON DELETE CASCADE,
    name VARCHAR(255) UNIQUE
);
CREATE INDEX idx_rooms_firm_id ON rooms (firm_id);
CREATE INDEX idx_rooms_name ON rooms (name);


CREATE TABLE appointments (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP,
    room_id UUID NOT NULL REFERENCES rooms(id) ON DELETE CASCADE,
    client_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    consultant_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP NOT NULL
);
CREATE INDEX idx_appointments_client_id ON appointments (client_id);
CREATE INDEX idx_appointments_room_id ON appointments (room_id);
CREATE INDEX idx_appointments_consultant_id ON appointments (consultant_id);


CREATE TABLE appointment_details (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    appointment_id UUID NOT NULL REFERENCES appointments(id) ON DELETE CASCADE,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted TIMESTAMP,
    notes TEXT
);
CREATE INDEX idx_appointment_details_appointment_id ON appointment_details (appointment_id);