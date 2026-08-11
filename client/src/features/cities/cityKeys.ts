export const cityKeys = {
  all: ["cities"] as const,
  byName: (name: string) => [...cityKeys.all, name.toLowerCase()] as const,
};
