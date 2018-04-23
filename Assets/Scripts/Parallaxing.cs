using UnityEngine;

/**
 * This class is responsible for scaling the background to add a parallax effect to the game
 */ 
public class Parallaxing : MonoBehaviour {

	// Backgrounds to be transformed
	public Transform[] backgrounds;
	// How quick do the backgrounds change position (0-1)
	public float smoothing = 1f;

	// Main camera
	private Transform cam;
	// Camera position in the last frame
	private Vector3 previousCamPos;
	// How much should the backgrounds change their position
	private float[] parallaxScales;

	void Start () {

		// Get camera transform
		cam = Camera.main.transform;
		// Get initial camera position
		previousCamPos = cam.position;
		// Instantiate array according to the number of backgrounds
		parallaxScales = new float[backgrounds.Length];

		// Loop through each background and set scales based on their position
		for (int i = 0; i < backgrounds.Length; i++) {
			parallaxScales [i] = backgrounds [i].position.z * -1;
		}

	}

	/**
	 * Called once a frame
	 */ 
	void Update () {

		float parallax = 0;
		float backgroundTargetPosX = 0;
		Vector3 backgroundTargetPos;

		// Loop through each background
		for (int i = 0; i < backgrounds.Length; i++) {

			// Parallax = difference in camera position * the scale that was calculated based on the initial position of the background
			parallax = (previousCamPos.x - cam.position.x) * parallaxScales [i];
			// Calculate new position on the X axis based on the current position and calculated parallax
			backgroundTargetPosX = backgrounds [i].position.x + parallax;
			// Set target position with the new X
			backgroundTargetPos = new Vector3 (backgroundTargetPosX, backgrounds [i].position.y, backgrounds [i].position.z);
			// Update background's position
			backgrounds [i].position = Vector3.Lerp (backgrounds [i].position, backgroundTargetPos, smoothing * Time.deltaTime);
		}
	
		// Update camera position
		previousCamPos = cam.position;
	}
}
