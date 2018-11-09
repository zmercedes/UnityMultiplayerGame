# Bronze Royale (working title)

Project files for a unity multiplayer game implemented using UNet. 

# Features
### Player
- PlayerController moves character in 8 directions and rotates to face mouse direction. it also has a basic attack animation that plays on click or pressing spacebar.
- PlayerSetup readies prefab by deactivating specific components that all players contain which should only be active on the local player owned prefab.
- CharacterInfo is a networkbehaviour that keeps track of player attributes such as health, coins, etc. it also handles displaying this information to the players. localplayers will have a healthbar and coin counter, while remote players display a smaller healthbar beneath them.
- Uses NetworkTransform and NetworkTransformChild to sync its position.

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

# Upcoming Features
- multiple classes to choose from
- player combat
- world mechanics (shops, forges, world bosses)
- game modes (ffa, coop level progression, etc)
- polish! (better character sprites, more interesting environments, visual effects, etc)

### Technical Desirables
- separation of client/server
- implementing observer pattern for UI/other things