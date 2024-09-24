<h1><em>📜 2D Video Game with AI Applied to NPCs</em></h1>

<p align="left">
   <img src="https://img.shields.io/badge/STATUS-EN%20DESAROLLO-green">
   </p>

<h2>🎮 Descripción General</h2>


Este es un videojuego en 2D que combina dos géneros clásicos: plataformas y combate por turnos. El objetivo principal es que el jugador avance a través de distintos niveles, enfrentándose a enemigos controlados por IA, que ofrecen un desafío tanto en el entorno de plataformas como en los combates estratégicos por turnos.

El proyecto fue desarrollado utilizando el motor Unity y hace uso de varias técnicas de Inteligencia Artificial (IA) para controlar el comportamiento de los NPCs (personajes no jugables). Esto proporciona al juego una experiencia dinámica y desafiante, ya que los enemigos responden a las acciones del jugador de manera adaptativa.


<h2>📂 Estructura del Proyecto</h2>


<li>Assets/: Contiene todos los recursos necesarios para el juego, incluidos los scripts, gráficos, sonidos y otros elementos multimedia.</li>

<li>Scenes/: Aquí se encuentran las distintas escenas del juego, incluidas las de combate por turnos y los niveles de plataformas.</li>

<li>Scripts/: Contiene los scripts de IA y otros componentes del juego.</li>

<li>Prefabs/: Plantillas reutilizables para objetos como enemigos, plataformas y otros elementos interactivos del juego.</li>


<h2>🧠 IA Implementada</h2>

El juego emplea dos tipos principales de IA para los enemigos en el juego:

<h3>1 - IA de Enemigos en Plataformas.</h3> 

Esta IA se encarga del comportamiento de los enemigos en los niveles de plataformas. Los enemigos patrullan, persiguen y atacan al jugador de acuerdo con una serie de reglas predefinidas. Los componentes de IA utilizados son:

***Máquina de Estados Finita (FSM):***

Controla el comportamiento de los enemigos a través de varios estados como patrullar, perseguir y atacar. Los enemigos cambian de estado según las acciones del jugador y el entorno.

Implementación: Se utiliza una FSM para manejar las transiciones de estado de manera eficiente y asegurarse de que los enemigos actúen de manera predecible pero desafiante.

***Algoritmo de Búsqueda de Caminos (Pathfinding):***

Los enemigos en las plataformas utilizan el algoritmo A* para navegar por el nivel y encontrar al jugador.

Implementación: El algoritmo A* permite a los enemigos encontrar el camino óptimo hacia el jugador, evitando obstáculos y tomando decisiones de movimiento eficientes.

***Lógica Difusa (Fuzzy Logic):***

Este sistema se utiliza para ajustar el comportamiento de los enemigos dependiendo de la situación. Por ejemplo, si la vida del enemigo es baja, puede decidir huir o ser más agresivo.

Implementación: Basado en reglas lógicas que permiten decisiones más "humanas" y adaptativas según el estado del enemigo y del jugador.

<h3>2 - IA de Enemigos en Combate por Turnos.</h3>
        
En los enfrentamientos por turnos, la IA de los enemigos sigue una lógica más estratégica:

***Aprendizaje por Refuerzo (Reinforcement Learning):***

Los enemigos aprenden a través de recompensas y castigos. Cada decisión que toma el enemigo es evaluada con una recompensa que le ayuda a mejorar en futuras batallas.

Implementación: El enemigo puede aprender las mejores acciones durante el combate, evaluando si es mejor atacar, defenderse o utilizar alguna habilidad especial.

***Toma de Decisiones Basada en Heurísticas:***

Además del aprendizaje, se emplea un conjunto de heurísticas para guiar las decisiones de la IA. Por ejemplo, atacar al jugador cuando su salud es baja o aplicarle una alteración de estado si lo ve oportuno.

Implementación: Estas reglas aseguran que el enemigo actúe de manera coherente y lógica, basándose en el estado actual de la batalla.


<h2>🎯 Objetivos del Proyecto</h2>


<li>Desarrollar un videojuego 2D que combine plataformas y combate por turnos, brindando una experiencia variada al jugador.</li>

<li>Aplicar técnicas de IA para controlar el comportamiento de los enemigos, haciendo que el juego sea dinámico y adaptable a las acciones del jugador.</li>

<li>Proporcionar un sistema de combate táctico por turnos, donde cada decisión del jugador puede influir en el resultado de la batalla.</li>

<li>Crear un sistema de plataformas fluido y desafiante, utilizando IA para los enemigos que naveguen inteligentemente por el entorno.</li>


<h2>🛠️ Tecnologías Utilizadas</h2>

<li>Unity: Motor principal para el desarrollo del videojuego.</li>

<li>C#: Lenguaje de programación utilizado para los scripts y la lógica del juego.</li>

<li>ML-Agents: Herramienta utilizada para implementar la IA basada en aprendizaje por refuerzo.</li>

<li>A Pathfinding Project*: Utilizado para el algoritmo de búsqueda de caminos de los enemigos en plataformas.</li>
