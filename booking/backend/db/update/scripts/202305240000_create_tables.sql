CREATE EXTENSION "uuid-ossp";

CREATE TABLE person (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE client (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    person_id UUID NOT NULL REFERENCES person(id)
);

CREATE TABLE room (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE consultant (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    person_id UUID NOT NULL REFERENCES person(id)
);

CREATE TABLE appointment (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    client_id UUID NOT NULL REFERENCES client(id),
    room_id UUID NOT NULL REFERENCES room(id),
    consultant_id UUID NOT NULL REFERENCES consultant(id)
);

CREATE TABLE personal_details (
    id UUID NOT NULL DEFAULT uuid_generate_v4() PRIMARY KEY,
    person_id UUID NOT NULL REFERENCES person(id),
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(255) NOT NULL
);