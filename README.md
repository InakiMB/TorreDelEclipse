# Torre del Eclipse

Proyecto individual de **Iñaki Bertran** para Programación de Videojuegos II y Audio y Sonido para Videojuegos.

## Experiencia prevista

Juego de acción 3D con cámara superior. Un mago despeja la planta baja de una torre, obtiene y utiliza recursos y asciende a la terraza para enfrentar a un jefe. La versión base incluye proyectiles mágicos y un ataque cuerpo a cuerpo con bastón.

## Estado actual

**Hitos 0 y 1 completos. Hito 2 pendiente.** Actualizado al **12/11/2026**.

Implementado y probado por el autor en `Assets/Scenes/PrototipoTorre.unity`:

- Sala de prueba con piso, paredes y un obstáculo.
- Mago de Quaternius con animaciones de reposo, desplazamiento y muerte.
- Movimiento WASD en direcciones fijas respecto de la pantalla, independiente del apuntado con mouse. El mago puede desplazarse de costado o retroceder mientras apunta y dispara en otra dirección.
- Cámara superior con seguimiento y colisiones del jugador mediante Character Controller.
- Proyectiles mágicos: disparo con clic izquierdo, daño, colisión con el escenario y tiempo de vida limitado.
- Dos enemigos de prueba que detectan al jugador, navegan alrededor de obstáculos, atacan cuerpo a cuerpo, reciben daño y mueren.
- Vida del jugador, derrota, panel de vida de jugador y enemigos, aviso de zona despejada y reinicio de la escena.
- Integración inicial FMOD-Unity: el evento `PruebaConexion` se reproduce en la escena `PruebaAudio`.
- Packs retro importados y fuentes de recursos documentadas.

La escena permite repetir el combate básico completo. Los enemigos se representan con cápsulas y el proyectil con una esfera; sus modelos, animaciones y efectos finales siguen pendientes. El aviso de zona despejada corresponde al encuentro de prueba. La progresión hacia la terraza, el jefe y la victoria del nivel se desarrollan en los siguientes hitos.

Siguen pendientes el golpe de bastón, loot y pociones, progresión, variantes de enemigos, jefe, menús finales y audio adaptativo. El sonido disponible hasta ahora es la prueba de conexión.

## Controles actuales

| Entrada | Acción implementada |
|---|---|
| W / S | Moverse hacia arriba / abajo de la pantalla. |
| A / D | Moverse hacia la izquierda / derecha de la pantalla. |
| Mouse | Apuntar, independientemente del movimiento. |
| Clic izquierdo, mantenido o repetido | Lanzar proyectiles mágicos. |
| R / botón Reiniciar | Reiniciar la escena de combate. |

El movimiento responde al teclado sin inercia horizontal añadida; las diagonales conservan la misma velocidad total. Los controles de bastón, interacción y pociones siguen previstos en el concepto.

## Abrir y probar el proyecto

1. En Unity Hub, añadir esta carpeta de repositorio como proyecto existente.
2. Abrir con **Unity 6000.3.10f1** y esperar la importación de paquetes y recursos.
3. Abrir `Assets/Scenes/PrototipoTorre.unity` para probar movimiento y combate, o `Assets/Scenes/PruebaAudio.unity` para revisar la prueba de sonido.
4. Entrar en Play y hacer clic dentro de Game para recibir los controles.
5. Moverse mientras se apunta en otra dirección, disparar a los enemigos y comprobar el daño en el panel. Al eliminar a ambos aparece el aviso de zona despejada; al perder toda la vida se detienen los controles del mago.
6. Pulsar R o el botón Reiniciar para restablecer el encuentro. `PrototipoTorre` debe permanecer habilitada en la Scene List del Build Profile.

## Herramientas y configuración

- Unity **6000.3.10f1**, Universal Render Pipeline **17.3.0**.
- Input System **1.18.0** y AI Navigation **2.0.10**.
- FMOD Studio **2.04.00**; integración de Unity `com.fmod.fmod-for-unity.v204`, versión **2.4.0**.
- Proyecto de audio: `Audio/FMOD/TorreDelEclipseAudio/TorreDelEclipseAudio.fspro`.
- Reaper para la edición, producción y mezcla del audio que se incorporará.
- Git y GitHub para el historial de desarrollo.

## Arte y recursos

Fantasía oscura con estética low poly y texturas retro inspiradas en PS1/PS2. Alzaluna, de Baldur's Gate 3, orienta la arquitectura y la ambientación; Diablo II, la perspectiva y el ciclo de acción y recursos; Enter the Gungeon, la separación entre movimiento y apuntado.

- **Kenney - Retro Fantasy Kit:** exterior y terraza.
- **BlenderVoyage - PSX Style Modular Low Poly Dungeon, Free Edition:** interiores.
- **Quaternius - RPG Character Pack, Wizard:** mago, bastón, texturas y animaciones.

Las licencias, enlaces, usos y atribuciones figuran en [Fuentes y créditos](Docs/Fuentes.md).

## Documentación

- [Game Concept, dos carillas](Docs/GameConcept.pdf).
- [Concepto editable](Docs/GameConcept.md).
- [Tareas, hitos y criterios de finalización](Docs/Backlog.md).
- [Fuentes y créditos de recursos](Docs/Fuentes.md).

## Organización

- `Assets/Scripts`: componentes de movimiento, apuntado, cámara, animación, combate, salud, enemigos y panel de prototipo.
- `Assets/Art`: modelos, texturas, materiales y controladores de animación.
- `Assets/Prefabs`: objetos reutilizables, incluido el proyectil mágico.
- `Assets/Scenes`: escenas de prueba y desarrollo, y datos de navegación del escenario.
- `Audio/Fuentes`: fuentes sonoras de trabajo.
- `Audio/FMOD`: proyecto completo de FMOD, fuentes y bancos exportados.
- `Docs`: concepto, planificación y fuentes.

Las carpetas temporales de Unity y FMOD se excluyen del historial; se conservan los proyectos editables y los bancos necesarios para reproducir la prueba de audio.

## Hitos y entregas

El desarrollo sigue preproducción, core gameplay, vertical slice, producción, feature complete y release. La preparación de audio avanza en paralelo para una primera muestra el **13/11/2026**. Entrega final de ambas materias: **20/11/2026**.

## Alcance de la versión base

Un mago jugable, un nivel con planta baja y terraza, ambos tipos de combate, pociones, tres tipos de enemigos comunes (guerrero cuerpo a cuerpo, mago a distancia y apoyo que cura o potencia aliados), jefe, menús, audio adaptativo y flujo completo de victoria/derrota con reinicio. Cada tipo de enemigo tendrá valores propios de vida, daño, velocidad y tiempo entre acciones; las variantes finales siguen pendientes.

El diseño del jefe se definirá con el autor antes de implementarlo. La vara secreta y su acertijo son ampliaciones; guerrero y arquero jugables quedan para después de completar y probar la base.
