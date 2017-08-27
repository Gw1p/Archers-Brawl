using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Archer")]
public class ArchersSets : ScriptableObject {

	public Sprite archerImg;
	public RuntimeAnimatorController anim;
	public int health;
	public float speed;
	public GameObject arrowPrefab;
	public float jumpForce;
	public float attackCd;
	public float shootDelay;

}
