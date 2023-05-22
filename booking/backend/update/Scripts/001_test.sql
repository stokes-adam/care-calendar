CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    
CREATE TABLE person (
    id UUID NOT NULL DEFAULT uuid_generate_v4(),
    active BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (id)
);
CREATE TABLE client (
    id UUID NOT NULL DEFAULT uuid_generate_v4(),
    person_id INT NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (person_id) REFERENCES person(id)
);

CREATE TABLE room (
    id UUID NOT NULL DEFAULT uuid_generate_v4(),
    active BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (id)
);

CREATE TABLE consultant (
    id UUID NOT NULL DEFAULT uuid_generate_v4(),
    PRIMARY KEY (id),
    FOREIGN KEY (person_id) REFERENCES person(id)
);

CREATE TABLE appointment (
    id UUID NOT NULL DEFAULT uuid_generate_v4(),
    active BOOLEAN NOT NULL DEFAULT TRUE,
    client_id UUID NOT NULL,
    room_id UUID NOT NULL,
    consultant_id UUID NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (client_id) REFERENCES client(id),
    FOREIGN KEY (room_id) REFERENCES room(id),
    FOREIGN KEY (consultant_id) REFERENCES consultant(id)
);

CREATE TABLE personal_details (
    
);