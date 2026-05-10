// src/components/QRDisplay.tsx
// QR grande + countdown 60s + botón regenerar.
// Vista que el estudiante le muestra al vigilante.

import React, { useEffect, useState, useCallback } from 'react';
import {
  View, Text, TouchableOpacity, StyleSheet, ActivityIndicator,
} from 'react-native';
import QRCode from 'react-native-qrcode-svg';
import { generateQR } from '../api/qrApi';
import { colors, spacing, radius, fontSize } from '../theme';

interface Props {
  userId: string;
}

type State =
  | { status: 'idle' }
  | { status: 'loading' }
  | { status: 'ready'; token: string; expiresAt: number }
  | { status: 'expired' }
  | { status: 'error'; message: string };

export default function QRDisplay({ userId }: Props) {
  const [state, setState] = useState<State>({ status: 'idle' });
  const [secondsLeft, setSecondsLeft] = useState(0);

  const generate = useCallback(async () => {
    setState({ status: 'loading' });
    try {
      const data = await generateQR(userId);
      setState({ status: 'ready', token: data.token, expiresAt: Date.now() + 60_000 });
      setSecondsLeft(60);
    } catch {
      setState({ status: 'error', message: 'No se pudo generar el QR. Intenta de nuevo.' });
    }
  }, [userId]);

  // Auto-generar al montar
  useEffect(() => { generate(); }, [generate]);

  // Countdown
  useEffect(() => {
    if (state.status !== 'ready') return;
    const interval = setInterval(() => {
      const remaining = Math.ceil((state.expiresAt - Date.now()) / 1000);
      if (remaining <= 0) {
        setState({ status: 'expired' });
        setSecondsLeft(0);
        clearInterval(interval);
      } else {
        setSecondsLeft(remaining);
      }
    }, 500);
    return () => clearInterval(interval);
  }, [state]);

  const expiringSoon = secondsLeft > 0 && secondsLeft <= 10;

  return (
    <View style={styles.container}>

      {/* QR Box */}
      <View style={[styles.qrBox, expiringSoon && styles.qrBoxWarning]}>
        {state.status === 'loading' && (
          <ActivityIndicator size="large" color={colors.purple} />
        )}
        {state.status === 'ready' && (
          <QRCode
            value={state.token}
            size={220}
            color="#000000"
            backgroundColor="#FFFFFF"
          />
        )}
        {state.status === 'expired' && (
          <View style={styles.expiredBox}>
            <Text style={styles.expiredIcon}>⚠</Text>
            <Text style={styles.expiredText}>QR expirado</Text>
          </View>
        )}
        {(state.status === 'idle' || state.status === 'error') && (
          <Text style={styles.placeholderText}>Generando QR...</Text>
        )}
      </View>

      {/* Countdown */}
      {state.status === 'ready' && (
        <Text style={[styles.countdown, expiringSoon && styles.countdownWarning]}>
          {secondsLeft}s
        </Text>
      )}

      {/* Error */}
      {state.status === 'error' && (
        <Text style={styles.errorText}>{state.message}</Text>
      )}

      {/* Botón */}
      {(state.status === 'expired' || state.status === 'error') && (
        <TouchableOpacity style={styles.button} onPress={generate}>
          <Text style={styles.buttonText}>
            {state.status === 'expired' ? 'Regenerar QR' : 'Reintentar'}
          </Text>
        </TouchableOpacity>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
    gap: spacing.lg,
  },
  qrBox: {
    backgroundColor: '#FFFFFF',
    borderRadius: radius.lg,
    padding: spacing.lg,
    alignItems: 'center',
    justifyContent: 'center',
    width: 260,
    height: 260,
    shadowColor: '#000',
    shadowOpacity: 0.2,
    shadowRadius: 12,
    elevation: 6,
  },
  qrBoxWarning: {
    borderWidth: 3,
    borderColor: colors.error,
  },
  expiredBox: {
    alignItems: 'center',
  },
  expiredIcon: {
    fontSize: 48,
    marginBottom: spacing.sm,
  },
  expiredText: {
    fontSize: fontSize.md,
    fontWeight: '600',
    color: colors.textMuted,
  },
  placeholderText: {
    color: colors.textMuted,
    fontSize: fontSize.sm,
  },
  countdown: {
    fontSize: fontSize.xxxl,
    fontWeight: '700',
    color: colors.textPrimary,
    fontVariant: ['tabular-nums'],
  },
  countdownWarning: {
    color: colors.error,
  },
  errorText: {
    color: colors.error,
    fontSize: fontSize.sm,
    textAlign: 'center',
  },
  button: {
    backgroundColor: colors.purple,
    borderRadius: radius.md,
    paddingVertical: spacing.md,
    paddingHorizontal: spacing.xl,
    width: 260,
    alignItems: 'center',
  },
  buttonText: {
    color: '#FFFFFF',
    fontSize: fontSize.md,
    fontWeight: '700',
  },
});
