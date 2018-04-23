using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Archer")]
/**
 * This scriptable object represents a character (archer)
 * It contains relevant attributes that distinguish archers
 */ 
public class ArchersSets : ScriptableObject {

	// Archer's sprite
	public Sprite archerImg;
	// Animator controller to play archer animations
	public RuntimeAnimatorController anim;
	// Archer's health
	public int health;
	// Movement speed
	public float speed;
	// Prefab of the arrow
	public GameObject arrowPrefab;
	// How high does the archer jump
	public float jumpForce;
	// Attack cooldown
	public float attackCd;
	// Time it takes after the player pressed 'fire' to shoot out an arrow
	public float shootDelay;

}
