import { Accessor, createContext, createSignal } from "solid-js";
import { Option, none, fromEither } from "fp-ts/lib/Option";
import { Client } from "../models";
import { get } from "../utils/networking";
import { queryBuilder } from "../utils/queryBuilder";

type BookingApiContext = [
  {
    clients: Accessor<Option<Client[]>>;
    refresh: () => void;
  }
];

const testFirmId = "935d03ba-146b-4c26-bb2a-1411d5418a74";

export const BookingApiContext = createContext<BookingApiContext>();

export const BookingApiProvider = (props: any) => {
  const [clients, setClients] = createSignal<Option<Client[]>>(none);

  const [count, setCount] = createSignal(0);

  setCount((count) => count + 1);
  console.log(count());

  const qry = queryBuilder(`client/firm/${testFirmId}`);
  console.log(qry);

  const refresh = () => {
    get<Client[]>(qry).subscribe((clients) => {
      setClients(fromEither(clients));
    });
  };

  refresh();

  return (
    <BookingApiContext.Provider value={[{ clients, refresh }]}>
      {props.children}
    </BookingApiContext.Provider>
  );
};
