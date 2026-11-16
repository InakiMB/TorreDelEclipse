# Audio del Custodio del Eclipse

Fuentes seleccionadas para Torre del Eclipse, 16/11/2026.

- Música: **Blackmoor Colossus / Colossal Boss Battle Theme**, **Matthew Pablo**. Fuente: https://opengameart.org/content/colossal-boss-battle-theme . Compositor: https://www.matthewpablo.com/ . Licencia **CC BY 3.0**, https://creativecommons.org/licenses/by/3.0/ . Se utilizan los loops originales con y sin voces/coros, convertidos a PCM estéreo 16 bits, 48 kHz. FMOD ajusta sus niveles y realiza una transición entre ambas mezclas completas según la fase, manteniendo la línea de tiempo. No son stems ni una composición original del proyecto.
- Metal y madera: **100 CC0 metal and wood SFX**, **rubberduck**. https://opengameart.org/content/100-cc0-metal-and-wood-sfx . Licencia CC0: https://creativecommons.org/publicdomain/zero/1.0/ . Martillos, impactos, láminas, madera y caídas.
- Criaturas, armas y magia: **80 CC0 RPG SFX**, **rubberduck**. https://opengameart.org/content/80-cc0-rpg-sfx . Licencia CC0: https://creativecommons.org/publicdomain/zero/1.0/ . Barridos, cadenas, voces de criaturas, piedra y hechizos.
- Alas: **Large Wings Flap**, **AntumDeluge**, basado en un sonido de **dave.des**. https://opengameart.org/content/large-wings-flap . Grabación base: https://freesound.org/s/127197/ . CC0: https://creativecommons.org/publicdomain/zero/1.0/ . Se conserva el README del paquete con su procedencia.

`Originales/` conserva 32 archivos seleccionados. `Inventario.json` identifica cada combinación y sus velocidades, niveles y posiciones. Los efectos se convierten a mono de 48 kHz, se combinan por capas, se ajustan en tono/duración mediante velocidad, reciben fades y se normalizan a un pico máximo aproximado de -2,5 dBFS antes de la mezcla de FMOD. La música conserva su estructura y duración original.

Las 22 exportaciones se conservan en `Audio/ExportadosFMOD/Bloque12/`. `Audio/ExportadosFMOD/Reaper/Jefe_Efectos.rpp` y `Jefe_Musica.rpp` incluyen sus medios vinculados. El proyecto de FMOD conserva las copias importadas, doce eventos SFX/Jefe y MUS/Jefe con Fase. Sus rutas y autores se registran también en Docs/Fuentes.md.
