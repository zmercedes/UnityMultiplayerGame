using System.Collections; 		// IEnumerator
using UnityEngine;				
using UnityEngine.Networking;	// NetworkBehaviour

public class Coin : NetworkBehaviour {

	public int RespawnTime = 5;

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			GetComponent<CircleCollider2D>().enabled = false;
			transform.GetChild(0).gameObject.SetActive(false);
			CmdCoinToggle(false);
			StartCoroutine(Respawn());	
		}
	}

	IEnumerator Respawn(){
		yield return new WaitForSeconds(RespawnTime);
		GetComponent<CircleCollider2D>().enabled = true;
		transform.GetChild(0).gameObject.SetActive(true);
		CmdCoinToggle(true);
	}

	// sends command to the server that the coin needs to be turned off/on
	// called by client local objects
	[Command]
	void CmdCoinToggle(bool toggle){
		RpcToggle(toggle);
	}

	// sends rpc from server to all clients to toggle their coinds off/on
	// must be sent from command to be called by a client local object
	[ClientRpc]
	void RpcToggle(bool toggle){
		GetComponent<CircleCollider2D>().enabled = toggle;
		transform.GetChild(0).gameObject.SetActive(toggle);
	}
}