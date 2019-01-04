using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager {

	GameObject localPlayer;

	public event Action clientConnected;
	public event Action clientDisconnected;
	
	void Awake(){
		// destroys networkmanager if one is already active
		// when entering offline scene
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);

		Application.targetFrameRate = 60; // possibly does not belong here
	}
	
	public override void OnClientConnect(NetworkConnection conn){
		StartCoroutine(ClientSpawn(conn));
	}

	public override void OnClientDisconnect(NetworkConnection conn){
		clientDisconnected();
	}

	IEnumerator ClientSpawn(NetworkConnection conn){
		// make sure the correct scene is loaded		
		Scene currentScene = SceneManager.GetActiveScene();
		while(currentScene.name != "World"){
			currentScene = SceneManager.GetActiveScene();
			yield return null;
		}

		// make sure the map is loaded
		GameObject map = GameObject.FindWithTag("Map");
		while(map == null){
			map = GameObject.FindWithTag("Map");
			yield return null;
		}

		// can display character select screen here?
		
		ClientScene.AddPlayer(conn, 0);
		clientConnected();
		localPlayer = GameObject.Find("Local");
		yield return null;
	}

	public void StartingServer(){
		// StartingServer UI update
		networkAddress = "localhost";
		StartHost();
	}

	public void JoinServer(Text ipText){
		
		if(ipText.text != "")
			networkAddress = ipText.text;
		else
			networkAddress = "localhost";

		StartClient();
	}

	public void Disconnect(){
		StopHost();
		if(!IsClientConnected())
			StopServer();

	}

	public void Cancel(){
		StopClient();
	}

	public void Respawn(){
		Transform respawnPoint = GetStartPosition();
		NetworkServer.UnSpawn(localPlayer);
		localPlayer.transform.position = respawnPoint.position;
		NetworkServer.Spawn(localPlayer);
	}
}
