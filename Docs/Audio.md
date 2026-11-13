# Audio - Torre del Eclipse

Registro de diseño e integración de la maqueta del **13/11/2026**. Responsable del proyecto: **Iñaki Bertran**. La maqueta está integrada en `Assets/Scenes/PrototipoTorre.unity`; no representa todavía el audio de todo el nivel final.

## Intención sonora

El interior busca una sensación cerrada y arcana; el exterior abre el espacio mediante viento y una textura más expuesta. La música mantiene una base de exploración y aumenta su actividad durante el combate. Los ataques, impactos y recursos deben reconocerse sobre esas capas. La fuente de trabajo es síntesis original; los pasos y las heridas son representaciones abstractas, sin voces grabadas.

## Eventos y estados

| Evento FMOD | Función | Grupo |
|---|---|---|
| `event:/AMB/Torre` | Mezcla interior/exterior mediante `Zona`, de 0 a 1. | Ambiente |
| `event:/MUS/Torre` | Mezcla exploración/combate mediante `Combate`, de 0 a 1. | Musica |
| `event:/SFX/Pasos` | Desplazamiento real del mago; tres variantes. | Efectos |
| `event:/SFX/Hechizo` | Lanzamiento de proyectil; tres variantes. | Efectos |
| `event:/SFX/Baston` | Golpe de bastón; tres variantes. | Efectos |
| `event:/SFX/Imapcto` | Impacto del proyectil o acierto del bastón; tres variantes. | Efectos |
| `event:/SFX/Herida` | Daño no letal del jugador y los enemigos; tres variantes. | Efectos |
| `event:/SFX/Muerte` | Muerte del jugador o de un enemigo. | Efectos |
| `event:/SFX/Recoger` | Recogida de una poción. | Efectos |
| `event:/SFX/Curar` | Consumo de una poción que recupera vida. | Efectos |
| `event:/UI/Acceso` | Todos los enemigos requeridos han sido eliminados. | Interfaz |
| `event:/UI/Victoria` | Uso de la salida habilitada del prototipo. | Interfaz |
| `event:/UI/Derrota` | El jugador se queda sin vida. | Interfaz |

`Imapcto` conserva aquí la escritura exacta del evento existente, para que la documentación coincida con el proyecto. La prueba técnica `event:/PruebaConexion` también se conserva, separada de los trece eventos de la maqueta.

`Zona` y `Combate` son parámetros locales. `TowerAudioDirector` interpola `Zona` entre dos puntos de referencia según la posición del jugador, con transición de 1,2 s; `Combate` responde a enemigos activos que detectan al mago, con transición de 0,8 s. Al terminar la partida baja la música para dar espacio al aviso de resultado. Reiniciar detiene y libera los loops anteriores.

Los ambientes duran 16 s. Las dos capas musicales tienen igual duración y progresión: 90 BPM, 4/4, ocho compases, 1.024.000 muestras a 48 kHz (21,333333 s). Esto permite reproducirlas sincronizadas durante los cambios de intensidad.

## Mezcla y procesamiento

Los eventos se incluyen en `Master.bank`; `Master.strings.bank` conserva sus rutas. Tener un único banco no impide controlar categorías: el mezclador separa `bus:/Musica`, `bus:/Ambiente`, `bus:/Efectos` y `bus:/Interfaz`, bajo el volumen general `bus:/`.

Reaper conserva ReaEQ en las pistas de ambientes, música y pasos. Las copias de ambientes y música importadas en FMOD coinciden con sus exportaciones. Los tres pasos importados todavía corresponden a las fuentes originales mono de 16 bits; sus exportaciones editadas de Reaper son estéreo de 24 bits y quedan conservadas para revisar el formato e importar la revisión. FMOD incorpora variación de sonidos con multi-instrumentos y ajustes aleatorios, reverberación en el interior y el hechizo, y espacialización de efectos del mundo. El listener está en la cámara y utiliza al jugador como referencia de atenuación.

`AudioVolumeSettings` está incorporado a `AudioSystems`, con controles general, música, ambiente, efectos e interfaz y almacenamiento mediante PlayerPrefs. Todavía falta confirmar los cinco controles y la persistencia al reabrir; la pantalla de Opciones se implementará en H3-03. El código preparado no equivale a una interfaz de opciones terminada.

## Integración en el nivel construido

`Assets/Scenes/TorreEclipse.unity` reutiliza la maqueta. Un volumen espacial identifica la terraza para llevar `Zona` de interior a exterior; el prototipo conserva su interpolación entre puntos. Los diez perseguidores de la planta baja alimentan el estado musical de combate. El desbloqueo de la escalera reproduce el aviso de acceso; la llegada a la terraza no dispara victoria porque el jefe sigue pendiente.

Los suministros utilizan provisionalmente el evento de impacto existente para su feedback de rotura, además de partículas. Su timbre final podrá ajustarse durante la producción sonora. La prueba en Play comprobó que, después de ascender y reiniciar, queda una sola instancia de ambiente y una de música y se recupera la zona interior.

## Archivos conservados

| Ubicación | Contenido |
|---|---|
| `Audio/Fuentes/Bloque06/Originales/` | 25 WAV originales PCM de 16 bits, 48 kHz. |
| `Audio/Fuentes/Bloque06/Fuentes.md` e `Inventario.json` | Método, procedencia y características de las fuentes. |
| `Audio/ExportadosFMOD/Reaper/` | `Ambientes.rpp`, `Musica.rpp`, `Pasos.rpp` y siete medios vinculados en `Media/`. |
| `Audio/ExportadosFMOD/Bloque06/` | 25 WAV de trabajo: siete de 24 bits y dieciocho de 16 bits, todos a 48 kHz. |
| `Audio/FMOD/TorreDelEclipseAudio/` | Proyecto `.fspro`, copias importadas, metadatos de eventos y mezcla, y bancos Desktop. |
| `Assets/Scripts/Audio/` | Reproducción de acciones, salud, impactos, dirección musical/ambiental y volúmenes. |

Los medios de Reaper se conservan aunque también exista una exportación con nombre similar: las sesiones los necesitan para poder abrirse sin archivos faltantes. Los respaldos y las cachés temporales de Reaper y FMOD quedan fuera del historial.

## Verificación y alcance pendiente

El autor confirmó que completó las pruebas del Bloque 6. La revisión de archivos comprueba que las tres sesiones encuentran sus siete medios, que están las 25 exportaciones, que las parejas de loops tienen igual duración y que los eventos están asignados al banco y a los cuatro grupos de mezcla. La escena guardada incluye el director, las referencias de eventos y el componente de volumen; el prefab del proyectil incluye su sonido de impacto.

Quedan pendientes la revisión e importación de los tres pasos editados, la prueba específica del volumen persistente, el balance en el recorrido final, los sonidos de variantes de enemigos, jefe, destructibles y menús, y la comprobación del ejecutable fuera del editor. La victoria actual corresponde al prototipo; el final del juego será el combate de la terraza, cuyo diseño se acordará con el autor antes de implementarlo.

La procedencia y los créditos de recursos se registran en [Fuentes](Fuentes.md); los estados de producción, en [Backlog](Backlog.md).
