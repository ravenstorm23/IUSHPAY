# ADR-005 — Arquitectura y Tecnología del Frontend

| Campo | Valor |
|---|---|
| **Estado** | Aceptado |
| **Versión** | 1.0 |
| **Fecha** | Abril 2026 |
| **Autor** | Equipo de Frontend — IUSHPay |
| **Proyecto** | IUSHPay — IUSH |

---

## 1. Contexto

El prototipo de alto nivel de IUSHPay define cinco flujos de pantalla visibles para el usuario final: Autenticación, Home / Dashboard, Scanner / Acceso por QR, Recarga de Wallet (Top-Up) e Historial de Pagos. Estas pantallas operan sobre dispositivos móviles y deben integrarse con la wallet digital, el sistema de control de acceso QR y la pasarela PSE.

La ausencia de una decisión explícita sobre la arquitectura del frontend puede derivar en inconsistencia visual entre pantallas, almacenamiento inseguro de tokens en el dispositivo, acoplamiento excesivo entre lógica de negocio y componentes de UI, y dificultad para mantener el estado global de saldo y sesión.

| Pantalla | Propósito | Componentes clave |
|---|---|---|
| Authentication | Inicio de sesión con Student ID | Campo de texto, botón LOG IN, enlace "Forgot ID" |
| Home / Dashboard | Vista principal post-login | Saldo actual, acceso rápido a parqueadero, actividad reciente, notificaciones |
| Scanner / Gate Access | Código QR dinámico para acceso al parqueadero | Imagen QR animada, temporizador de 04:59, opción NFC, indicador de estado ACTIVE |
| Top-Up Wallet | Recarga del saldo de la wallet | Selector de monto ($10, $20, $50, $100), campo personalizado, Apple Pay, botón de confirmación con slide |
| Payment History | Historial de transacciones y recargas | Filtros ALL / PARKING / TOP-UPS, lista agrupada por fecha, buscador |

Dado que la aplicación es de naturaleza móvil y maneja datos financieros sensibles, se requiere una decisión arquitectónica que garantice rendimiento, seguridad en cliente y alineación con el [ADR-004](./ADR-004-seguridad-transversal.md).

---

## 2. Decisión

Se adopta una arquitectura de frontend móvil basada en **React Native con Expo** para las cinco pantallas identificadas en el prototipo, complementada con las siguientes decisiones específicas por área.

### 2.1. Stack Tecnológico

- **Framework principal:** React Native con Expo (SDK estable más reciente). Permite desarrollo multiplataforma iOS y Android desde una sola base de código.
- **Lenguaje:** TypeScript obligatorio en todo el proyecto. El tipado estático reduce errores en el manejo del JWT y los montos financieros.
- **Estilos:** StyleSheet de React Native. La paleta de diseño (púrpura, blanco, negro) se centraliza en un archivo `theme.ts` como tokens de color.
- **Navegación:** React Navigation v6 con un Bottom Tab Navigator que agrupa las pestañas: Home, Scan, Wallet e History.
- **Generación de QR:** librería `react-native-qrcode-svg` para renderizar el código QR dinámico en la pantalla Scanner / Gate Access.
- **NFC:** `expo-nfc` para la opción "Use NFC Instead" presente en la pantalla de acceso al parqueadero.

### 2.2. Estructura de Navegación y Pantallas

- La aplicación usará un **Stack Navigator raíz** con dos ramas: Auth Stack (pantalla Authentication) y App Stack (tabs).
- La pantalla Authentication es la única accesible sin sesión válida. Cualquier intento de navegación sin token redirige automáticamente a esta pantalla.
- El Bottom Tab Navigator incluye cuatro pestañas con íconos alineados con la barra inferior del prototipo: Home, Scan, Wallet e History.
- La pantalla Scanner / Gate Access mostrará un **temporizador regresivo (04:59)** implementado con un hook `useCountdown`. Al expirar, el QR se regenera solicitando un nuevo token al backend.
- La pantalla Top-Up Wallet implementará el gesto **slide-to-confirm** mediante `react-native-gesture-handler` para mayor seguridad en la confirmación de montos.

### 2.3. Gestión de Estado y Datos

- **Estado global:** Zustand. Almacena el saldo actual, el perfil del usuario y el token de sesión en memoria durante la sesión activa.
- **Peticiones HTTP:** Axios con interceptores para inyectar el JWT en cada solicitud y manejar renovación automática del access token mediante refresh token (alineado con ADR-004, Capa 1).
- **Caché de datos:** React Query (TanStack Query) para el historial de transacciones y el saldo, con TTL cortos: 30 segundos para saldo, 2 minutos para historial.
- El historial de pagos implementará **paginación tipo cursor** y filtros locales (ALL / PARKING / TOP-UPS) sin recargas adicionales al servidor.

### 2.4. Seguridad y Almacenamiento en Cliente

