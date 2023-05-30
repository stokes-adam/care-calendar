import { Link, Outlet } from "react-router-dom";

import './Layout.css';

function Layout() {
  return (
    <div className="layout">
      <div className="app-bar">
        <Link to="/">Home</Link>
        <Link to="/clients">Clients</Link>
        <Link to="/consultants">Consultants</Link>
        <Link to="/rooms">Rooms</Link>
      </div>
      <div className="content">
      <Outlet />
      </div>
    </div>
  )
}

export default Layout;