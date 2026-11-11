# Torre del Eclipse - Game Concept

Borrador v0.3 | 11/11/2026 | Desarrollo individual

Autor: Iñaki Bertran. Desarrollo individual. Nombre del juego provisional.

## 1. Visión general del juego

Género y público. Acción y exploración de fantasía oscura, con cámara superior (Top View / Bird View), para jugadores que disfrutan el combate con magia y los secretos del escenario.

Premisa y experiencia. Un mago entra en una antigua torre ocupada por fuerzas hostiles. Debe despejar la planta baja y alcanzar la terraza para derrotar a su guardián. El recorrido busca transmitir una amenaza creciente: del encierro de los salones a la exposición de la cima.

## 2. Jugabilidad

2.1. Mecánicas principales. El jugador se desplaza libremente y combina proyectiles mágicos a distancia con golpes de bastón cuerpo a cuerpo. Ambos ataques causan daño y cumplen una función en combate. Los enemigos detectan al jugador, se aproximan o posicionan, atacan, reciben daño y mueren.

Recursos e interacción. Los enemigos pueden dejar pociones de vida. El jugador las recoge y decide cuándo consumirlas. Una interfaz indica cuántas tiene disponibles. Al eliminar a los defensores de la planta baja se habilita el acceso a la terraza, con una señal visual y sonora.

Jefe, victoria y derrota. El guardián de la terraza presenta ataques reconocibles que el jugador puede evitar mediante el movimiento. Derrotarlo completa el nivel; quedarse sin vida provoca la derrota y permite reiniciar la partida.

Controles propuestos. WASD para moverse; mouse para apuntar; clic izquierdo para el proyectil; clic derecho para el bastón; E para interactuar y Q para usar una poción.

2.2. Flujo de juego. Menú inicial - explorar la planta baja - combatir y utilizar recursos - desbloquear el ascenso - enfrentar al jefe - victoria o derrota. Se prevé un nivel completo con dos zonas principales y al menos tres minutos de juego efectivo.

## 3. Arte y estilo visual

Fantasía medieval oscura: piedra envejecida, arcos altos, telas deterioradas, antorchas y símbolos arcanos. Se propone una paleta fría con luces ámbar y acentos violetas para la magia. La cámara superior debe permitir distinguir enemigos, proyectiles y objetos interactuables.

3.1. Elementos clave. Mago con bastón, enemigos de siluetas legibles, pociones destacadas, acceso bloqueado y una terraza abierta para el jefe. Se utilizarán modelos y animaciones existentes de estilo coherente; la IA podrá apoyar retratos, iconos o texturas, con revisión de sus condiciones de uso.

## 4. Audio

Ambiente adaptativo. Dentro de una misma escena, pasar del salón a la escalera modifica progresivamente las capas de fuego, viento y murmullos. En la terraza predomina el viento exterior. El cambio debe percibirse durante el desplazamiento, sin depender de una carga de escena.

Música adaptativa. La exploración utiliza una base contenida; al entrar en combate aumenta la intensidad y al finalizar vuelve a la exploración. El enfrentamiento con el jefe constituye el punto de mayor tensión musical.

Efectos y procesamiento. Pasos, bastón, hechizos, impactos y puertas representan sonidos del mundo (diegéticos). Botones y avisos de victoria o derrota aportan señales extradiegéticas. Se usarán multi-instrumentos para variar pasos o impactos y procesamiento de resonancia y filtrado con una función reconocible.

Flujo de producción. Preparación, edición y mezcla de sonidos en Reaper; configuración de eventos y parámetros en FMOD; creación de bancos de audio e integración en Unity. El volumen general será ajustable desde las opciones.

## 5. Interfaz de usuario

Menú y pantallas. Título con Play, Créditos, Opciones y Exit. Pantallas de victoria y derrota con opciones para reiniciar o volver al menú. Los créditos identifican al autor y las fuentes de los recursos utilizados.

Durante la partida. Vida del mago, cantidad de pociones, objetivo actual y vida del jefe durante su combate. Las interacciones y el acceso desbloqueado reciben feedback visual y sonoro. Si se incorpora la vara secreta, se muestra si conserva su carga.

## 6. Consideraciones técnicas y de desarrollo

Herramientas previstas. Unity 6.3 LTS con Universal 3D, C#, Reaper y FMOD Studio 2.04 con su integración compatible. Git y GitHub registrarán la evolución. Las tareas indicarán responsable, estado, hito y criterio de finalización; se adaptarán al sistema que indique la cursada.

Arquitectura y producción. Separar movimiento, combate, salud, objetos y audio en componentes con responsabilidades claras. Aplicar los conceptos de Programación II según el problema que resuelvan. Avanzar por preproducción, core gameplay, vertical slice, producción, feature complete y release, con pruebas en cada etapa.

Alcance base. Un mago jugable, ambos tipos de ataque, enemigos, pociones, planta baja, terraza, jefe, menús y flujo completo de victoria y derrota. La entrega incluye ejecutable, repositorio, documentación y proyectos completos de Unity y FMOD.

Ampliación prioritaria. Un acertijo breve de tres runas revela una vara oculta. Su rayo oscuro inflige un daño extraordinario y dispone de una sola carga por partida, con un sonido propio de carga, descarga e impacto. Es opcional y el jefe puede derrotarse sin ella. Guerrero y arquero seleccionables quedan como ampliaciones posteriores a la versión base.

## 7. Juegos similares para referencia

Baldur's Gate 3: Alzaluna como referencia espacial, visual y de descubrimiento de secretos. Diablo II: lectura del espacio desde arriba y ciclo de exploración, combate y recursos. Las referencias orientan la experiencia y la selección de materiales.
