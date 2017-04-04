using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerShooting : NetworkBehaviour {

	[SerializeField] float shotCooldown = .75f;
	[SerializeField] Transform firePos;

	float ellapsedTime;
	bool canShoot;

	public GameObject arrow;

	void Start () {

		if (isLocalPlayer)
			canShoot = true;

	}
	
	void Update () {
	
		if (!canShoot)
			return;

		ellapsedTime += Time.deltaTime;

		if (Input.GetButtonDown ("Fire1") && ellapsedTime > shotCooldown) {

			ellapsedTime = 0;

			RpcShotEffect ();

		}

	}

	[ClientRpc]
	void RpcShotEffect(){

		Instantiate (arrow, firePos.position, Quaternion.identity);

	}

}
