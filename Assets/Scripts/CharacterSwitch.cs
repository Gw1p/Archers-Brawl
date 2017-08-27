using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterSwitch : NetworkBehaviour {

	public SpriteRenderer img;
	public PlayerMovement mov;
	public PlayerHealth health;
	public PlayerShooting shoot;
	public Animator anim;
	public ArchersSets[] archer = new ArchersSets[3];

	public void ChangeCharacter(int num, GameObject obj){
		CmdChange (obj, num);
	}

	[Command]
	void CmdChange(GameObject obj, int num){

		CharacterSwitch local = obj.GetComponent<CharacterSwitch> ();

		local.img.sprite = archer [num].archerImg;
		local.mov.ChangeSpeed (archer [num].speed, archer [num].jumpForce);
		local.health.ChangeHealth (archer [num].health);
		local.shoot.ChangeCd (archer [num].arrowPrefab, archer [num].attackCd, archer [num].shootDelay);
		local.anim.runtimeAnimatorController = archer [num].anim;
		RpcChange (obj, num);
	}

	[ClientRpc]
	void RpcChange(GameObject obj, int num){

		CharacterSwitch local = obj.GetComponent<CharacterSwitch> ();

		local.img.sprite = archer[num].archerImg;
		local.mov.ChangeSpeed (archer[num].speed, archer[num].jumpForce);
		local.health.ChangeHealth (archer[num].health);
		local.shoot.ChangeCd (archer[num].arrowPrefab, archer[num].attackCd, archer[num].shootDelay);
		local.anim.runtimeAnimatorController = archer [num].anim;
	}

}
