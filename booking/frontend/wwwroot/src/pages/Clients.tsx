import { Component, JSX, Show, useContext } from "solid-js";
import { BookingApiContext } from "../contexts/BookingApi";
import { option } from "fp-ts";
import { Client } from "../models";

const Clients: Component = () => {
  const [{ clients }] = useContext(BookingApiContext)!;

  return (
    <>
      <Show when={option.isNone(clients())}>loading...</Show>
      <Show when={option.isSome(clients())}>
        <table class="u-full-width">
          <thead>
            <tr>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Email</th>
              <th>Phone Number</th>
            </tr>
          </thead>
          <tbody>
            {option
              .getOrElse<Client[]>(() => [])(clients())
              .map((client) => (
                <tr>
                  <td>{client.firstName}</td>
                  <td>{client.lastName}</td>
                  <td>{client.email}</td>
                  <td>{client.phone}</td>
                </tr>
              ))}
          </tbody>
        </table>
      </Show>
    </>
  );
};

export default Clients;
