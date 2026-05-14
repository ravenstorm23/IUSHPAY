import React, { useState } from 'react';
import {
    View, Text, TextInput, TouchableOpacity,
    StyleSheet, ActivityIndicator, KeyboardAvoidingView, Platform, ScrollView,
} from 'react-native';

import { SafeAreaView } from 'react-native-safe-area-context';
import { useSessionStore } from '../store/useSessionStore';
import { colors, spacing, fontSize, radius } from '../theme';
import { login } from '../api/authApi';
import axios from 'axios';

interface Props {
    onRegisterSuccess: () => void;
    onBack: () => void;
}

export default function RegisterScreen({ onRegisterSuccess, onBack }: Props) {
    const [fullName, setFullName] = useState('');
    const [email, setEmail] = useState('');
    const [institutionalCode, setInstitutionalCode] = useState('');
    const [carnetNumber, setCarnetNumber] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const { setSession, setBalance } = useSessionStore();

    const handleRegister = async () => {
        if (!fullName || !email || !institutionalCode || !carnetNumber || !password || !confirmPassword) {
            setError('Todos los campos son obligatorios');
            return;
        }
        if (password !== confirmPassword) {
            setError('Las contraseñas no coinciden');
            return;
        }
        setLoading(true);
        setError('');
        try {
            // Registrar usuario
            await axios.post('https://iushpay.onrender.com/api/auth/register', {
                institutionalCode,
                fullName,
                email,
                password,
                carnetNumber,
            });

            // Login automático después del registro
            const data = await login(email, password);
            setSession(data.userId, data.fullName, data.email, data.token, data.role);

            const { getWallet } = await import('../api/authApi');
            const wallet = await getWallet(data.userId);
            setBalance(wallet.balance);

            onRegisterSuccess();
        } catch (err: any) {
            const message = err?.response?.data?.message ?? err?.response?.data ?? '';
            if (message) {
                setError(String(message));
            } else {
                setError('Error al registrarse. Intenta de nuevo.');
            }
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
                <ScrollView
                    showsVerticalScrollIndicator={false}
                    contentContainerStyle={styles.scrollContent}
                    keyboardShouldPersistTaps="handled"
                >
                    {/* Header */}
                    <View style={styles.header}>
                        <View style={styles.logo}>
                            <Text style={styles.logoText}>P</Text>
                        </View>
                        <Text style={styles.appName}>IUSHPAY</Text>
                        <Text style={styles.appSub}>CAMPUS ACCESS</Text>
                    </View>

                    {/* Formulario */}
                    <View style={styles.form}>
                        <Text style={styles.formTitle}>Crear cuenta</Text>

                        <TextInput
                            style={styles.input}
                            placeholder="Nombre completo"
                            placeholderTextColor={colors.textMuted}
                            value={fullName}
                            onChangeText={setFullName}
                            autoCapitalize="words"
                        />

                        <TextInput
                            style={styles.input}
                            placeholder="Código institucional"
                            placeholderTextColor={colors.textMuted}
                            value={institutionalCode}
                            onChangeText={setInstitutionalCode}
                            autoCapitalize="none"
                        />

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
                            placeholder="Número de carnet"
                            placeholderTextColor={colors.textMuted}
                            value={carnetNumber}
                            onChangeText={setCarnetNumber}
                            autoCapitalize="none"
                        />
                        <View style={styles.inputRow}>
                            <TextInput
                                style={styles.inputFlex}
                                placeholder="Contraseña"
                                placeholderTextColor={colors.textMuted}
                                value={password}
                                onChangeText={setPassword}
                                secureTextEntry={!showPassword}
                            />
                            <TouchableOpacity
                                style={styles.eyeBtn}
                                onPress={() => setShowPassword(!showPassword)}
                            >
                                <Text style={styles.eyeText}>{showPassword ? 'OCULTAR' : 'VER'}</Text>
                            </TouchableOpacity>
                        </View>
                        
                        <View style={styles.inputRow}>
                            <TextInput
                                style={styles.inputFlex}
                                placeholder="Confirmar contraseña"
                                placeholderTextColor={colors.textMuted}
                                value={confirmPassword}
                                onChangeText={setConfirmPassword}
                                secureTextEntry={!showConfirmPassword}
                            />
                            <TouchableOpacity
                                style={styles.eyeBtn}
                                onPress={() => setShowConfirmPassword(!showConfirmPassword)}
                            >
                                <Text style={styles.eyeText}>{showConfirmPassword ? 'OCULTAR' : 'VER'}</Text>
                            </TouchableOpacity>
                        </View>

                        {error !== '' && (
                            <Text style={styles.errorText}>{error}</Text>
                        )}

                        <TouchableOpacity
                            style={[styles.button, loading && styles.buttonDisabled]}
                            onPress={handleRegister}
                            disabled={loading}
                        >
                            {loading
                                ? <ActivityIndicator color="#FFF" />
                                : <Text style={styles.buttonText}>CREAR CUENTA</Text>
                            }
                        </TouchableOpacity>

                        <TouchableOpacity onPress={onBack}>
                            <Text style={styles.backText}>Ya tengo cuenta → Iniciar sesion</Text>
                        </TouchableOpacity>
                    </View>
                </ScrollView>
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
        paddingVertical: spacing.xl,
        paddingHorizontal: spacing.lg,
        gap: spacing.sm,
    },
    logo: {
        width: 80,
        height: 80,
        borderRadius: radius.lg,
        backgroundColor: colors.primary,
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
        flexGrow: 1,
        backgroundColor: colors.bgCard,
        borderTopLeftRadius: 30,
        borderTopRightRadius: 30,
        padding: spacing.xl,
        paddingTop: spacing.xl,
        paddingBottom: spacing.xxl,
        gap: spacing.md,
    },
    formTitle: {
        color: colors.textPrimary,
        fontSize: fontSize.xl,
        fontWeight: '700',
        marginBottom: spacing.sm,
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
        backgroundColor: colors.primary,
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
    backText: {
        color: colors.primaryLight,
        fontSize: fontSize.sm,
        textAlign: 'center',
        marginTop: spacing.sm,
    },
    scrollContent: {
        flexGrow: 1,
    },
    inputRow: {
        flexDirection: 'row',
        alignItems: 'center',
        borderWidth: 1.5,
        borderColor: colors.bgCardBorder,
        borderRadius: radius.full,
        backgroundColor: colors.bgDark,
        paddingHorizontal: spacing.lg,
    },
    inputFlex: {
        flex: 1,
        padding: spacing.md,
        fontSize: fontSize.md,
        color: colors.textPrimary,
    },
    eyeBtn: {
        padding: spacing.sm,
    },
    eyeText: {
        fontSize: fontSize.xs,
        color: colors.textSecondary,
        fontWeight: '700',
    },
});