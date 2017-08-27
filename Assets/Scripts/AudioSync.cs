using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioSync : NetworkBehaviour {

	public AudioSource audio;
	public AudioClip fs1;
	public AudioClip fs2;
	public AudioClip jumpStart;
	public AudioClip jumpEnd;

	[Command]
	public void CmdFootstep(){
		if (audio.isPlaying == false) {
			int randomStep = Random.Range (0, 2);
			if (randomStep == 0) {
				audio.clip = fs1;
				RpcFootstep (gameObject);
			} else {
				audio.clip = fs2;
				RpcFootstep (gameObject);
			}
			audio.Play ();
		}
	}

	[ClientRpc]
	void RpcFootstep(GameObject sourceObj){
		AudioSource source = sourceObj.GetComponent<AudioSource> ();
		int randomStep = Random.Range (0, 2);
		if (randomStep == 0)
			source.clip = fs1;
		else
			source.clip = fs2;
		
		source.Play ();
	}

	[Command]
	public void CmdJump(){
		audio.clip = jumpStart;
		audio.Play ();
		RpcJump (gameObject);
	}

	[ClientRpc]
	void RpcJump(GameObject sourceObj){
		AudioSource source = sourceObj.GetComponent<AudioSource> ();
		source.clip = jumpStart;
		source.Play ();
	}

	[Command]
	public void CmdLand(){
		audio.clip = jumpEnd;
		audio.Play ();
		RpcLand (gameObject);
	}

	[ClientRpc]
	void RpcLand(GameObject sourceObj){
		AudioSource source = sourceObj.GetComponent<AudioSource> ();
		source.clip = jumpEnd;
		source.Play ();
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.collider.gameObject.CompareTag ("Ground"))
			CmdLand ();
	}

}
