using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIfade : MonoBehaviour {

	void Start(){

		gameObject.GetComponent<Image> ().canvasRenderer.SetAlpha (0.0f);

	}

	public IEnumerator Flash(float duration){

		gameObject.GetComponent<Image> ().CrossFadeAlpha (1f, duration / 2, false);

		yield return new WaitForSeconds (duration / 2);

		gameObject.GetComponent<Image> ().CrossFadeAlpha (.01f, duration / 2, false);

	}

}
