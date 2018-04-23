using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/**
 * This class is responsible for changing chraracters.
 * It contains relevant character attributes and provides methods to change them
 */ 
public class CharacterSwitch : NetworkBehaviour {

	// The sprite of the archer
	public SpriteRenderer img;
	// Movement of an archer
	public PlayerMovement mov;
	// Health of an archer
	public PlayerHealth health;
	// Shooting attributes of an archer
	public PlayerShooting shoot;
	// Archer's animator
	public Animator anim;
	// Array of all characters
	public ArchersSets[] archer = new ArchersSets[3];

	/**
	 * This method invokes command to change the character
	 * 
	 * @param num index of the archer
	 * @param obj that is going to be changed
	 */ 
	public void ChangeCharacter(int num, GameObject obj){
		CmdChange (obj, num);
	}

	/**
	 * This method changes player's stats to the one of the chosen archer (server)
	 * 
	 * @param obj that is going to be changed
	 * @param num index of the archer
	 */ 
	[Command]
	private void CmdChange(GameObject obj, int num){

		// Get character switch of the obj
		CharacterSwitch local = obj.GetComponent<CharacterSwitch> ();

		// Set appropriate fields
		local.img.sprite = archer [num].archerImg;
		local.mov.ChangeSpeed (archer [num].speed, archer [num].jumpForce);
		local.health.ChangeHealth (archer [num].health);
		local.shoot.ChangeCd (archer [num].arrowPrefab, archer [num].attackCd, archer [num].shootDelay);
		local.anim.runtimeAnimatorController = archer [num].anim;

		// Call it on all clients
		RpcChange (obj, num);
	}

	/**
	 * This method changes player's stats to the one of the chosen archer (clients)
	 * 
	 * @param obj
	 * @param num
	 */ 
	[ClientRpc]
	void RpcChange(GameObject obj, int num){

		// Get character switch of the obj
		CharacterSwitch local = obj.GetComponent<CharacterSwitch> ();

		// Set appropriate fields
		local.img.sprite = archer[num].archerImg;
		local.mov.ChangeSpeed (archer[num].speed, archer[num].jumpForce);
		local.health.ChangeHealth (archer[num].health);
		local.shoot.ChangeCd (archer[num].arrowPrefab, archer[num].attackCd, archer[num].shootDelay);
		local.anim.runtimeAnimatorController = archer [num].anim;
	}

}
