using UnityEngine;

public class EdgeScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){

		if (col.CompareTag ("Player")) {
			col.GetComponent<PlayerMovement> ().CamFollow ();
		}

	}

	void OnTriggerExit2D(Collider2D col){

		if (col.CompareTag ("Player")) {

			col.GetComponent<PlayerMovement> ().CamFollow ();

		}

	}

}
