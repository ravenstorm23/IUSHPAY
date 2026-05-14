import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { SafeAreaView, useSafeAreaInsets } from 'react-native-safe-area-context';
import QRDisplay from '../components/QRDisplay';
import { useSessionStore } from '../store/useSessionStore';
import { colors, spacing, fontSize, radius } from '../theme';

interface Props {
  onBack: () => void;
}

export default function QRScreen({ onBack }: Props) {
  const { userId } = useSessionStore();
  const insets = useSafeAreaInsets();

  if (!userId) return null;

  return (
    <SafeAreaView style={styles.safe}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={onBack} style={styles.backBtn}>
          <Text style={styles.backText}>← Volver</Text>
        </TouchableOpacity>
        <Text style={styles.title}>Tu QR de acceso</Text>
        <Text style={styles.subtitle}>Muestrale esta pantalla al vigilante</Text>
      </View>

      {/* QR */}
      <View style={[styles.content, { paddingBottom: insets.bottom + 20 }]}>
        <QRDisplay userId={userId} />
      </View>

      {/* Info */}
      <View style={styles.infoCard}>
        <Text style={styles.infoText}>
          El QR expira en 60 segundos y se regenera automaticamente. Es unico e intransferible.
        </Text>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.bgDark,
  },
  header: {
    alignItems: 'center',
    padding: spacing.lg,
    gap: spacing.xs,
  },
  backBtn: {
    alignSelf: 'flex-start',
    marginBottom: spacing.sm,
  },
  backText: {
    color: colors.purpleLight,
    fontSize: fontSize.md,
    fontWeight: '600',
  },
  title: {
    color: colors.textPrimary,
    fontSize: fontSize.xl,
    fontWeight: '700',
  },
  subtitle: {
    color: colors.textSecondary,
    fontSize: fontSize.sm,
  },
  content: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: spacing.lg,
  },
  infoCard: {
    backgroundColor: colors.bgCard,
    borderRadius: radius.md,
    borderWidth: 1,
    borderColor: colors.bgCardBorder,
    padding: spacing.md,
    margin: spacing.lg,
  },
  infoText: {
    color: colors.textSecondary,
    fontSize: fontSize.sm,
    lineHeight: 20,
    textAlign: 'center',
  },
});