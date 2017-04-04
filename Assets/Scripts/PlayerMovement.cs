using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	public float maxSpeed = 10f;
	bool facingRight = true;
	Rigidbody2D rgb;

	Animator anim;

	bool grounded = true;
	float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	public float jumpForce = 700;

	public bool camFollow = true;
	Transform cam;

	void Start () {

		cam = transform.FindChild ("Main Camera");
		rgb = gameObject.GetComponent<Rigidbody2D> ();
		anim = transform.FindChild("Archer").GetComponent<Animator> ();

	}
	
	void FixedUpdate () {

		//movement
		float move = Input.GetAxis ("Horizontal");

		if (move > 0 || move < 0) {

			anim.SetBool ("Walk", true);

		} else if (move == 0) {

			anim.SetBool ("Walk", false);

		}
			

		rgb.velocity = new Vector2 (move * maxSpeed, rgb.velocity.y);

		if (move > 0 && !facingRight) {

			Flip ();

		} else if (move < 0 && facingRight) {

			Flip ();

		}

		if (camFollow == true) {

			cam.transform.localPosition = new Vector3 (0, 2.5f, -12.3f);

		}

	}

	void Update(){

		if (grounded && Input.GetKeyDown (KeyCode.Space)) {

			StartCoroutine ("Jump");
		}

	}

	void Flip(){

		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

	}

	IEnumerator Jump(){

		anim.Play ("Jump");
		grounded = false;

		yield return new WaitForSeconds (0.25f);
		rgb.AddForce (new Vector2 (0, jumpForce));

		yield return new WaitForSeconds (0.55f);

		grounded = true;

	}

	public void CamFollow(){
	
		if (isLocalPlayer) {
			
			if (camFollow == true) {

				camFollow = false;

				cam.transform.parent = null;

			} else if (camFollow == false) {

				camFollow = true;

				cam.transform.parent = gameObject.transform;

			}

		}

	}

}
