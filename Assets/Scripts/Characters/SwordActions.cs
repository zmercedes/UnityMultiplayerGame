using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SwordActions : UnitActions {

	// weapon collider gameobject
	GameObject weaponCollider;

	// Swing animation times
	float windUpTime = 0.12f;
	float spinTime = 0.12f;
	float recoveryTime = 0.25f;

	public override void Awake() {
		base.Awake();
		weaponCollider = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
	}

	public override IEnumerator Attack(){

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
}