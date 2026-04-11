# ADR-003: Arquitectura y Tecnologia del Backend

Seleccion del stack tecnico y patron arquitectonico interno del backend IUSHPay

---

| Campo | Detalle |
|---|---|
| Identificador | ADR-003 |
| Titulo | Arquitectura y Tecnologia del Backend |
| Estado | Aprobado |
| Fecha | Abril 2025 |
| Autores | Equipo de Desarrollo Backend — IUSHPay |
| Depende de | ADR-001 — Arquitectura del Sistema IUSHPay |

---

## 1. Contexto

Con la arquitectura general del sistema definida en ADR-001 como Monolito Modular, el equipo de backend necesita precisar las tecnologias concretas con las que se implementara ese monolito y el patron interno que organizara el codigo dentro de cada modulo. Esta decision afecta directamente la mantenibilidad del codigo, la seguridad en el acceso a datos, la facilidad para incorporar nuevos desarrolladores y la capacidad de evolucion del sistema a largo plazo.

Los sistemas de parqueadero gestionados de forma manual o con herramientas no integradas presentan multiples ineficiencias operativas. Este documento establece las decisiones tecnicas fundamentales para construir un backend robusto, escalable y mantenible que centralice y automatice dichos procesos bajo un stack tecnologico probado y con soporte a largo plazo.

---

## 2. Problemas que Motivan esta Decision

| # | Problema | Impacto en el negocio |
|---|---|---|
| 1 | Perdida de informacion de entradas y salidas | Imposibilidad de auditar operaciones pasadas; riesgo de fraude y litigios. |
| 2 | Falta de control sobre disponibilidad de espacios | Sobreocupacion o subutilizacion del parqueadero; mala experiencia del usuario. |
| 3 | Baja trazabilidad de transacciones | Ausencia de reportes confiables para la toma de decisiones gerenciales. |
| 4 | Calculo manual de tarifas | Errores en el cobro, inconsistencias entre operarios y perdida de ingresos. |
| 5 | Ausencia de integracion con medios de pago digitales | Dependencia del efectivo, sin registro automatico de las transacciones realizadas. |

---

## 3. Decision Tecnologica

Se adopta ASP.NET Core Web API como framework principal del backend, fundamentado en los principios de Clean Architecture como patron de organizacion interna del codigo, y Entity Framework Core como mecanismo de acceso a datos.

| Aspecto | Detalle |
|---|---|
| Tecnologia principal | ASP.NET Core Web API |
| Lenguaje | C# |
| Protocolo | HTTP / HTTPS — REST |
| Formato de respuesta | JSON |
| ORM de acceso a datos | Entity Framework Core |
| Patron arquitectonico interno | Clean Architecture |
| Autenticacion | JWT (JSON Web Tokens) — alineado con ADR-001 |
| Plataforma de despliegue | Multiplataforma (Windows, Linux, Docker) |
| Ciclo de soporte | Microsoft LTS (soporte a largo plazo) |

---

## 4. Clean Architecture como Patron Interno

A medida que un sistema crece, mezclar la logica de negocio con el acceso a datos o con los controladores genera un codigo dificil de mantener, probar y escalar. Clean Architecture resuelve esto estableciendo capas con responsabilidades claras y una regla fundamental: las capas internas no conocen a las externas. Esto significa que la logica de negocio y las entidades del dominio no dependen de ningun framework, base de datos ni detalle de infraestructura; son la parte mas estable del sistema.

Las cuatro capas que estructuran cada modulo del backend son las siguientes:

| Capa | Responsabilidad | Ejemplos de contenido |
|---|---|---|
| Domain | Nucleo del sistema. Contiene reglas y entidades puras sin dependencia de ningun framework o herramienta externa. | Entidades, interfaces, enumeraciones, excepciones de dominio |
| Application | Orquesta los casos de uso del sistema. Coordina el dominio con el mundo exterior sin conocer detalles de infraestructura. | Casos de uso, DTOs, servicios de aplicacion, validaciones |
| Infrastructure | Implementa los contratos definidos en las capas internas. Es la unica capa que conoce la base de datos y los servicios externos. | Repositorios EF Core, contexto de BD, clientes HTTP, servicios externos |
| Presentation | Punto de entrada del sistema. Recibe las peticiones HTTP, las delega a la capa de aplicacion y devuelve las respuestas. | Controllers, middlewares, filtros, configuracion de la API |

La dependencia siempre apunta hacia adentro: Presentation depende de Application, Application depende de Domain. La capa Domain no depende de ninguna otra. Infrastructure implementa las interfaces definidas en Domain y Application, pero no es conocida por ellas.

---

## 5. Justificacion de la Decision

La eleccion de ASP.NET Core Web API se fundamenta en su madurez tecnologica, alto rendimiento y el respaldo oficial de Microsoft a largo plazo, lo que asegura estabilidad y una amplia comunidad de desarrollo. Su integracion nativa con mecanismos de autenticacion como JWT refuerza la proteccion del backend, alineandose directamente con la estrategia de seguridad definida en ADR-001.

