using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Collectables : NetworkBehaviour {

	[SerializeField]
	GameObject collectablePrefab;

	GameObject collectable;

	// how many coins? 
	[SerializeField]
	int numberOfCollectables;

	// generates collectibles
	public void GenerateCollectables(List<Coord> tiles){
		HashSet<Coord> occupied = new HashSet<Coord>();
		for (int i = 0; i <numberOfCollectables; i ++){
			int randomTile = Random.Range(0,tiles.Count-1);
			while(occupied.Contains(tiles[randomTile])){
				randomTile = Random.Range(0,tiles.Count-1);
			}
			Vector3 position = Coord.ToWorldPoint(tiles[randomTile]);
			collectable = Instantiate (collectablePrefab, position, Quaternion.identity, transform); 
			occupied.Add(tiles[randomTile]);
			// spawn coins across all clients
			NetworkServer.Spawn(collectable);
		}
	}
}