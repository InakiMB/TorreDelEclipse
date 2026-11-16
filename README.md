# Torre del Eclipse

Proyecto individual de **Iñaki Bertran** para **Programación de Videojuegos II** y **Audio y Sonido para Videojuegos**. Entrega final: **20/11/2026**.

Juego de acción 3D con cámara superior y estética de fantasía oscura low poly. Un mago debe despejar la planta baja de una torre y ascender a la terraza para enfrentar al jefe.

## Estado del proyecto

Actualizado al **16/11/2026**. Hitos **0, 1 y 2 completos**; **Producción en curso**, con los tres enemigos de planta baja, esquive, jefe de dos fases e interfaces implementados.

- Movimiento con WASD y apuntado independiente con mouse; ataques con magia y bastón.
- Rodada con Espacio en la dirección de movimiento o del apuntado al estar quieto, con animación, sonido, invulnerabilidad parcial y recarga visible en el HUD.
- Salud, daño, loot de enemigos con 25% de probabilidad, recogida y consumo de pociones.
- Planta baja y terraza construidas, con veinte enemigos y seis cajas o barriles que dejan una poción garantizada al romperse.
- Guerrero enemigo con espada, escudo y animaciones de reposo, carrera, ataque y muerte; el tajo sincroniza daño y sonido y permite esquivarlo durante la preparación.
- Mago enemigo de túnica rojo oscuro: combate a distancia, retrocede al acercarse el jugador y anuncia sus proyectiles con carga y animación. Sus disparos pueden esquivarse o detenerse con coberturas y no dañan a sus aliados; morir cancela la carga pendiente.
- Clérigo de apoyo con ropa verde oscuro y armadura color hueso: cura a un aliado herido un 15% de su vida máxima cada 6 segundos —5,2 en los apoyos del santuario—. En TorreEclipse otorga +25% de movimiento y +35% de velocidad de ataque a aliados visibles dentro de 8 metros. Las bonificaciones no se acumulan entre apoyos y se retiran al salir del alcance o morir el clérigo; el daño de cada golpe no aumenta.
- Ascenso y regreso con E al despejar la planta baja; entrar en el círculo de la terraza invoca al Custodio del Eclipse y sella el acceso.
- Jefe de 1800 de vida con martillo: combo con avance y reorientación del segundo golpe, golpe cargado y embestida. Al 50% despliega alas negras, acelera y combina los golpes físicos con abanicos, anillos, espirales y saltos. La introducción y la transformación tienen protección temporal para ambos participantes.
- Barra del jefe, señales de ataque, cámara de combate y victoria final. Al morir se puede reintentar desde la terraza con la vida y las pociones guardadas al activar el círculo, conservando despejada la planta baja.
- Los cuatro torreones de la terraza pasan gradualmente al 30% de opacidad cuando tapan al jugador o al jefe. Sus bases visibles cierran los rincones contra el pretil y la navegación reserva espacio para el cuerpo del jefe.
- Los siete guardias del santuario responden juntos al entrar en la sala o atacar a uno: tres guerreros, dos magos y dos apoyos tienen ajustes propios de velocidad y cadencia. Los otros trece enemigos conservan sus valores.
- Ambiente variable, música de exploración/combate y efectos integrados mediante FMOD.
- Menú principal, pausa, opciones de audio, derrota, victoria con la torre purificada y créditos. Las cinco categorías de volumen guardan sus preferencias.
- HUD con esfera de vida, magia, bastón, recarga de esquive y pociones; barras discretas sobre enemigos durante el combate, incluidas las curaciones. La cámara de combate reserva espacio para la interfaz y la hotbar se atenúa si un personaje pasa por detrás.

Siguen pendientes el balance final con jugadores y la revisión final de requisitos y ejecutable. Llegar a la terraza permite prepararse; la victoria requiere derrotar al jefe.

## Abrir y probar

1. Añadir la carpeta del repositorio a Unity Hub y abrirla con **Unity 6000.3.10f1**.
2. Abrir `Assets/Scenes/MenuPrincipal.unity`, entrar en Play y pulsar **Jugar**. `TorreEclipse.unity` permite entrar directamente al nivel para trabajar en él.
3. Eliminar a los veinte enemigos de la planta baja para habilitar el ascenso del santuario; acercarse a la escalera y pulsar E. En la terraza hay un acceso de regreso.
4. Prepararse antes de entrar en el círculo: ese momento guarda vida y pociones para los reintentos. Derrotar al Custodio completa el nivel.

`PrototipoTorre.unity` conserva el encuentro corto anterior, con victoria mediante su salida. `PruebaAudio.unity` conserva la prueba de conexión de FMOD.

## Controles

| Entrada | Acción |
|---|---|
| WASD | Moverse respecto de la pantalla. |
| Mouse | Apuntar. |
| Clic izquierdo | Lanzar magia. |
| Clic derecho | Golpear con el bastón. |
| Espacio | Rodar hacia WASD; sin movimiento, hacia donde apunta el mago. |
| E | Recoger pociones o utilizar la escalera. |
| Q | Consumir una poción si falta vida. |
| Esc | Pausar o volver a la pantalla anterior del menú. |
| R | Reintentar tras la derrota; durante la partida, solicitar un reinicio completo del nivel. |

