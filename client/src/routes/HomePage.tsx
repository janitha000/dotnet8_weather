import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";

export function HomePage() {
  const [cityName, setCityName] = useState("Colombo");
  const navigate = useNavigate();

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmed = cityName.trim();
    if (!trimmed) return;
    navigate(`/cities/${encodeURIComponent(trimmed)}`);
  }

  return (
    <main>
      <h1>City Lookup</h1>
      <p>Search seeded cities: Colombo, Tehran, Mumbai</p>

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
    </main>
  );
}
