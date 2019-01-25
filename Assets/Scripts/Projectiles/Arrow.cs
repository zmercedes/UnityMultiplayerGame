using UnityEngine;

public class Arrow : MonoBehaviour {

	Transform parent;

	void Awake() {
		parent = transform.parent;
	}

	void OnEnable(){
		transform.SetParent(null);		
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag != "Collect"){
			transform.SetParent(parent);
			gameObject.SetActive(false);
		}
	}
}
