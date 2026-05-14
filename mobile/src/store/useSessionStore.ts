// src/store/useSessionStore.ts
import { create } from 'zustand';

interface SessionState {
  userId: string | null;
  userName: string | null;
  userEmail: string | null;
  token: string | null;
  balance: number | null;
  isLoggedIn: boolean;
  role: string | null;
  
  setSession: (userId: string, userName: string, userEmail: string, token: string, role: string) => void;
  setBalance: (balance: number) => void;
  clearSession: () => void;
}

export const useSessionStore = create<SessionState>((set) => ({
  userId: null,
  userName: null,
  userEmail: null,
  token: null,
  balance: null,
  isLoggedIn: false,
  role: null,

  setSession: (userId, userName, userEmail, token, role) =>
    set({ userId, userName, userEmail, token, role, isLoggedIn: true }),
  setBalance: (balance) => set({ balance }),
  clearSession: () =>
    set({ userId: null, userName: null, userEmail: null, token: null, balance: null, isLoggedIn: false }),
}));