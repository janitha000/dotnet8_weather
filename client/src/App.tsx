import "./App.css";
import { Route, Routes } from "react-router-dom";
import { Layout } from "./routes/Layout";
import { HomePage } from "./routes/HomePage";
import { CityPage } from "./routes/CityPage";
import { WeatherPage } from "./routes/WeatherPage";
import { LoginPage } from "./routes/LoginPage";
import { NotFoundPage } from "./routes/NotFoundPage";
import { AddCityPage } from "./routes/AddCityPage";
import { ProtectedRoute } from "./auth/ProtectedRoute";

function App() {
  return (
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
  );
}

export default App;
