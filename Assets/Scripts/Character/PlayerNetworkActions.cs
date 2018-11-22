using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetworkActions : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	GameObject UIPrefab;
	
	public GameObject UI;

	// player transform
	Transform player;
	// attack animation times
	float windUpTime = 0.1f;
	float spinTime = 0.1f;
	float recoveryTime = 0.2f;

	// is the player currently attacking?
	bool attacking = false;
	// main camera object
	Camera cam;

	// weapon collider gameobject
	GameObject weaponCollider;

	// player forward direction at the start of attack
	Vector3 up;

	void Start(){
		cam = Camera.main;
		if(!isLocalPlayer){
			for(int i = 0; i< componentsToDisable.Length; i++)
				componentsToDisable[i].enabled = false;
		} else {
			GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
			UI = Instantiate(UIPrefab, canvas.transform) as GameObject;
			cam.gameObject.SetActive(false);
		}
		player = transform.GetChild(0);
		weaponCollider = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
	}

	void OnDisable(){
		// Debug.Log("Disabling!");
		if(isLocalPlayer){
			if(cam != null)
				cam.gameObject.SetActive(true);
			Destroy(UI);
		}
		Destroy(this.gameObject);
	}

	public Vector3 Up{
		get{
			return up;
		}
	}

	public void AttackToggle(){
		if(!attacking)
			CmdAttackToggle();
	}

	[Command]
	void CmdAttackToggle(){
		RpcAttackToggle();
	}

	[ClientRpc]
	void RpcAttackToggle(){
		StartCoroutine(Attack());
	}

	IEnumerator Attack(){
		up = player.up;
		attacking = true;
		weaponCollider.SetActive(isLocalPlayer);
		Quaternion initial = player.rotation;
		Quaternion from = player.rotation * Quaternion.Euler(transform.forward * 30f);
		Quaternion to = player.rotation * Quaternion.Euler(transform.forward * -90f);

		float elapsed = 0f;

		while(elapsed < windUpTime){
			player.rotation = Quaternion.Slerp(initial, from, elapsed/windUpTime);
			elapsed += Time.fixedDeltaTime;
			yield return null;
		}
		elapsed = 0f;

		while(elapsed < spinTime){
			player.rotation = Quaternion.Slerp(from, to, elapsed/spinTime);
			elapsed += Time.fixedDeltaTime;
			yield return null;
		}
		elapsed = 0f;

		while(elapsed < recoveryTime){
			player.rotation = Quaternion.Slerp(to, initial, elapsed/recoveryTime);
			elapsed += Time.fixedDeltaTime;
			yield return null;
		}

		player.rotation = initial;
		attacking = false;
		weaponCollider.SetActive(false);
	}
}
