import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getCityByName } from "../api/citiesApi";
import { getErrorMessage } from "../api/errorMapping";
import { ErrorAlert } from "../components/ErrorAlert";
import { PageHeader } from "../components/PageHeader";
import type { City } from "../types/api";
import { useAppDispatch, useAppSelector } from "../store/hooks";
import { addToWatchlist, removeFromWatchlist } from "../store/watchlistSlice";

export function CityPage() {
  const { name } = useParams();
  const [city, setCity] = useState<City | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const dispatch = useAppDispatch();
  const watchlist = useAppSelector((state) => state.watchlist.cities);
  const pinned = watchlist.some(
    (c) => c.toLowerCase() === (name ?? "").toLowerCase(),
  );

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
        setError(getErrorMessage(err));
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
        {city && (
          <>
            {" · "}
            <Link to={`/weather/${encodeURIComponent(city.name)}`}>
              Weather
            </Link>
            {" · "}
            <Link to={`/weather/compound/${encodeURIComponent(city.name)}`}>
              Compound Weather
            </Link>
          </>
        )}
      </p>
      <p>
        <button
          type="button"
          onClick={() => {
            const target = city?.name ?? name;
            if (!target) return;
            if (pinned) dispatch(removeFromWatchlist(target));
            else dispatch(addToWatchlist(target));
          }}
        >
          {pinned ? "Unpin" : "Pin to watchlist"}
        </button>
      </p>

      <PageHeader title={`City: ${name ?? ""}`} />

      {isLoading && <p>Loading…</p>}
      {error && <ErrorAlert message={error} />}

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
