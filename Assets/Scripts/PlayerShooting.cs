using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

public class PlayerShooting : NetworkBehaviour {

	[SerializeField] float shotCooldown = .75f;
	[SerializeField] int killsToWin = 3;
	[SerializeField] Transform firePos;
	[SerializeField] float attackDelay;

	[SyncVar (hook = "OnScoreChanged")] int score;

	public AudioSource audio;
	public AudioClip bowClip;

	float ellapsedTime;
	bool canShoot;

	public GameObject arrow;
	public GameObject arrowSpawn;

	PlayerMovement mov;
	Player player;
	NetworkAnimator anim;

	void Start () {	
		
		player = gameObject.GetComponent<Player> ();
		anim = GetComponent<NetworkAnimator> ();
		mov = GetComponent<PlayerMovement> ();

		if (isLocalPlayer)
			canShoot = true;

	}
	
	void Update () {

		if (!canShoot)
			return;

		ellapsedTime += Time.deltaTime;

		if (Input.GetButtonDown ("Fire1") && ellapsedTime > shotCooldown) {
			anim.animator.SetTrigger ("Attack1");
			ellapsedTime = 0;
			audio.clip = bowClip;
			audio.Play ();
			StartCoroutine ("shootingDelay");
		}

	}

	[Command]
	void CmdShoot(bool arrowRight, GameObject shoot){
		GameObject newArrow = Instantiate (arrow, firePos.position, transform.rotation) as GameObject;
		if (arrowRight == false)
			newArrow.transform.Rotate (0, 180, 0);
		
		newArrow.GetComponent<Arrow> ().player = shoot.GetComponent<PlayerShooting>();
		GameObject newSpawn = Instantiate (arrowSpawn, firePos.position, Quaternion.identity);
		NetworkServer.Spawn (newArrow);
		NetworkServer.Spawn (newSpawn);
	}

	IEnumerator shootingDelay(){
		mov.speed = 0;
		yield return new WaitForSeconds (attackDelay);
		CmdShoot (mov.facingRight, gameObject);
		yield return new WaitForSeconds (attackDelay / 2);
		mov.speed = mov.maxSpeed;
	}

	[Server]
	public void AddScore(GameObject thePlayer){
		score++;
		if (score >= killsToWin)
			thePlayer.GetComponent<Player>().Won ();

	}

	void OnScoreChanged(int value){

		score = value;

		if (isLocalPlayer)
			PlayerCanvas.canvas.SetKills (value);
	}	

	public void ChangeCd(GameObject newArrow, float newCd, float newCdDelay){
		arrow = newArrow;
		shotCooldown = newCd;
		attackDelay = newCdDelay;
	}

}