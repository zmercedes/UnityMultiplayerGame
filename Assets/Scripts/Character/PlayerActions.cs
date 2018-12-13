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
	bool isAttacking = false;

	// dash info
	float dashTime = 0.2f;
	public float dashDistance = 1.5f;

	// is player currently dashing?
	bool isDashing = false;

	// weapon collider gameobject
	GameObject weaponCollider;

	// player forward direction at the start of attack/dash
	[SyncVar]
	Vector3 up;

	[SyncVar]
	Vector3 start;
	Vector3 end;

	void Awake(){
		player = transform.GetChild(0);
		weaponCollider = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
	}

	public Vector3 Up{
		get{
			return up;
		}
	}

	public bool IsDashing{
		get{
			return isDashing;
		}
	}

	public void AttackToggle(){
		if(!isAttacking)
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

	public void DashToggle(){
		if(!isDashing)
			CmdDashToggle();
	}

	[Command]
	void CmdDashToggle(){
		RpcDashToggle();
	}

	[ClientRpc]
	void RpcDashToggle(){
		StartCoroutine(Dash());
	}

	IEnumerator Attack(){
		if(isLocalPlayer)
			up = player.up;

		isAttacking = true;
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
		isAttacking = false;
		weaponCollider.SetActive(false);
	}

	IEnumerator Dash(){
		if(isLocalPlayer){
			start = transform.position;
			up = player.up;
			end = start + up*dashDistance;
		}

		isDashing = true;
		float elapsed = 0f;

		while(elapsed < dashTime){
			transform.position = Vector3.Lerp(start, end, elapsed/dashTime);
			elapsed += Time.fixedDeltaTime;
			yield return null;
		}
		
		isDashing = false;
		yield return null;
	}
}
