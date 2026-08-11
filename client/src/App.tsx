import { lazy, Suspense } from "react";
import { Route, Routes } from "react-router-dom";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { PageFallback } from "./components/PageFallback";
import { Layout } from "./routes/Layout";
import { HomePage } from "./routes/HomePage";
import "./App.css";

// Eager: shell + landing (fast first paint)
// Lazy: heavier / less-visited routes — separate JS chunks
const CityPage = lazy(() =>
  import("./routes/CityPage").then((m) => ({ default: m.CityPage })),
);
const WeatherPage = lazy(() =>
  import("./routes/WeatherPage").then((m) => ({ default: m.WeatherPage })),
);
const LoginPage = lazy(() =>
  import("./routes/LoginPage").then((m) => ({ default: m.LoginPage })),
);
const AddCityPage = lazy(() =>
  import("./routes/AddCityPage").then((m) => ({ default: m.AddCityPage })),
);
const NotFoundPage = lazy(() =>
  import("./routes/NotFoundPage").then((m) => ({ default: m.NotFoundPage })),
);

function App() {
  return (
    <Suspense fallback={<PageFallback />}>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<HomePage />} />
          <Route
            path="cities/new"
            element={
              <ProtectedRoute>
                <AddCityPage />
              </ProtectedRoute>
            }
          />
          <Route path="cities/:name" element={<CityPage />} />
          <Route path="weather/:city" element={<WeatherPage />} />
          <Route path="login" element={<LoginPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </Suspense>
  );
}

export default App;
