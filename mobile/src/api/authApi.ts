// src/api/authApi.ts
// API de autenticación — consume endpoints de P1
// POST /api/auth/login
// GET /api/wallet/{userId}

import axios from 'axios';
import { useSessionStore } from '../store/useSessionStore';

const API_BASE = 'https://iushpay.onrender.com';
const USE_MOCK = false;

const client = axios.create({
  baseURL: API_BASE,
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
});

// ─── Tipos ───────────────────────────────────────────────

export type LoginResponse = {
  token: string;
  email: string;
  fullName: string;
  role: string;
  userId: string;
};

export type WalletResponse = {
  walletId: string;
  balance: number;
  lastUpdated: string;
};

// ─── Mock ─────────────────────────────────────────────────

async function mockLogin(email: string, password: string): Promise<LoginResponse> {
  await new Promise(r => setTimeout(r, 600));
  if (password !== '1234') throw new Error('Credenciales incorrectas');
  return {
    token: 'mock.bearer.token',
    email,
    fullName: 'Estudiante',
    role: 'Student',
    userId: 'mock-user-123',
  };
}

async function mockGetWallet(userId: string): Promise<WalletResponse> {
  await new Promise(r => setTimeout(r, 400));
  return {
    walletId: 'mock-wallet-001',
    balance: 45500,
    lastUpdated: new Date().toISOString(),
  };
}

// ─── Funciones reales ─────────────────────────────────────

async function realLogin(email: string, password: string): Promise<LoginResponse> {
  try {
    const res = await client.post('/api/auth/login', { email, password });
    console.log('Login response:', JSON.stringify(res.data));
    return res.data;
  } catch (err: any) {
    console.log('Login error completo:', err?.message);
    console.log('Login error response:', JSON.stringify(err?.response?.data));
    console.log('Login status:', err?.response?.status);
    console.log('Login code:', err?.code);
    throw err;
  }
}

async function realGetWallet(userId: string): Promise<WalletResponse> {
  const token = useSessionStore.getState().token;
  
  // Retry hasta 3 veces con espera de 2 segundos entre intentos
  for (let attempt = 1; attempt <= 3; attempt++) {
    try {
      console.log(`Wallet intento ${attempt}`);
      const res = await client.get(`/api/wallet/${userId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      return res.data;
    } catch (err: any) {
      console.log(`Wallet intento ${attempt} fallido:`, err?.message);
      if (attempt < 3) {
        await new Promise(r => setTimeout(r, 2000));
      } else {
        throw err;
      }
    }
  }
  throw new Error('No se pudo obtener el saldo');
}

// ─── Exports ──────────────────────────────────────────────

export async function login(email: string, password: string): Promise<LoginResponse> {
  if (USE_MOCK) return mockLogin(email, password);
  return realLogin(email, password);
}

export async function getWallet(userId: string): Promise<WalletResponse> {
  if (USE_MOCK) return mockGetWallet(userId);
  return realGetWallet(userId);
}