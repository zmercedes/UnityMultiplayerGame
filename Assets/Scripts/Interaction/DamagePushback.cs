using System.Collections;
using UnityEngine;

public class DamagePushback : MonoBehaviour {

	public float pushBackValue = 10f;
	public int damageValue = 2;
	public float time = 0.25f;
	public Vector3 direction;

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Player"){
			Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
			
			direction = transform.up;
			if(gameObject.layer == 8)
				direction = transform.parent.parent.parent.gameObject.GetComponent<PlayerNetworkActions>().Up;
			
			rb.velocity = direction * pushBackValue;
			other.gameObject.GetComponent<CharacterInfo>().decreaseHealth(damageValue);
			StartCoroutine(PushBackLerp(rb));
			Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>());
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