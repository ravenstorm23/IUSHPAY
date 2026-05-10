// src/screens/LoginScreen.tsx
import React, { useState } from 'react';
import {
  View, Text, TextInput, TouchableOpacity,
  StyleSheet, ActivityIndicator, KeyboardAvoidingView, Platform,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { login, getWallet } from '../api/authApi';
import { useSessionStore } from '../store/useSessionStore';
import { colors, spacing, radius, fontSize } from '../theme';

interface Props {
  onLoginSuccess: () => void;
}

export default function LoginScreen({ onLoginSuccess }: Props) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const { setSession, setBalance } = useSessionStore();

  const handleLogin = async () => {
    if (!email || !password) {
      setError('Ingresa tu email y contrasena');
      return;
    }
    setLoading(true);
    setError('');
    try {
      const data = await login(email, password);
      setSession(data.userId, data.fullName, data.email, data.token, data.role);

      // Obtener saldo
      const wallet = await getWallet(data.userId);
      setBalance(wallet.balance);

      onLoginSuccess();
    } catch (err: any) {
      setError('Credenciales incorrectas. Intenta de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <SafeAreaView style={styles.safe}>
      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        style={styles.container}
      >
        {/* Header morado */}
        <View style={styles.header}>
          <View style={styles.logo}>
            <Text style={styles.logoText}>P</Text>
          </View>
          <Text style={styles.appName}>IUSHPAY</Text>
          <Text style={styles.appSub}>CAMPUS ACCESS</Text>
        </View>

        {/* Formulario */}
        <View style={styles.form}>
          <TextInput
            style={styles.input}
            placeholder="Email institucional"
            placeholderTextColor={colors.textMuted}
            value={email}
            onChangeText={setEmail}
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="off"
          />

          <TextInput
            style={styles.input}
            placeholder="Contrasena"
            placeholderTextColor={colors.textMuted}
            value={password}
            onChangeText={setPassword}
            secureTextEntry
          />

          {error !== '' && (
            <Text style={styles.errorText}>{error}</Text>
          )}

          <TouchableOpacity
            style={[styles.button, loading && styles.buttonDisabled]}
            onPress={handleLogin}
            disabled={loading}
          >
            {loading
              ? <ActivityIndicator color="#FFF" />
              : <Text style={styles.buttonText}>INICIAR SESION  →</Text>
            }
          </TouchableOpacity>

          <TouchableOpacity>
            <Text style={styles.forgotText}>Olvide mi ID</Text>
          </TouchableOpacity>
        </View>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.bgDark,
  },
  container: {
    flex: 1,
  },
  header: {
    backgroundColor: colors.bgDark,
    alignItems: 'center',
    paddingVertical: spacing.xxl,
    paddingHorizontal: spacing.lg,
    gap: spacing.sm,
  },
  logo: {
    width: 80,
    height: 80,
    borderRadius: radius.lg,
    backgroundColor: colors.purple,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: spacing.sm,
  },
  logoText: {
    color: '#FFFFFF',
    fontSize: 40,
    fontWeight: '900',
  },
  appName: {
    color: '#FFFFFF',
    fontSize: fontSize.xxl,
    fontWeight: '900',
    letterSpacing: 4,
  },
  appSub: {
    color: colors.textSecondary,
    fontSize: fontSize.sm,
    letterSpacing: 3,
    fontWeight: '600',
  },
  form: {
    flex: 1,
    backgroundColor: colors.bgCard,
    borderTopLeftRadius: 30,
    borderTopRightRadius: 30,
    padding: spacing.xl,
    paddingTop: spacing.xxl,
    gap: spacing.md,
  },
  input: {
    borderWidth: 1.5,
    borderColor: colors.bgCardBorder,
    borderRadius: radius.full,
    padding: spacing.md,
    paddingHorizontal: spacing.lg,
    fontSize: fontSize.md,
    color: colors.textPrimary,
    backgroundColor: colors.bgDark,
  },
  errorText: {
    color: colors.error,
    fontSize: fontSize.sm,
    textAlign: 'center',
  },
  button: {
    backgroundColor: colors.purple,
    borderRadius: radius.full,
    padding: spacing.md,
    alignItems: 'center',
    marginTop: spacing.sm,
  },
  buttonDisabled: {
    opacity: 0.6,
  },
  buttonText: {
    color: '#FFFFFF',
    fontSize: fontSize.md,
    fontWeight: '700',
    letterSpacing: 1,
  },
  forgotText: {
    color: colors.purpleLight,
    fontSize: fontSize.sm,
    textAlign: 'center',
    marginTop: spacing.sm,
  },
});