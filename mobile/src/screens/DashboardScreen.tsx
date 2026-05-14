import React from 'react';
import {
  View, Text, StyleSheet, ScrollView, TouchableOpacity,
} from 'react-native';
import { SafeAreaView, useSafeAreaInsets } from 'react-native-safe-area-context';
import { useSessionStore } from '../store/useSessionStore';
import { colors, spacing, fontSize, radius } from '../theme';

interface Props {
  onNavigateToQR: () => void;
  onNavigateToWallet: () => void;
  onNavigateToHistory: () => void;
}

export default function DashboardScreen({ onNavigateToQR, onNavigateToWallet, onNavigateToHistory }: Props) {
  const { userName, balance } = useSessionStore();
  const insets = useSafeAreaInsets();

  const balanceFormatted = (balance ?? 0).toLocaleString('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
  });

  return (
    <SafeAreaView style={styles.safe}>
      <View style={styles.header}>
        <View style={styles.headerTop}>
          <View style={styles.avatarBox}>
            <Text style={styles.avatarText}>
              {userName?.split(' ').map(n => n[0]).join('').slice(0, 2).toUpperCase()}
            </Text>
          </View>
          <View>
            <Text style={styles.headerLabel}>Bienvenido</Text>
            <Text style={styles.headerName}>{userName}</Text>
          </View>
        </View>
        <Text style={styles.balanceLabel}>SALDO ACTUAL</Text>
        <Text style={styles.balanceAmount}>{balanceFormatted}</Text>
        <TouchableOpacity style={styles.topUpBtn} onPress={onNavigateToWallet}>
          <Text style={styles.topUpText}>RECARGAR BILLETERA</Text>
        </TouchableOpacity>
      </View>

      <ScrollView
        contentContainerStyle={[styles.scroll, { paddingBottom: insets.bottom + 80 }]}
        showsVerticalScrollIndicator={false}
      >
        <View style={styles.card}>
          <View style={styles.cardHeader}>
            <View>
              <Text style={styles.cardTitle}>Acceso Parqueadero</Text>
              <View style={styles.grantedRow}>
                <View style={styles.grantedDot} />
                <Text style={styles.grantedText}>HABILITADO</Text>
              </View>
            </View>
            <TouchableOpacity style={styles.qrBtn} onPress={onNavigateToQR}>
              <Text style={styles.qrBtnText}>QR</Text>
            </TouchableOpacity>
          </View>

          <View style={styles.divider} />

          <Text style={styles.activityLabel}>ACTIVIDAD RECIENTE</Text>
          <View style={styles.activityRow}>
            <View style={styles.activityIcon}>
              <Text style={{ color: colors.purple, fontWeight: '700' }}>P</Text>
            </View>
            <View style={{ flex: 1 }}>
              <Text style={styles.activityTitle}>Garaje Norte - Entrada</Text>
              <Text style={styles.activitySub}>Hoy</Text>
            </View>
            <Text style={styles.activityAmount}>-$4.000</Text>
          </View>
        </View>

        <View style={styles.quickRow}>
          <TouchableOpacity style={styles.quickCard} onPress={onNavigateToHistory}>
            <Text style={styles.quickIcon}>H</Text>
            <Text style={styles.quickLabel}>Historial</Text>
          </TouchableOpacity>
          <TouchableOpacity style={styles.quickCard} onPress={onNavigateToWallet}>
            <Text style={styles.quickIcon}>R</Text>
            <Text style={styles.quickLabel}>Recargar</Text>
          </TouchableOpacity>
        </View>

        <View style={styles.banner}>
          <Text style={styles.bannerTag}>AVISO</Text>
          <Text style={styles.bannerText}>
            Recuerda recargar tu billetera antes de ingresar al parqueadero.
          </Text>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.bgDark,
  },
  header: {
    backgroundColor: colors.purple,
    padding: spacing.lg,
    paddingBottom: spacing.xl,
    gap: spacing.sm,
  },
  headerTop: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
    marginBottom: spacing.md,
  },
  avatarBox: {
    width: 48,
    height: 48,
    borderRadius: radius.full,
    backgroundColor: 'rgba(255,255,255,0.2)',
    alignItems: 'center',
    justifyContent: 'center',
  },
  avatarText: {
    color: '#FFFFFF',
    fontWeight: '700',
    fontSize: fontSize.md,
  },
  headerLabel: {
    color: 'rgba(255,255,255,0.7)',
    fontSize: fontSize.xs,
    letterSpacing: 1,
  },
  headerName: {
    color: '#FFFFFF',
    fontSize: fontSize.md,
    fontWeight: '700',
  },
  balanceLabel: {
    color: 'rgba(255,255,255,0.7)',
    fontSize: fontSize.xs,
    letterSpacing: 2,
    textAlign: 'center',
  },
  balanceAmount: {
    color: '#FFFFFF',
    fontSize: 42,
    fontWeight: '900',
    textAlign: 'center',
  },
  topUpBtn: {
    borderWidth: 1.5,
    borderColor: 'rgba(255,255,255,0.5)',
    borderRadius: radius.full,
    paddingVertical: spacing.sm,
    paddingHorizontal: spacing.lg,
    alignSelf: 'center',
    marginTop: spacing.sm,
  },
  topUpText: {
    color: '#FFFFFF',
    fontSize: fontSize.xs,
    fontWeight: '700',
    letterSpacing: 1,
  },
  scroll: {
    padding: spacing.lg,
    gap: spacing.lg,
  },
  card: {
    backgroundColor: '#FFFFFF',
    borderRadius: radius.xl,
    padding: spacing.lg,
    gap: spacing.md,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  cardTitle: {
    fontSize: fontSize.lg,
    fontWeight: '700',
    color: '#111827',
  },
  grantedRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
    marginTop: 4,
  },
  grantedDot: {
    width: 8,
    height: 8,
    borderRadius: radius.full,
    backgroundColor: colors.success,
  },
  grantedText: {
    color: colors.success,
    fontSize: fontSize.xs,
    fontWeight: '700',
    letterSpacing: 1,
  },
  qrBtn: {
    width: 48,
    height: 48,
    borderRadius: radius.full,
    backgroundColor: '#1A1A2E',
    alignItems: 'center',
    justifyContent: 'center',
  },
  qrBtnText: {
    color: '#FFFFFF',
    fontSize: fontSize.sm,
    fontWeight: '700',
  },
  divider: {
    height: 1,
    backgroundColor: '#F3F4F6',
  },
  activityLabel: {
    color: '#9CA3AF',
    fontSize: fontSize.xs,
    letterSpacing: 1,
    fontWeight: '600',
  },
  activityRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },
  activityIcon: {
    width: 36,
    height: 36,
    borderRadius: radius.full,
    backgroundColor: '#F3F4F6',
    alignItems: 'center',
    justifyContent: 'center',
  },
  activityTitle: {
    fontSize: fontSize.sm,
    fontWeight: '600',
    color: '#111827',
  },
  activitySub: {
    fontSize: fontSize.xs,
    color: '#9CA3AF',
  },
  activityAmount: {
    fontSize: fontSize.sm,
    fontWeight: '700',
    color: colors.error,
  },
  quickRow: {
    flexDirection: 'row',
    gap: spacing.md,
  },
  quickCard: {
    flex: 1,
    backgroundColor: '#FFFFFF',
    borderRadius: radius.lg,
    padding: spacing.lg,
    alignItems: 'center',
    gap: spacing.sm,
  },
  quickIcon: {
    fontSize: 28,
    color: colors.purple,
    fontWeight: '700',
  },
  quickLabel: {
    fontSize: fontSize.sm,
    fontWeight: '700',
    color: '#111827',
  },
  banner: {
    backgroundColor: '#1A1A2E',
    borderRadius: radius.lg,
    padding: spacing.lg,
    gap: spacing.xs,
  },
  bannerTag: {
    color: colors.purpleLight,
    fontSize: fontSize.xs,
    fontWeight: '700',
    letterSpacing: 1,
  },
  bannerText: {
    color: '#FFFFFF',
    fontSize: fontSize.sm,
    lineHeight: 20,
  },
});