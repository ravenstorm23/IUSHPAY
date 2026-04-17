# IUSHPAY
Estrategia de Branching
Para el desarrollo del proyecto se adoptó la estrategia GitHub Flow, ya que proporciona un flujo de trabajo simple, ágil y alineado con las necesidades actuales del proyecto IUSHPay, permitiendo mantener una integración continua y una colaboración eficiente entre los miembros del equipo.

La elección de esta estrategia se fundamenta en los siguientes criterios:

Frecuencia de despliegue esperada
El proyecto busca realizar integraciones frecuentes durante el desarrollo, permitiendo probar nuevas funcionalidades de forma continua. GitHub Flow facilita este enfoque porque cada nueva funcionalidad se desarrolla en una rama independiente que posteriormente se integra a main mediante pull requests, permitiendo despliegues rápidos y controlados.

Tamaño del equipo
El equipo está compuesto por tres desarrolladores, lo que hace innecesario implementar estrategias de branching más complejas. GitHub Flow ofrece un flujo de trabajo sencillo que facilita la coordinación, reduce conflictos entre ramas y permite que cada integrante trabaje en funcionalidades específicas sin afectar la estabilidad del código principal.

Madurez del pipeline CI/CD
El pipeline de integración y despliegue continuo del proyecto se encuentra en una etapa inicial de implementación. Debido a esto, se prioriza una estrategia que sea fácil de integrar con procesos básicos de validación automática, pruebas y revisión de código. GitHub Flow se adapta bien a este contexto porque está diseñado para trabajar con integración continua y revisiones mediante pull requests.

Necesidad de soporte de múltiples versiones en paralelo
El sistema no requiere mantener múltiples versiones activas del software al mismo tiempo, ya que el desarrollo está orientado a una única versión del sistema que evoluciona progresivamente. Por esta razón, no es necesario utilizar estrategias como Git Flow, que están diseñadas para manejar múltiples versiones o ciclos de mantenimiento simultáneos.

Complejidad del proceso de release
El proceso de liberación del proyecto es relativamente simple. Las nuevas funcionalidades o correcciones se integran a la rama principal después de ser revisadas y aprobadas. Esto permite mantener la rama main en un estado estable y listo para despliegue, reduciendo la complejidad del proceso de release y facilitando la entrega continua de mejoras al sistema.

En conclusión, GitHub Flow se ajusta adecuadamente a las características del proyecto IUSHPay, ya que permite mantener un flujo de desarrollo claro, colaborativo y eficiente, optimizando el trabajo del equipo y facilitando la integración continua del sistema.
