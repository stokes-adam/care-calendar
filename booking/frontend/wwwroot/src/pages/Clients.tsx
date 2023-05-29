import { Component, JSX, useContext } from "solid-js";
import { BookingApiContext } from "../contexts/BookingApi";
import { fold } from "fp-ts/lib/Option";
import { Client } from "../models";

const Clients: Component = () => {
  const [{ clients }] = useContext(BookingApiContext)!;

  return (
    <ul>
      {fold<Client[], JSX.Element>(
        () => <li>loading...</li>,
        (clients) => clients.map((c) => <li>{c.firstName}</li>)
      )(clients())}
    </ul>
  );
};

export default Clients;
