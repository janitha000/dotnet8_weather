import "./App.css";
import { Route, Routes } from "react-router-dom";
import { Layout } from "./routes/Layout";
import { HomePage } from "./routes/HomePage";
import { CityPage } from "./routes/CityPage";
import { WeatherPage } from "./routes/WeatherPage";
import { LoginPage } from "./routes/LoginPage";
import { NotFoundPage } from "./routes/NotFoundPage";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="cities/:name" element={<CityPage />} />
        <Route path="weather" element={<WeatherPage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}

export default App;
