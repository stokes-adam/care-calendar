import { Link } from "@solidjs/router";
import styles from "./AppBar.module.scss";

const AppBar = () => {
  return (
    <div class={styles.AppBar}>
      <div class={styles.profile}>
        <img></img>
        <label>jane@doe.com</label>
        <div>signout</div>
      </div>
      <nav>
        <Link href="/">Home</Link>
        <Link href="/about">About</Link>
        <Link href="/clients">Clients</Link>
      </nav>
    </div>
  );
};

export default AppBar;
