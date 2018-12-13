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
		// moves spawn locations within map borders
		foreach(Transform child in transform){
			int position = Random.Range(0,mapGen.WalkableTiles.Count -1);
			while(occupied.Contains(mapGen.WalkableTiles[position]))
				position = Random.Range(0,mapGen.WalkableTiles.Count -1);

			child.position = Coord.ToWorldPoint(mapGen.WalkableTiles[position]);
			occupied.Add(mapGen.WalkableTiles[position]);
		}

		// initializing coins
		for (int i = 0; i <numberOfCollectables; i ++){
			int randomTile = Random.Range(0, mapGen.WalkableTiles.Count -1);

			while(occupied.Contains(mapGen.WalkableTiles[randomTile]))
				randomTile = Random.Range(0, mapGen.WalkableTiles.Count-1);
			
			Vector3 position = Coord.ToWorldPoint(mapGen.WalkableTiles[randomTile]);
			collectable = Instantiate (collectablePrefab, position, Quaternion.identity, transform); 
			occupied.Add(mapGen.WalkableTiles[randomTile]);
			// spawn coin across all clients
			NetworkServer.Spawn(collectable);
		}

		NetworkServer.Spawn(map);
		yield return null;
	}
}
