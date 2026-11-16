# Backlog - Torre del Eclipse

Responsable de todas las tareas: **Iñaki Bertran (InakiMB)**. Trabajo individual.

Actualizado al **16/11/2026**: Hitos 0, 1 y 2 completos; Producción en curso con maqueta de audio, planta baja, terraza, tres variantes enemigas, rodada, jefe de dos fases e interfaces integrados. Cada tarea registra su estado y una evidencia: commit, captura o prueba reproducible. Estados: Pendiente / En curso / Preparado / Hecho. Preparado significa que existe el material, pero falta incorporarlo o verificarlo en el proyecto.

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

La versión corta del prototipo integra combate a distancia y cuerpo a cuerpo, loot, consumo de pociones, apertura de un acceso y victoria o derrota con reinicio. La planta baja, la terraza y el jefe con victoria final se incorporaron durante Producción.

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H2-01|Golpe de bastón|Hecho|El ataque cuerpo a cuerpo tiene alcance limitado, daño y recuperación; convive con el proyectil.|
|H2-02|Loot y pociones|Hecho|Los enemigos pueden dejar una poción con probabilidad configurable, inicialmente 25%; se recoge, se cuenta y se consume para recuperar vida sin superar el máximo.|
|H2-03|Objetivo y progresión|Hecho|Derrotar a los enemigos requeridos abre un acceso e informa el nuevo objetivo.|
|H2-04|Victoria, derrota y reinicio|Hecho|Se gana y pierde en el prototipo; reiniciar restablece jugador, enemigos, objetos y progreso.|
|H2-05|Validar el ciclo completo|Hecho|Una sesión corta integra exploración, amenazas, daño, recursos, progreso y final de partida.|

## Hito 3 - Producción

La maqueta de audio está integrada en el prototipo y en TorreEclipse. Se conservan fuentes, sesiones de Reaper, proyecto de FMOD y bancos. La planta baja combina veinte enemigos entre guerreros, magos y clérigos de apoyo. Tras despejarla se puede ascender e invocar al Custodio del Eclipse, con dos fases, música propia y reintento desde la terraza. El jugador dispone de una rodada defensiva. Continúan pendientes los menús y el balance final.

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H3-A0|Prueba de conexión FMOD-Unity|Hecho|Un evento de prueba se escucha al iniciar la escena en Unity; no quedan errores rojos.|
|H3-A1|Preparar audios en Reaper|Hecho|Se conservan sesiones y fuentes; se exportan clips editados y se registran sus créditos.|
|H3-A2|Ambiente variable|Hecho|El ambiente cambia perceptiblemente al recorrer dos zonas sin cargar otra escena.|
|H3-A3|Música adaptativa|Hecho|En combate cambia la música y al finalizar vuelve a exploración; la transición responde al estado del juego.|
|H3-A4|Efectos, variación y procesamiento|Hecho|Hay efectos del mundo y de interfaz; pasos/impactos usan variaciones; el procesamiento tiene una función demostrable.|
|H3-01|Planta baja y terraza|Hecho|Ambas zonas forman un recorrido coherente, accesible y sin puntos que bloqueen al jugador.|
|H3-E1|Guerrero enemigo|Hecho|Persigue y ataca cuerpo a cuerpo; dispone de vida, daño, velocidad y cadencia configurables y se distingue visual y sonoramente.|
|H3-E2|Mago enemigo|Hecho|Se posiciona a distancia y lanza proyectiles evitables; sus valores de combate son configurables y su ataque tiene una señal audiovisual reconocible.|
|H3-E3|Enemigo de apoyo|Hecho|Se posiciona cerca de aliados y cura un 15% de la vida máxima de uno cada 6 segundos, o 5,2 en el santuario. La escena TorreEclipse utiliza +25% de movimiento y +35% de velocidad de ataque dentro de 8 metros y sin paredes intermedias, sin aumentar daño ni acumular apoyos. Vida, velocidades y tiempos son configurables; presenta señales audiovisuales y morir cancela la cura y retira el aura.|
|H3-P1|Esquive del jugador|Hecho|Espacio inicia una rodada animada en la dirección de WASD o del apuntado al estar quieto. Distancia, duración, recuperación, recarga y ventana parcial de invulnerabilidad son configurables. Respeta obstáculos, impide atacar o usar objetos durante la acción, reproduce sonido y muestra su disponibilidad; morir cancela el esquive y reiniciar restablece su estado.|
|H3-J0|Definir el diseño del jefe|Hecho|Custodio del Eclipse: caballero de gran tamaño con rostro visible, barba canosa, martillo y alas negras en segunda fase. Invocación mediante círculo, fase física y fase con proyectiles al 50%, señales audiovisuales, música orquestal con coros, reintento con recursos guardados y victoria final definidos.|
|H3-02|Combinar variantes y desarrollar jefe|Hecho|Guerrero, mago y apoyo conviven en planta baja. El jefe dispone de combo, golpe cargado, embestida, abanicos, anillos, espirales y salto; los ataques permiten esquivar o salir de las marcas. Vida y daño son configurables. El balance con jugadores se revisa en H4-02.|
|H3-03|Menús, HUD y volumen|Hecho|Play, Créditos, Opciones y Exit funcionan; se muestran vida, pociones, objetivos y barras de enemigos durante el combate; el volumen general controla todo el audio y hay controles independientes de música, ambiente, efectos e interfaz, con preferencias guardadas entre sesiones.|
|H3-04|Integrar modelos y efectos|En curso|Se distinguen ataques, enemigos y objetos desde la cámara superior; se registran las fuentes.|
|H3-I1|Cajas o barriles destructibles con poción garantizada|Hecho|Reciben daño de los proyectiles, se rompen una sola vez y dejan exactamente una poción con probabilidad 100%; tienen feedback de impacto y rotura, y se restauran al reiniciar el nivel.|

