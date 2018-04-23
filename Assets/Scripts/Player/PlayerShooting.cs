using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

/**
 * This class is responsible for player shooting.
 * It contains attributes for damage, cooldown, and provides methods for shooting.
 */ 
public class PlayerShooting : NetworkBehaviour {

	// Cooldown of a shot
	[SerializeField] float shotCooldown = .75f;
	// Number of kills needed to win
	[SerializeField] int killsToWin = 3;
	// Position from which arrows are being spawned
	[SerializeField] Transform firePos;
	// Delay between when a player presses fire and the arrow is spawned
	[SerializeField] float attackDelay;

	// How many other players did the player kill
	[SyncVar (hook = "OnScoreChanged")] int score;

	// AudioSource used for shooting sounds
	public AudioSource audio;
	// Clip for the bow sound
	public AudioClip bowClip;
	// The arrow prefab
	public GameObject arrow;
	// Particle effects for when the arrow is spawned
	public GameObject arrowSpawn;

	// Movement of the player
	private PlayerMovement mov;
	// The player script
	private Player player;
	// Animator to play animations
	private NetworkAnimator anim;
	// Time since the player shot
	private float ellapsedTime;
	// Can the player shoot?
	private bool canShoot;

	void Start () {	

		// Get relevant components
		player = gameObject.GetComponent<Player> ();
		anim = GetComponent<NetworkAnimator> ();
		mov = GetComponent<PlayerMovement> ();

		// Check if this is a local player
		if (isLocalPlayer) {
			// Enable shooting
			canShoot = true;
		}
	}

	/**
	 * Called once a frame
	 */ 
	void Update () {

		// Return if the player can't shoot
		if (!canShoot)
			return;

		// Add to the elapsed time
		ellapsedTime += Time.deltaTime;

		// If the player pressed fire and it has been more time since the player shot last time than the cooldown, shoot
		if (Input.GetButtonDown ("Fire1") && ellapsedTime > shotCooldown) {
			// Play animation
			anim.animator.SetTrigger ("Attack1");
			// Reset ellapsed time
			ellapsedTime = 0;

			// Set audio clip and play
			audio.clip = bowClip;
			audio.Play ();

			// Instantiate arrow
			StartCoroutine ("shootingDelay");
		}

	}

	/**
	 * 
	 * @param arrowRight is arrow facing right?
	 * @param shoot containing PlayerShooting
	 */ 
	[Command]
	private void CmdShoot(bool arrowRight, GameObject shoot){

		// Instantiate arrow
		GameObject newArrow = Instantiate (arrow, firePos.position, transform.rotation) as GameObject;

		// Rotate the arrow left if needed
		if (arrowRight == false) {
			newArrow.transform.Rotate (0, 180, 0);
		}

		// Set player of an arrow
		newArrow.GetComponent<Arrow> ().player = shoot.GetComponent<PlayerShooting>();
		// Instantiate particle effects
		GameObject newSpawn = Instantiate (arrowSpawn, firePos.position, Quaternion.identity);

		// Spawn particle effects and the arrow across the network
		NetworkServer.Spawn (newArrow);
		NetworkServer.Spawn (newSpawn);
	}

	/**
	 * Player movement is stopped until an arrow is fired
	 */ 
	private IEnumerator shootingDelay(){
		
		// Stop player movement
		mov.speed = 0;

		// Wait for attack delay
		yield return new WaitForSeconds (attackDelay);

		// Shoot an arrow
		CmdShoot (mov.facingRight, gameObject);

		// Wait for the animation to play out
		yield return new WaitForSeconds (attackDelay / 2);

		// Resume movement
		mov.speed = mov.maxSpeed;
	}

	/**
	 * This method adds score to a player when she kills another player
	 * 
	 * @param thePlayer that killed another player
	 */ 
	[Server]
	public void AddScore(GameObject thePlayer){

		// Increase the score
		score++;

		// Check if the player has won
		if (score >= killsToWin) {
			thePlayer.GetComponent<Player> ().Won ();
		}
	}

	/**
	 * This method is called whenever player score changes
	 * 
	 * @param value new score
	 */ 
	private void OnScoreChanged(int value){

		// Assign the score
		score = value;

		// Update local player's UI
		if (isLocalPlayer) {
			PlayerCanvas.canvas.SetKills (value);
		}
	}	

	/**
	 * This method is called to change cooldown of a player when selecting different archers
	 * 
	 * @param newArrow the arrow prefab
	 * @param newCd new cooldown of a shot
	 * @param newAttackDelay of the archer 
	 */ 
	public void ChangeCd(GameObject newArrow, float newCd, float newAttackDelay){
		arrow = newArrow;
		shotCooldown = newCd;
		attackDelay = newAttackDelay;
	}

}