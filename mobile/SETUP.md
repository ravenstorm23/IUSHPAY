# P2 — QR + Portería · Setup desde cero

## 1. Crear el proyecto

```bash
npx create-expo-app@latest p2-qr-porteria --template blank-typescript
cd p2-qr-porteria
```

## 2. Instalar dependencias

```bash
npx expo install expo-camera expo-secure-store
npm install react-native-qrcode-svg react-native-svg
npm install zustand
npm install axios
npm install @react-navigation/native @react-navigation/bottom-tabs @react-navigation/stack
npx expo install react-native-screens react-native-safe-area-context
```

## 3. Estructura de carpetas

Dentro de `p2-qr-porteria/` debe quedar así:

```
p2-qr-porteria/
├── app/                        ← NO usar (viene con Expo, lo ignoramos)
├── src/
│   ├── api/
│   │   └── qrApi.ts            ← ÚNICO lugar donde se cambia la URL de P1
│   ├── components/
│   │   ├── QRDisplay.tsx       ← QR + countdown + botón regenerar
│   │   └── GuardResult.tsx     ← Resultado validación en portería
│   ├── screens/
│   │   ├── DashboardScreen.tsx ← Vista estudiante
│   │   └── PorteriaScreen.tsx  ← Vista vigilante
│   ├── store/
│   │   └── useSessionStore.ts  ← Zustand: userId en memoria
│   └── theme.ts                ← Colores y fuentes del diseño
├── App.tsx                     ← Navegación raíz
└── .env                        ← Variables de entorno (en .gitignore)
```

## 4. Cuando P1 te pase la URL real

Solo tienes que ir a UN archivo:

```
src/api/qrApi.ts
```

Y cambiar esta línea:

```ts
// MOCK (semana 1-2)
const API_BASE = 'http://localhost:3000';

// PRODUCCIÓN (cuando P1 lo tenga)
const API_BASE = 'https://url-que-te-pase-p1.com';
```

Nada más. Todo el resto del código llama a las funciones de ese archivo.

## 5. Correr el proyecto

```bash
npx expo start
```

Escanea el QR con la app **Expo Go** en tu celular.
Para la tablet de portería, usa el mismo comando y escanea desde esa tablet.

## 6. Variables de entorno

Crea un archivo `.env` en la raíz:

```
EXPO_PUBLIC_API_BASE=http://localhost:3000
```

Y agrégalo al `.gitignore`:

```
.env
.env.local
```
