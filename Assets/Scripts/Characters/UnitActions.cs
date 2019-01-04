using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UnitActions : NetworkBehaviour {

	protected Transform player;
	Rigidbody2D rb;

	// is the player currently attacking?
	protected bool isAttacking = false;

	// dash info
	float dashTime = 0.2f;
	Vector3 dashVelocity;
	float dashSpeed = 25f;

	// is player currently dashing?
	bool isDashing = false;

	// player forward direction at the start of attack/dash
	[SyncVar]
	protected Vector3 up;

	public virtual void Awake(){
		if(isLocalPlayer)
			gameObject.name = "Local";
		
		player = transform.GetChild(0);
		rb = GetComponent<Rigidbody2D>();
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

	public virtual IEnumerator Attack(){
		yield return null;
	}

	IEnumerator Dash(){
		if(isLocalPlayer){
			up = player.up;
			dashVelocity = up * dashSpeed;
		}

		isDashing = true;
		float elapsed = 0f;

		Vector3 initial = rb.velocity;

		while(elapsed < dashTime){
			rb.velocity = dashVelocity;
			elapsed += Time.deltaTime;
			yield return null;
		}

		rb.velocity = Vector3.zero;
		
		isDashing = false;
		yield return null;
	}
}
