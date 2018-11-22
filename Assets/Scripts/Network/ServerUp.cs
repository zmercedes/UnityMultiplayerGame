using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerUp : NetworkBehaviour {

	public GameObject MapPrefab;
	public Collectables collectables;
	GameObject map;

	public override void OnStartServer(){
		foreach(Transform child in transform)
			child.position = Vector3.zero;

		StartCoroutine(UpSequence());
	}

	IEnumerator UpSequence(){
		map = Instantiate(MapPrefab) as GameObject;
		MapGen mapGen = map.GetComponent<MapGen>();
		mapGen.SetSeed();
		map.SetActive(true);
		while(!mapGen.Ready)
			yield return null;

		// moves spawn locations within map borders
		foreach(Transform child in transform){
			int position = UnityEngine.Random.Range(0,mapGen.WalkableTiles.Count -1);
			child.position = Coord.ToWorldPoint(mapGen.WalkableTiles[position]);
		}
		NetworkServer.Spawn(map);

		// send in walkable tiles to generate collectibles inside the map
		collectables.GenerateCollectables(mapGen.WalkableTiles);
		yield return null;
	}
}
