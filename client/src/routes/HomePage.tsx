import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { PageHeader } from "../components/PageHeader";
import { useDebouncedValue } from "../hooks/useDebouncedValue";
import { useAppDispatch, useAppSelector } from "../store/hooks";
import { trackRecentSearch } from "../store/watchlistSlice";

export function HomePage() {
  const [cityName, setCityName] = useState("Colombo");
  const debouncedCityName = useDebouncedValue(cityName, 400);
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const recent = useAppSelector((state) => state.watchlist.recent);

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmed = cityName.trim();
    if (!trimmed) return;
    dispatch(trackRecentSearch(trimmed));
    navigate(`/cities/${encodeURIComponent(trimmed)}`);
  }

  return (
    <main>
      <PageHeader title="City Lookup">
        <p>Search seeded cities: Colombo, Tehran, Mumbai</p>
      </PageHeader>

      <form onSubmit={handleSearch}>
        <label htmlFor="cityName">City name</label>
        <input
          id="cityName"
          type="text"
          value={cityName}
          onChange={(e) => setCityName(e.target.value)}
        />
        <button type="submit">Search</button>
      </form>

      <p>
        Debounced preview: <strong>{debouncedCityName || "—"}</strong>
      </p>

      {recent.length > 0 && (
        <section>
          <h2>Recent</h2>
          <ul>
            {recent.map((name) => (
              <li key={name}>
                <Link to={`/cities/${encodeURIComponent(name)}`}>{name}</Link>
              </li>
            ))}
          </ul>
        </section>
      )}
    </main>
  );
}
