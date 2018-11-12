# Bronze Royale (working title)

Project files for a unity multiplayer game implemented using UNet. 

## Features
### Player
- PlayerController moves character in 8 directions and rotates to face mouse direction. it also has a basic attack animation that plays on click or pressing spacebar.
- PlayerSetup readies prefab by deactivating specific components that all players contain which should only be active on the local player owned prefab.
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
- multiple classes to choose from
- player combat
- world mechanics (shops, forges, world bosses)
- game modes (ffa, coop level progression, etc)
- polish! (better character sprites, more interesting environments, visual effects, etc)

### Technical Desirables
- separation of client/server
- implementing observer pattern for UI/other things
..* the way UI is implemented (attached to netmanager) is okay for now, but this would decouple the UI and make it function independently
- input handling

## Issues
1. getting player/coins to spawn after map had established reachable locations
- solved by implementing a client startup coroutine which waits on the server to be finished generating the map. The client has to wait on several things to happen before it can spawn in a player. As such, this coroutine has 4 steps:
..1. wait on scene load. Scene will change on connecting to the server, so client waits until the online scene is loaded.
..2. wait on map spawn. Map is spawned from the server and the client must wait to receive it.
..3. wait on map to be ready. Map contains a ready flag that is accessible by the client.
..4. generate player/modify UI. 
- Since the network manager is not destroyed on load, when a player disconnects, they load the offline scene which contains a new copy of the network manager. There must only be one network manager active at a time.
..* solved by adding a check in networkmanager.Awake() which causes copies to self destruct when more than one exists.