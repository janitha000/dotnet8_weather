import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
} from "react";
import type { LoginDto } from "../types/api";
import {
  clearAccessToken,
  getAccessToken,
  setAccessToken,
} from "./tokenStorage";
import { login as loginRequest } from "../api/authApi";

type AuthContextValue = {
  token: string | null;
  isAuthenticated: boolean;
  login: (dto: LoginDto) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(() => getAccessToken());

  const login = useCallback(async (dto: LoginDto) => {
    const response = await loginRequest(dto);
    setAccessToken(response.accessToken);
    setToken(response.accessToken);
  }, []);

  const logout = useCallback(() => {
    clearAccessToken();
    setToken(null);
  }, []);

  const value = useMemo(
    () => ({
      token,
      isAuthenticated: !!token,
      login,
      logout,
    }),
    [token, login, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}
