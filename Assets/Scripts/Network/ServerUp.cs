using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ServerUp : NetworkBehaviour {

	public GameObject MapPrefab;
	GameObject map;

	[SerializeField]
	GameObject collectablePrefab;

	GameObject collectable;

	// how many coins? 
	[SerializeField]
	int numberOfCollectables;

	public override void OnStartServer(){
		StartCoroutine(UpSequence());
	}

	IEnumerator UpSequence(){
		map = Instantiate(MapPrefab) as GameObject;
		MapGen mapGen = map.GetComponent<MapGen>();
		mapGen.SetSeed();
		map.SetActive(true);
		while(!mapGen.Ready)
			yield return null;

		HashSet<Coord> occupied = new HashSet<Coord>();
		List<List<Coord>> rooms = mapGen.RoomCoordinates;
		// moves spawn locations within map borders
		foreach(Transform child in transform){
			int randomRoom = Random.Range(0,rooms.Count-1);
			int position = Random.Range(0,rooms[randomRoom].Count -1);
			while(occupied.Contains(rooms[randomRoom][position]))
				position = Random.Range(0,rooms[randomRoom].Count -1);

			child.position = Coord.ToWorldPoint(rooms[randomRoom][position]);
			occupied.Add(rooms[randomRoom][position]);
		}

		// initializing coins
		List<Coord> coords = mapGen.WalkableCoordinates;
		for (int i = 0; i <numberOfCollectables; i++){

			int randomTile = Random.Range(0, coords.Count -1);

			while(occupied.Contains(coords[randomTile]))
				randomTile = Random.Range(0, coords.Count-1);
			
			Vector3 position = Coord.ToWorldPoint(coords[randomTile]);
			collectable = Instantiate (collectablePrefab, position, Quaternion.identity, transform); 
			occupied.Add(coords[randomTile]);
			// spawn coin across all clients
			NetworkServer.Spawn(collectable);
		}

		NetworkServer.Spawn(map);
		yield return null;
	}
}
