export const weatherKeys = {
  all: ["weather"] as const,
  current: (city: string) =>
    [...weatherKeys.all, "current", city.toLowerCase()] as const,
  forecast: (city: string) =>
    [...weatherKeys.all, "forecast", city.toLowerCase()] as const,
};
