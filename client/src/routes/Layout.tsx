import { Link, Outlet } from "react-router-dom";

export function Layout() {
  return (
    <div className="app">
      <header>
        <nav>
          <Link to="/">Home</Link>
          {" · "}
          <Link to="/weather">Weather</Link>
          {" · "}
          <Link to="/login">Login</Link>
        </nav>
      </header>

      <Outlet />
    </div>
  );
}
