using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArcherActions : UnitActions {

	[SerializeField]
	GameObject arrow;
	[SerializeField]
	float arrowSpeed = 20f;

	ObjectPool pool;
	Vector3 arrowPosition;

	public override void Awake() {
		base.Awake();

		pool = new ObjectPool(arrow, 10, transform);
	}

	public override IEnumerator Attack(){
		isAttacking = true;
		arrowPosition = player.GetChild(0).GetChild(1).position;
		
		GameObject currentArrow = pool.Next();
		Physics2D.IgnoreCollision(currentArrow.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		currentArrow.SetActive(true);

		currentArrow.transform.up = up;
		currentArrow.transform.position = arrowPosition;

		currentArrow.GetComponent<Rigidbody2D>().velocity = up * arrowSpeed;

		yield return new WaitForSeconds(1f);
		isAttacking = false;
		yield return null;
	}

	void OnDisable(){
		pool = null;
	}
}