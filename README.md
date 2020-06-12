
# Spelunca
A game about exploring caves and killin beasts.<br> 
In this game you explore caves and and try to survive against huge wave of monsters.<br>
Mine crystals and gather resources for better chance of survival as you go deeper and deeper in the world<br>
<br>
This is a rogue-lite so death will come sooner than you think. <br>

![alt text](https://i.imgur.com/xvhHypW.png)
<hr>

## Controle
up      : Z<br>
down    : S<br>
left    : Q<br>
right   : D<br>
Jump    : SPACE<br>
T       : Chat<br>
1,2,3   : switch item<br>
X       : Scanner<br>
E       : Interacte<br>
Tab     : list of minerals<br>
Mouse 1 : Shoot<br>
Mouse 2 : Aim<br>
Scroll  : graplin hook<br>

## CHEAT

press T to open the chatbox and type /help

## world generation

- [x] Worldgen done with marching cube algorithm
- [x] 3D noise for caverne and overhang
- [x] procedural generated mesh
- [x] Marching cube done in compute shader
- [x] biomes and structures
- [ ] user interaction with the terrain
- [x] frustum culling
- [ ] save and load chunk from files
- [x] cel shading for rendering
- [x] triplanar mapping

## Scoring

- [x] website displaying the score
- [x] login in game
- [x] timing based score
- [x] killing based score

## Gameplay programmation
- [x] Scanner to detect ores
- [x] Player physics and controls
- [x] FPS view without gun clipping
- [x] Gun animations (idle, run, aim, reload)
- [x] Shooting with raycast
- [x] Game manager 
- [x] Gun magazine
- [x] Resource types
- [x] Resources stock in a scriptable object singleton automatically created from existing resource types(work but there's some bugs)
- [X] Resources selection menu to charge the gun
- [x] Gun resource gauge (There's some bugs to fix)
- [x] Player life gauge
- [x] Player hit direction marker
- [x] Player statistics (life, stamina)
- [x] Player can damage enemy
- [x] Switch weapon (gun and pickaxe)
- [x] Pickaxe animations
- [x] Can collect resources with pickaxe
- [x] Inventory
- [x] Artefacts ammo per types (grenade-launcher, flame-thrower, machine gun)
- [x] Consumables (life potion)
- [ ] Artefacts utilitary (shield with an energy gauge, compass ,rope)
- [ ] IA enemy player-like