El inventario admite cinco pociones; cada una recupera hasta 35 de vida. El reinicio completo restaura enemigos, suministros, inventario y progreso. El reintento del jefe restaura el estado guardado al activar el círculo y comienza otra vez su primera fase; ese punto de control dura durante la partida actual.

La rodada recorre 3,2 metros en 0,5 segundos, seguida de 0,15 segundos de recuperación. Se puede iniciar otra cada 0,95 segundos y hay invulnerabilidad entre los segundos 0,04 y 0,30. Durante la acción no se puede atacar, consumir pociones ni interactuar; las paredes y los cuerpos siguen bloqueando el paso. Cada pulsación inicia como máximo una rodada. Sus valores se ajustan en `PlayerDodge`.

El jefe persigue a 4,6 m/s en fase uno y aproximadamente 5,06 m/s en fase dos. Sus preparaciones y pausas son cortas, con posición o dirección anunciada antes del impacto. La fase dos emite abanicos de cuatro tandas de once orbes, anillos de tres tandas de veinticuatro y espirales de tres brazos con 72 orbes; los proyectiles salen de forma progresiva, respetan coberturas y se limitan a 160 simultáneos. Vida, daño, ritmo, preparación y densidad se ajustan en el prefab `CustodioDelEclipse`.

En el santuario, los guerreros se mueven a 3,2 m/s y atacan cada 1,05 s; los magos se mueven a 2,9 m/s y lanzan cada 1,9 s; los apoyos se mueven a 2,7 m/s y curan cada 5,2 s. Son valores base antes de recibir bonificaciones. Se conservan su vida y daño por golpe. `Enemies/SanctuaryEncounter` coordina exclusivamente ese grupo. Las barras de vida muestran el daño y las curaciones durante el combate.

Las pantallas y sus controles están en el objeto `UI` de cada escena. Las opciones controlan Volumen general, Música, Ambiente, Efectos e Interfaz. Reiniciar el nivel, abandonar la partida desde pausa o salir desde ella solicita confirmación. La victoria continúa a los créditos, donde se incluyen las fuentes y licencias.

## Herramientas y audio

- **Unity 6000.3.10f1**, con URP, Input System y AI Navigation.
- **FMOD Studio 2.04.00**, integrado en Unity.
- **Reaper**, para edición y procesamiento de audio.
- **Blender 5.3**, para adaptar el modelo, el martillo, las alas y las animaciones del jefe. El editable se conserva en `Art/Blender/CustodioDelEclipse.blend`.

El proyecto de FMOD está en `Audio/FMOD/TorreDelEclipseAudio/`, con el archivo `.fspro`, los audios importados, eventos y bancos en `Build/Desktop/`. Las sesiones de Reaper y sus medios están en `Audio/ExportadosFMOD/Reaper/`; las fuentes y exportaciones se conservan en `Audio/Fuentes/` y `Audio/ExportadosFMOD/`. `Bloque06` corresponde a la maqueta general, `Bloque08` al guerrero y `Bloque12` al jefe.

El ambiente cambia entre interior y exterior; la música responde al combate de guerreros, perseguidores, magos y apoyos. Los efectos acompañan acciones, daño, recursos y resultados. El mago enemigo utiliza un tono más grave para el lanzamiento. El apoyo reutiliza los eventos de hechizo y curación, con tonos y niveles diferentes para la carga, la cura y la aceleración; no requiere eventos ni bancos nuevos. El mezclador separa música, ambiente, efectos e interfaz; la pantalla de Opciones está pendiente.

El esquive reutiliza el evento de barrido de bastón con tono y nivel reducidos, y un paso al aterrizar. Los pasos de caminata se suspenden durante la rodada; no se requieren nuevos eventos ni bancos de FMOD.

El jefe incorpora doce eventos en `SFX/Jefe` y `MUS/Jefe`, con el parámetro `Fase`: cambia de la versión instrumental a la versión con coros de **Blackmoor Colossus**, de **Matthew Pablo**, sin reiniciar la música. Ambas son mezclas alternativas completas, no pistas instrumentales separadas. Los efectos salen por Efectos y la música por Música, dentro de Master; se conservan las sesiones `Jefe_Efectos.rpp` y `Jefe_Musica.rpp`. Fuentes, atribuciones y licencias están en el registro de créditos.

Los eventos 2D `UI/Seleccionar` y `UI/Confirmar` utilizan el grupo Interfaz. Sus dos fuentes de síntesis y exportaciones se conservan en `Audio/Fuentes/Interfaz/` y `Audio/ExportadosFMOD/Interfaz/`.

## Documentación

- [Game Concept, dos carillas](Docs/GameConcept.pdf).
- [Tareas, hitos y criterios de finalización](Docs/Backlog.md).
- [Fuentes y créditos de recursos](Docs/Fuentes.md).
