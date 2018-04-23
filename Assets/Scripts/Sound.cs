using UnityEngine;
using UnityEngine.UI;

/**
 * This class is responsible for managing background sounds and overall volume of the game.
 */ 
public class Sound : MonoBehaviour {

	// ScriptableObject to hold values for music and volume between different scenes
	public MusicObject values;
	// AudioSource that plays background music
	public AudioSource music;
	// Slider that controls music volume
	public Slider musicSliderLobby;
	// Slider that controls overall game volume
	public Slider volumeSlider;

	/**
	 * Called once when the component is loaded in the scene
	 * Sets volume of the music and overall game
	 */
	void Start () {
		// Run application when application window is not focused
		Application.runInBackground = true;
		// Check if PlayerPrefs contain volume values
		if (PlayerPrefs.HasKey ("musicValue")) {
			// Retrieve Music and Volume saved in the previous session
			values.music = PlayerPrefs.GetFloat ("musicValue");
			values.volume = PlayerPrefs.GetFloat ("volumeValue");
		}

		// Check if this is a player canvas
		if (name == "PlayerCanvas") {
			// Find AudioSource that plays background music
			music = GameObject.Find ("LobbyManager").GetComponent<AudioSource> ();
			// Set background music and overall game volumes
			volumeSlider.value = values.volume;
			AudioListener.volume = values.volume;
		}

		// Set slider values
		musicSliderLobby.value = values.music;
		music.volume = values.music;
	}

	/**
	 * Set background music volume
	 */ 
	public void ChangeMusicLobby(){
		music.volume = musicSliderLobby.value;
		values.music = musicSliderLobby.value;
	}

	/**
	 * Set overall game volume
	 */ 
	public void ChangeVolumeGame(){
		AudioListener.volume = volumeSlider.value;
		values.volume = volumeSlider.value;
	}

	/**
	 * Called when application is being closed
	 */ 
	void OnApplicationQuit(){
		PlayerPrefs.SetFloat ("musicValue", values.music);
		PlayerPrefs.SetFloat ("volumeValue", values.volume);
	}

}
