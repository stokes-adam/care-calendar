CREATE EXTENSION "uuid-ossp";

CREATE TABLE persons (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE firms (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    person_id UUID NOT NULL REFERENCES persons(id)
);

CREATE TABLE clients (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    person_id UUID NOT NULL REFERENCES persons(id),
    firm_id UUID NOT NULL REFERENCES firms(id)
);

CREATE TABLE rooms (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    firm_id UUID NOT NULL REFERENCES firms(id)
);

CREATE TABLE consultants (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    person_id UUID NOT NULL REFERENCES persons(id),
    firm_id UUID NOT NULL REFERENCES firms(id)
);

CREATE TABLE appointments (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    client_id UUID NOT NULL REFERENCES clients(id),
    room_id UUID NOT NULL REFERENCES rooms(id),
    consultant_id UUID NOT NULL REFERENCES consultants(id)
);

CREATE TABLE person_details (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    person_id UUID NOT NULL REFERENCES persons(id),
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(255) NOT NULL
);