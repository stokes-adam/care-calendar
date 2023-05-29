import { Component, createSignal, lazy } from "solid-js";
import { Route, Routes } from "@solidjs/router";

// lazy load pages
const Home = lazy(() => import("./pages/Home"));
const Clients = lazy(() => import("./pages/Clients"));

import styles from "./App.module.scss";
import AppBar from "./components/AppBar";

const App: Component = () => {
  const [theme] = createSignal("light");

  return (
    <div class={styles.App + " " + theme()}>
      <AppBar />
      <div class={styles.contentWrapper}>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/about" element={<div>My about page woohoo</div>} />
          <Route path="/clients" element={<Clients />} />
        </Routes>
      </div>
    </div>
  );
};

export default App;
