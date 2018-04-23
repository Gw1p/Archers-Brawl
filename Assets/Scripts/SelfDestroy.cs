using System.Collections;
using UnityEngine;

/**
 * This class is used to destroy a gameObject after a delay
 */ 
public class SelfDestroy : MonoBehaviour {

	// Time in seconds after which gameObject will be destroyed
	public float deathDelay;

	void Start () {
		// Start death coroutine
		StartCoroutine ("death");
	}
	

	/**
	 * This coroutine waits a number of seconds and then calls Destroy on gameObject
	 */ 
	private IEnumerator death(){

		// Wait for the delay
		yield return new WaitForSeconds (deathDelay);

		// Destroy the gameObject
		Destroy (gameObject);
	}
}
