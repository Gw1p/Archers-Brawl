using UnityEngine;
using UnityEngine.Networking;

public class Aid : NetworkBehaviour {

	public int health;
	public GameObject death;

	PlayerHealth player;

	void OnTriggerEnter2D(Collider2D col){

		if (col.CompareTag("Player")){
			CmdHealth (col.gameObject, health, gameObject.transform.position);
			Destroy (gameObject);
		}

	}

	[Command]
	void CmdHealth(GameObject player, int hp, Vector3 pos){
		GameObject destroyParticle = Instantiate (death, pos, Quaternion.identity) as GameObject;
		NetworkServer.Spawn (destroyParticle);
		player.GetComponent<PlayerHealth>().AddHealth(hp);
	}
	
}
