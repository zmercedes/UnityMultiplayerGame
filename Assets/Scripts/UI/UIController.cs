using System;
using System.Collections;
using UnityEngine;

public class UIController : MonoBehaviour {

	// delegates

	GameObject title,menu,loadText,disconnectButton,cancelButton,death;

	void Awake() {
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		MyNetworkManager netManager = GameObject.FindGameObjectWithTag("NetManager").GetComponent<MyNetworkManager>();
		netManager.clientConnected += ClientConnected;
		netManager.clientDisconnected += ClientDisconnected;

		title = transform.GetChild(0).gameObject;
		menu = transform.GetChild(1).gameObject;
		loadText = transform.GetChild(2).gameObject;
		disconnectButton = transform.GetChild(3).gameObject;
		cancelButton = transform.GetChild(4).gameObject;
		death = transform.GetChild(6).gameObject;
		
	}
	
	void ClientConnected(){
		title.SetActive(false);
		loadText.SetActive(false);
		cancelButton.SetActive(false);
		disconnectButton.SetActive(true);
	}

	void ClientDisconnected(){
		disconnectButton.SetActive(false);
		title.SetActive(true);
		menu.SetActive(true);
	}

	public void StartServerButton(){
		menu.SetActive(false);
		loadText.SetActive(true);
	}

	public void JoinServerButton(){
		menu.SetActive(false);
		loadText.SetActive(true);
		cancelButton.SetActive(true);
	}

	public void DisconnectButton(){
		disconnectButton.SetActive(false);
		title.SetActive(true);
		menu.SetActive(true);
	}

	public void CancelButton(){
		cancelButton.SetActive(false);
		loadText.SetActive(false);
		menu.SetActive(true);
	}

	public void RespawnButton(){
		GameObject player = GameObject.Find("Local");
		while(player == null)
			player = GameObject.Find("Local");
		
		player.GetComponent<UnitActions>().CmdRespawn();
		death.SetActive(false);
	}
}
