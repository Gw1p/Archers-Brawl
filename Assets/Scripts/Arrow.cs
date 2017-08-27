 using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Arrow : NetworkBehaviour {

	[SerializeField] float speed;
	[SerializeField] public int damage;
	public GameObject impact;
	public PlayerShooting player;
	bool canHit = true;

	void FixedUpdate () {
		transform.localPosition += transform.right * speed * Time.deltaTime;
	}

	public void OnTriggerEnter2D(Collider2D col){

		if (col.CompareTag ("Player") && canHit) {
			canHit = false;
			CmdHitPlayer (col.gameObject, col.transform.position); 
			StartCoroutine ("hitPlayer");
		}

		if (col.CompareTag ("Ground") && canHit) {
			canHit = false;
			StartCoroutine ("hitPlayer");
		}

	}

	IEnumerator hitPlayer(){
		speed = 0;
		yield return new WaitForSeconds(1f);
		Destroy (gameObject);
	}

	[Command]
	void CmdHitPlayer(GameObject hit, Vector3 pos){
		PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth> ();
		GameObject newExp = Instantiate (impact, pos, Quaternion.identity) as GameObject;
		NetworkServer.Spawn (newExp);
		if (enemy != null) {
			bool wasKillShot = enemy.TakeDamage (damage);

			if (wasKillShot)
				player.AddScore (player.gameObject);
			
		}

	}

}
