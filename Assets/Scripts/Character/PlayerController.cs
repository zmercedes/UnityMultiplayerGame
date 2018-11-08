using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 5f;
	public float spinTime = 0.25f;
	public float windUpTime = 0.1f;
	public float recoveryTime = 0.25f;

	// mouse info
	Vector3 lastMousePosition;
	Vector2 input;
	Transform player;

	// 
	Vector3 initialPosition;
	Vector3 initialRotation;
	Vector3 spinPosition;

	Camera cam;

	bool attacking = false;

	void Awake () {
		cam = transform.GetChild(1).gameObject.GetComponent<Camera>();
		lastMousePosition = Input.mousePosition;
		player = transform.GetChild(0);
		Rotation();
	}

	// Update is called once per frame
	void Update () {
		if(Input.mousePosition != lastMousePosition){

			Rotation();

			lastMousePosition = Input.mousePosition;
		}

		input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));

		input = input.normalized;

		// if(input.x == -1)
		// 	transform.localScale = new Vector3(-1,1,1);
		// else
		// 	transform.localScale = new Vector3(1,1,1);

		// transform.Translate(input * moveSpeed * Time.fixedDeltaTime);
	}

	void FixedUpdate(){
		if((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && !attacking)
			StartCoroutine(RotateSword());
			
		transform.Translate (input * moveSpeed *Time.fixedDeltaTime);
	}

	void Rotation(){
		Vector3 mousePoint = cam.ScreenToWorldPoint(lastMousePosition);
		Vector3 diff = mousePoint - transform.position;
		diff.Normalize();
		float rot_z= Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		player.rotation = Quaternion.Euler(0f,0f, rot_z-90f);
	}

	IEnumerator RotateSword(){
		attacking = true;
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
	}

	// void OnCollisionEnter2D(Collision2D coll){
	// 	Debug.Log("colliding with " + coll.collider.name);
	// }
}