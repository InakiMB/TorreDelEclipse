# Backlog inicial - Torre del Eclipse

Responsable de todas las tareas: **Iñaki Bertran (InakiMB)**. Trabajo individual.

Lista provisional hasta que la cátedra indique su sistema de producción. Mantener en cada tarea el estado y una evidencia: commit, captura o prueba reproducible. Estados: Pendiente / En curso / Preparado / Hecho. Preparado significa que existe el material, pero falta incorporarlo o verificarlo en el proyecto.

## Hito 0 - Preproducción

|ID|Tarea|Estado inicial|Terminado cuando|
|-|-|-|-|
|H0-01|Crear proyecto Unity|Hecho|Abre con 6000.3.10f1 y plantilla Universal 3D.|
|H0-02|Iniciar historial Git|Hecho|Existe el commit 23196e4; Assets, Packages y ProjectSettings están versionados; las cachés quedan excluidas.|
|H0-03|Identificar autor y alcance|Hecho|Iñaki Bertran figura como único responsable; el concepto distingue base y ampliaciones.|
|H0-04|Preparar Game Concept|Hecho|El PDF de dos carillas y su versión editable están revisados e incorporados al repositorio.|
|H0-05|Incorporar documentación y backlog|Hecho|README y Docs están confirmados en un commit y visibles en GitHub; todos los trabajos tienen responsable, hito y criterio.|

## Hito 1 - Core gameplay

|ID|Tarea|Estado inicial|Terminado cuando|
|-|-|-|-|
|H1-01|Escenario de prueba, movimiento y cámara|Hecho|El jugador se mueve con WASD, colisiona con paredes y se ve desde arriba.|
|H1-02|Proyectil mágico|Pendiente|Se apunta y dispara; el proyectil acierta, causa daño y desaparece correctamente.|
|H1-03|Enemigo y sistema de salud|Pendiente|El enemigo detecta, se aproxima, ataca, recibe daño y muere; el jugador puede recibir daño.|
|H1-04|Primera versión jugable|Pendiente|Se puede abrir y repetir la secuencia moverse-combatir-recibir daño-eliminar enemigo sin errores bloqueantes.|

## Hito 2 - Vertical slice

|ID|Tarea|Estado inicial|Terminado cuando|
|-|-|-|-|
|H2-01|Golpe de bastón|Pendiente|El ataque cuerpo a cuerpo tiene alcance limitado, daño y recuperación; convive con el proyectil.|
|H2-02|Loot y pociones|Pendiente|Un enemigo deja una poción, se recoge, se cuenta y se consume para recuperar vida sin superar el máximo.|
|H2-03|Objetivo y progresión|Pendiente|Derrotar a los enemigos requeridos abre un acceso e informa el nuevo objetivo.|
|H2-04|Victoria, derrota y reinicio|Pendiente|Se gana y pierde; reiniciar restablece jugador, enemigos, objetos y progreso.|
|H2-05|Validar el ciclo completo|Pendiente|Una sesión corta integra exploración, amenazas, daño, recursos, progreso y final de partida.|

## Hito 3 - Producción

El audio se prepara en paralelo desde el comienzo para la muestra del viernes. Esto no cambia el orden de los hitos de jugabilidad.

|ID|Tarea|Estado inicial|Terminado cuando|
|-|-|-|-|
|H3-A0|Prueba de conexión FMOD-Unity|Hecho|Un evento de prueba se escucha al iniciar la escena en Unity; no quedan errores rojos.|
|H3-A1|Preparar audios en Reaper|Pendiente|Se conservan sesiones y fuentes; se exportan clips editados y se registran sus créditos.|
|H3-A2|Ambiente variable|Pendiente|El ambiente cambia perceptiblemente al recorrer dos zonas sin cargar otra escena.|
|H3-A3|Música adaptativa|Pendiente|En combate cambia la música y al finalizar vuelve a exploración; la transición responde al estado del juego.|
|H3-A4|Efectos, variación y procesamiento|Pendiente|Hay efectos del mundo y de interfaz; pasos/impactos usan variaciones; el procesamiento tiene una función demostrable.|
|H3-01|Planta baja y terraza|Pendiente|Ambas zonas forman un recorrido coherente, accesible y sin puntos que bloqueen al jugador.|
|H3-02|Variante de enemigo y jefe|Pendiente|Las amenazas son distinguibles y el jefe tiene ataques legibles y evitables.|
|H3-03|Menús, HUD y volumen|Pendiente|Play, Créditos, Opciones y Exit funcionan; se muestran vida, pociones y objetivos; el volumen general controla todo el audio.|
|H3-04|Integrar modelos y efectos|Pendiente|Se distinguen ataques, enemigos y objetos desde la cámara superior; se registran las fuentes.|

## Hito 4 - Feature complete

|ID|Tarea|Estado inicial|Terminado cuando|
|-|-|-|-|
|H4-01|Revisar requisitos|Pendiente|Todos los sistemas obligatorios están implementados y hay al menos tres minutos de juego efectivo.|
|H4-02|Corregir y balancear|Pendiente|Se prueban victoria, derrota, reinicio y audio; no hay errores bloqueantes; el jefe se puede vencer sin la vara.|
|H4-03|Revisar arquitectura y explicación|Pendiente|El autor puede explicar responsabilidades, comunicación entre sistemas y decisiones de código.|

## Hito 5 - Release

|ID|Tarea|Estado inicial|Terminado cuando|
|-|-|-|-|
|H5-01|Generar y probar ejecutable|Pendiente|El juego funciona fuera del editor, con sonido, volumen y todos los finales.|
|H5-02|Preparar entrega|Pendiente|Incluye ejecutable, repositorio final, documentación, créditos y proyectos completos de Unity y FMOD con sus fuentes.|

## Ampliaciones - solo después de completar la base

|ID|Tarea|Estado inicial|Terminado cuando|
|-|-|-|-|
|X-01|Acertijo y vara secreta|Pendiente|Las pistas permiten resolver el acertijo; la vara se obtiene y solo permite un rayo por partida, con feedback audiovisual.|
|X-02|Guerrero y arquero|Pendiente|Se seleccionan y completan el nivel sin romper el ciclo de juego ni la entrega base.|

## Muestra del viernes 13/11

Objetivo propuesto: una escena simple con cambio de ambiente al desplazarse, una transición musical y efectos. Si la jugabilidad aún no está lista, identificar claramente los controles de demostración que cambian estados de prueba. La muestra intermedia no sustituye la integración final exigida.

## Registro de evidencia

|Fecha|Tarea|Resultado comprobado|Commit o evidencia|
|-|-|-|-|
|11/11/2026|H0-01 / H0-02|Proyecto local revisado: Unity 6000.3.10f1, URP 17.3.0, sin cambios pendientes ni cachés versionadas.|23196e4|
|12/11/2026|H3-A0|Evento de prueba reproducido desde Unity; proyecto FMOD y bancos guardados.|cfa7c26|
|12/11/2026|H1-01|Mago animado, movimiento relativo a su orientación, colisiones y cámara superior probados.|bfce002|
