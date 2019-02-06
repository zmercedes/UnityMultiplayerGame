using System;
using System.Collections;
using UnityEngine;

public class UIController : MonoBehaviour {

	// delegates

	GameObject title,menu,loadText,disconnectButton,cancelButton,info,death,characterSelect;

	void Awake() {
		if (FindObjectsOfType(GetType()).Length > 1)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		title = transform.GetChild(0).gameObject;
		menu = transform.GetChild(1).gameObject;
		loadText = transform.GetChild(2).gameObject;
		disconnectButton = transform.GetChild(3).gameObject;
		cancelButton = transform.GetChild(4).gameObject;
		info = transform.GetChild(5).gameObject;
		death = transform.GetChild(6).gameObject;
		characterSelect = transform.GetChild(7).gameObject;
	}
	
	public void ClientConnected(){
		title.SetActive(false);
		loadText.SetActive(false);
		cancelButton.SetActive(false);
		disconnectButton.SetActive(true);
	}

	public void CharacterSelect(){
		title.SetActive(false);
		loadText.SetActive(false);
		characterSelect.SetActive(true);
	}

	public void DefaultState(){
		title.SetActive(true);
		menu.SetActive(true);
		loadText.SetActive(false);
		disconnectButton.SetActive(false);
		cancelButton.SetActive(false);
		info.SetActive(false);
		death.SetActive(false);
		characterSelect.SetActive(false);
	}

	public void CloseCharacterSelect(){
		characterSelect.SetActive(false);
		title.SetActive(true);
		loadText.SetActive(true);
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
		info.SetActive(false);
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
