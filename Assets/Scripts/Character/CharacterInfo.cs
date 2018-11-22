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

	void DecreaseHealth (int number){
		health -= number;
		if (health < 0)
			health = 0;

		healthBar.UpdateBar( health, maxHealth );
	}

	void IncreaseHealth (int number){
		health += number;
		if (health > maxHealth)
			health = maxHealth;

		healthBar.UpdateBar( health, maxHealth );
	}

	public void HealthDecrease(int number){
		CmdHealthDecrease(number);
	}

	public void HealtIncrease(int number){
		CmdHealthIncrease(number);
	}

	[Command]
	void CmdHealthDecrease(int number){
		RpcHealthDecrease(number);
	}

	[ClientRpc]
	void RpcHealthDecrease(int number){
		DecreaseHealth(number);
	}

	[Command]
	void CmdHealthIncrease(int number){
		RpcHealthIncrease(number);
	}

	[ClientRpc]
	void RpcHealthIncrease(int number){
		IncreaseHealth(number);
	}

	void OnTriggerEnter2D(Collider2D col){
		if (isLocalPlayer && col.tag == "Collect") {
			coinCounter++;
			coinText.text = "Coins: " + coinCounter.ToString();
		}
	}
}