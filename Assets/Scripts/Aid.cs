using UnityEngine;
using UnityEngine.Networking;

/**
 * This component adds health to a player when it is picked up
 */
public class Aid : NetworkBehaviour {

	// Health amount that the player will receive
	public int health;
	// Prefab that is spawned when aid is picked up
	public GameObject death;

	/**
	 * This method is called when a GameObject enters Trigger that is attached to the same object Aid.cs is attached to
	 * 
	 * @param col that entered the Trigger
	 */ 
	private void OnTriggerEnter2D(Collider2D col){

		// Check if a player entered the trigger area
		if (col.CompareTag("Player")){
			// Add health to the player and spawn death particles
			CmdHealth (col.gameObject, health, gameObject.transform.position);
			// Destroy gameObject
			Destroy (gameObject);
		}

	}

	/**
	 * This method adds health to a player and spawns death particles
	 * 
	 * @param player to add health to
	 * @param pos where to instantiate death particles
	 */ 
	[Command]
	private void CmdHealth(GameObject player, int hp, Vector3 pos){
		
		// Instantiate death particles
		GameObject destroyParticle = Instantiate (death, pos, Quaternion.identity) as GameObject;
		// Display death particles across all users
		NetworkServer.Spawn (destroyParticle);
		// Add health
		player.GetComponent<PlayerHealth>().AddHealth(hp);
	}
	
}