## Hito 4 - Feature complete

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H4-01|Revisar requisitos|Pendiente|Todos los sistemas obligatorios están implementados y hay al menos tres minutos de juego efectivo.|
|H4-02|Corregir y balancear|En curso|Se prueban victoria, derrota, reinicio y audio; no hay errores bloqueantes; el jefe se puede vencer con magia, bastón, esquive y las pociones disponibles.|
|H4-03|Revisar arquitectura y explicación|Pendiente|Las responsabilidades de las clases, la comunicación entre sistemas y las decisiones de código están revisadas y pueden explicarse con ejemplos del proyecto.|

## Hito 5 - Release

|ID|Tarea|Estado|Terminado cuando|
|-|-|-|-|
|H5-01|Generar y probar ejecutable|Pendiente|El juego funciona fuera del editor, con sonido, volumen y todos los finales.|
|H5-02|Preparar entrega|Pendiente|Incluye ejecutable, repositorio final, documentación, créditos y proyectos completos de Unity y FMOD con sus fuentes.|

## Alcance de la entrega

La entrega se centra en el mago jugable, la planta baja y el jefe de la terraza. El acertijo con vara secreta y los personajes jugables adicionales quedan fuera del alcance de esta entrega.

## Muestra del viernes 13/11

La escena `PrototipoTorre` incorpora cambio de ambiente por desplazamiento, transición musical por detección de enemigos y efectos de combate, recursos y resultado. Se utilizan los controles de juego, sin un selector manual de estados. La muestra intermedia permite revisar estos sistemas; la validación final de los requisitos corresponde a H4-01.

## Registro de evidencia

