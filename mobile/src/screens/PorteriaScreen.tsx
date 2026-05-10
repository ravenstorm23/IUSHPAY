// src/screens/PorteriaScreen.tsx
// Vista del vigilante — cámara, escaneo QR, resultado.
// Usa expo-camera para acceder a la cámara sin app nativa.

import React, { useState, useRef, useCallback } from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { CameraView, useCameraPermissions } from 'expo-camera';
import GuardResult from '../components/GuardResult';
import { validateQR, ValidateQRResponse } from '../api/qrApi';
import { colors, spacing, fontSize, radius } from '../theme';

type Phase =
  | { name: 'scanning' }
  | { name: 'loading' }
  | { name: 'result'; result: ValidateQRResponse }
  | { name: 'error'; message: string };

interface Props {
  onBack?: () => void;
}

export default function PorteriaScreen({ onBack }: Props) {
  const [permission, requestPermission] = useCameraPermissions();
  const [phase, setPhase] = useState<Phase>({ name: 'scanning' });
  const isProcessing = useRef(false);

  const handleBarcode = useCallback(async ({ data }: { data: string }) => {
    if (isProcessing.current) return; // evitar doble lectura
    isProcessing.current = true;

    setPhase({ name: 'loading' });
    try {
      const result = await validateQR(data);
      setPhase({ name: 'result', result });
    } catch {
      setPhase({ name: 'error', message: 'Error al conectar con el servidor.' });
    }
  }, []);

  const handleReset = useCallback(() => {
    isProcessing.current = false;
    setPhase({ name: 'scanning' });
  }, []);

  // ─── Sin permiso de cámara ───
  if (!permission) return <View style={styles.safe} />;

  if (!permission.granted) {
    return (
      <SafeAreaView style={styles.safe}>
        <View style={styles.center}>
          <Text style={styles.permText}>
            Se necesita acceso a la cámara para escanear QR
          </Text>
          <TouchableOpacity style={styles.permBtn} onPress={requestPermission}>
            <Text style={styles.permBtnText}>Conceder permiso</Text>
          </TouchableOpacity>
        </View>
      </SafeAreaView>
    );
  }

  return (
    <SafeAreaView style={styles.safe}>
      <View style={styles.container}>

        {/* Header */}
        <View style={styles.header}>
          <Text style={styles.title}>Control de Acceso</Text>
          <Text style={styles.subtitle}>Garaje · Portería</Text>
          {onBack && (
            <TouchableOpacity onPress={onBack} style={{ marginTop: 8 }}>
              <Text style={{ color: colors.purpleLight, fontSize: fontSize.sm }}>
                Cerrar sesión
              </Text>
            </TouchableOpacity>
          )}
        </View>

        {/* Scanner */}
        {phase.name === 'scanning' && (
          <View style={styles.scannerSection}>
            <View style={styles.cameraWrapper}>
              <CameraView
                style={styles.camera}
                facing="back"
                barcodeScannerSettings={{ barcodeTypes: ['qr'] }}
                onBarcodeScanned={handleBarcode}
              />
              {/* Overlay con guía visual */}
              <View style={styles.overlay}>
                <View style={styles.scanFrame} />
              </View>
            </View>
            <Text style={styles.hint}>
              Apunta la cámara al QR del estudiante
            </Text>
          </View>
        )}

        {/* Cargando */}
        {phase.name === 'loading' && (
          <View style={styles.center}>
            <Text style={styles.loadingText}>Validando QR...</Text>
          </View>
        )}

        {/* Resultado */}
        {phase.name === 'result' && (
          <View style={styles.resultSection}>
            <GuardResult result={phase.result} onReset={handleReset} />
          </View>
        )}

        {/* Error de red */}
        {phase.name === 'error' && (
          <View style={styles.center}>
            <Text style={styles.errorText}>{phase.message}</Text>
            <TouchableOpacity style={styles.retryBtn} onPress={handleReset}>
              <Text style={styles.retryText}>Reintentar</Text>
            </TouchableOpacity>
          </View>
        )}

      </View>
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
    padding: spacing.lg,
    gap: spacing.lg,
  },
  header: {
    alignItems: 'center',
  },
  title: {
    color: colors.textPrimary,
    fontSize: fontSize.xl,
    fontWeight: '700',
  },
  subtitle: {
    color: colors.textSecondary,
    fontSize: fontSize.sm,
    marginTop: 2,
  },
  scannerSection: {
    flex: 1,
    alignItems: 'center',
    gap: spacing.md,
  },
  cameraWrapper: {
    width: '100%',
    maxWidth: 380,
    aspectRatio: 1,
    borderRadius: radius.lg,
    overflow: 'hidden',
    position: 'relative',
    borderWidth: 2,
    borderColor: colors.purple,
  },
  camera: {
    flex: 1,
  },
  overlay: {
    ...StyleSheet.absoluteFillObject,
    alignItems: 'center',
    justifyContent: 'center',
  },
  scanFrame: {
    width: 200,
    height: 200,
    borderWidth: 2,
    borderColor: 'rgba(255,255,255,0.6)',
    borderRadius: radius.sm,
  },
  hint: {
    color: colors.textSecondary,
    fontSize: fontSize.sm,
    textAlign: 'center',
  },
  resultSection: {
    flex: 1,
  },
  center: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    gap: spacing.lg,
  },
  loadingText: {
    color: colors.textPrimary,
    fontSize: fontSize.lg,
  },
  permText: {
    color: colors.textSecondary,
    fontSize: fontSize.md,
    textAlign: 'center',
    marginBottom: spacing.md,
  },
  permBtn: {
    backgroundColor: colors.purple,
    borderRadius: radius.md,
    paddingVertical: spacing.md,
    paddingHorizontal: spacing.xl,
  },
  permBtnText: {
    color: '#FFF',
    fontWeight: '700',
    fontSize: fontSize.md,
  },
  errorText: {
    color: colors.error,
    fontSize: fontSize.md,
    textAlign: 'center',
  },
  retryBtn: {
    backgroundColor: colors.purple,
    borderRadius: radius.md,
    paddingVertical: spacing.md,
    paddingHorizontal: spacing.xl,
  },
  retryText: {
    color: '#FFF',
    fontWeight: '700',
    fontSize: fontSize.md,
  },
});
