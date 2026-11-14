# Backlog - Torre del Eclipse

Responsable de todas las tareas: **Iñaki Bertran (InakiMB)**. Trabajo individual.

Actualizado al **13/11/2026**: Hitos 0, 1 y 2 completos; Producción en curso con maqueta de audio integrada. Lista provisional hasta que la cátedra indique su sistema de producción. Mantener en cada tarea el estado y una evidencia: commit, captura o prueba reproducible. Estados: Pendiente / En curso / Preparado / Hecho. Preparado significa que existe el material, pero falta incorporarlo o verificarlo en el proyecto.

## Hito 0 - Preproducción

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H0-01|Crear proyecto Unity|Hecho|Abre con 6000.3.10f1 y plantilla Universal 3D.|
|H0-02|Iniciar historial Git|Hecho|Existe el commit 23196e4; Assets, Packages y ProjectSettings están versionados; las cachés quedan excluidas.|
|H0-03|Identificar autor y alcance|Hecho|Iñaki Bertran figura como único responsable; el concepto distingue base y ampliaciones.|
|H0-04|Preparar Game Concept|Hecho|El PDF de dos carillas y su versión editable están revisados e incorporados al repositorio.|
|H0-05|Incorporar documentación y backlog|Hecho|README y Docs están confirmados en un commit y visibles en GitHub; todos los trabajos tienen responsable, hito y criterio.|

## Hito 1 - Core gameplay

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H1-01|Escenario de prueba, movimiento y cámara|Hecho|El jugador se mueve con WASD en direcciones fijas de pantalla, apunta con mouse de forma independiente, colisiona con paredes y se ve desde arriba.|
|H1-02|Proyectil mágico|Hecho|Se apunta y dispara; el proyectil acierta, causa daño y desaparece correctamente.|
|H1-03|Enemigo y sistema de salud|Hecho|El enemigo detecta, se aproxima, ataca, recibe daño y muere; el jugador puede recibir daño.|
|H1-04|Primera versión jugable|Hecho|Se puede abrir y repetir la secuencia moverse-combatir-recibir daño-eliminar enemigo sin errores bloqueantes.|

## Hito 2 - Vertical slice

La versión corta del prototipo integra combate a distancia y cuerpo a cuerpo, loot, consumo de pociones, apertura de un acceso y victoria o derrota con reinicio. El escenario final, la terraza y el jefe siguen pendientes en Producción.

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H2-01|Golpe de bastón|Hecho|El ataque cuerpo a cuerpo tiene alcance limitado, daño y recuperación; convive con el proyectil.|
|H2-02|Loot y pociones|Hecho|Los enemigos pueden dejar una poción con probabilidad configurable, inicialmente 25%; se recoge, se cuenta y se consume para recuperar vida sin superar el máximo.|
|H2-03|Objetivo y progresión|Hecho|Derrotar a los enemigos requeridos abre un acceso e informa el nuevo objetivo.|
|H2-04|Victoria, derrota y reinicio|Hecho|Se gana y pierde en el prototipo; reiniciar restablece jugador, enemigos, objetos y progreso.|
|H2-05|Validar el ciclo completo|Hecho|Una sesión corta integra exploración, amenazas, daño, recursos, progreso y final de partida.|

## Hito 3 - Producción

