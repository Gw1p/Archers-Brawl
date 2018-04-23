using UnityEngine;
using UnityEngine.Networking;

/**
 * This class is responsible for player health and all relevant data manipulations.
 * Methods for adding and removing health are provided.
 */ 
public class PlayerHealth : NetworkBehaviour {

	// Max health of a player
	[SerializeField] int maxHealth = 100;

	// Synchronized variable with a hook to know when health changes
	// Actual player health
	[SyncVar (hook = "OnHealthChanged")] private int health;

	// AudioSource attached to the player
	public AudioSource audio;
	// Hit sound
	public AudioClip hit;
	// Death sound
	public AudioClip death;

	// Player script that is attached to a player
	private Player player;
	// NetworkAnimator to control animation across the network
	private NetworkAnimator anim;

	/**
	 * This method is called when PlayerHealth.cs component is enabled
	 */
	[ServerCallback]
	void OnEnable()
	{
		// Set actual health to max health
		health = maxHealth;
	}

	/**
	 * This method is called once when this component is loaded in the scene
	 */ 
	void Start(){
		// Get relevant components
		anim = gameObject.transform.GetComponent<NetworkAnimator> ();
		player = gameObject.transform.GetComponent<Player> ();

		// Set actual health to max health
		health = maxHealth;
	}

	/**
	 * This method reduces player's health by a given amount and returns if the player was killed.
	 * 
	 * @param damage that the player takes
	 * @return has the player died?
	 */ 
	[Server]
	public bool TakeDamage(int damage)
	{
		// Local variable to check if the player has died
		bool died = false;

		// If health is already less than or equal to 0, the player is already dead
		if (health <= 0)
			return died;

		// Reduce health
		health -= damage;
		died = health <= 0;

		// Set health to 0 if the player has died
		if (died)
			health = 0;

		// Send a call to all clients
		RpcTakeDamage (died);

		// Return if the player has died
		return died;
	}

	/**
	 * This method is called on all clients.
	 * It displays appropriate UI and plays relevant audio to give the user feedback.
	 */ 
	[ClientRpc]
	private void RpcTakeDamage(bool died){

		// Check if the player is local (if the user is the same one that received the damage)
		if (isLocalPlayer) {
			// Play a flash on canvas to let the player know that she received damage
			PlayerCanvas.canvas.FlashDamageEffect ();
		}

		// Set appropriate clip of an AudioSource and play it
		audio.clip = hit;
		audio.Play ();

		// Play relevant animation
		anim.animator.SetTrigger ("Hit");

		// Check if the player died
		if (died) {
			// Play appropriate audio and animation
			audio.clip = death;
			audio.Play ();	
			player.Die ();
		}
	}

	/**
	 * This method is called when health of a player changes
	 * 
	 * @param value the player's health
	 */ 
	private void OnHealthChanged(int value){

		// Update health
		health = value;

		// Check if this is the player that received health
		if (isLocalPlayer) {
			if (PlayerCanvas.canvas) {
				// Update health UI
				PlayerCanvas.canvas.SetHealth (value);
			}
		}
	}

	/**
	 * This method adds health to a player.
	 * 
	 * @param hp to be added to the player
	 */ 
	[Server]
	public void AddHealth(int hp){

		// Increase player health
		health += hp;

		// Cap current health by the max health
		if (health > maxHealth)
			health = maxHealth;
	}

	/**
	 * This method updates player's health, max health, and relevant UI.
	 * it is called when the player is switching characters.
	 */ 
	public void ChangeHealth(int newHp){
		// Update health and maxHealth
		maxHealth = newHp;
		health = maxHealth;

		// Update relevant UI
		PlayerCanvas.canvas.hpBar.maxValue = maxHealth;
		PlayerCanvas.canvas.hpBar.value = maxHealth;
	}
}
