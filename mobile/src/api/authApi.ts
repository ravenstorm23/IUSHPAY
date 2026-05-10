// src/api/authApi.ts
// API de autenticación — consume endpoints de P1
// POST /api/auth/login
// GET /api/wallet/{userId}

import axios from 'axios';

const API_BASE = 'http://localhost:5000';
const USE_MOCK = true;

const client = axios.create({
  baseURL: API_BASE,
  timeout: 5000,
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
  const res = await client.post('/api/auth/login', { email, password });
  return res.data;
}

async function realGetWallet(userId: string): Promise<WalletResponse> {
  const res = await client.get(`/api/wallet/${userId}`);
  return res.data;
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