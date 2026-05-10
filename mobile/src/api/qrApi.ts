// src/api/qrApi.ts
// ─────────────────────────────────────────────────────────
// ÚNICO lugar donde se configura la conexión con P1.
// Para pasar a producción hacer DOS cambios:
//   1. Cambiar API_BASE por la URL real de P1
//   2. Cambiar USE_MOCK a false
// ─────────────────────────────────────────────────────────

import axios from 'axios';

// ↓↓↓ CAMBIAR cuando P1 entregue la URL de deploy ↓↓↓
const API_BASE = 'http://localhost:5000';
// ↑↑↑ ─────────────────────────────────────────── ↑↑↑

const USE_MOCK = true; // ← cambiar a false cuando P1 esté listo

// ─── Cliente axios ────────────────────────────────────────
const client = axios.create({
  baseURL: API_BASE,
  timeout: 5000,
  headers: { 'Content-Type': 'application/json' },
});

// Interceptor: agrega el Bearer token en cada petición
// Cuando P1 integre auth, el token vendrá del store de sesión
client.interceptors.request.use((config) => {
  const token = getSessionToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// TODO: reemplazar por useSessionStore cuando P1 integre auth
function getSessionToken(): string | null {
  return null; // en producción: leer de useSessionStore
}

// ─── Tipos — contrato real de P1 ─────────────────────────

// GET /api/access/qr/{userId}
// Respuesta: string (el JWT directamente)
export type GenerateQRResponse = {
  token: string;
};

// POST /api/access/validate
// Body:
export type ValidateQRRequest = {
  token: string;
  method: AccessMethod;
  fee: number;
};

// AccessMethod enum — igual que en P1
export type AccessMethod = 'QR' | 'Carnet';

// Respuesta exitosa de P1: { authorized, remainingBalance }
export type ValidateQRSuccess = {
  valid: true;
  authorized: boolean;
  remainingBalance: number;
};

export type ValidateQRFailure = {
  valid: false;
  reason: 'expired' | 'invalid' | 'insufficient_balance' | 'wallet_not_found';
};

export type ValidateQRResponse = ValidateQRSuccess | ValidateQRFailure;

// ─── Mock local (semana 1-2) ──────────────────────────────

async function mockGenerateQR(): Promise<GenerateQRResponse> {
  await new Promise(r => setTimeout(r, 400));
  return { token: 'mock.jwt.token.' + Date.now() };
}

async function mockValidateQR(token: string): Promise<ValidateQRResponse> {
  await new Promise(r => setTimeout(r, 600));
  if (token.includes('expired')) {
    return { valid: false, reason: 'expired' };
  }
  return {
    valid: true,
    authorized: true,
    remainingBalance: 45500,
  };
}

// ─── Funciones reales (se activan con USE_MOCK = false) ───

async function realGenerateQR(userId: string): Promise<GenerateQRResponse> {
  // GET /api/access/qr/{userId}
  const res = await client.get(`/api/access/qr/${userId}`);
  // P1 devuelve el JWT como string directo
  const token = typeof res.data === 'string' ? res.data : res.data;
  return { token };
}

async function realValidateQR(token: string): Promise<ValidateQRResponse> {
  try {
    // POST /api/access/validate
    const body: ValidateQRRequest = {
      token,
      method: 'QR',
      fee: 0, // ← P3 define la tarifa; por ahora 0 para la demo
    };
    const res = await client.post('/api/access/validate', body);
    const data = res.data as { authorized: boolean; remainingBalance: number };
    return {
      valid: true,
      authorized: data.authorized,
      remainingBalance: data.remainingBalance,
    };
  } catch (err: any) {
    const message: string = err?.response?.data?.message ?? '';
    if (message.toLowerCase().includes('inválido') || message.toLowerCase().includes('invalid')) {
      return { valid: false, reason: 'invalid' };
    }
    if (message.toLowerCase().includes('saldo') || message.toLowerCase().includes('balance')) {
      return { valid: false, reason: 'insufficient_balance' };
    }
    if (message.toLowerCase().includes('wallet')) {
      return { valid: false, reason: 'wallet_not_found' };
    }
    return { valid: false, reason: 'invalid' };
  }
}

// ─── Exports públicos ─────────────────────────────────────

export async function generateQR(userId: string): Promise<GenerateQRResponse> {
  if (USE_MOCK) return mockGenerateQR();
  return realGenerateQR(userId);
}

export async function validateQR(token: string): Promise<ValidateQRResponse> {
  if (USE_MOCK) return mockValidateQR(token);
  return realValidateQR(token);
}
