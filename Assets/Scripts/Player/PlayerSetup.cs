using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;
	
	// main camera object
	Camera cam;

	void Start(){
		cam = Camera.main;
		if(!isLocalPlayer){
			for(int i = 0; i< componentsToDisable.Length; i++)
				componentsToDisable[i].enabled = false;
		} else {
			gameObject.name = "Local";
			cam.gameObject.SetActive(false);
		}
	}

	void OnDisable(){
		// Debug.Log("Disabling!");
		if(isLocalPlayer){
			if(cam != null)
				cam.gameObject.SetActive(true);
		}
		Destroy(this.gameObject);
	}
}
