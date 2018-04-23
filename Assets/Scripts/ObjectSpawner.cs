using UnityEngine;
using UnityEngine.Networking;

/**
 * This class is responsible for spawning pickups
 */ 
public class ObjectSpawner : NetworkBehaviour {

	// Cooldown of the pickup spawn
	public float aidCd;
	// Prefab of the health pickup
	public GameObject aidPrefab;
	// Spawn locations
	public GameObject[] aidSpawn = new GameObject[3];

	// Time passed since the last spawn
	private float aidTime;

	/**
	 * Called once a frame
	 */
	void Update () {

		// Increase the timer
		aidTime += Time.deltaTime;

		// Check if enough time has passed since the last spawn
		if (aidTime > aidCd) {
			
			// Randomise cooldown between 10 and 40 seconds
			aidCd = Random.Range (10f, 40f);
			// Reset timer
			aidTime = 0;

			// Get random spawn location
			int randomSpawn = Random.Range (0, aidSpawn.Length);
			// Instantiate the pickup at random spawn location
			GameObject newAid = Instantiate (aidPrefab, aidSpawn [randomSpawn].transform.position, Quaternion.identity) as GameObject;

			CmdSpawnAid (newAid);
		}

	}

	/**
	 * This method instantiates an object across the network
	 * 
	 * @param obj pickup to be spawned across the network
	 */ 
	[Command]
	void CmdSpawnAid(GameObject obj){
		NetworkServer.Spawn (obj);
	}

}
