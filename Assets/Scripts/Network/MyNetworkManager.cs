using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager {

	public event Action clientConnected;
	public event Action clientDisconnected;
	public event Action showCharacterSelect;

	[SerializeField]
	GameObject[] classes;

	ClassMessage classMessage;
	bool classSelected = false;
	public GameObject selectedClass;
	
	void Awake(){
		// destroys networkmanager if one is already active
		// when entering offline scene
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);

		Application.targetFrameRate = 60; // possibly does not belong here
	}

	public class ClassMessage : MessageBase {
		public int chosenClass;
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader){

		ClassMessage message = extraMessageReader.ReadMessage<ClassMessage>();
		selectedClass = classes[message.chosenClass];

		GameObject player;

		Transform startPos = GetStartPosition();

		Vector3 pos = startPos != null ? startPos.position : Vector3.zero;

		player = Instantiate(selectedClass, pos, Quaternion.identity) as GameObject;

		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
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
		showCharacterSelect();
		while(!classSelected)
			yield return null;

		ClientScene.AddPlayer(conn, 0, classMessage);
		clientConnected();

		yield return null;
	}

	public void StartingServer(){
		// StartingServer UI update
		networkAddress = "localhost";
		StartHost();
	}

	public void JoinServer(Text ipText){
		
		networkAddress = ipText.text != "" ? ipText.text : "localhost";

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

	public void SelectClass(int choice){
		classMessage = new ClassMessage();
		classMessage.chosenClass = choice;
		classSelected = true;
	}
}
