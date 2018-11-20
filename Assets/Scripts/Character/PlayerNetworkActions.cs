using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkActions : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	GameObject UIPrefab;
	
	public GameObject UI;

	// main camera object
	Camera cam;

	// weapon collider gameobject
	GameObject weaponCollider;

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

	public void AttackToggle(bool flag){
		CmdAttackToggle(flag);
	}

	[Command]
	void CmdAttackToggle(bool flag){
		RpcAttackToggle(flag);
	}

	[ClientRpc]
	void RpcAttackToggle(bool flag){
		weaponCollider.SetActive(flag);
	}
}
