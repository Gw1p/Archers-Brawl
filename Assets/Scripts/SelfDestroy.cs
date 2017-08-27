using System.Collections;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {

	public float deathDelay;

	void Start () {

		StartCoroutine ("death");
		
	}
	

	IEnumerator death(){

		yield return new WaitForSeconds (deathDelay);

		Destroy (gameObject);

	}

}
