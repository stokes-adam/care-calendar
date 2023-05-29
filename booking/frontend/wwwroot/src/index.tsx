import { render } from "solid-js/web";
import { Router } from "@solidjs/router";

import "./index.scss";
import App from "./App";
import { BookingApiProvider } from "./contexts/BookingApi";

const root = document.getElementById("root");

render(
  () => (
    <BookingApiProvider>
      <Router>
        <App />
      </Router>
    </BookingApiProvider>
  ),
  root!
);