- El refresh token se almacenará en **`expo-secure-store`** (Keychain en iOS / Keystore en Android). Queda estrictamente **prohibido** usar `AsyncStorage` para datos de sesión o credenciales (alineado con ADR-004, Capa 1).
- El access token JWT vive **únicamente en memoria** (estado Zustand). No se persiste en disco bajo ninguna circunstancia.
- Las pantallas que muestran saldo o datos financieros deben **oscurecer el contenido** cuando la aplicación pasa a segundo plano (evento `AppState` change), previniendo capturas del sistema.
- No se almacenarán datos de tarjetas, cuentas bancarias ni información de PSE en el dispositivo. El frontend únicamente envía referencias de pago al backend (alineado con ADR-004, Capa 5).
- El campo Student ID en la pantalla de autenticación deshabilitará el autocompletado del sistema operativo para evitar caché de credenciales.

### 2.5. Accesibilidad y Diseño Visual

- Los colores primarios (púrpura, blanco y negro) extraídos del prototipo se definirán como tokens en un archivo `theme.ts` centralizado.
- Las tarjetas de acceso rápido (Parking Access) y los botones de recarga deben tener tamaños de toque mínimos de **44 × 44 pt** (directriz Apple HIG y Android Material).
- El código QR incluirá texto alternativo de accesibilidad para lectores de pantalla (`"Código QR de acceso activo"`).
- La pantalla de recarga garantizará contraste suficiente (**≥ 4.5:1 WCAG AA**) entre el texto del monto y el fondo del botón de confirmación.
- Las transiciones de pantalla no superarán los **200 ms** de duración para garantizar fluidez en dispositivos de gama media.

---

## 3. Alternativas Consideradas

| Alternativa | Ventaja | Razón de descarte |
|---|---|---|
| Flutter (Dart) | Rendimiento nativo cercano al 100%; componentes propios consistentes entre plataformas | El equipo no tiene experiencia en Dart; aumenta la curva de aprendizaje y el tiempo de entrega |
| Aplicación web progresiva (PWA) | No requiere instalación; único código para web y móvil | Acceso limitado a NFC y Keychain del dispositivo; experiencia inferior para el código QR dinámico |
| React Native CLI sin Expo | Mayor control sobre el código nativo y los módulos | Expo SDK cubre todas las necesidades identificadas; el CLI añade complejidad de configuración sin beneficio tangible en esta fase |
| JavaScript puro sin TypeScript | Menor configuración inicial | Sin tipado, los errores en el manejo del JWT y los montos financieros son más probables y difíciles de detectar |

---

## 4. Consecuencias

### Consecuencias Positivas

- Una única base de código en React Native + Expo cubre iOS y Android, reduciendo el costo de mantenimiento.
- TypeScript + React Query + Zustand conforman un stack predecible y ampliamente documentado, que acelera la incorporación de nuevos desarrolladores.
- El uso de `expo-secure-store` garantiza que el refresh token esté protegido por el hardware del dispositivo (enclave seguro), alineado con ADR-004.
- La arquitectura de navegación refleja fielmente el flujo del prototipo, minimizando discrepancias entre diseño e implementación.

### Compromisos y Consideraciones

- Expo Managed Workflow limita el acceso a módulos nativos no soportados oficialmente. Si surgen requerimientos de hardware avanzados, puede ser necesario migrar a Expo Bare Workflow.
- La lógica del temporizador QR y la regeneración del token implican coordinación estrecha con el backend, que debe documentarse en un ADR de API.
- El gesto slide-to-confirm requiere pruebas de usabilidad para asegurar que el flujo de recarga no resulte frustrante en dispositivos con pantallas pequeñas.

### Impacto en Atributos de Calidad

| Atributo | Impacto | Detalle |
|---|---|---|
| Seguridad | Alto (+) | Tokens en Secure Store; oscurecimiento en segundo plano; sin datos bancarios en cliente |
| Usabilidad | Alto (+) | Navegación con tabs, toque mínimo 44 pt, contraste WCAG AA |
| Rendimiento | Medio (+) | React Query con TTL corto reduce solicitudes; transiciones ≤ 200 ms |
| Portabilidad | Alto (+) | Código único para iOS y Android con Expo |
| Mantenibilidad | Medio (+) | TypeScript y Zustand reducen efectos secundarios inesperados |
| Fiabilidad | Medio (+) | Renovación automática del QR y del JWT evita sesiones caducadas en uso activo |

---

## 5. Notas Adicionales

Este ADR complementa y extiende el [ADR-004](./ADR-004-seguridad-transversal.md) (Estrategia de Seguridad Transversal). Cualquier cambio en el mecanismo de autenticación o en la gestión de tokens definido en ADR-004 debe evaluarse también en el contexto de este documento.

**Momentos de revisión sugeridos:**

- Al seleccionar la versión de Expo SDK y React Navigation para el proyecto.
- Antes de implementar la pantalla Scanner / Gate Access, verificando la disponibilidad del módulo NFC en los dispositivos de prueba.
- Al definir el contrato de API con el backend para el endpoint de regeneración del código QR.
- Antes del primer despliegue en dispositivos físicos, ejecutando una revisión de seguridad en cliente (Secure Store, oscurecimiento, ausencia de datos sensibles en AsyncStorage).
