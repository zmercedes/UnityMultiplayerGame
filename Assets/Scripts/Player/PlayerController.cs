﻿using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 5f;
	
	// mouse info
	Vector3 lastMousePosition;

	Vector2 input;
	Transform player;

	UnitActions unitActions;

	// camera reference for rotating to mouse
	Camera cam;

	void Awake () {
		cam = transform.GetChild(1).gameObject.GetComponent<Camera>();
		lastMousePosition = Input.mousePosition;
		player = transform.GetChild(0);
		unitActions = GetComponent<UnitActions>();
		MouseRotation();
	}

	void Update () {
		if(Input.mousePosition != lastMousePosition){

			MouseRotation();

			lastMousePosition = Input.mousePosition;
		}

		input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));

		input = input.normalized;

		if(Input.GetButtonDown("Fire1"))
			unitActions.AttackToggle();

		if(Input.GetButtonDown("Jump"))
			unitActions.DashToggle();
	}

	void FixedUpdate(){
		if(input != Vector2.zero && !unitActions.IsDashing)
			transform.Translate (input * moveSpeed *Time.fixedDeltaTime);
	}

	void MouseRotation(){
		Vector3 mousePoint = cam.ScreenToWorldPoint(lastMousePosition);
		Vector3 diff = mousePoint - transform.position;
		diff.Normalize();
		float rot_z= Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		player.rotation = Quaternion.Euler(0f,0f, rot_z-90f);
	}
}