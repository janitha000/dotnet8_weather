import { Link, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { useAppDispatch, useAppSelector } from "../store/hooks";
import { clearWatchlist, setUnit } from "../store/watchlistSlice";
import { useTheme } from "../theme/ThemeContext";

export function Layout() {
  const { isAuthenticated, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const dispatch = useAppDispatch();
  const cities = useAppSelector((state) => state.watchlist.cities);
  const unit = useAppSelector((state) => state.watchlist.unit);

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
          {" · "}
          <button
            type="button"
            onClick={() => dispatch(setUnit(unit === "C" ? "F" : "C"))}
          >
            Unit: °{unit}
          </button>
        </nav>

        {cities.length > 0 && (
          <section aria-label="Watchlist">
            <p>
              Watchlist:{" "}
              {cities.map((name, index) => (
                <span key={name}>
                  {index > 0 ? ", " : ""}
                  <Link to={`/cities/${encodeURIComponent(name)}`}>{name}</Link>
                </span>
              ))}{" "}
              <button type="button" onClick={() => dispatch(clearWatchlist())}>
                Clear
              </button>
            </p>
          </section>
        )}
      </header>
      <Outlet />
    </div>
  );
}
