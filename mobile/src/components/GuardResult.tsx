// src/components/GuardResult.tsx
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { ValidateQRSuccess, ValidateQRFailure } from '../api/qrApi';
import { colors, spacing, radius, fontSize } from '../theme';

type Props = {
  result: ValidateQRSuccess | ValidateQRFailure;
  onReset: () => void;
};

const REASON_LABELS: Record<string, string> = {
  expired:              'QR expirado — pedir al estudiante que regenere',
  invalid:              'QR no válido',
  insufficient_balance: 'Saldo insuficiente',
  wallet_not_found:     'Sin billetera asociada',
};

export default function GuardResult({ result, onReset }: Props) {

  // ─── Acceso denegado ───
  if (!result.valid) {
    return (
      <View style={styles.container}>
        <View style={[styles.card, styles.cardError]}>
          <Text style={styles.bigIcon}>✗</Text>
          <Text style={styles.titleError}>Acceso denegado</Text>
          <Text style={styles.reasonText}>
            {REASON_LABELS[result.reason] ?? result.reason}
          </Text>
        </View>
        <TouchableOpacity style={[styles.btn, styles.btnGray]} onPress={onReset}>
          <Text style={styles.btnText}>Escanear otro</Text>
        </TouchableOpacity>
      </View>
    );
  }

  // ─── Acceso permitido ───
  const balanceFormatted = (result.remainingBalance ?? 0).toLocaleString('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
  });

  // Si authorized es false pero valid es true = saldo insuficiente
  if (!result.authorized) {
    return (
      <View style={styles.container}>
        <View style={[styles.card, styles.cardError]}>
          <Text style={styles.bigIcon}>✗</Text>
          <Text style={styles.titleError}>Acceso denegado</Text>
          <Text style={styles.reasonText}>Saldo insuficiente</Text>
          <View style={styles.balanceBox}>
            <Text style={styles.balanceLabel}>Saldo actual</Text>
            <Text style={[styles.balanceAmount, { color: '#991B1B' }]}>
              {balanceFormatted}
            </Text>
          </View>
        </View>
        <TouchableOpacity style={[styles.btn, styles.btnGray]} onPress={onReset}>
          <Text style={styles.btnText}>Escanear otro</Text>
        </TouchableOpacity>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={[styles.card, styles.cardSuccess]}>
        {/* Ícono de éxito */}
        <View style={styles.successIcon}>
          <Text style={styles.successIconText}>✓</Text>
        </View>

        <Text style={styles.titleSuccess}>Acceso autorizado</Text>

        {/* Saldo */}
        <View style={styles.balanceBox}>
          <Text style={styles.balanceLabel}>Saldo restante</Text>
          <Text style={styles.balanceAmount}>{balanceFormatted}</Text>
        </View>
      </View>

      {/* Botones */}
      <View style={styles.btnRow}>
        <TouchableOpacity
          style={[styles.btn, styles.btnDeny, { flex: 1 }]}
          onPress={onReset}
        >
          <Text style={styles.btnText}>Denegar</Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.btn, styles.btnAllow, { flex: 2 }]}
          onPress={onReset}
        >
          <Text style={[styles.btnText, { fontSize: fontSize.lg }]}>
            ✓ Permitir acceso
          </Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    alignItems: 'center',
    gap: spacing.lg,
  },
  card: {
    width: '100%',
    borderRadius: radius.xl,
    padding: spacing.lg,
    borderWidth: 3,
    alignItems: 'center',
  },
  cardSuccess: {
    backgroundColor: colors.successBg,
    borderColor: colors.success,
  },
  cardError: {
    backgroundColor: colors.errorBg,
    borderColor: colors.error,
    paddingVertical: spacing.xxl,
  },
  successIcon: {
    width: 80,
    height: 80,
    borderRadius: radius.full,
    backgroundColor: colors.success,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: spacing.md,
  },
  successIconText: {
    fontSize: 40,
    color: '#FFFFFF',
    fontWeight: '700',
  },
  titleSuccess: {
    fontSize: fontSize.xl,
    fontWeight: '700',
    color: '#14532D',
    marginBottom: spacing.lg,
  },
  bigIcon: {
    fontSize: 64,
    marginBottom: spacing.md,
  },
  titleError: {
    fontSize: fontSize.xl,
    fontWeight: '700',
    color: '#991B1B',
    marginBottom: spacing.sm,
  },
  reasonText: {
    fontSize: fontSize.md,
    color: '#B91C1C',
    textAlign: 'center',
    marginBottom: spacing.md,
  },
  balanceBox: {
    backgroundColor: 'rgba(0,0,0,0.06)',
    borderRadius: radius.md,
    padding: spacing.md,
    alignItems: 'center',
    width: '100%',
  },
  balanceLabel: {
    fontSize: fontSize.sm,
    color: '#166534',
    marginBottom: 4,
  },
  balanceAmount: {
    fontSize: fontSize.xxl,
    fontWeight: '700',
    color: '#14532D',
  },
  btnRow: {
    flexDirection: 'row',
    gap: spacing.sm,
    width: '100%',
  },
  btn: {
    borderRadius: radius.md,
    paddingVertical: spacing.md + 4,
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: 56,
  },
  btnText: {
    color: '#FFFFFF',
    fontSize: fontSize.md,
    fontWeight: '700',
  },
  btnGray: {
    backgroundColor: colors.textMuted,
    width: '100%',
  },
  btnDeny: {
    backgroundColor: colors.error,
  },
  btnAllow: {
    backgroundColor: colors.success,
  },
});