La maqueta de audio del Bloque 6 está integrada en el prototipo para la muestra del 13/11. El autor confirmó las pruebas del bloque. Se conservan fuentes, sesiones de Reaper, proyecto de FMOD y bancos. Los sistemas restantes continúan en Producción.

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H3-A0|Prueba de conexión FMOD-Unity|Hecho|Un evento de prueba se escucha al iniciar la escena en Unity; no quedan errores rojos.|
|H3-A1|Preparar audios en Reaper|Hecho|Se conservan sesiones y fuentes; se exportan clips editados y se registran sus créditos.|
|H3-A2|Ambiente variable|Hecho|El ambiente cambia perceptiblemente al recorrer dos zonas sin cargar otra escena.|
|H3-A3|Música adaptativa|Hecho|En combate cambia la música y al finalizar vuelve a exploración; la transición responde al estado del juego.|
|H3-A4|Efectos, variación y procesamiento|Hecho|Hay efectos del mundo y de interfaz; pasos/impactos usan variaciones; el procesamiento tiene una función demostrable.|
|H3-01|Planta baja y terraza|Hecho|Ambas zonas forman un recorrido coherente, accesible y sin puntos que bloqueen al jugador.|
|H3-E1|Guerrero enemigo|Hecho|Persigue y ataca cuerpo a cuerpo; dispone de vida, daño, velocidad y cadencia configurables y se distingue visual y sonoramente.|
|H3-E2|Mago enemigo|Pendiente|Se posiciona a distancia y lanza proyectiles evitables; sus valores de combate son configurables y su ataque tiene una señal audiovisual reconocible.|
|H3-E3|Enemigo de apoyo|Pendiente|Se posiciona cerca de aliados y los cura o potencia con alcance y tiempo de espera definidos; vida y velocidad son configurables, el efecto es reconocible y termina o deja de renovarse al morir el apoyo.|
|H3-J0|Definir el jefe con el autor|Pendiente|Antes de diseñarlo e implementarlo, consultar a Iñaki y acordar apariencia, identidad, arena, movimientos, ataques, fases, defensas, debilidades, vida, daño, dificultad, señales visuales, sonidos, música, recompensas y cierre del nivel.|
|H3-02|Combinar variantes y desarrollar jefe|Pendiente|Guerrero, mago y apoyo cumplen roles diferentes en un encuentro; sus valores se ajustan tras probarlos y el jefe tiene ataques legibles y evitables.|
|H3-03|Menús, HUD y volumen|Pendiente|Play, Créditos, Opciones y Exit funcionan; se muestran vida, pociones y objetivos; el volumen general controla todo el audio y hay controles independientes de música, ambiente, efectos e interfaz, con preferencias guardadas entre sesiones.|
|H3-04|Integrar modelos y efectos|En curso|Se distinguen ataques, enemigos y objetos desde la cámara superior; se registran las fuentes.|
|H3-I1|Cajas o barriles destructibles con poción garantizada|Hecho|Reciben daño de los proyectiles, se rompen una sola vez y dejan exactamente una poción con probabilidad 100%; tienen feedback de impacto y rotura, y se restauran al reiniciar el nivel.|

## Hito 4 - Feature complete

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H4-01|Revisar requisitos|Pendiente|Todos los sistemas obligatorios están implementados y hay al menos tres minutos de juego efectivo.|
|H4-02|Corregir y balancear|Pendiente|Se prueban victoria, derrota, reinicio y audio; no hay errores bloqueantes; el jefe se puede vencer sin la vara.|
|H4-03|Revisar arquitectura y explicación|Pendiente|El autor puede explicar responsabilidades, comunicación entre sistemas y decisiones de código.|

## Hito 5 - Release

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H5-01|Generar y probar ejecutable|Pendiente|El juego funciona fuera del editor, con sonido, volumen y todos los finales.|
|H5-02|Preparar entrega|Pendiente|Incluye ejecutable, repositorio final, documentación, créditos y proyectos completos de Unity y FMOD con sus fuentes.|

## Ampliaciones - solo después de completar la base

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|X-01|Acertijo y vara secreta|Pendiente|Las pistas permiten resolver el acertijo; la vara se obtiene y solo permite un rayo por partida, con feedback audiovisual.|
|X-02|Guerrero y arquero|Pendiente|Se seleccionan y completan el nivel sin romper el ciclo de juego ni la entrega base.|

## Muestra del viernes 13/11

La escena `PrototipoTorre` incorpora cambio de ambiente por desplazamiento, transición musical por detección de enemigos y efectos de combate, recursos y resultado. Se utilizan los controles de juego, sin un selector manual de estados. La muestra está preparada; su presentación y devolución docente aún no están registradas. La muestra intermedia no sustituye la revisión final de los requisitos.

