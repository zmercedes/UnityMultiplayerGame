using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	GameObject UIPrefab;
	
	public GameObject UI;

	Camera cam;

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
}
