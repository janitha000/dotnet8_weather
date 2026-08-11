import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <main>
      <h1>Not found</h1>
      <Link to="/">Go home</Link>
    </main>
  );
}
