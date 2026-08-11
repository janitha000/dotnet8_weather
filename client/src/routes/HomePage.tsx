import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { PageHeader } from "../components/PageHeader";
import { useDebouncedValue } from "../hooks/useDebouncedValue";

export function HomePage() {
  const [cityName, setCityName] = useState("Colombo");
  const debouncedCityName = useDebouncedValue(cityName, 400);
  const navigate = useNavigate();

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmed = cityName.trim();
    if (!trimmed) return;
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
    </main>
  );
}
