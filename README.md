# Torre del Eclipse

Proyecto individual de **Iñaki Bertran** para Programación de Videojuegos II y Audio y Sonido para Videojuegos. Nombre del juego provisional.

## Experiencia prevista

Juego de acción 3D con cámara superior. Un mago despeja la planta baja de una torre, obtiene y utiliza recursos y asciende a la terraza para enfrentar a un jefe. La versión base incluye proyectiles mágicos y un ataque cuerpo a cuerpo con bastón.

## Estado actual

**Hito 0 completo. Hito 1 en desarrollo.**

Implementado y probado por el autor:

- Escena `Assets/Scenes/PrototipoTorre.unity` con piso, paredes y obstáculo de prueba.
- Mago de Quaternius con animaciones de reposo y desplazamiento.
- Movimiento con WASD relativo a la orientación del mago, apuntado con el mouse y cámara superior con seguimiento.
- Colisiones del jugador mediante Character Controller.
- Integración inicial FMOD-Unity: el evento `PruebaConexion` se reproduce en la escena `PruebaAudio`.
- Packs retro importados y fuentes de recursos documentadas.

Pendiente de integrar y comprobar: proyectiles, enemigo, sistema de daño y primera secuencia de combate del Hito 1. Los demás sistemas de juego y el audio adaptativo siguen planificados en el backlog.

## Controles actuales

| Entrada | Acción implementada |
|---|---|
| Mouse | Orientar al mago. |
| W | Avanzar hacia donde mira el mago. |
| S | Retroceder respecto de su orientación. |
| A / D | Desplazarse hacia su izquierda / derecha. |

El ataque con clic izquierdo se incorpora en el siguiente bloque de combate y todavía no forma parte de esta versión probada.

## Abrir el proyecto

1. En Unity Hub, añadir esta carpeta de repositorio como proyecto existente.
2. Abrir con **Unity 6000.3.10f1** y esperar la importación de paquetes y recursos.
3. Abrir `Assets/Scenes/PrototipoTorre.unity` para probar movimiento o `Assets/Scenes/PruebaAudio.unity` para revisar la prueba de sonido.
4. Entrar en Play y hacer clic dentro de Game para recibir los controles.

## Herramientas y configuración

- Unity **6000.3.10f1**, Universal Render Pipeline **17.3.0**.
- Input System **1.18.0** y AI Navigation **2.0.10**.
- FMOD Studio **2.04.00**; integración de Unity `com.fmod.fmod-for-unity.v204`, versión **2.4.0**.
- Proyecto de audio: `Audio/FMOD/TorreDelEclipseAudio/TorreDelEclipseAudio.fspro`.
- Reaper para la edición, producción y mezcla del audio que se incorporará.
- Git y GitHub para el historial de desarrollo.

## Arte y recursos

Fantasía oscura con estética low poly y texturas retro inspiradas en PS1/PS2. La referencia arquitectónica es Alzaluna, de Baldur's Gate 3; Diablo II orienta la perspectiva y el ciclo de acción y recursos.

- **Kenney — Retro Fantasy Kit:** exterior y terraza.
- **BlenderVoyage — PSX Style Modular Low Poly Dungeon, Free Edition:** interiores.
- **Quaternius — RPG Character Pack, Wizard:** mago, bastón, texturas y animaciones.

Las licencias, enlaces, usos y atribuciones figuran en [Fuentes y créditos](Docs/Fuentes.md).

## Documentación

- [Game Concept, dos carillas](Docs/GameConcept.pdf).
- [Concepto editable](Docs/GameConcept.md).
- [Tareas, hitos y criterios de finalización](Docs/Backlog.md).
- [Fuentes y créditos de recursos](Docs/Fuentes.md).

## Organización

- `Assets/Scripts`: componentes de movimiento, apuntado, cámara y animación.
- `Assets/Art`: modelos, texturas, materiales y controladores de animación.
- `Assets/Scenes`: escenas de prueba y desarrollo.
- `Audio/Fuentes`: fuentes sonoras de trabajo.
- `Audio/FMOD`: proyecto completo de FMOD, fuentes y bancos exportados.
- `Docs`: concepto, planificación y fuentes.

Las carpetas temporales de Unity y FMOD se excluyen del historial; se conservan los proyectos editables y los bancos necesarios para reproducir la prueba de audio.

## Hitos y entregas

El desarrollo sigue preproducción, core gameplay, vertical slice, producción, feature complete y release. La preparación de audio avanza en paralelo para una primera muestra el **13/11/2026**. Entrega final de ambas materias: **20/11/2026**.

## Alcance de la versión base

Un mago jugable, un nivel con planta baja y terraza, ambos tipos de combate, pociones, enemigos, jefe, menús, audio adaptativo y flujo completo de victoria/derrota con reinicio. La vara secreta y su acertijo son ampliaciones; guerrero y arquero quedan para después de completar y probar la base.