La incorporacion de Entity Framework Core como ORM agrega una capa de seguridad y simplicidad en el acceso a los datos. Al abstraer las consultas mediante objetos tipados y proteger contra vulnerabilidades como la inyeccion SQL, se garantiza que la interaccion con la base de datos sea confiable y mantenible, eliminando la necesidad de queries SQL crudas en el codigo.

Clean Architecture como patron interno complementa esta decision al garantizar que la logica de negocio sea independiente del framework y de la base de datos. Esto permite que el sistema pueda evolucionar, cambiar de motor de base de datos o adoptar nuevas herramientas sin necesidad de reescribir el nucleo funcional.

---

## 6. Alternativas Consideradas y Descartadas

| Criterio | Monolito Modular con Clean Arch (elegido) | Monolito por capas globales | Microservicios |
|---|---|---|---|
| Complejidad inicial | Media | Baja | Alta |
| Testabilidad | Alta | Media | Alta |
| Mantenibilidad | Alta | Media | Alta |
| Curva de aprendizaje | Media | Baja | Alta |
| Adecuacion al proyecto | Alta | Insuficiente | Excesiva |
| Escalabilidad futura | Preparado | Limitada | Nativa |

### 6.1 Por que se descarto el Monolito por Capas Globales

Una estructura con carpetas globales por tipo de clase (Controllers, Services, Repositories) mezcla logica de distintos modulos en los mismos directorios. A medida que el sistema crece, esta organizacion genera acoplamiento implicito y dificulta la identificacion de responsabilidades. Ademas, no alinea con los principios de Clean Architecture ni con la estructura de repositorio definida en ADR-002.

### 6.2 Por que se descarto Queries SQL Crudas en lugar de EF Core

Aunque las queries SQL directas ofrecen mayor control en consultas muy complejas, introducen un riesgo elevado de inyeccion SQL si no se manejan correctamente y aumentan la probabilidad de error humano. Entity Framework Core ofrece la misma capacidad de consulta mediante LINQ y queries parametrizadas, con mayor seguridad y sin sacrificar flexibilidad para los casos que lo requieran.

### 6.3 Por que se descarto Microservicios

Los microservicios implican orquestacion de multiples servicios, comunicacion por red entre ellos, gestion distribuida de transacciones y mayor infraestructura de monitoreo. Para un equipo de desarrollo academico con plazos definidos, esta complejidad no esta justificada en la fase actual del proyecto.

---

## 7. Consecuencias

### 7.1 Consecuencias positivas

- Modularidad dentro de un solo despliegue, permitiendo organizar el sistema por dominios sin necesidad de multiples servicios.
- Separacion clara de responsabilidades gracias a Clean Architecture, lo que facilita el mantenimiento y la evolucion del sistema.
- Bajo acoplamiento entre modulos, permitiendo modificar funcionalidades sin afectar otras partes del sistema.
- Facilidad para pruebas unitarias al aislar la logica de negocio de la infraestructura.
- Menor complejidad operativa en comparacion con microservicios: un solo despliegue, menor configuracion.
- Escalabilidad progresiva, con posibilidad de migrar modulos especificos a microservicios si el sistema lo requiere en el futuro.

### 7.2 Limitaciones conocidas

- Cualquier cambio requiere redesplegar toda la aplicacion, incluso si el cambio es menor.
- No es posible escalar modulos de forma independiente sin refactorizacion previa.
- Todos los modulos comparten el mismo stack tecnologico, lo que limita la flexibilidad tecnologica por modulo.
- Riesgo de acoplamiento indebido si no se respetan las reglas de la arquitectura, como dependencias incorrectas entre capas.

---

## 8. Buenas Practicas Aplicadas

| Practica | Aplicacion en IUSHPay |
|---|---|
| Dependencia hacia el dominio | Ninguna capa interna (Domain, Application) importa clases de Infrastructure o Presentation. Las dependencias fluyen siempre hacia adentro. |
| ORM como unica puerta a la base de datos | Todo acceso a datos pasa por Entity Framework Core. No se permiten queries SQL crudas en ninguna capa del sistema. |
| Contratos mediante interfaces | Los repositorios y servicios externos se definen como interfaces en la capa Application y se implementan en Infrastructure. |
| DTOs en la capa de presentacion | Las entidades del dominio nunca se exponen directamente en la API. Se utilizan DTOs para mapear la respuesta, evitando fugas de informacion. |
| Validacion en la capa de entrada | Toda entrada de datos es validada antes de llegar a la logica de negocio, previniendo inconsistencias desde el origen. |
| Alineacion con ADR-001 (seguridad) | El uso de EF Core elimina el riesgo de inyeccion SQL. JWT como mecanismo de autenticacion cumple con la Capa 1 de seguridad definida en ADR-001. |

---

## 9. Estado y Vigencia

| Estado | Revisado por | Vigencia |
|---|---|---|
| Aprobado | Equipo de Desarrollo Backend | Hasta cambio de framework o motor de base de datos |

Este ADR debe revisarse si el equipo decide cambiar el motor de base de datos, adoptar un framework diferente a ASP.NET Core, o si algun modulo requiere una tecnologia especializada que no sea compatible con el stack actual.
