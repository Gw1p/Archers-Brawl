using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * This class is responsible for flashing an Image component by controlling the alpha
 */ 
public class UIfade : MonoBehaviour {

	void Start(){
		// Set initial alpha to 0
		gameObject.GetComponent<Image> ().canvasRenderer.SetAlpha (0.0f);
	}

	/**
	 * This coroutine increases alpha of an image from 0 to 1 and then from 1 to 0
	 * 
	 * @param duration in seconds how long will the image be flashing (actual duration is 1.5 the param, 50% duration fade in, 50% duration fade out and 50% duration delay between the two)
	 */ 
	public IEnumerator Flash(float duration){

		// Get the image component
		Image img = gameObject.GetComponent<Image> ();

		// Change alpha to 1 over half the duration
		img.CrossFadeAlpha (1f, duration / 2, false);

		// Wait so the player can see the image
		yield return new WaitForSeconds (duration / 2);

		// Fade out
		img.CrossFadeAlpha (.01f, duration / 2, false);
	}

}
