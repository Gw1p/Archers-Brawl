using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * This class makes sure that audio is played on all clients at the same time.
 */ 
public class AudioSync : NetworkBehaviour {

	// AudioSource that is playing player sounds related to movement
	public AudioSource audio;

	// Sounds for footsteps
	[Header ( " Step Sounds " )]
	public AudioClip fs1;
	public AudioClip fs2;

	// Sounds for jump
	[Header ( " Jump Sounds " )]
	public AudioClip jumpStart;
	public AudioClip jumpEnd;

	/**
	 * This method plays footstep sound if audio is not playing already 
	 * and calls another method to play a footstep on all other clients.
	 */ 
	[Command]
	public void CmdFootstep(){

		// Check if audio is playing already
		if (audio.isPlaying == false) {
			
			// Get random step
			int randomStep = Random.Range (0, 2);
			if (randomStep == 0) {
				audio.clip = fs1;
			} else {
				audio.clip = fs2;
			}

			// Play step on all clients
			RpcFootstep (gameObject);

			// Play step
			audio.Play ();
		}
	}

	/**
	 * This method plays a footstep sound on all clients.
	 * 
	 * @param sourceObj that initiated sound
	 */ 
	[ClientRpc]
	private void RpcFootstep(GameObject sourceObj){
		
		// Get AudioSource of the initiating object
		AudioSource source = sourceObj.GetComponent<AudioSource> ();

		// Get random step
		int randomStep = Random.Range (0, 2);
		if (randomStep == 0) {
			source.clip = fs1;
		} else {
			source.clip = fs2;
		}

		// Play step
		source.Play ();
	}

	/**
	 * This method is called when a player jumps.
	 * It plays the jump sound and calls another method to play jump sound on all clients.
	 */ 
	[Command]
	public void CmdJump(){

		// Assign correct audio clip and play audio
		audio.clip = jumpStart;
		audio.Play ();

		// Play jump sound on all clients
		RpcJump (gameObject);
	}

	/**
	 * This method plays jump sound on all clients.
	 * 
	 * @param sourceObj that initiated the jump sound
	 */ 
	[ClientRpc]
	private void RpcJump(GameObject sourceObj){

		// Get AudioSource of the initiating object
		AudioSource source = sourceObj.GetComponent<AudioSource> ();

		// Assign correct audio clip and play
		source.clip = jumpStart;
		source.Play ();
	}

	/**
	 * This method is called when a player lands.
	 * It plays landing sound and calls a relevant function to play landing sound on all clients
	 */ 
	[Command]
	public void CmdLand(){
		
		// Assign relevant clip and play audio
		audio.clip = jumpEnd;
		audio.Play ();

		// Play landing sound on all clients
		RpcLand (gameObject);
	}

	/**
	 * This method plays landing at a particular point on all clients.
	 * 
	 * @param sourceObj that initiated landing sound
	 */ 
	[ClientRpc]
	private void RpcLand(GameObject sourceObj){

		// Get audio source of the player that has landed
		AudioSource source = sourceObj.GetComponent<AudioSource> ();

		// Set relevant audio clip and play
		source.clip = jumpEnd;
		source.Play ();
	}

	/**
	 * This method is called when an object with a Collider2D component enters a trigger that is attached to gameObject 
	 * 
	 * @param col that entered the trigger
	 */ 
	private void OnCollisionEnter2D(Collision2D col){
		
		// Check if the object is Ground
		if (col.collider.gameObject.CompareTag ("Ground")) {
			// Play landing sound
			CmdLand ();
		}
	}

}
