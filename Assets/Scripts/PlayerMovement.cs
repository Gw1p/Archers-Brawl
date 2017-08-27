using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	public float speed;
	public float maxSpeed = 10f;
	public bool facingRight = true;
	public Vector3 camPos;
	public GameObject playerCanvas;
	public AudioSync audio;

	private float startTime;
	private float journeyLength = 0.01f;

	Rigidbody2D rgb;

	NetworkAnimator anim;

	bool grounded = true;
	float groundRadius = 1.2f;
	public float jumpForce = 700;

	public bool camFollow = true;
	Transform cam;
	float distance;
	bool jumping = false;

	void Start () {
		cam = transform.Find ("Main Camera");
		cam.transform.parent = null;
		rgb = gameObject.GetComponent<Rigidbody2D> ();
		anim = GetComponent<NetworkAnimator> ();
		speed = maxSpeed;
	}
	
	void FixedUpdate () {

		if (!isLocalPlayer)
			return;

		RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, Mathf.Infinity, 1<<8);
		if (hit.collider != null) {
			distance = Mathf.Abs (hit.point.y - transform.position.y);
		}

		grounded = distance <= groundRadius;

		float move = Input.GetAxis ("Horizontal");

		if (move > 0 || move < 0) {
			anim.animator.SetBool ("Walk", true);

			if (grounded)
				audio.CmdFootstep ();

		} else if (move == 0) {
			anim.animator.SetBool ("Walk", false);
		}

		rgb.velocity = new Vector2 (move * speed, rgb.velocity.y);

		if (facingRight && move < 0) {
			facingRight = false;
			CmdFlipLeft ();
		}

		if (!facingRight && move > 0) {
			facingRight = true;		
			CmdFlipRight ();
		}

		if (camFollow == true) {
			camPos.x = gameObject.transform.position.x;
			float distCovered = (Time.time - startTime) * 0.01f;
			float fracJourney = distCovered / journeyLength;
			cam.transform.position = Vector3.Lerp (cam.transform.position, camPos, fracJourney);
		}

	}

	void Update(){

		if (grounded && Input.GetKeyDown (KeyCode.Space) && jumping == false) {
			StartCoroutine ("Jump");
		}

		if (jumping) {
			if (grounded == false) {
				jumping = false;
			}
		}

	}

	[Command]
	void CmdFlipLeft(){

		Vector3 theScale = transform.localScale;
		theScale.x = -0.6f;

		Vector3 canvasScale = transform.localScale;
		canvasScale.x = -0.005724837f;
		canvasScale.y = 0.005724837f;
		canvasScale.z = 0.005724837f;

		RpcFlip (theScale, canvasScale);

	}

	[Command]
	void CmdFlipRight(){

		Vector3 theScale = transform.localScale;
		theScale.x = 0.6f;

		Vector3 canvasScale = transform.localScale;
		canvasScale.x = 0.005724837f; 
		canvasScale.y = 0.005724837f;
		canvasScale.z = 0.005724837f;

		RpcFlip (theScale, canvasScale);

	}

	[ClientRpc]
	void RpcFlip(Vector3 newScale, Vector3 newCanvasScale){

		transform.localScale = newScale;
		playerCanvas.transform.localScale = newCanvasScale;

	}

	IEnumerator Jump(){
		anim.animator.Play ("Jump");
		jumping = true;
		yield return new WaitForSeconds (0.25f);
		audio.CmdJump ();
		rgb.AddForce (new Vector2 (0, jumpForce));
	}

	public void CamFollow(){
	
		if (isLocalPlayer) {
			
			if (camFollow == true) {

				camFollow = false;

				cam.transform.parent = null;

			} else if (camFollow == false) {

				camFollow = true;
				startTime = Time.time;
				//cam.transform.parent = gameObject.transform;

			}

		}

	}

	public void ChangeSpeed(float speed, float jumpF){
		maxSpeed = speed;
		jumpForce = jumpF;
	}

}
