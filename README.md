<h1><em>📜 2D Video Game with AI Applied to NPCs</em></h1> <p align="left"> <img src="https://img.shields.io/badge/STATUS-IN%20DEVELOPMENT-green"> </p> <h2>🎮 General Description</h2>
This is a 2D video game that combines two classic genres: platformer and turn-based combat. The main objective is for the player to advance through different levels, facing AI-controlled enemies that present a challenge both in the platforming environment and in strategic turn-based combat.

The project was developed using the Unity engine and utilizes several Artificial Intelligence (AI) techniques to control the behavior of NPCs (non-playable characters). This provides the game with a dynamic and challenging experience, as enemies adapt to the player’s actions.

<h2>📂 Project Structure</h2> <li>Assets/: Contains all necessary resources for the game, including scripts, graphics, sounds, and other multimedia elements.</li> <li>Scenes/: Contains the different game scenes, including turn-based combat and platformer levels.</li> <li>Scripts/: Contains the AI scripts and other game components.</li> <li>Prefabs/: Reusable templates for objects like enemies, platforms, and other interactive game elements.</li> <h2>🧠 Implemented AI</h2>
The game employs two main types of AI for the enemies:

<h3>1 - Platformer Enemy AI</h3>
This AI manages the behavior of enemies in platformer levels. Enemies patrol, chase, and attack the player according to a set of predefined rules. The AI components used are:

Finite State Machine (FSM):

Controls enemy behavior through various states like patrolling, chasing, and running away. Enemies switch between states based on the player’s position and the enemy´s current health.

Implementation: An FSM is used to efficiently manage state transitions and ensure that enemies behave in a predictable but challenging way.

Pathfinding Algorithm:

Enemies in platformer levels use the A* algorithm to navigate the level and find the player.

Implementation: The A* algorithm allows enemies to find the optimal path to the player, avoiding obstacles and making efficient movement decisions.

Fuzzy Logic:

This system is used to adjust enemy behavior depending on the situation. The enemy speed is going to be updated depending on the player´s current health and the enemy´s current health. This will make the enemy to move slower when, for example, his current health is low. 

Implementation: Based on logical rules that allow for more "human-like" and adaptive decisions depending on the state of the enemy and the player.

<h3>2 - Turn-Based Combat Enemy AI</h3>
In turn-based encounters, the enemy AI follows a more strategic logic:

Reinforcement Learning:

Enemies learn through rewards and punishments. Each decision made by the enemy is evaluated with a reward that helps it improve in future battles.

Implementation: The enemy learns the best actions during combat, evaluating whether to attack, defend, or use a special ability.

Heuristic-Based Decision Making:

In addition to learning, a set of heuristics is employed to guide the AI’s decisions. For example, attacking the player when their health is low or applying a status effect when deemed appropriate.

Implementation: These rules ensure that the enemy acts in a consistent and logical manner based on the current state of the battle.

<h2>🎯 Project Objectives</h2> <li>Develop a 2D video game that combines platforming and turn-based combat, offering a varied experience to the player.</li> <li>Apply AI techniques to control enemy behavior, making the game dynamic and adaptable to the player’s actions.</li> <li>Provide a tactical turn-based combat system, where every decision the player makes can influence the outcome of the battle.</li> <li>Create a fluid and challenging platformer system, using AI to allow enemies to intelligently navigate the environment.</li> <h2>🛠️ Technologies Used</h2> <li>Unity: Main engine for video game development.</li> <li>C#: Programming language used for scripts and game logic.</li> <li>ML-Agents: Tool used to implement AI based on reinforcement learning.</li> <li>A* Pathfinding Project: Used for the pathfinding algorithm of the platformer enemies.</li>