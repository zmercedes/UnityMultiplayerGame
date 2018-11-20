using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager {

	GameObject title,menu,loadText,disconnectButton,cancelButton;

	void Awake(){
		// destroys networkmanager if one is already active
		// when entering offline scene
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);

		title = transform.GetChild(0).GetChild(0).gameObject;
		menu = transform.GetChild(0).GetChild(1).gameObject;
		loadText = transform.GetChild(0).GetChild(2).gameObject;
		disconnectButton = transform.GetChild(0).GetChild(3).gameObject;
		cancelButton = transform.GetChild(0).GetChild(4).gameObject;
	}
	
	public override void OnClientConnect(NetworkConnection conn){
		StartCoroutine(ClientSpawn(conn));
	}

	public override void OnClientDisconnect(NetworkConnection conn){
		disconnectButton.SetActive(false);
		title.SetActive(true);
		menu.SetActive(true);
	}

	IEnumerator ClientSpawn(NetworkConnection conn){
		// if(isLocalPlayer)
		
		Scene currentScene = SceneManager.GetActiveScene();
		while(currentScene.name != "World"){
			currentScene = SceneManager.GetActiveScene();
			yield return null;
		}

		GameObject map = GameObject.FindWithTag("Map");
		
		while(map == null){
			map = GameObject.FindWithTag("Map");
			yield return null;
		}

		MapGen mapGen = map.GetComponent<MapGen>();

		while(!mapGen.Ready)
			yield return null;
		
		ClientScene.AddPlayer(conn, 0);
		title.SetActive(false);
		loadText.SetActive(false);
		cancelButton.SetActive(false);
		disconnectButton.SetActive(true);
		yield return null;
	}

	public void StartServerButton(){
		menu.SetActive(false);
		loadText.SetActive(true);
		networkAddress = "localhost";
		StartHost();
	}

	public void JoinServerButton(Text ipText){
		menu.SetActive(false);
		loadText.SetActive(true);
		cancelButton.SetActive(true);

		if(ipText.text != "")
			networkAddress = ipText.text;
		else
			networkAddress = "localhost";

		StartClient();
	}

	public void DisconnectButton(){
		StopHost();
		if(!IsClientConnected())
			StopServer();

		disconnectButton.SetActive(false);
		title.SetActive(true);
		menu.SetActive(true);
	}

	public void CancelButton(){
		cancelButton.SetActive(false);
		loadText.SetActive(false);
		menu.SetActive(true);

		StopClient();
	}
}
