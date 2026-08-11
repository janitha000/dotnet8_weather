import { Link, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { useTheme } from "../theme/ThemeContext";

export function Layout() {
  const { isAuthenticated, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();

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
          {" · "}
          <button type="button" onClick={toggleTheme}>
            Theme: {theme}
          </button>
        </nav>
      </header>
      <Outlet />
    </div>
  );
}
