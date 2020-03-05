
# Spelunca
A game about exploring caves and killin beasts.<br> 
In this game you explore caves and and try to survive against huge wave of monsters.<br>
Mine crystals and gather resources for better chance of survival as you go deeper and deeper in the world<br>
<br>
This is a rogue-lite so death will come sooner than you think. <br>





![alt text](https://i.imgur.com/F5XwrFB.png)
<hr>

## Controle
up      : Z<br>
down    : S<br>
left    : Q<br>
right   : D<br>
Jump    : SPACE<br>

## CHEAT
F1 : TP TO SPAWN<br>
F2 : TP TO BOSS<br>
TAB : SPAWN SPIDER<br>

## world generation
the world generation plan (branch: worldgen):

- [x] Worldgen done with marching cube algorithm
- [x] 3D noise for caverne and overhang
- [x] procedural generated mesh
- [ ] Marching cube done in compute shader
- [ ] biomes and structures
- [ ] user interaction with the terrain
- [x] frustum culling
- [ ] save and load chunk from files
- [x] cel shading for rendering
- [ ] triplanar mapping

## Gameplay programmation
Developpement roadmap for this part :
- [x] Player physics and controls
- [x] FPS view without gun clipping
- [x] Gun animations (idle, run, aim, reload)
- [x] Shooting with raycast
- [ ] Game manager (Work In Progress)
- [x] Gun magazine (actually named Gun loader but it will change)
- [x] Resource types
- [ ] Resources stock in a scriptable object singleton automatically created from existing resource types(work but there's some bugs)
- [X] Resources selection menu to charge the gun
- [ ] Gun resource gauge (There's some bugs to fix)
- [x] Player life gauge
- [x] Player hit direction marker
- [ ] Player statistics (life, stamina)
- [ ] Player can damage enemy
- [ ] Switch weapon (gun and pickaxe)
- [ ] Pickaxe animations
- [ ] Can collect resources with pickaxe
- [ ] Inventory
- [ ] Artefacts ammo per types (grenade-launcher, flame-thrower, machine gun)
- [ ] Consumables (life potion)
- [ ] Artefacts utilitary (shield with an energy gauge, compass ,rope)
- [ ] IA enemy player-like
