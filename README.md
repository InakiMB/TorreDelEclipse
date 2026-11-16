# Torre del Eclipse

Proyecto individual de **Iñaki Bertran** para **Programación de Videojuegos II** y **Audio y Sonido para Videojuegos**. Entrega final: **20/11/2026**.

Juego de acción 3D con cámara superior y estética de fantasía oscura low poly. Un mago debe despejar la planta baja de una torre y ascender a la terraza para enfrentar al jefe.

## Estado del proyecto

Actualizado al **16/11/2026**. Hitos **0, 1 y 2 completos**; **Producción en curso**, con los tres enemigos de planta baja, esquive y jefe de dos fases implementados.

- Movimiento con WASD y apuntado independiente con mouse; ataques con magia y bastón.
- Rodada con Espacio en la dirección de movimiento o del apuntado al estar quieto, con animación, sonido, invulnerabilidad parcial y recarga visible en el HUD.
- Salud, daño, loot de enemigos con 25% de probabilidad, recogida y consumo de pociones.
- Planta baja y terraza construidas, con veinte enemigos y seis cajas o barriles que dejan una poción garantizada al romperse.
- Guerrero enemigo con espada, escudo y animaciones de reposo, carrera, ataque y muerte; el tajo sincroniza daño y sonido y permite esquivarlo durante la preparación.
- Mago enemigo de túnica rojo oscuro: combate a distancia, retrocede al acercarse el jugador y anuncia sus proyectiles con carga y animación. Sus disparos pueden esquivarse o detenerse con coberturas y no dañan a sus aliados; morir cancela la carga pendiente.
- Clérigo de apoyo con ropa verde oscuro y armadura color hueso: cura a un aliado herido un 15% de su vida máxima cada 6 segundos y otorga +20% de movimiento y +25% de velocidad de ataque a aliados visibles dentro de 8 metros. Las bonificaciones no se acumulan entre apoyos y se retiran al salir del alcance o morir el clérigo; el daño de cada golpe no aumenta.
- Ascenso y regreso con E al despejar la planta baja; entrar en el círculo de la terraza invoca al Custodio del Eclipse y sella el acceso.
- Jefe de 1800 de vida con martillo: combo, golpe cargado y embestida; al 50% despliega alas negras y añade abanicos, anillos, espirales y saltos. La introducción y la transformación tienen protección temporal para ambos participantes.
- Barra del jefe, señales de ataque, cámara de combate y victoria final. Al morir se puede reintentar desde la terraza con la vida y las pociones guardadas al activar el círculo, conservando despejada la planta baja.
- Ambiente variable, música de exploración/combate y efectos integrados mediante FMOD.

Siguen pendientes los menús con Opciones y Créditos, el balance con jugadores y la revisión final de requisitos y ejecutable. Llegar a la terraza permite prepararse; la victoria requiere derrotar al jefe.

## Abrir y probar

1. Añadir la carpeta del repositorio a Unity Hub y abrirla con **Unity 6000.3.10f1**.
2. Abrir `Assets/Scenes/TorreEclipse.unity`, entrar en Play y hacer clic dentro de Game.
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
| R | Reintentar al jefe tras morir en su arena; en los demás casos, reiniciar el nivel. |

El inventario admite cinco pociones; cada una recupera hasta 35 de vida. El reinicio completo restaura enemigos, suministros, inventario y progreso. El reintento del jefe restaura el estado guardado al activar el círculo y comienza otra vez su primera fase; ese punto de control dura durante la partida actual.

La rodada recorre 3,2 metros en 0,5 segundos, seguida de 0,15 segundos de recuperación. Se puede iniciar otra cada 0,95 segundos y hay invulnerabilidad entre los segundos 0,04 y 0,30. Durante la acción no se puede atacar, consumir pociones ni interactuar; las paredes y los cuerpos siguen bloqueando el paso. Cada pulsación inicia como máximo una rodada. Sus valores se ajustan en `PlayerDodge`.

## Herramientas y audio

- **Unity 6000.3.10f1**, con URP, Input System y AI Navigation.
- **FMOD Studio 2.04.00**, integrado en Unity.
- **Reaper**, para edición y procesamiento de audio.
- **Blender 5.3**, para adaptar el modelo, el martillo, las alas y las animaciones del jefe. El editable se conserva en `Art/Blender/CustodioDelEclipse.blend`.

El proyecto de FMOD está en `Audio/FMOD/TorreDelEclipseAudio/`, con el archivo `.fspro`, los audios importados, eventos y bancos en `Build/Desktop/`. Las sesiones de Reaper y sus medios están en `Audio/ExportadosFMOD/Reaper/`; las fuentes y exportaciones se conservan en `Audio/Fuentes/` y `Audio/ExportadosFMOD/`. `Bloque06` corresponde a la maqueta general, `Bloque08` al guerrero y `Bloque12` al jefe.

El ambiente cambia entre interior y exterior; la música responde al combate de guerreros, perseguidores, magos y apoyos. Los efectos acompañan acciones, daño, recursos y resultados. El mago enemigo utiliza un tono más grave para el lanzamiento. El apoyo reutiliza los eventos de hechizo y curación, con tonos y niveles diferentes para la carga, la cura y la aceleración; no requiere eventos ni bancos nuevos. El mezclador separa música, ambiente, efectos e interfaz; la pantalla de Opciones está pendiente.

El esquive reutiliza el evento de barrido de bastón con tono y nivel reducidos, y un paso al aterrizar. Los pasos de caminata se suspenden durante la rodada; no se requieren nuevos eventos ni bancos de FMOD.

El jefe incorpora doce eventos en `SFX/Jefe` y `MUS/Jefe`, con el parámetro `Fase`: cambia de la versión instrumental a la versión con coros de **Blackmoor Colossus**, de **Matthew Pablo**, sin reiniciar la música. Ambas son mezclas alternativas completas, no pistas instrumentales separadas. Los efectos salen por Efectos y la música por Música, dentro de Master; se conservan las sesiones `Jefe_Efectos.rpp` y `Jefe_Musica.rpp`. Fuentes, atribuciones y licencias están en el registro de créditos.

## Documentación

- [Game Concept, dos carillas](Docs/GameConcept.pdf).
- [Tareas, hitos y criterios de finalización](Docs/Backlog.md).
- [Fuentes y créditos de recursos](Docs/Fuentes.md).
