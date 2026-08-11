import { useState } from "react";
import "./App.css";
import { ApiError, type City } from "./types/api";
import { getCityByName } from "./api/citiesApi";

function App() {
  const [cityName, setCityName] = useState("");
  const [city, setCity] = useState<City | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const trimmed = cityName.trim();
    if (!trimmed) {
      setError("Enter a city name");
      setCity(null);
      return;
    }

    setIsLoading(true);
    setError(null);
    setCity(null);

    try {
      const result = await getCityByName(trimmed);
      setCity(result);
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message);
      } else {
        setError("Something went wrong");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <main className="app">
      <h1>City Lookup</h1>
      <p>Search seeded cities: Colombo, Tehran, Mumbai</p>

      <form onSubmit={handleSearch}>
        <label htmlFor="cityName">City name</label>
        <input
          id="cityName"
          type="text"
          value={cityName}
          onChange={(e) => setCityName(e.target.value)}
          disabled={isLoading}
        />
        <button type="submit" disabled={isLoading}>
          {isLoading ? "Searching…" : "Search"}
        </button>
      </form>

      {error && (
        <p role="alert" className="error">
          {error}
        </p>
      )}

      {city && (
        <section>
          <h2>{city.name}</h2>
          <p>Country: {city.country}</p>
          <p>Latitude: {city.latitude}</p>
          <p>Longitude: {city.longitude}</p>
          <p>Time zone: {city.timeZone || "—"}</p>
        </section>
      )}
    </main>
  );
}

export default App;
