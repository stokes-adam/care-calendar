CREATE EXTENSION "uuid-ossp";

CREATE TABLE persons (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE person_details (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE UNIQUE,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    phone VARCHAR(255) NOT NULL UNIQUE
);
CREATE INDEX idx_person_details_person_id ON person_details (person_id);
CREATE INDEX idx_person_details_email ON person_details (email);
CREATE INDEX idx_person_details_phone ON person_details (phone);


CREATE TABLE firms (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    owner_person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL UNIQUE
);
CREATE INDEX idx_firms_owner_person_id ON firms (owner_person_id);
CREATE INDEX idx_firms_name ON firms (name);


CREATE TABLE admins (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    firm_id UUID NOT NULL REFERENCES firms(id) ON DELETE CASCADE,
    UNIQUE (person_id, firm_id)
);
CREATE INDEX idx_admins_person_id ON admins (person_id);
CREATE INDEX idx_admins_firm_id ON admins (firm_id);


CREATE TABLE users (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    firm_id UUID NOT NULL REFERENCES firms(id) ON DELETE CASCADE,
    UNIQUE (person_id, firm_id)
);
CREATE INDEX idx_users_person_id ON users (person_id);
CREATE INDEX idx_users_firm_id ON users (firm_id);


CREATE TABLE clients (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    firm_id UUID NOT NULL REFERENCES firms(id) ON DELETE CASCADE,
    UNIQUE (person_id, firm_id)
);
CREATE INDEX idx_clients_person_id ON clients (person_id);
CREATE INDEX idx_clients_firm_id ON clients (firm_id);


CREATE TABLE consultants (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    person_id UUID NOT NULL REFERENCES persons(id) ON DELETE CASCADE,
    firm_id UUID NOT NULL REFERENCES firms(id) ON DELETE CASCADE,
    UNIQUE (person_id, firm_id)    
);
CREATE INDEX idx_consultants_person_id ON consultants (person_id);
CREATE INDEX idx_consultants_firm_id ON consultants (firm_id);


CREATE TABLE rooms (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    firm_id UUID NOT NULL REFERENCES firms(id) ON DELETE CASCADE,
    name VARCHAR(255)
);
CREATE INDEX idx_rooms_firm_id ON rooms (firm_id);
CREATE INDEX idx_rooms_name ON rooms (name);


CREATE TABLE appointments (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    created TIMESTAMP NOT NULL DEFAULT NOW(),
    updated TIMESTAMP NOT NULL DEFAULT NOW(),
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    client_id UUID NOT NULL REFERENCES clients(id) ON DELETE CASCADE,
    room_id UUID NOT NULL REFERENCES rooms(id) ON DELETE CASCADE,
    consultant_id UUID NOT NULL REFERENCES consultants(id) ON DELETE CASCADE,
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
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    notes TEXT
);
CREATE INDEX idx_appointment_details_appointment_id ON appointment_details (appointment_id);