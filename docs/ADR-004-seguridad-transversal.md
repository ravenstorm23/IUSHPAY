# ADR-004 — Estrategia de Seguridad Transversal

| Campo | Valor |
|---|---|
| **Estado** | Aceptado |
| **Versión** | 1.0 |
| **Fecha** | Abril 2026 |
| **Autor** | Equipo de Seguridad — IUSHPay |
| **Proyecto** | IUSHPay — IUSH |

---

## 1. Contexto

IUSHPay es una plataforma institucional que integra una wallet digital, control de acceso al parqueadero mediante QR dinámico y conexión con pasarelas de pago externas (PSE). El sistema gestiona datos financieros reales de estudiantes y visitantes, movimientos de saldo, transacciones con entidades bancarias y validación de identidad en portería.

La ausencia de una estrategia de seguridad definida desde el inicio expone el sistema a los siguientes riesgos:

| Riesgo | Impacto potencial |
|---|---|
| Inyección SQL / XML | Acceso no autorizado a la base de datos de estudiantes y saldos |
| Credenciales en código fuente | Exposición de claves de PSE, BD y servicios internos en repositorios |
| Base de datos expuesta | Consulta o modificación directa de saldos y transacciones sin autenticación |
| Tokens de sesión débiles | Suplantación de identidad en portería o en la wallet del estudiante |
| Datos de pago en logs | Filtración de información financiera sensible en registros del sistema |

Dado que el equipo de desarrollo aún no ha definido el stack tecnológico final, esta decisión establece principios y restricciones de seguridad agnósticos al framework, de modo que sean válidos independientemente de la tecnología que se elija.

---

## 2. Decisión

Se adopta una estrategia de seguridad transversal basada en cinco capas de protección, cada una con restricciones obligatorias para todo el sistema IUSHPay.

### 2.1. Capa 1 — Autenticación y Autorización

- Se implementará autenticación basada en **JWT (JSON Web Token)** con tiempo de expiración corto (máximo 15 minutos para el access token).
- Se usará un **refresh token** de larga duración almacenado de forma segura (HttpOnly cookie o equivalente), nunca en `localStorage`.
- El control de acceso seguirá el **principio de mínimo privilegio**: cada rol (estudiante, visitante, portero, administrador) tendrá permisos explícitos y acotados.
- Las sesiones del portal de portería deberán invalidarse automáticamente tras **inactividad de 10 minutos**.

### 2.2. Capa 2 — Protección contra Inyecciones

- Queda **prohibido** el uso de queries SQL crudas (raw queries) en cualquier parte del código. Toda interacción con la base de datos debe realizarse mediante ORM o prepared statements parametrizados.
- Todo input del usuario (formularios, parámetros de URL, headers) deberá ser **validado y sanitizado** antes de ser procesado.
- Se aplicará validación estricta de esquema para datos XML o JSON recibidos desde pasarelas externas, incluyendo PSE.
- Las respuestas de error **nunca** deben exponer detalles internos del sistema (nombre de tablas, rutas de archivos, stack traces).

### 2.3. Capa 3 — Variables de Entorno y Gestión de Secretos

- Ninguna credencial, API key, cadena de conexión o secreto puede estar escrito directamente en el código fuente (**hardcoded**).
- Todas las variables sensibles deben gestionarse mediante variables de entorno (`.env`) o un gestor de secretos compatible con el entorno de despliegue.
- Los archivos `.env` deben estar incluidos en el `.gitignore` del repositorio. Su presencia en el control de versiones constituye una **falla crítica de seguridad**.
- El pipeline CI/CD debe incluir una etapa de detección de secretos expuestos (por ejemplo, con **Gitleaks**) que bloquee el merge si detecta credenciales.

### 2.4. Capa 4 — Protección y Aislamiento de la Base de Datos