|Fecha|Tarea|Resultado comprobado|Commit o evidencia|
|-|-|-|-|
|11/11/2026|H0-01 / H0-02|Proyecto local revisado: Unity 6000.3.10f1, URP 17.3.0, sin cambios pendientes ni cachés versionadas.|23196e4|
|12/11/2026|H3-A0|Evento de prueba reproducido desde Unity; proyecto FMOD y bancos guardados.|cfa7c26|
|12/11/2026|H1-01|Mago animado, movimiento relativo a su orientación, colisiones y cámara superior probados.|bfce002|
|12/11/2026|H1-02|Proyectiles mágicos probados: lanzamiento, daño, colisión con el escenario y desaparición.|b582c7a; prueba de jugabilidad en Unity.|
|12/11/2026|H1-03|Dos enemigos persiguen mediante NavMesh, atacan y reciben daño; jugador y enemigos pueden morir.|b582c7a; prueba de jugabilidad en Unity.|
|12/11/2026|H1-04|Encuentro repetible con panel de vida, zona despejada, derrota y reinicio mediante R o botón.|b582c7a; prueba de jugabilidad en Unity.|
|12/11/2026|H1-01, ajuste de control|WASD independiente del apuntado con mouse, respuesta inmediata y diagonales con velocidad normalizada.|b582c7a; prueba de jugabilidad en Unity.|
|12/11/2026|H2-01|Bastón probado con animación, preparación, recuperación, arco frontal y bloqueo por obstáculos; convive con los proyectiles.|40a4b6b; prueba de jugabilidad en Unity.|
|12/11/2026|H2-02|Loot con probabilidad 25%, recogida con E, inventario y curación con Q sin superar el máximo.|40a4b6b; prueba de jugabilidad en Unity.|
|12/11/2026|H2-03|Eliminar a los dos enemigos desactiva la barrera y habilita la interacción con la salida.|40a4b6b; prueba de jugabilidad en Unity.|
|12/11/2026|H2-04|Victoria del prototipo mediante la salida, derrota por falta de vida y reinicio completo con R o botón.|40a4b6b; prueba de jugabilidad en Unity.|
|12/11/2026|H2-05|Ciclo corto de combate, daño, recursos, progresión y resultado terminado y probado.|40a4b6b; prueba de jugabilidad en Unity.|
|13/11/2026|H3-A1|Tres sesiones de Reaper con sus siete medios vinculados, 25 fuentes originales y 25 exportaciones conservadas.|Archivos en Audio; referencias de medios y formatos revisados.|
|13/11/2026|H3-A2 / H3-A3 / H3-A4|Ambiente por zona, música de combate y efectos integrados al prototipo y probados en Unity.|dc1fedd; prueba de jugabilidad en Unity.|
|13/11/2026|H3-03, avance|Cuatro grupos de mezcla y AudioVolumeSettings guardado en AudioSystems; falta comprobar volúmenes y persistencia y completar la pantalla de Opciones.|Componente incorporado; la tarea de menús y volumen sigue pendiente.|
|13/11/2026|H3-01 / H3-I1|TorreEclipse construida con dos zonas, diez perseguidores, seis suministros, ascenso y regreso; revisión de navegación sin observaciones.|b834733; escena TorreEclipse y comprobaciones de integración en Play.|
|13/11/2026|H3-04|Arquitectura de packs, materiales URP, iluminación, ocultación de paredes, HUD compacto y efecto de rotura incorporados; modelos de enemigos y efectos finales pendientes.|b834733; escena TorreEclipse. Revisión del recorrido pendiente.|
|13/11/2026|H3-E1|Guerrero con modelo, espada, escudo y audio; animaciones originales de reposo, carrera, ataque y muerte conectadas. El tajo aplica un único impacto tras la preparación y permite esquivarlo; la muerte desactiva colisiones y termina antes de retirar el cuerpo.|12 comprobaciones del guerrero y 24 del nivel correctas en Play; prefab EnemyWarrior y escena TorreEclipse guardados.|
|13/11/2026|H3-E2|Dos magos de túnica rojo oscuro integrados. Se aproximan y retroceden por NavMesh, cargan hechizos animados, disparan en una dirección fija y pueden morir durante la carga. Los proyectiles respetan coberturas y no dañan aliados; la música reconoce su estado de combate.|15 comprobaciones del mago y 24 del ciclo del nivel correctas en Play, en copia de validación; 10/10 enemigos accesibles y cero observaciones de navegación. Prefabs EnemyMage y EnemyMagicBolt, y escena TorreEclipse integrados.|
|14/11/2026|H3-E3 / H3-A3|Dos clérigos de Quaternius sustituyen a los magos de las alas laterales, conservando el encuentro actual de ocho enemigos. Curación individual, aura de aceleración, animaciones y señales audiovisuales integradas; guerreros y magos aceleran sus acciones sin aumentar el daño. El apoyo también mantiene la música de combate.|31 comprobaciones del apoyo, 2 de música y 24 del ciclo del nivel correctas en Play, en copia de validación; 8/8 enemigos accesibles y cero observaciones de navegación. Prefab EnemySupport y escena TorreEclipse integrados.|
|15/11/2026|H3-P1|Rodada del mago integrada en TorreEclipse y PrototipoTorre con la animación nativa Roll, sonido reutilizado, recarga visible e invulnerabilidad parcial. Cancela el golpe pendiente del bastón y respeta paredes y restricciones de acciones.|Prueba de jugabilidad en Unity: distancia y diagonal, controles, recarga, daño, proyectiles, animación, obstáculos, pociones y muerte verificados. Ciclo de planta baja, terraza y reinicio comprobado con el encuentro actual de veinte enemigos.|
|16/11/2026|H3-J0 / H3-02 / H3-A3 / H3-04, avance|Custodio del Eclipse integrado: modelo de Quaternius adaptado en Blender, once animaciones, martillo, alas, dos fases, patrones de proyectiles, cámara, invocación, victoria y reintento desde terraza. Veinte efectos y dos mezclas musicales incorporados a FMOD con parámetro Fase y fuentes registradas.|47 comprobaciones del jefe y 24 del recorrido correctas en Play, en copia de validación. Veinte enemigos accesibles, seis suministros y navegación sin observaciones; proyecto FMOD validado y bancos compilados. El balance final y los menús siguen pendientes.|
|16/11/2026|H4-02, avance|Dificultad del jefe ajustada: persecución más rápida, preparaciones y pausas más cortas, combo con avance y reorientación, embestida de mayor alcance y patrones más densos. La segunda fase combina golpes y orbes. Se conservan 1800 de vida y el daño por impacto.|32 comprobaciones correctas en Play, en copia de validación: presión del combo, esquive, señales, coberturas, persecución, emisión y límite de orbes, derrota, reintento y victoria. Pendiente nueva prueba de balance con jugadores.|
|16/11/2026|H3-01 / H3-04 / H4-02, avance|Torreones con transición a 30% de opacidad al tapar al jugador o al jefe; bases de piedra cierran los rincones y las rutas reservan espacio para el cuerpo del jefe. Se refuerzan únicamente tres guerreros, dos magos y dos apoyos del santuario, con alerta coordinada y ajustes propios de cadencia y velocidad. Barras de enemigos pendientes en H3-03.|34 comprobaciones correctas en Play: transparencia y restauración de materiales, colisiones, rodada, exclusión de rincones, alerta del grupo, aislamiento de los otros trece enemigos, salto del jefe y victoria. Veinte enemigos, ascenso y terraza accesibles; revisión de navegación sin observaciones.|
|16/11/2026|H3-03|Menú principal, HUD con esfera de vida, barras de enemigos, pausa, cinco sliders de volumen, derrota, victoria y créditos integrados. Los textos usan una fuente distribuible y las pantallas conservan sus botones funcionales. Dos sonidos 2D de interfaz incorporados a FMOD.|Navegación, pausa y restricciones de acciones, buses de audio, registro de fuentes, daño y curación en barras, reintento del jefe con recursos guardados, victoria y regreso al menú comprobados en Play. Escenas MenuPrincipal y TorreEclipse guardadas.|
