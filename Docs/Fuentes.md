# Fuentes y créditos

Autor del proyecto: **Iñaki Bertran**. Desarrollo individual.

Este registro identifica los autores, fuentes y licencias de los recursos incorporados al proyecto. Baldur's Gate 3 y Diablo II orientan la ambientación y el diseño; Enter the Gungeon orienta el esquema de movimiento y apuntado independientes. Sus modelos, texturas y músicas no se han incorporado como recursos del juego.

## Modelos, texturas y animaciones

| Recurso | Autor y fuente | Licencia | Ubicación | Uso y estado |
|---|---|---|---|---|
| Retro Fantasy Kit, versión 2.0 | **Kenney** — [Página del pack](https://kenney.nl/assets/retro-fantasy-kit) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | `Assets/Art/External/KenneyRetroFantasy/` | Modelos y texturas retro importados para exterior y terraza de la torre; textura de piedra para el prototipo. |
| RPG Character Pack — Wizard | **Quaternius** — [Página del pack](https://quaternius.com/packs/rpgcharacters.html) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | `Assets/Art/External/QuaterniusWizard/` | Modelo del mago, bastón, texturas y animaciones incluidas en `Wizard.fbx`. Se utiliza en el personaje jugable para reposo, desplazamiento, ataque con bastón y muerte. |
| PSX Style Modular Low Poly Dungeon — Free Edition | **BlenderVoyage** — [Página del pack](https://blendervoyage.itch.io/psx-style-modular-low-poly-dungeon) | [Creative Commons Attribution 4.0 International — CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) | `Assets/Art/External/BlenderVoyageDungeon/` | Piezas y texturas importadas para construir la planta baja: paredes, pisos, columnas, escaleras, puertas y otros elementos del pack gratuito. |

Se seleccionaron los modelos FBX y texturas de los paquetes originales, se reorganizaron sus carpetas y se configuraron materiales e importación para Unity URP y el aspecto retro del proyecto. Las licencias originales de Kenney y Quaternius se conservan como `License.txt` junto a sus recursos. Cada carpeta incluye un `Source.txt` con el origen del pack; el de BlenderVoyage contiene también el enlace a CC BY 4.0 y su atribución.

Las cápsulas de los enemigos, las esferas del proyectil, poción y marcador de salida, y el cubo de la barrera son primitivas de Unity con materiales configurados en el proyecto. Representan objetos de prueba y se reemplazarán durante la producción visual.

## Construcción de TorreEclipse

La escena `Assets/Scenes/TorreEclipse.unity` utiliza pisos, paredes, columnas, arcos, escaleras y altares del dungeon de BlenderVoyage; la terraza y los elementos de exterior utilizan modelos de Kenney, incluidos el cuerpo de torre, torreones, almenas, cajas y barriles. Se crearon materiales URP que reutilizan las texturas de estos packs y conservan sus atribuciones originales.

El suelo octogonal de la terraza, pretiles simples, alfombra, estandartes con motivo de eclipse y partes de los braseros se construyeron con geometría básica del proyecto. La rotura utiliza partículas simples. Los enemigos y las pociones mantienen sus representaciones de prueba.

Alzaluna se utilizó como referencia de organización espacial y ambientación mediante las páginas de [Moonrise Towers](https://bg3.wiki/wiki/Moonrise_Towers) y [Moonrise Towers Rooftop](https://bg3.wiki/wiki/Moonrise_Towers_Rooftop). Sus imágenes y mapas se consultaron como referencia; no se incorporaron como texturas ni modelos de esta escena.

## Audio

| Archivo o recurso | Autor/origen | Condiciones registradas | Uso y estado |
|---|---|---|---|
| `Audio/Fuentes/PruebaConexion.wav` | Señal sintética preparada con asistencia de Codex, sin grabaciones de terceros. | Creada para esta prueba técnica. | Comprobar que FMOD se escucha dentro de Unity; no es el diseño sonoro definitivo. FMOD conserva una copia importada en `Audio/FMOD/TorreDelEclipseAudio/Assets/PruebaConexion.wav`. |

### Maqueta sonora integrada el 13/11/2026

Se incorporaron **25 WAV originales de síntesis procedural preparados para Torre del Eclipse**, sin grabaciones, voces, samples ni composiciones extraídas de terceros. Se utilizaron ruido filtrado, osciladores, envolventes, barridos y secuencias de notas y percusión por síntesis. Los pasos y las heridas representan acciones de forma abstracta; no son grabaciones de personas.

| Familia | Archivos originales | Uso |
|---|---|---|
| Ambiente | `AMB_Interior.wav`, `AMB_Exterior.wav` | Capas de interior y exterior; loops estéreo de 16 segundos. |
| Música | `MUS_Exploracion.wav`, `MUS_Combate.wav` | Capas sincronizadas, estéreo, 90 BPM, 4/4 y ocho compases (21,333333 s). |
| Variantes de efectos | `Paso_01..03`, `Hechizo_01..03`, `Baston_01..03`, `Impacto_01..03`, `Herida_01..03` | Quince WAV mono para acciones y daño, con selección de variantes en FMOD. |
| Recursos y muerte | `Muerte.wav`, `Recoger.wav`, `Curar.wav` | Muerte, recogida y consumo de poción. |
| Progreso y resultado | `Acceso.wav`, `Victoria.wav`, `Derrota.wav` | Señales de acceso desbloqueado y fin del prototipo. |

Las fuentes PCM de 16 bits y 48 kHz están en `Audio/Fuentes/Bloque06/Originales/`. Su procedencia, método y características se conservan en [Fuentes del bloque](../Audio/Fuentes/Bloque06/Fuentes.md) e [Inventario](../Audio/Fuentes/Bloque06/Inventario.json). No se atribuye a estos archivos una licencia de un proveedor externo.

Iñaki realizó la edición y el procesamiento en Reaper de ambientes, música y pasos. Se conservan las tres sesiones `.rpp` y sus siete medios vinculados en `Audio/ExportadosFMOD/Reaper/`; incluyen ReaEQ. Las 25 exportaciones de trabajo destinadas a FMOD están en `Audio/ExportadosFMOD/Bloque06/`: siete exportaciones PCM de 24 bits y el resto PCM de 16 bits, todas a 48 kHz. FMOD conserva los tres pasos originales mono; las versiones editadas de pasos quedan pendientes de revisión de formato e importación. Las copias importadas y los eventos, parámetros y mezcla están en `Audio/FMOD/TorreDelEclipseAudio/`.

La maqueta ya está integrada en el prototipo y puede ajustarse durante la producción final. Los enlaces de música de Baldur's Gate 3 y Diablo compartidos como referencias no corresponden a archivos incorporados al juego. El [registro de audio](Audio.md) identifica el uso de cada evento y el estado de verificación.

## Créditos de recursos para la entrega

- **Retro Fantasy Kit**, por **Kenney** — [kenney.nl](https://kenney.nl/assets/retro-fantasy-kit) — [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/).
- **RPG Character Pack — Wizard**, por **Quaternius** — [quaternius.com](https://quaternius.com/packs/rpgcharacters.html) — [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/).
- **PSX Style Modular Low Poly Dungeon — Free Edition**, por **BlenderVoyage** — [Fuente original](https://blendervoyage.itch.io/psx-style-modular-low-poly-dungeon) — [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/). Integración en Unity con reorganización de carpetas y ajustes de materiales/importación.

La atribución de BlenderVoyage debe figurar también en la pantalla de créditos del juego con referencia a su fuente y licencia. El registro incluye la maqueta sonora actual y se actualizará con los recursos de los sistemas pendientes y cualquier modificación adicional de los recursos externos.
