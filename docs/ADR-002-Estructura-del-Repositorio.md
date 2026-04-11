# ADR-002: Estructura Interna del Repositorio

Organizacion del codigo fuente del repositorio IUSHPay

---

| Campo | Detalle |
|---|---|
| Identificador | ADR-002 |
| Titulo | Estructura Interna del Repositorio |
| Estado | Aprobado |
| Fecha | Abril 2025 |
| Autores | Equipo de Arquitectura |
| Depende de | ADR-001 — Arquitectura del Sistema IUSHPay |

---

## 1. Contexto

Una vez definida la arquitectura de Monolito Modular para IUSHPay (ADR-001), es necesario establecer como se organiza fisicamente el codigo fuente dentro del repositorio. Sin una estructura acordada, los equipos tienden a ubicar archivos de forma inconsistente, generar conflictos de merge frecuentes y dificultar la navegacion del codigo por parte de nuevos integrantes.

Definir estas decisiones de forma explicita reduce la friccion operativa y establece una base comun para el trabajo colaborativo. La estructura debe reflejar directamente los modulos definidos en el ADR-001, de modo que cualquier desarrollador pueda inferir la ubicacion de un archivo a partir de su responsabilidad funcional.

---

## 2. Decision: Organizacion de Carpetas del Repositorio

Se adopta una estructura de carpetas que refleja directamente los modulos definidos en la arquitectura. Cada modulo contiene sus propias capas internas (dominio, aplicacion, infraestructura), y existe una capa compartida para elementos transversales como autenticacion, configuracion y utilidades comunes.

| Ruta en el repositorio | Contenido |
|---|---|
| `src/IUSHPay.API` | Proyecto principal: controladores, middlewares, configuracion de la API y punto de entrada de la aplicacion. |
| `src/Modules/Auth` | Modulo de autenticacion y gestion de usuarios: registro, login, roles y permisos. |
| `src/Modules/Wallet` | Modulo de wallet institucional: saldo, recargas y movimientos por usuario. |
| `src/Modules/Payments` | Modulo de pagos: integracion con PSE, procesamiento y confirmacion de transacciones. |
| `src/Modules/Access` | Modulo de control de acceso: generacion y validacion de codigos QR para porteria. |
| `src/Modules/Tariffs` | Modulo de tarifas: definicion, calculo automatico y vigencia de tarifas. |
| `src/Modules/Audit` | Modulo de auditoria: registro inmutable de eventos y transacciones. |
| `src/Modules/Notifications` | Modulo de notificaciones: envio de alertas y confirmaciones al usuario. |
| `src/Shared` | Elementos transversales: contratos compartidos, utilidades, excepciones base y constantes. |
| `tests/Unit` | Pruebas unitarias organizadas por modulo, sin dependencia de infraestructura externa. |
| `tests/Integration` | Pruebas de integracion que validan la interaccion entre modulos y la base de datos. |
| `docs/adr` | Registro de decisiones de arquitectura (ADR-001, ADR-002 y ADR-003). |
| `.github/workflows` | Pipelines de integracion continua: build, pruebas automatizadas y analisis de calidad. |

---

## 3. Alternativas Consideradas y Descartadas

| Alternativa | Descripcion | Razon del descarte |
|---|---|---|
| Estructura por capa global | Una carpeta Controllers, una carpeta Services y una carpeta Repositories para toda la aplicacion. | A medida que el sistema crece, los archivos de distintos modulos se mezclan en las mismas carpetas, dificultando la navegacion y el mantenimiento. |
| Estructura plana sin modulos | Todos los archivos en una sola carpeta raiz organizada por tipo de clase. | Genera confusion en equipos con multiples integrantes y no refleja la arquitectura modular definida en ADR-001. |

---

## 4. Buenas Practicas Aplicadas

| Practica | Aplicacion en IUSHPay |
|---|---|
| Coherencia entre arquitectura y estructura | La organizacion de carpetas refleja directamente los modulos del ADR-001, facilitando la navegacion y la comprension del sistema. |
| Un modulo, una carpeta | Cada modulo del sistema tiene su propio espacio en el repositorio, evitando que logicas distintas compartan el mismo directorio. |
| Documentacion versionada | Los ADRs, diagramas y decisiones tecnicas se versionan junto con el codigo en la carpeta docs/adr. |
| Pruebas separadas por tipo | Las pruebas unitarias y de integracion se ubican en directorios distintos, con estructura interna espejo de los modulos. |
| Pipeline en el repositorio | Los archivos de CI/CD se almacenan en .github/workflows, garantizando que la configuracion del pipeline sea parte del historial de versiones. |

---

## 5. Consecuencias

### 5.1 Consecuencias positivas

- La estructura de carpetas facilita el trabajo paralelo de distintos equipos sobre modulos diferentes sin conflictos frecuentes.
- Cualquier desarrollador puede inferir la ubicacion de un archivo a partir de su responsabilidad, sin necesidad de conocer el proyecto de antemano.
- Los ADRs versionados en el repositorio garantizan que las decisiones tecnicas sean accesibles y rastreables para todo el equipo.

### 5.2 Limitaciones conocidas

- La disciplina en el respeto de la estructura depende de acuerdos de equipo que deben reforzarse en las revisiones de codigo.
- Si se incorporan modulos nuevos en el futuro, la estructura debe actualizarse de forma explicita y documentarse en este ADR.

---

## 6. Estado y Vigencia

| Estado | Revisado por | Vigencia |
|---|---|---|
| Aprobado | Equipo de Arquitectura | Hasta incorporacion de nuevos modulos mayores |
