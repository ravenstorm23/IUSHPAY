#  IUSHPay

> Plataforma institucional de pagos y control de acceso al parqueadero — Institución Universitaria Salazar y Herrera

---

##  Descripción

IUSHPay es una plataforma digital que integra una **wallet institucional**, **validación de acceso por QR dinámico** y conexión con pasarelas de pago (PSE) para optimizar el sistema de pagos y control de acceso al parqueadero de la IUSH.

Elimina los procesos manuales en portería, las inconsistencias de saldo y el fraude por uso indebido de documentos.

---

##  Problema que resuelve

| Problema | Impacto |
|---|---|
| Pagos que no se reflejan en tiempo real | Conflictos y retrasos en portería |
| Recargas erróneas en cédulas o carnets | Pérdida de dinero y desconfianza |
| Sin trazabilidad de transacciones | Imposible auditar operaciones |
| Procesos manuales en portería | Filas de 5 a 15 minutos en hora pico |
| Fraude por uso de cédula de terceros | Suplantación de identidad |

---

##  Funcionalidades principales

-  **Wallet institucional** — Saldo unificado por usuario, recarga vía PSE
-  **QR dinámico** — Código único por sesión, expira en 60 segundos
-  **Control de parqueadero** — Tarifas automáticas, cobro al salir
-  **Autenticación segura** — JWT + refresh tokens, roles por usuario
-  **Auditoría completa** — Logs inmutables de cada transacción
-  **Notificaciones** — Alertas de recarga, cobro y saldo bajo

---

##  Stack tecnológico

| Capa | Tecnología |
|---|---|
| Backend | .NET Core 8 + ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Frontend | React Native + Expo |
| Base de datos | PostgreSQL (Supabase) |
| Autenticación | JWT + Refresh Tokens |
| Pasarela de pagos | PSE / Wompi |
| CI/CD | GitHub Actions + Gitleaks |

---

## Arquitectura

El sistema sigue una arquitectura de **Monolito Modular** con **Clean Architecture** como patrón interno.

```
IUSHPay/
├── src/
│   ├── IUSHPay.API/              # Controladores, middlewares, punto de entrada
│   ├── Modules/
│   │   ├── Auth/                 # Registro, login, roles
│   │   ├── Wallet/               # Saldo, recargas, movimientos
│   │   ├── Payments/             # Integración PSE, webhooks
│   │   ├── Access/               # QR dinámico, validación portería
│   │   ├── Parking/              # Entrada, salida, tarifas
│   │   ├── Audit/                # Registro inmutable de eventos
│   │   └── Notifications/        # Alertas al usuario
│   └── Shared/                   # Contratos, utilidades, constantes
├── tests/
│   ├── Unit/                     # Pruebas unitarias por módulo
│   └── Integration/              # Pruebas de integración
├── docs/
│   └── adr/                      # Decisiones de arquitectura
└── .github/
    └── workflows/                # Pipeline CI/CD
```

---

##  Roles del sistema

| Rol | Permisos |
|---|---|
|  Estudiante | Ver saldo, recargar, generar QR, ver historial |
|  Visitante | Recargar, generar QR de acceso |
|  Portería | Escanear QR, registrar entrada/salida, confirmar cobro |
|  Administrador | Configurar tarifas, ver reportes, gestionar usuarios |

---

##  Instalación y configuración

### Prerrequisitos

- .NET Core 8 SDK
- Node.js 20+
- Expo CLI
- PostgreSQL (Supabase)

### Backend (.NET)

```bash
# Clonar el repositorio
git clone https://github.com/org/iushpay.git
cd iushpay/src/IUSHPay.API

# Restaurar dependencias
dotnet restore

# Configurar variables de entorno
cp appsettings.Example.json appsettings.Development.json
# Editar appsettings.Development.json con tus credenciales

# Correr migraciones
dotnet ef database update

# Levantar el servidor
dotnet run
```

### Frontend (React Native)

```bash
cd iushpay/mobile

# Instalar dependencias
npm install

# Levantar en modo desarrollo
npx expo start
```

---

##  Variables de entorno

** Nunca subir estos archivos al repositorio.**

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Port=6543;Database=postgres;Username=...;Password=..."
  },
  "Jwt": {
    "Secret": "tu-secret-key",
    "ExpiresInMinutes": 15,
    "RefreshExpiresInDays": 7
  },
  "Wompi": {
    "PublicKey": "pub_test_...",
    "PrivateKey": "prv_test_...",
    "WebhookSecret": "..."
  }
}
```

---

##  Seguridad

El sistema implementa una estrategia de seguridad transversal en 5 capas (ver [ADR-004](./docs/adr/ADR-004-seguridad-transversal.md)):

- **Capa 1** — JWT con expiración de 15 min + refresh token en almacenamiento seguro
- **Capa 2** — ORM obligatorio, sin queries SQL crudas, validación de inputs
- **Capa 3** — Variables de entorno, Gitleaks en CI/CD, sin secretos en código
- **Capa 4** — Base de datos sin exposición pública, TLS/SSL obligatorio
- **Capa 5** — Webhooks PSE firmados, sin datos bancarios en el cliente

---

##  Estrategia de ramas

El proyecto usa **GitHub Flow**:

```
main                    → producción, siempre estable
feature/nombre-tarea    → desarrollo de funcionalidades
```

**Reglas:**
- Nunca trabajar directamente en `main`
- Todo cambio entra por Pull Request con mínimo 1 aprobación
- El pipeline CI/CD debe pasar verde antes de mergear

---

##  Decisiones de arquitectura (ADRs)

| ADR | Título | Estado |
|---|---|---|
| [ADR-001](./docs/ADR-001-Arquitectura-del-Sistema.md) | Arquitectura del Sistema | ✅ Aprobado |
| [ADR-002](./docs/ADR-002-Estructura-del-Repositorio.md) | Estructura del Repositorio | ✅ Aprobado |
| [ADR-003](./docs/ADR-003-Arquitectura-y-Tecnologia-del-Backend.md) | Tecnología del Backend | ✅ Aprobado |
| [ADR-004](./docs/ADR-004-seguridad-transversal.md) | Seguridad Transversal | ✅ Aprobado |
| [ADR-005](./docs/ADR-005-frontend.md) | Arquitectura del Frontend | ✅ Aprobado |

---

##  Equipo

| Persona | Rol | Módulos |
|---|---|---|
| Dario Borja Gamboa | Backend — Auth + Wallet | Auth, Wallet, Payments |
| Mateo Rivera Maya | Frontend — QR + Portería | Access, UI móvil |
| Santiago Taborda Valencia | Parqueadero + DevOps | Parking, CI/CD, Infra |

---

##  Licencia

Proyecto académico — Institución Universitaria Salazar y Herrera © 2026
