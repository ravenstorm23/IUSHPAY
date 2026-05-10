import React, { useState } from 'react';
import {
  View, Text, StyleSheet, ScrollView, TouchableOpacity,
} from 'react-native';
import { SafeAreaView, useSafeAreaInsets } from 'react-native-safe-area-context';
import { colors, spacing, fontSize, radius } from '../theme';

interface Props {
  onBack: () => void;
}

type FilterType = 'ALL' | 'PARKING' | 'TOP-UPS';

const TRANSACTIONS = [
  {
    id: '1',
    date: 'Oct 12',
    month: 'Octubre 2026',
    title: 'Garaje Norte',
    sub: 'Pase diario - 4h 20m',
    amount: -4000,
    type: 'PARKING',
  },
  {
    id: '2',
    date: 'Oct 10',
    month: 'Octubre 2026',
    title: 'Recarga Billetera',
    sub: 'PSE',
    amount: 20000,
    type: 'TOP-UPS',
  },
  {
    id: '3',
    date: 'Oct 08',
    month: 'Octubre 2026',
    title: 'Laboratorio de Ciencias',
    sub: 'Tarifa por hora - 6h 00m',
    amount: -6500,
    type: 'PARKING',
  },
  {
    id: '4',
    date: 'Oct 05',
    month: 'Octubre 2026',
    title: 'Puerta Sur',
    sub: 'Parqueo especial',
    amount: -4000,
    type: 'PARKING',
  },
  {
    id: '5',
    date: 'Sep 28',
    month: 'Septiembre 2026',
    title: 'Biblioteca Principal',
    sub: 'Tarifa nocturna',
    amount: -12000,
    type: 'PARKING',
  },
  {
    id: '6',
    date: 'Sep 20',
    month: 'Septiembre 2026',
    title: 'Recarga Billetera',
    sub: 'PSE',
    amount: 50000,
    type: 'TOP-UPS',
  },
];

export default function HistoryScreen({ onBack }: Props) {
  const [filter, setFilter] = useState<FilterType>('ALL');
  const insets = useSafeAreaInsets();

  const filtered = TRANSACTIONS.filter(t =>
    filter === 'ALL' ? true : t.type === filter
  );

  const grouped = filtered.reduce((acc, t) => {
    if (!acc[t.month]) acc[t.month] = [];
    acc[t.month].push(t);
    return acc;
  }, {} as Record<string, typeof TRANSACTIONS>);

  return (
    <SafeAreaView style={styles.safe}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={onBack} style={styles.backBtn}>
          <Text style={styles.backText}>← Volver</Text>
        </TouchableOpacity>
        <Text style={styles.title}>Historial</Text>
      </View>

      {/* Filtros */}
      <View style={styles.filters}>
        {(['ALL', 'PARKING', 'TOP-UPS'] as FilterType[]).map(f => (
          <TouchableOpacity
            key={f}
            style={[styles.filterBtn, filter === f && styles.filterBtnActive]}
            onPress={() => setFilter(f)}
          >
            <Text style={[styles.filterText, filter === f && styles.filterTextActive]}>
              {f}
            </Text>
          </TouchableOpacity>
        ))}
      </View>

      <ScrollView
        contentContainerStyle={[styles.scroll, { paddingBottom: insets.bottom + 40 }]}
        showsVerticalScrollIndicator={false}
      >
        {Object.entries(grouped).map(([month, transactions]) => (
          <View key={month} style={styles.group}>
            <Text style={styles.monthLabel}>{month}</Text>
            <View style={styles.groupCard}>
              {transactions.map((t, index) => (
                <View key={t.id}>
                  {index > 0 && <View style={styles.divider} />}
                  <View style={styles.transactionRow}>
                    <View style={[
                      styles.transactionIcon,
                      t.amount > 0 ? styles.iconTopUp : styles.iconParking
                    ]}>
                      <Text style={styles.transactionIconText}>
                        {t.amount > 0 ? '+' : 'P'}
                      </Text>
                    </View>
                    <View style={styles.transactionInfo}>
                      <Text style={styles.transactionTitle}>{t.title}</Text>
                      <Text style={styles.transactionSub}>{t.sub}</Text>
                      <Text style={styles.transactionDate}>{t.date}</Text>
                    </View>
                    <Text style={[
                      styles.transactionAmount,
                      t.amount > 0 ? styles.amountPositive : styles.amountNegative
                    ]}>
                      {t.amount > 0 ? '+' : ''}
                      {t.amount.toLocaleString('es-CO', {
                        style: 'currency',
                        currency: 'COP',
                        minimumFractionDigits: 0,
                      })}
                    </Text>
                  </View>
                </View>
              ))}
            </View>
          </View>
        ))}
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
    padding: spacing.lg,
    gap: spacing.xs,
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
    fontSize: fontSize.xxl,
    fontWeight: '900',
  },
  filters: {
    flexDirection: 'row',
    paddingHorizontal: spacing.lg,
    gap: spacing.sm,
    marginBottom: spacing.md,
  },
  filterBtn: {
    borderWidth: 1,
    borderColor: colors.bgCardBorder,
    borderRadius: radius.full,
    paddingVertical: spacing.xs,
    paddingHorizontal: spacing.md,
  },
  filterBtnActive: {
    backgroundColor: colors.purple,
    borderColor: colors.purple,
  },
  filterText: {
    color: colors.textSecondary,
    fontSize: fontSize.xs,
    fontWeight: '700',
    letterSpacing: 1,
  },
  filterTextActive: {
    color: '#FFFFFF',
  },
  scroll: {
    padding: spacing.lg,
    gap: spacing.lg,
  },
  group: {
    gap: spacing.sm,
  },
  monthLabel: {
    color: colors.textPrimary,
    fontSize: fontSize.md,
    fontWeight: '700',
  },
  groupCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: radius.lg,
    overflow: 'hidden',
  },
  divider: {
    height: 1,
    backgroundColor: '#F3F4F6',
    marginHorizontal: spacing.md,
  },
  transactionRow: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: spacing.md,
    gap: spacing.md,
  },
  transactionIcon: {
    width: 40,
    height: 40,
    borderRadius: radius.full,
    alignItems: 'center',
    justifyContent: 'center',
  },
  iconParking: {
    backgroundColor: '#EDE9FE',
  },
  iconTopUp: {
    backgroundColor: '#D1FAE5',
  },
  transactionIconText: {
    fontSize: fontSize.md,
    fontWeight: '700',
    color: colors.purple,
  },
  transactionInfo: {
    flex: 1,
    gap: 2,
  },
  transactionTitle: {
    fontSize: fontSize.sm,
    fontWeight: '700',
    color: '#111827',
  },
  transactionSub: {
    fontSize: fontSize.xs,
    color: '#6B7280',
  },
  transactionDate: {
    fontSize: fontSize.xs,
    color: '#9CA3AF',
  },
  transactionAmount: {
    fontSize: fontSize.sm,
    fontWeight: '700',
  },
  amountPositive: {
    color: colors.success,
  },
  amountNegative: {
    color: colors.error,
  },
});