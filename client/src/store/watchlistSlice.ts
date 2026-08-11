import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

export type Unit = "C" | "F";

type WatchlistState = {
  cities: string[];
  recent: string[];
  unit: Unit;
};

const initialState: WatchlistState = {
  cities: [],
  recent: [],
  unit: "C",
};

const watchlistSlice = createSlice({
  name: "watchlist",
  initialState,
  reducers: {
    addToWatchlist(state, action: PayloadAction<string>) {
      const name = action.payload.trim();
      if (!name) return;
      if (!state.cities.some((c) => c.toLowerCase() === name.toLowerCase())) {
        state.cities.push(name);
      }
    },
    removeFromWatchlist(state, action: PayloadAction<string>) {
      const target = action.payload.toLowerCase();
      state.cities = state.cities.filter((c) => c.toLowerCase() !== target);
    },
    clearWatchlist(state) {
      state.cities = [];
    },
    trackRecentSearch(state, action: PayloadAction<string>) {
      const name = action.payload.trim();
      if (!name) return;
      state.recent = [
        name,
        ...state.recent.filter((c) => c.toLowerCase() !== name.toLowerCase()),
      ].slice(0, 5);
    },
    setUnit(state, action: PayloadAction<Unit>) {
      state.unit = action.payload;
    },
  },
});

export const {
  addToWatchlist,
  removeFromWatchlist,
  clearWatchlist,
  trackRecentSearch,
  setUnit,
} = watchlistSlice.actions;

export default watchlistSlice.reducer;
