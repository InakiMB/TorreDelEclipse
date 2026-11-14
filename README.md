# Torre del Eclipse

Proyecto individual de **Iñaki Bertran** para **Programación de Videojuegos II** y **Audio y Sonido para Videojuegos**. Entrega final: **20/11/2026**.

Juego de acción 3D con cámara superior y estética de fantasía oscura low poly. Un mago debe despejar la planta baja de una torre y ascender a la terraza para enfrentar al jefe.

## Estado del proyecto

Actualizado al **13/11/2026**. Hitos **0, 1 y 2 completos**; **Producción en curso**.

- Movimiento con WASD y apuntado independiente con mouse; ataques con magia y bastón.
- Salud, daño, loot de enemigos con 25% de probabilidad, recogida y consumo de pociones.
- Planta baja y terraza construidas, con diez enemigos y seis cajas o barriles que dejan una poción garantizada al romperse.
- Guerrero enemigo con espada, escudo y animaciones de reposo, carrera, ataque y muerte; el tajo sincroniza daño y sonido y permite esquivarlo durante la preparación.
- Mago enemigo de túnica rojo oscuro: combate a distancia, retrocede al acercarse el jugador y anuncia sus proyectiles con carga y animación. Sus disparos pueden esquivarse o detenerse con coberturas.
- Ascenso y regreso con E al despejar la planta baja, derrota y reinicio del nivel.
- Ambiente variable, música de exploración/combate y efectos integrados mediante FMOD.

Siguen pendientes el enemigo de apoyo, el jefe y la victoria final, los menús con Opciones y Créditos, el balance y la prueba del ejecutable. La terraza actual permite explorar y regresar; su llegada todavía no completa el juego.

## Abrir y probar

1. Añadir la carpeta del repositorio a Unity Hub y abrirla con **Unity 6000.3.10f1**.
2. Abrir `Assets/Scenes/TorreEclipse.unity`, entrar en Play y hacer clic dentro de Game.
3. Eliminar a los diez enemigos —incluidos un guerrero y dos magos— para habilitar el ascenso del santuario; acercarse a la escalera y pulsar E. En la terraza hay un acceso de regreso.

`PrototipoTorre.unity` conserva el encuentro corto anterior, con victoria mediante su salida. `PruebaAudio.unity` conserva la prueba de conexión de FMOD.

## Controles

| Entrada | Acción |
|---|---|
| WASD | Moverse respecto de la pantalla. |
| Mouse | Apuntar. |
| Clic izquierdo | Lanzar magia. |
| Clic derecho | Golpear con el bastón. |
| E | Recoger pociones o utilizar la escalera. |
| Q | Consumir una poción si falta vida. |
| R | Reiniciar el nivel. |

El inventario admite cinco pociones; cada una recupera hasta 35 de vida. Reiniciar restaura enemigos, suministros, inventario y progreso.

## Herramientas y audio

- **Unity 6000.3.10f1**, con URP, Input System y AI Navigation.
- **FMOD Studio 2.04.00**, integrado en Unity.
- **Reaper**, para edición y procesamiento de audio.

El proyecto de FMOD está en `Audio/FMOD/TorreDelEclipseAudio/`, con el archivo `.fspro`, los audios importados, eventos y bancos en `Build/Desktop/`. Las sesiones de Reaper y sus medios están en `Audio/ExportadosFMOD/Reaper/`; las fuentes y exportaciones se conservan en `Audio/Fuentes/` y `Audio/ExportadosFMOD/`. `Bloque06` corresponde a la maqueta general y `Bloque08` al audio del guerrero.

El ambiente cambia entre interior y exterior; la música responde al combate. Los efectos acompañan acciones, daño, recursos y resultados. El mezclador separa música, ambiente, efectos e interfaz; la pantalla de Opciones está pendiente.

## Documentación

- [Game Concept, dos carillas](Docs/GameConcept.pdf).
- [Tareas, hitos y criterios de finalización](Docs/Backlog.md).
- [Fuentes y créditos de recursos](Docs/Fuentes.md).
