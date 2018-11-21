﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkActions : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	GameObject UIPrefab;
	
	public GameObject UI;

	Vector3 up;

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

	public Vector3 Up{
		get{
			return up;
		}
	}

	public void AttackToggle(bool flag, Vector3 direction){
		CmdAttackToggle(flag, direction);
	}

	[Command]
	void CmdAttackToggle(bool flag, Vector3 direction){
		RpcAttackToggle(flag, direction);
	}

	[ClientRpc]
	void RpcAttackToggle(bool flag, Vector3 direction){
		weaponCollider.SetActive(flag);
		up = direction;
	}
}
