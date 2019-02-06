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
	protected Vector3 up;

	public virtual void Awake(){		
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
		if(isLocalPlayer)
			CmdUp(player.up);
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
		if(isLocalPlayer)
			CmdUp(player.up);
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
		
		dashVelocity = up * dashSpeed;
	
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

	[Command]
	public void CmdRespawn(){
		NetworkManager netManager = NetworkManager.singleton;
		Transform respawnPoint = netManager.GetStartPosition();
		// NetworkIdentity playerID =  GetComponent<NetworkIdentity>();
		NetworkServer.Destroy(gameObject);
		GameObject newPlayer = GameObject.Instantiate(netManager.playerPrefab, respawnPoint.position, respawnPoint.rotation);
		NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer, this.playerControllerId);
	}

	[Command]
	public void CmdUp(Vector3 newUp){
		RpcUp(newUp);
	}

	[ClientRpc]
	void RpcUp(Vector3 newUp){
		up = newUp;
	}
}
