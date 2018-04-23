using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/**
 * This class is responsible for all player movement
 */ 
public class PlayerMovement : NetworkBehaviour {

	// Current speed of a player
	public float speed;
	// Max speed of a player
	public float maxSpeed = 10f;
	// Is the player facing right? (if false - the player is facing left)
	public bool facingRight = true;
	// Camera offset
	public Vector3 camPos;
	// Player canvas to display name, color, etc
	public GameObject playerCanvas;
	// AudioSync used for playing audio (steps, jumps, etc)
	public AudioSync audio;
	// Force that is applied upwards to jump
	public float jumpForce = 700;
	// Should the camera follow the player?
	public bool camFollow = true;

	// When the camera started following the player
	private float startTime;
	// Used to position camera
	private float journeyLength = 0.01f;
	// Rigibody of a player
	private Rigidbody2D rgb;
	// Used to play animations
	private NetworkAnimator anim;
	// Is the player on the ground?
	private bool grounded = true;
	// How far under the player is considered to be the ground
	private float groundRadius = 1.2f;
	// Camera transform
	private Transform cam;
	// How far is the ground beneath the player
	private float distance;
	// Is the player jumping?
	private bool jumping = false;

	/**
	 * This method is called once when the component is loaded in the scene
	 */ 
	void Start () {
		// Get camera's transform
		cam = transform.Find ("Main Camera");
		// Set parent to nothing
		cam.transform.parent = null;

		// Get relevant components
		rgb = gameObject.GetComponent<Rigidbody2D> ();
		anim = GetComponent<NetworkAnimator> ();

		// Set actual speed to maximum speed
		speed = maxSpeed;
	}

	/**
	 * This method is called once every 0.02s 
	 */
	void FixedUpdate () {

		// If not a local player - return
		if (!isLocalPlayer)
			return;

		// Raycast downwards
		RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, Mathf.Infinity, 1<<8);
		if (hit.collider != null) {
			// Get hit distance
			distance = Mathf.Abs (hit.point.y - transform.position.y);
		}

		// Check if the player is on the ground
		grounded = distance <= groundRadius;

		// Access horizontal axis
		float move = Input.GetAxis ("Horizontal");

		// If input is not null, the player is moving
		if (move > 0 || move < 0) {
			// Play relevant animation
			anim.animator.SetBool ("Walk", true);

			// If the player is on the ground, play footstep sound
			if (grounded) {
				audio.CmdFootstep ();
			}

		} else if (move == 0) {
			// Stop animation
			anim.animator.SetBool ("Walk", false);
		}

		// Set rigibody velocity based on the horizontal axis
		rgb.velocity = new Vector2 (move * speed, rgb.velocity.y);

		// If the player is facing right and moving left, flip
		if (facingRight && move < 0) {
			facingRight = false;
			CmdFlipLeft ();
		}

		// If the player is facing left and moving right, flip
		if (!facingRight && move > 0) {
			facingRight = true;		
			CmdFlipRight ();
		}

		// If camera is following, transform its position
		if (camFollow == true) {
			camPos.x = gameObject.transform.position.x;
			float distCovered = (Time.time - startTime) * 0.01f;
			float fracJourney = distCovered / journeyLength;
			cam.transform.position = Vector3.Lerp (cam.transform.position, camPos, fracJourney);
		}

	}

	/**
	 * This method is called every frame
	 */ 
	void Update(){

		// If the player is grounded and pressed Space, jump
		if (grounded && Input.GetKeyDown (KeyCode.Space) && jumping == false) {
			StartCoroutine ("Jump");
		}

		// If the player has jumped and left the ground, set jumping to false
		if (jumping) {
			if (grounded == false) {
				jumping = false;
			}
		}

	}

	/**
	 * This method changes the scale of the object so that the sprite appears to be facing left
	 */ 
	[Command]
	private void CmdFlipLeft(){

		Vector3 theScale = transform.localScale;
		theScale.x = -0.6f;

		Vector3 canvasScale = transform.localScale;
		canvasScale.x = -0.005724837f;
		canvasScale.y = 0.005724837f;
		canvasScale.z = 0.005724837f;

		RpcFlip (theScale, canvasScale);

	}

	/**
	 * This method changes the scale of the object so that the sprite appears to be facing right
	 */ 
	[Command]
	private void CmdFlipRight(){

		Vector3 theScale = transform.localScale;
		theScale.x = 0.6f;

		Vector3 canvasScale = transform.localScale;
		canvasScale.x = 0.005724837f; 
		canvasScale.y = 0.005724837f;
		canvasScale.z = 0.005724837f;

		RpcFlip (theScale, canvasScale);

	}

	/**
	 * This method changes the scale of the gameObject and relevant player canvas
	 * 
	 * @param newScale of the player
	 * @param newCanvasScale of the player canvas
	 */ 
	[ClientRpc]
	private void RpcFlip(Vector3 newScale, Vector3 newCanvasScale){

		transform.localScale = newScale;
		playerCanvas.transform.localScale = newCanvasScale;

	}

	/**
	 * This coroutine is responsible for making the player jump 
	 */ 
	private IEnumerator Jump(){
		
		// Play animation
		anim.animator.Play ("Jump");
		jumping = true;

		// Wait for animation to play out
		yield return new WaitForSeconds (0.25f);

		// Play jump sound
		audio.CmdJump ();
		// Add appropriate force to the rigibody
		rgb.AddForce (new Vector2 (0, jumpForce));
	}

	/**
	 * This method toggles camera follow
	 */ 
	public void CamFollow(){
		
		// Check if its local player
		if (isLocalPlayer) {
			// If camera is already following, stop
			if (camFollow) {
				
				camFollow = false;
				cam.transform.parent = null;

			} else { // Camera is not following, start
				
				camFollow = true;
				startTime = Time.time;

			}
		}
	}

	/**
	 * This method sets the speed and jump force of a player
	 * 
	 * @param speed new maxSpeed
	 * @param jumpF jumpForce of a player
	 */ 
	public void ChangeSpeed(float speed, float jumpF){
		maxSpeed = speed;
		jumpForce = jumpF;
	}

}