- La base de datos **nunca** debe estar expuesta públicamente. Solo la capa de servicios internos del backend puede acceder a ella, mediante red privada o VPC.
- Se deben usar cuentas de base de datos con **privilegios mínimos**: el usuario de la aplicación no debe tener permisos de `DROP`, `ALTER` o acceso a tablas que no le correspondan.
- Toda comunicación entre el backend y la base de datos debe estar cifrada (**TLS/SSL**).
- Se deben habilitar **logs de auditoría** en la base de datos para registrar operaciones de escritura críticas (modificación de saldos, inserción de transacciones).

### 2.5. Capa 5 — Seguridad en la Integración con Pasarelas de Pago (PSE)

- Las credenciales de PSE (`client_id`, `client_secret`, API keys) deben almacenarse exclusivamente como variables de entorno, nunca en el código.
- Toda comunicación con PSE debe realizarse exclusivamente sobre **HTTPS**. Cualquier llamada por HTTP debe ser rechazada.
- IUSHPay **no debe almacenar** datos de tarjetas ni información bancaria del usuario. La responsabilidad del dato financiero sensible recae en PSE.
- Los datos de transacciones (montos, referencias) no deben registrarse en texto plano en logs del sistema.
- Se debe validar la autenticidad de los webhooks o callbacks enviados por PSE mediante **firma digital** o token de verificación provisto por la pasarela.

---

## 3. Alternativas Consideradas

| Alternativa | Ventaja | Razón de descarte |
|---|---|---|
| Sesiones tradicionales en servidor en lugar de JWT | Revocación inmediata de sesiones | Mayor acoplamiento al servidor; dificulta escalabilidad horizontal |
| Queries SQL crudas con escape manual | Flexibilidad en consultas complejas | Alta probabilidad de error humano; el ORM ofrece la misma flexibilidad con mayor seguridad |
| Almacenar secretos en base de datos cifrada internamente | Centralización de secretos | Introduce dependencia circular; las variables de entorno son el estándar de la industria |
| Base de datos con IP pública restringida por firewall | Simplicidad de configuración | Una mala configuración de firewall expone toda la BD; el aislamiento por red es más robusto |

---

## 4. Consecuencias

### Consecuencias Positivas

- El sistema queda protegido contra los vectores de ataque más comunes: inyección SQL, XSS, robo de credenciales y exposición de datos financieros.
- La estrategia es **agnóstica al stack**: aplica independientemente del framework backend que elija el equipo.
- Se establece una base de auditoría y trazabilidad que facilita el cumplimiento de normativas de protección de datos.
- La integración con PSE cumple con buenas prácticas del sector financiero desde el primer día de desarrollo.

### Compromisos y Consideraciones

- El equipo deberá familiarizarse con el uso correcto del ORM elegido, lo cual puede implicar una curva de aprendizaje inicial.
- La gestión de variables de entorno requiere coordinación entre miembros del equipo para compartir configuraciones locales sin subirlas al repositorio.
- Los JWT de corta duración implican implementar lógica de renovación automática (refresh token flow), lo cual añade complejidad al frontend.

### Impacto en Atributos de Calidad

| Atributo | Impacto | Detalle |
|---|---|---|
| Seguridad | Alto (+) | Protección multicapa desde el diseño |
| Fiabilidad | Medio (+) | Auditoría y logs reducen errores silenciosos |
| Rendimiento | Neutro | JWT es ligero; impacto mínimo en latencia |
| Interoperabilidad | Medio (+) | Cumplimiento con estándares facilita integraciones futuras |
| Mantenibilidad | Medio (+) | Código sin secretos hardcoded es más fácil de auditar |

---

## 5. Notas Adicionales

Este ADR debe considerarse como un **contrato de seguridad mínimo** para el proyecto IUSHPay. Cualquier decisión técnica posterior que entre en conflicto con los principios aquí establecidos requerirá una revisión formal y la creación de un nuevo ADR que documente el cambio y su justificación.

**Momentos de revisión sugeridos:**

- Al momento de definir el stack tecnológico del backend.
- Antes del primer despliegue a un entorno de pruebas con datos reales.
- Al integrar formalmente con la pasarela PSE.
