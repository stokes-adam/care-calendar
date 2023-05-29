export type guid = string;

export type Person = {
  id: guid;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
};

export type Client = {
  id: guid;
  personId: guid;
} & Person;

export type Consultant = {
  id: guid;
  personId: guid;
} & Person;

export type Appointment = {
  id: guid;
  clientId: guid;
  consultantId: guid;
  startTime: Date;
  endTime: Date;
  notes: string; // optional and sensitive
};
