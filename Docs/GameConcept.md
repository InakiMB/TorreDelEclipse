# Torre del Eclipse - Game Concept

Borrador v0.6 | 13/11/2026 | Desarrollo individual

Autor: Iñaki Bertran. Desarrollo individual.

## 1. Visión general del juego

Género y público. Acción y exploración de fantasía oscura, con cámara superior (Top View / Bird View), para jugadores que disfrutan el combate con magia y los secretos del escenario.

Premisa y experiencia. Un mago entra en una antigua torre ocupada por fuerzas hostiles. Debe despejar la planta baja y alcanzar la terraza para derrotar a su guardián. El recorrido busca transmitir una amenaza creciente: del encierro de los salones a la exposición de la cima.

## 2. Jugabilidad

2.1. Mecánicas principales. El jugador combina proyectiles mágicos y golpes de bastón. Se prevén guerreros cuerpo a cuerpo, magos a distancia y apoyos que curan o potencian aliados; cada tipo tendrá vida, daño, velocidad y cadencia propios. El prototipo implementa la base del perseguidor.

Recursos e interacción. Los enemigos tienen 25% de probabilidad configurable de dejar una poción. Se recoge con E y se consume con Q para recuperar vida sin superar el máximo; la interfaz muestra las disponibles. Se prevén cajas o barriles que, al romperse con disparos, dejan una poción garantizada. Despejar la planta baja habilita el acceso a la terraza, con una señal visual y sonora.

Jefe, victoria y derrota. El guardián de la terraza presenta ataques reconocibles que el jugador puede evitar mediante el movimiento. Su diseño detallado se definirá con el autor antes de implementarlo. Derrotarlo completa el nivel; quedarse sin vida provoca la derrota y permite reiniciar la partida.

Controles. WASD mueve en direcciones fijas de pantalla; el mouse apunta de forma independiente. Clic izquierdo dispara, clic derecho golpea con el bastón, E interactúa, Q consume una poción y R reinicia el prototipo.

2.2. Flujo de juego. Menú inicial - explorar la planta baja - combatir y utilizar recursos - desbloquear el ascenso - enfrentar al jefe - victoria o derrota. Se prevé un nivel completo con dos zonas principales y al menos tres minutos de juego efectivo.

## 3. Arte y estilo visual

Fantasía medieval oscura con aspecto low poly y texturas retro PS1/PS2: piedra envejecida, arcos altos, telas deterioradas, antorchas y símbolos arcanos. Se propone una paleta fría con luces ámbar y acentos violetas para la magia. La cámara superior debe permitir distinguir enemigos, proyectiles y objetos interactuables.

3.1. Elementos clave. El mago utiliza el Wizard de Quaternius; exterior y terraza se construirán con Kenney Retro Fantasy Kit y los interiores con el dungeon PSX de BlenderVoyage. Se conservarán sus licencias y atribuciones. Enemigos, magia y objetos deben distinguirse desde la cámara superior.

## 4. Audio

Ambiente adaptativo. La maqueta mezcla progresivamente interior y exterior según la posición del jugador, mediante el parámetro local Zona de FMOD. El cambio ya se prueba en una misma escena. El nivel final llevará esta transición desde los salones cerrados hasta el viento de la terraza.

Música adaptativa. Dos capas sincronizadas de ocho compases, a 90 BPM y 4/4, cambian con el parámetro Combate al detectar enemigos y vuelven a exploración al terminar la amenaza. El jefe tendrá su tratamiento musical cuando se acuerde su diseño.

Efectos y procesamiento. La maqueta incluye pasos, bastón, hechizos, impactos, daño, muerte y pociones como sonidos del mundo; acceso, victoria y derrota aportan avisos de estado. Los multi-instrumentos varían las repeticiones. ReaEQ en Reaper y reverberación en FMOD definen timbre y espacio; botones y sistemas futuros quedan pendientes.

Flujo de producción. Se conservan 25 fuentes de síntesis original, exportaciones y tres sesiones de Reaper, junto al proyecto FMOD y sus bancos. Hay trece eventos integrados y grupos de música, ambiente, efectos e interfaz. El componente de volumen general y por grupo está incorporado; falta comprobar su persistencia y conectarlo a la pantalla de Opciones.

## 5. Interfaz de usuario

Menú y pantallas. Título con Play, Créditos, Opciones y Exit. Pantallas de victoria y derrota con opciones para reiniciar o volver al menú. Los créditos identifican al autor y las fuentes de los recursos utilizados.

Durante la partida. Vida del mago, cantidad de pociones, objetivo actual y vida del jefe durante su combate. Las interacciones y el acceso desbloqueado reciben feedback visual y sonoro. Si se incorpora la vara secreta, se muestra si conserva su carga.

## 6. Consideraciones técnicas y de desarrollo

Herramientas. Unity 6000.3.10f1 con URP, C#, Reaper y FMOD Studio 2.04.00 con su integración de Unity. Git y GitHub registrarán la evolución. Las tareas indicarán responsable, estado, hito y criterio de finalización; se adaptarán al sistema que indique la cursada.

Arquitectura y producción. Separar movimiento, combate, salud, objetos y audio en componentes con responsabilidades claras. Aplicar los conceptos de Programación II según el problema que resuelvan. Avanzar por preproducción, core gameplay, vertical slice, producción, feature complete y release, con pruebas en cada etapa.

Alcance base. Un mago jugable, ambos tipos de ataque, enemigos, pociones, planta baja, terraza, jefe, menús y flujo completo de victoria y derrota. La entrega incluye ejecutable, repositorio, documentación y proyectos completos de Unity y FMOD.

Ampliación prioritaria. Un acertijo breve de tres runas revela una vara oculta. Su rayo oscuro inflige un daño extraordinario y dispone de una sola carga por partida, con un sonido propio de carga, descarga e impacto. Es opcional y el jefe puede derrotarse sin ella. Guerrero y arquero seleccionables quedan como ampliaciones posteriores a la versión base.

## 7. Juegos similares para referencia

Baldur's Gate 3: Alzaluna como referencia espacial, visual y de descubrimiento de secretos. Diablo II: lectura del espacio desde arriba y ciclo de exploración, combate y recursos. Enter the Gungeon: movimiento y apuntado independientes. Las referencias orientan la experiencia y la selección de materiales.
