using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// attack rotation info
	public float moveSpeed = 5f;
	public float spinTime = 0.25f;
	public float windUpTime = 0.1f;
	public float recoveryTime = 0.25f;
	Vector3 initialPosition;
	Vector3 initialRotation;
	Vector3 spinPosition;
	
	// mouse info
	Vector3 lastMousePosition;

	Vector2 input;
	Transform player;

	PlayerNetworkActions netActions;

	// camera reference for rotating to mouse
	Camera cam;

	bool attacking = false;

	void Awake () {
		cam = transform.GetChild(1).gameObject.GetComponent<Camera>();
		lastMousePosition = Input.mousePosition;
		player = transform.GetChild(0);
		netActions = GetComponent<PlayerNetworkActions>();
		MouseRotation();
	}

	// Update is called once per frame
	void Update () {
		if(Input.mousePosition != lastMousePosition){

			MouseRotation();

			lastMousePosition = Input.mousePosition;
		}

		input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));

		input = input.normalized;
	}

	void FixedUpdate(){
		if((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && !attacking)
			StartCoroutine(Attack());

		if(input != Vector2.zero)
			transform.Translate (input * moveSpeed *Time.fixedDeltaTime);
	}

	void MouseRotation(){
		Vector3 mousePoint = cam.ScreenToWorldPoint(lastMousePosition);
		Vector3 diff = mousePoint - transform.position;
		diff.Normalize();
		float rot_z= Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		player.rotation = Quaternion.Euler(0f,0f, rot_z-90f);
	}

	IEnumerator Attack(){
		attacking = true;
		netActions.AttackToggle(attacking);
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
		netActions.AttackToggle(attacking);
	}

	// void OnCollisionEnter2D(Collision2D coll){
	// 	Debug.Log("colliding with " + coll.collider.name);
	// }
}