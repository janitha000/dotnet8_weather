export type Theme = "light" | "dark";

const THEME_KEY = "interview_theme";

export function getStoredTheme(): Theme {
  const value = localStorage.getItem(THEME_KEY);
  return value === "dark" ? "dark" : "light";
}

export function setStoredTheme(theme: Theme): void {
  localStorage.setItem(THEME_KEY, theme);
}
