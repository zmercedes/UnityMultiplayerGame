using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerActions : NetworkBehaviour {

	Transform player;

	// attack animation times
	float windUpTime = 0.12f;
	float spinTime = 0.12f;
	float recoveryTime = 0.25f;

	// is the player currently attacking?
	bool attacking = false;

	// weapon collider gameobject
	GameObject weaponCollider;

	// player forward direction at the start of attack/dash
	[SyncVar]
	Vector3 up;

	void Awake(){
		player = transform.GetChild(0);
		weaponCollider = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
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
		weaponCollider.SetActive(!isLocalPlayer);
		Quaternion initial = player.rotation;
		Quaternion from = player.rotation * Quaternion.Euler(transform.forward * 30f);
		Quaternion to = player.rotation * Quaternion.Euler(transform.forward * -120f);

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

	IEnumerator Dash(){
		// stub
		yield return null;
	}
}
