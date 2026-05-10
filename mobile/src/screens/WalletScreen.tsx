import React, { useState } from 'react';
import {
  View, Text, StyleSheet, TouchableOpacity,
  TextInput, ScrollView, ActivityIndicator,
} from 'react-native';
import { SafeAreaView, useSafeAreaInsets } from 'react-native-safe-area-context';
import { useSessionStore } from '../store/useSessionStore';
import { colors, spacing, fontSize, radius } from '../theme';

interface Props {
  onBack: () => void;
}

const AMOUNTS = [10000, 20000, 50000, 100000];

export default function WalletScreen({ onBack }: Props) {
  const { balance, setBalance } = useSessionStore();
  const [selected, setSelected] = useState<number | null>(20000);
  const [custom, setCustom] = useState('');
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const insets = useSafeAreaInsets();

  const finalAmount = custom ? parseInt(custom) : selected ?? 0;

  const balanceFormatted = (balance ?? 0).toLocaleString('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
  });

  const amountFormatted = finalAmount.toLocaleString('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
  });

  const handleRecharge = async () => {
    if (!finalAmount || finalAmount <= 0) return;
    setLoading(true);
    await new Promise(r => setTimeout(r, 1000));
    setBalance((balance ?? 0) + finalAmount);
    setLoading(false);
    setSuccess(true);
    setTimeout(() => {
      setSuccess(false);
      onBack();
    }, 1500);
  };

  return (
    <SafeAreaView style={styles.safe}>
      <ScrollView
        contentContainerStyle={[styles.scroll, { paddingBottom: insets.bottom + 40 }]}
        showsVerticalScrollIndicator={false}
      >
        {/* Header */}
        <View style={styles.header}>
          <TouchableOpacity onPress={onBack} style={styles.backBtn}>
            <Text style={styles.backText}>← Volver</Text>
          </TouchableOpacity>
          <Text style={styles.title}>Recargar Billetera</Text>
          <View style={styles.currentBalance}>
            <Text style={styles.currentLabel}>Saldo actual</Text>
            <Text style={styles.currentAmount}>{balanceFormatted}</Text>
          </View>
        </View>

        {/* Metodo de pago */}
        <Text style={styles.sectionLabel}>METODO DE PAGO</Text>
        <View style={styles.paymentMethod}>
          <View style={styles.paymentIcon}>
            <Text style={styles.paymentIconText}>PSE</Text>
          </View>
          <View>
            <Text style={styles.paymentName}>PSE</Text>
            <Text style={styles.paymentSub}>Pago seguro en linea</Text>
          </View>
        </View>

        {/* Montos */}
        <Text style={styles.sectionLabel}>SELECCIONA EL MONTO</Text>
        <View style={styles.amountsGrid}>
          {AMOUNTS.map(amount => {
            const isSelected = selected === amount && !custom;
            const label = amount.toLocaleString('es-CO', {
              style: 'currency',
              currency: 'COP',
              minimumFractionDigits: 0,
            });
            return (
              <TouchableOpacity
                key={amount}
                style={[styles.amountCard, isSelected && styles.amountCardSelected]}
                onPress={() => { setSelected(amount); setCustom(''); }}
              >
                {amount === 20000 && (
                  <Text style={styles.popularTag}>POPULAR</Text>
                )}
                <Text style={[styles.amountText, isSelected && styles.amountTextSelected]}>
                  {label}
                </Text>
              </TouchableOpacity>
            );
          })}
        </View>

        {/* Monto personalizado */}
        <Text style={styles.sectionLabel}>O INGRESA UN MONTO</Text>
        <View style={styles.customInput}>
          <Text style={styles.currencySymbol}>$</Text>
          <TextInput
            style={styles.customTextInput}
            placeholder="0"
            placeholderTextColor={colors.textMuted}
            keyboardType="numeric"
            value={custom}
            onChangeText={text => {
              setCustom(text);
              setSelected(null);
            }}
          />
        </View>

        {/* Banner instant top-up */}
        <View style={styles.instantBanner}>
          <Text style={styles.instantTitle}>Recarga Instantanea</Text>
          <Text style={styles.instantSub}>Disponible de inmediato</Text>
        </View>

        {/* Botón confirmar */}
        <TouchableOpacity
          style={[styles.confirmBtn, loading && styles.confirmBtnDisabled]}
          onPress={handleRecharge}
          disabled={loading || !finalAmount}
        >
          {loading ? (
            <ActivityIndicator color="#FFF" />
          ) : success ? (
            <Text style={styles.confirmText}>Recarga exitosa</Text>
          ) : (
            <Text style={styles.confirmText}>
              CONFIRMAR {amountFormatted}
            </Text>
          )}
        </TouchableOpacity>
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.bgDark,
  },
  scroll: {
    padding: spacing.lg,
    gap: spacing.lg,
  },
  header: {
    gap: spacing.sm,
    marginBottom: spacing.sm,
  },
  backBtn: {
    alignSelf: 'flex-start',
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
  currentBalance: {
    backgroundColor: colors.bgCard,
    borderRadius: radius.md,
    borderWidth: 1,
    borderColor: colors.bgCardBorder,
    padding: spacing.md,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  currentLabel: {
    color: colors.textSecondary,
    fontSize: fontSize.sm,
  },
  currentAmount: {
    color: colors.textPrimary,
    fontSize: fontSize.lg,
    fontWeight: '700',
  },
  sectionLabel: {
    color: colors.textMuted,
    fontSize: fontSize.xs,
    fontWeight: '600',
    letterSpacing: 1,
  },
  paymentMethod: {
    backgroundColor: colors.bgCard,
    borderRadius: radius.md,
    borderWidth: 1,
    borderColor: colors.bgCardBorder,
    padding: spacing.md,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },
  paymentIcon: {
    width: 44,
    height: 44,
    borderRadius: radius.sm,
    backgroundColor: colors.purple,
    alignItems: 'center',
    justifyContent: 'center',
  },
  paymentIconText: {
    color: '#FFFFFF',
    fontWeight: '900',
    fontSize: fontSize.xs,
  },
  paymentName: {
    color: colors.textPrimary,
    fontSize: fontSize.md,
    fontWeight: '600',
  },
  paymentSub: {
    color: colors.textSecondary,
    fontSize: fontSize.xs,
  },
  amountsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
  },
  amountCard: {
    width: '47%',
    borderRadius: radius.lg,
    borderWidth: 1.5,
    borderColor: colors.primaryLight,
    backgroundColor: colors.primaryLight,
    padding: spacing.lg,
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: 90,
    gap: spacing.xs,
  },
  amountCardSelected: {
    backgroundColor: colors.primary,
    borderColor: colors.primary,
  },
  popularTag: {
    color: 'rgba(255,255,255,0.7)',
    fontSize: fontSize.xs,
    fontWeight: '700',
    letterSpacing: 1,
  },
  amountText: {
    color: '#FFFFFF',
    fontSize: fontSize.lg,
    fontWeight: '700',
  },
  amountTextSelected: {
    color: '#FFFFFF',
  },
  customInput: {
    backgroundColor: colors.bgCard,
    borderRadius: radius.md,
    borderWidth: 1,
    borderColor: colors.bgCardBorder,
    padding: spacing.md,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },
  currencySymbol: {
    color: colors.textSecondary,
    fontSize: fontSize.lg,
    fontWeight: '700',
  },
  customTextInput: {
    flex: 1,
    color: colors.textPrimary,
    fontSize: fontSize.lg,
  },
  instantBanner: {
    backgroundColor: colors.purpleDark,
    borderRadius: radius.lg,
    padding: spacing.lg,
    alignItems: 'center',
    gap: spacing.xs,
  },
  instantTitle: {
    color: '#FFFFFF',
    fontSize: fontSize.md,
    fontWeight: '700',
  },
  instantSub: {
    color: 'rgba(255,255,255,0.7)',
    fontSize: fontSize.sm,
  },
  confirmBtn: {
    backgroundColor: colors.purple,
    borderRadius: radius.full,
    padding: spacing.md + 4,
    alignItems: 'center',
  },
  confirmBtnDisabled: {
    opacity: 0.6,
  },
  confirmText: {
    color: '#FFFFFF',
    fontSize: fontSize.md,
    fontWeight: '700',
    letterSpacing: 1,
  },
});