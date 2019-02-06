using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterInfo : NetworkBehaviour {
	public int maxHealth = 10;
	
	SimpleHealthBar healthBar;

	GameObject InfoUI;
	GameObject DeathUI;

	Text coinText;
	Text healthText;

	int coinCounter=0;
	int health;

	void Start(){
		health = maxHealth;
		if(isLocalPlayer){
			GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
			InfoUI = canvas.transform.GetChild(5).gameObject;
			InfoUI.SetActive(true);
			DeathUI = canvas.transform.GetChild(6).gameObject;
			coinText = InfoUI.transform.GetChild(1).GetComponent<Text>();
			coinText.text = "Coins: 0";
			healthBar = InfoUI.transform.GetChild(2).GetChild(0).gameObject.GetComponent<SimpleHealthBar>();
			healthText = InfoUI.transform.GetChild(3).GetComponent<Text>();
			healthText.text = health.ToString() + " / " + maxHealth.ToString();
		} else {
			healthBar = transform.GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<SimpleHealthBar>();
			transform.GetChild(2).gameObject.SetActive(true);
		}

		healthBar.UpdateBar(health, maxHealth);
	}

	void DecreaseHealth (int number){
		health -= number;
		if (health <= 0){
			health = 0;
			OnDeath();
		}

		healthBar.UpdateBar(health, maxHealth);
	}

	void IncreaseHealth (int number){
		health += number;
		if (health > maxHealth)
			health = maxHealth;

		healthBar.UpdateBar(health, maxHealth);
	}

	// health decrease functions
	public void HealthDecrease(int number){
		CmdHealthDecrease(number);
	}

	[Command]
	void CmdHealthDecrease(int number){
		RpcHealthDecrease(number);
	}

	[ClientRpc]
	void RpcHealthDecrease(int number){
		DecreaseHealth(number);
	}

	// health increase functions
	public void HealthIncrease(int number){
		CmdHealthIncrease(number);
	}

	[Command]
	void CmdHealthIncrease(int number){
		RpcHealthIncrease(number);
	}
	
	[ClientRpc]
	void RpcHealthIncrease(int number){
		IncreaseHealth(number);
	}

	// calls death ui screen
	void OnDeath(){
		// call death ui and deactivate movement
		// maybe set tombstone in the place player died
		if(isLocalPlayer){
			GetComponent<PlayerController>().enabled = false;
			DeathUI.SetActive(true);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (isLocalPlayer && col.tag == "Collect") {
			coinCounter++;
			coinText.text = "Coins: " + coinCounter.ToString();
		}
	}
}