## Registro de evidencia

|Fecha|Tarea|Resultado comprobado|Commit o evidencia|
|-|-|-|-|
|11/11/2026|H0-01 / H0-02|Proyecto local revisado: Unity 6000.3.10f1, URP 17.3.0, sin cambios pendientes ni cachés versionadas.|23196e4|
|12/11/2026|H3-A0|Evento de prueba reproducido desde Unity; proyecto FMOD y bancos guardados.|cfa7c26|
|12/11/2026|H1-01|Mago animado, movimiento relativo a su orientación, colisiones y cámara superior probados.|bfce002|
|12/11/2026|H1-02|Proyectiles mágicos probados: lanzamiento, daño, colisión con el escenario y desaparición.|b582c7a; prueba confirmada por el autor.|
|12/11/2026|H1-03|Dos enemigos persiguen mediante NavMesh, atacan y reciben daño; jugador y enemigos pueden morir.|b582c7a; prueba confirmada por el autor.|
|12/11/2026|H1-04|Encuentro repetible con panel de vida, zona despejada, derrota y reinicio mediante R o botón.|b582c7a; prueba confirmada por el autor.|
|12/11/2026|H1-01, ajuste de control|WASD independiente del apuntado con mouse, respuesta inmediata y diagonales con velocidad normalizada.|b582c7a; prueba confirmada por el autor.|
|12/11/2026|H2-01|Bastón probado con animación, preparación, recuperación, arco frontal y bloqueo por obstáculos; convive con los proyectiles.|40a4b6b; prueba confirmada por el autor.|
|12/11/2026|H2-02|Loot con probabilidad 25%, recogida con E, inventario y curación con Q sin superar el máximo.|40a4b6b; prueba confirmada por el autor.|
|12/11/2026|H2-03|Eliminar a los dos enemigos desactiva la barrera y habilita la interacción con la salida.|40a4b6b; prueba confirmada por el autor.|
|12/11/2026|H2-04|Victoria del prototipo mediante la salida, derrota por falta de vida y reinicio completo con R o botón.|40a4b6b; prueba confirmada por el autor.|
|12/11/2026|H2-05|Ciclo corto de combate, daño, recursos, progresión y resultado terminado y probado.|40a4b6b; prueba confirmada por el autor.|
|13/11/2026|H3-A1|Tres sesiones de Reaper con sus siete medios vinculados, 25 fuentes originales y 25 exportaciones conservadas.|Archivos en Audio; referencias de medios y formatos revisados.|
|13/11/2026|H3-A2 / H3-A3 / H3-A4|Ambiente por zona, música de combate y efectos integrados al prototipo; pruebas del bloque confirmadas por el autor.|dc1fedd; prueba confirmada por el autor.|
|13/11/2026|H3-03, avance|Cuatro grupos de mezcla y AudioVolumeSettings guardado en AudioSystems; falta comprobar volúmenes y persistencia y completar la pantalla de Opciones.|Componente incorporado; la tarea de menús y volumen sigue pendiente.|
|13/11/2026|H3-01 / H3-I1|TorreEclipse construida con dos zonas, diez perseguidores, seis suministros, ascenso y regreso; revisión de navegación sin observaciones.|b834733; escena TorreEclipse y comprobaciones de integración en Play.|
|13/11/2026|H3-04|Arquitectura de packs, materiales URP, iluminación, ocultación de paredes, HUD compacto y efecto de rotura incorporados; modelos de enemigos y efectos finales pendientes.|b834733; escena TorreEclipse. Revisión del recorrido pendiente.|
|13/11/2026|H3-E1|Guerrero con modelo, espada, escudo y audio; animaciones originales de reposo, carrera, ataque y muerte conectadas. El tajo aplica un único impacto tras la preparación y permite esquivarlo; la muerte desactiva colisiones y termina antes de retirar el cuerpo.|12 comprobaciones del guerrero y 24 del nivel correctas en Play; prefab EnemyWarrior y escena TorreEclipse guardados.|
