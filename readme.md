# Bronze Royale (working title)

Project files for a unity multiplayer game implemented using UNet. 

## Features
### UI
- A user interface is attached to the network manager which hooks to server creation/joining/disconnecting, and player information, such as hp and coin count.

### Player
- PlayerController moves character in 8 directions and rotates to face mouse direction. connects to PlayerActions to activate actions on button presses.
  - WASD/directional keys to move
  - right click on mouse for attack
  - left click on mouse for dash
- PlayerSetup readies prefab by deactivating specific components that all players contain which should only be active on the local player owned prefab. 
- PlayerActions contains commands for syncing player actions across clients, such as attacking or dashing.
- CharacterInfo is a networkbehaviour that keeps track of player attributes such as health, coins, etc. it also handles displaying this information to the players. localplayers will have a healthbar and coin counter, while remote players display a smaller healthbar beneath them.
- Uses NetworkTransform and NetworkTransformChild to sync its position on the map.

### Network
- Created a custom network manager that handles server creation/joining with a UI. 
- Not destroyed on load, so keeps a canvas as a child object across all scenes. This allows the UI to have consistent access to network functions.
- Waits on server to be up and generated to spawn players.

### Procedurally Generated Map
- Generates a different map every time server starts.
- Cave like structure generated using a random seed and cellular automata.
- Biome system allows the map to have different themes, such as forest or desert biomes. (not fully implemented)
- Syncing the seed across clients assures that map will always be the same across server/clients.
- Uses coordinate system to keep track of walkable positions so that objects/players can be generated within map borders, and lets server know when its ready so players can spawn in.

### Collectibles
- A collectable object generator lives on the server and spawns coins into map once it's ready.
- Coins are local player authority, deactivate on trigger, and reactivate after 5 seconds.

## Upcoming Features
- player death/respawning
- pause menu
- projectiles
- recovery items
- armor (reduces/blocks incoming damage? regenerates?)
- multiple combat classes to choose from
- weapon/armor upgrades
- enemy npcs?
- game modes (ffa, coop level progression, etc)
- world mechanics (shops, forges, world bosses)
- polish! (better character sprites, more interesting environments, visual effects, player abilities?, etc)

### Technical Desirables
- separation of client/server
  * look into headless servers, as well as lobby servers
- implementing observer pattern for UI/other things
  * the way UI is implemented (attached to netmanager) is okay for now, but this would decouple the UI and make it function independently
- input handling
- error handling (handling disconnection and other errors)

## Issues
#### Getting player/coins to spawn after map has established reachable locations.
- solved by implementing a client startup coroutine which waits on the server to be finished generating the map. The client has to wait on several things to happen before it can spawn in a player. As such, this coroutine has 3 steps:
  1. wait on scene load. Scene will change on connecting to the server, so client waits until the online scene is loaded.
  2. wait on map spawn. Map is spawned from the server and the client must wait to receive it.
  3. generate player/coins/modify UI. 

#### Since the network manager is not destroyed on load, when a player disconnects, they load the offline scene which contains a new copy of the network manager. There must only be one network manager active at a time.
  * solved by adding a check in networkmanager.Awake() which causes copies to self destruct when more than one exists.

#### Getting weapon collider/damage to activate across clients
  * solved by creating command/rpc in PlayerSetup, and changing the name to PlayerNetworkActions

#### Getting the forward direction of character as the launch direction of weapons
  * This issue is odd because using a syncvar to update the forward direction wasn't working. i'm assuming that's because the PlayerController script is deactivated on non local players, and for whatever reason if a script updates a syncvar on a client but the same script is deactivated on other clients, the syncvar will not update. instead, i made the attack animation enumerator get the up direction in PlayerAction.

#### Making attack animation smoother across clients
  * solved by moving the attack animation enumerator to PlayerActions and making that run on all clients on attacking. This makes it so that the player prefab does not rely on network to update its rotation when attacking, making it smoother and more consistent when dealing damage.