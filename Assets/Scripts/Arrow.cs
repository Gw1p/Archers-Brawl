 using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * This class is responsbile for arrow physics and interaction with players
 */ 
public class Arrow : NetworkBehaviour {

	// Arrow movement speed
	[SerializeField] private float speed;
	// Damage the arrow deals to players
	[SerializeField] public int damage;
	// Effect that is spawned when the arrow hits a player
	public GameObject impact;
	// PlayerShooting that instantiated the arrow
	public PlayerShooting player;
	// Can the arrow hit something? turned false when the arrow hits player/ground
	private bool canHit = true;

	/**
	 * Called every 0.02s
	 */ 
	void FixedUpdate () {
		// Move forward
		transform.localPosition += transform.right * speed * Time.deltaTime;
	}

	/**
	 * This method is called when a GameObject with Collider2D enters this object's trigger
	 * 
	 * @param col that entered the trigger
	 */ 
	public void OnTriggerEnter2D(Collider2D col){

		// Check if player entered the trigger
		if (col.CompareTag ("Player") && canHit) {
			canHit = false;

			// Deal damage to the player
			CmdHitPlayer (col.gameObject, col.transform.position); 
			StartCoroutine ("hitSomething");
		}

		// Check if the arrow hit the ground
		if (col.CompareTag ("Ground") && canHit) {
			canHit = false;
			StartCoroutine ("hitSomething");
		}

	}

	/**
	 * This coroutine is called when an arrow hits something
	 */ 
	private IEnumerator hitSomething(){
		
		// Stop arrow movement
		speed = 0;

		yield return new WaitForSeconds(1f);

		// Destroy the arrow
		Destroy (gameObject);
	}

	/**
	 * This method is called when the arrow hits a player.
	 * It dealth damage to the player and spawns particle effects
	 * 
	 * @param hit object that was hit (a player)
	 * @param pos where the arrow hit
	 */ 
	[Command]
	void CmdHitPlayer(GameObject hit, Vector3 pos){

		// Get player health
		PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth> ();

		// Spawn particles
		GameObject newExp = Instantiate (impact, pos, Quaternion.identity) as GameObject;
		// Spawn particles across the network
		NetworkServer.Spawn (newExp);

		// Validate that PlayerHealth was retrieved successfuly
		if (enemy != null) {
			
			// Check if the arrow killed the player
			if (enemy.TakeDamage (damage)) {
				// If it did, add score
				player.AddScore (player.gameObject);
			}
		}

	}

}
