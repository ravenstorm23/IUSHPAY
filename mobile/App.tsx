import React, { useState } from 'react';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import LoginScreen from './src/screens/LoginScreen';
import DashboardScreen from './src/screens/DashboardScreen';
import QRScreen from './src/screens/QRScreen';
import PorteriaScreen from './src/screens/PorteriaScreen';
import WalletScreen from './src/screens/WalletScreen';
import HistoryScreen from './src/screens/HistoryScreen';
import { useSessionStore } from './src/store/useSessionStore';

type Screen = 'login' | 'home' | 'qr' | 'wallet' | 'history';

function AppContent() {
  const [screen, setScreen] = useState<Screen>('login');
  const role = useSessionStore(state => state.role);

  if (screen === 'login') {
    return <LoginScreen onLoginSuccess={() => setScreen('home')} />;
  }

  if (screen === 'home' && role === 'Admin') {
    return (
      <PorteriaScreen onBack={() => {
        useSessionStore.getState().clearSession();
        setScreen('login');
      }} />
    );
  }

  if (screen === 'home') {
    return (
      <DashboardScreen
        onNavigateToQR={() => setScreen('qr')}
        onNavigateToWallet={() => setScreen('wallet')}
        onNavigateToHistory={() => setScreen('history')}
      />
    );
  }

  if (screen === 'qr') {
    return <QRScreen onBack={() => setScreen('home')} />;
  }

  if (screen === 'wallet') {
    return <WalletScreen onBack={() => setScreen('home')} />;
  }

  if (screen === 'history') {
    return <HistoryScreen onBack={() => setScreen('home')} />;
  }

  return null;
}

export default function App() {
  return (
    <SafeAreaProvider>
      <AppContent />
    </SafeAreaProvider>
  );
}