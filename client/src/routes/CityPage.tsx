import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getCityByName } from "../api/citiesApi";
import { ApiError, type City } from "../types/api";

export function CityPage() {
  const { name } = useParams();
  const [city, setCity] = useState<City | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!name) return;
    let cancelled = false;

    async function load() {
      setIsLoading(true);
      setError(null);
      setCity(null);
      try {
        const result = await getCityByName(name!);
        if (!cancelled) setCity(result);
      } catch (err) {
        if (cancelled) return;
        if (err instanceof ApiError) setError(err.message);
        else setError("Something went wrong");
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    }

    void load();
    return () => {
      cancelled = true;
    };
  }, [name]);

  return (
    <main>
      <p>
        <Link to="/">← Back</Link>
      </p>
      <h1>City: {name}</h1>
      {isLoading && <p>Loading…</p>}
      {error && <p role="alert">{error}</p>}
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
