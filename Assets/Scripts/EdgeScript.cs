using UnityEngine;

/**
 * This class controls camera movement as player reaches the end of a level.
 */ 
public class EdgeScript : MonoBehaviour {

	/**
	 * This method stops camera from following a player when one enters a trigger
	 */ 
	void OnTriggerEnter2D(Collider2D col){

		if (col.CompareTag ("Player")) {
			col.GetComponent<PlayerMovement> ().CamFollow ();
		}

	}

	/**
	 * This method resumes camera from following a player when one enters a trigger
	 */ 
	void OnTriggerExit2D(Collider2D col){

		if (col.CompareTag ("Player")) {
			col.GetComponent<PlayerMovement> ().CamFollow ();
		}

	}

}
