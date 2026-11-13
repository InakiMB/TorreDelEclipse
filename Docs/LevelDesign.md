# Diseño del nivel - Torre del Eclipse

Primera construcción del **13/11/2026**. Escena: `Assets/Scenes/TorreEclipse.unity`. Autor del proyecto: **Iñaki Bertran**.

## Referencia y recorrido

La organización se inspira en el gran vestíbulo, las alas laterales, el salón ceremonial y el ascenso de [Alzaluna](https://bg3.wiki/wiki/Moonrise_Towers). La [terraza](https://bg3.wiki/wiki/Moonrise_Towers_Rooftop) orienta el remate fortificado y abierto. Se adapta esa referencia a cámara superior y combate en tiempo real.

El recorrido parte del vestíbulo, abre el salón central y sus dos alas, llega al santuario y habilita el ascenso cuando se eliminan los diez enemigos requeridos. Las alas se conectan al salón por dos extremos, permitiendo rodear columnas y coberturas. Los enemigos actuales son perseguidores de prueba; sus posiciones se ajustarán al incorporar guerrero, mago y apoyo.

| Sector | Medidas aproximadas | Función |
|---|---|---|
| Aproximación exterior | 8 × 12 m | Enmarcar la entrada y ofrecer un espacio inicial seguro. |
| Vestíbulo | 24 × 12 m | Orientación, primeras amenazas y primer suministro visible. |
| Gran salón | 24 × 24 m | Eje central, seis columnas, circulación lateral y coberturas. |
| Alas oeste y este | 12 × 28 m cada una | Recorridos alternativos, suministros y encuentros separados. |
| Santuario | 24 × 16 m | Encuentro final de planta baja y señalización del ascenso. |
| Terraza | Octógono de hasta 32 × 32 m | Cima fortificada, cuatro torreones y centro amplio para adaptar al jefe. |

Las columnas y coberturas tienen colisión y bloquean proyectiles. Las paredes y los torreones que se interponen entre cámara y mago se ocultan durante la partida conservando sus colisiones. El techo interior queda abierto a la cámara superior.

## Conexión de zonas

La planta baja y la terraza están separadas espacialmente dentro de una misma escena. La escalera utiliza E como transición: mueve al jugador a una llegada segura en la terraza y reposiciona la cámara. Hay un acceso de regreso. El estado de combate, salud, inventario y objetos se mantiene entre ambas zonas.

La puerta del ascenso está bloqueada mientras queden enemigos requeridos. Al limpiarlos se retira el sello y suena el aviso de acceso. Alcanzar la terraza mantiene el control del mago. El jefe, sus ataques, la arena definitiva y el cierre del nivel se definirán con Iñaki antes de implementarse.

## Suministros

Hay seis destructibles: cuatro en la planta baja y dos en la terraza. Cada caja o barril tiene 25 de vida, recibe daño mediante el sistema Health/IDamageable y deja exactamente una poción al romperse. Disparos simultáneos no duplican el drop. El 25% de loot de enemigos se conserva como sistema independiente.

El suministro de entrada enseña la interacción en una zona segura; los de las alas y el santuario recompensan el recorrido. Las pociones siguen usando E para recogida y Q para consumo, con capacidad máxima de cinco y curación de hasta 35. Reiniciar restaura enemigos, suministros, acceso, inventario, salud y posición inicial.

## Construcción y organización

`01_Arquitectura` contiene superficies y colisiones; `02_Ambientacion`, iluminación y elementos decorativos; `03_Suministros`, los destructibles; `04_Encuentros`, los perseguidores; `05_Interfaz`, el HUD compacto; `06_Navegacion`, la superficie horneada. Las llegadas, escalera y zona de terraza tienen objetos identificados por nombre.

Los modelos conservan referencias a los recursos importados. Los materiales, la geometría octogonal y el efecto de astillas están en `Assets/LevelDesign/TorreEclipse/`. Las herramientas de editor están en `Assets/Editor/LevelDesign/`. La escena anterior `PrototipoTorre` conserva su recorrido corto.

## Verificación

La revisión de navegación encontró rutas desde el inicio a los diez enemigos, al acceso de ascenso y dentro de la terraza, sin observaciones de materiales o del punto de aparición. Una prueba automática dentro de Play completó **24 comprobaciones sin fallos**:

- Presencia de enemigos sobre NavMesh y bloqueo inicial del ascenso.
- Rotura por proyectil y una sola poción ante dos impactos simultáneos.
- Recogida, consumo y curación sin superar la vida máxima.
- Desbloqueo al limpiar y alcance/línea de visión de las interacciones de ascenso y regreso.
- Llegada segura, reposicionamiento de cámara, reconocimiento de terraza y cambio de ambiente.
- Conservación del control al llegar a la cima, derrota y reinicio completo.
- Restauración de objetos y progreso; una sola instancia de ambiente y música tras reiniciar.

Esta verificación cubre la integración funcional; el recorrido manual de Iñaki y el balance de dificultad y duración siguen pendientes. También quedan los enemigos definitivos, el jefe, menús, audio específico de los sistemas futuros y prueba del ejecutable.
