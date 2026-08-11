import { Link, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export function Layout() {
  const { isAuthenticated, logout } = useAuth();

  return (
    <div className="app">
      <header>
        <nav>
          <Link to="/">Home</Link>
          {" · "}
          <Link to="/cities/new">Add City</Link>
          {" · "}
          {isAuthenticated ? (
            <button type="button" onClick={logout}>
              Logout
            </button>
          ) : (
            <Link to="/login">Login</Link>
          )}
        </nav>
      </header>
      <Outlet />
    </div>
  );
}
