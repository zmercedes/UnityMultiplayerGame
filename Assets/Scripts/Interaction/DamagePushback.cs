using System.Collections;
using UnityEngine;

public class DamagePushback : MonoBehaviour {

	public float pushBackValue = 10f;
	public int damageValue = 2;
	public float time = 0.25f;
	public Transform guide;

	void OnCollisionEnter2D(Collision2D other){
		if(other.gameObject.tag == "Player"){
			Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
			Vector3 direction = (guide == null) ? transform.up : guide.up;
			rb.velocity = direction * pushBackValue;
			other.gameObject.GetComponent<CharacterInfo>().decreaseHealth(damageValue);
			StartCoroutine(PushBackLerp(rb));
		}
	}

	IEnumerator PushBackLerp(Rigidbody2D rb){
		float elapsed = 0f;
		Vector3 startV = rb.velocity;
		while(elapsed < time){
			rb.velocity = Vector3.Lerp(startV, Vector3.zero,elapsed/time);
			elapsed += Time.deltaTime;
			yield return null;
		}
		rb.velocity = Vector2.zero;
		yield return null;
	}
}