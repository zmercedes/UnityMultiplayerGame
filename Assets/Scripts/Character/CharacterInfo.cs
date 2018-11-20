using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterInfo : NetworkBehaviour {
	public int maxHealth = 10;
	
	SimpleHealthBar healthBar;

	GameObject UI;

	Text coinText;

	int coinCounter=0;
	int health;

	void Start (){
		if(isLocalPlayer){
			UI = GetComponent<PlayerNetworkActions>().UI;
			coinText = UI.transform.GetChild(1).GetComponent<Text>();
			healthBar = UI.transform.GetChild(2).GetChild(0).gameObject.GetComponent<SimpleHealthBar>();
		} else {
			healthBar = transform.GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<SimpleHealthBar>();
			transform.GetChild(2).gameObject.SetActive(true);
		}

		health = maxHealth;
	}

	public void decreaseHealth (int number){
		if(isLocalPlayer){
			health -= number;
			if (health < 0)
				health = 0;

			healthBar.UpdateBar( health, maxHealth );
			CmdHealthUpdate(health);
		}
	}

	public void increaseHealth (int number){
		if(isLocalPlayer){
			health += number;
			if (health > maxHealth)
				health = maxHealth;
	
			healthBar.UpdateBar( health, maxHealth );
			CmdHealthUpdate(health);
		}
	}

	[Command]
	void CmdHealthUpdate(int newHealth){
		RpcHealthUpdate(newHealth);
	}

	[ClientRpc]
	void RpcHealthUpdate(int newHealth){
		health = newHealth;
		healthBar.UpdateBar(newHealth, maxHealth);
	}

	void OnTriggerEnter2D(Collider2D col){
		if (isLocalPlayer && col.tag == "Collect") {
			coinCounter++;
			coinText.text = "Coins: " + coinCounter.ToString();
		}
	}
}