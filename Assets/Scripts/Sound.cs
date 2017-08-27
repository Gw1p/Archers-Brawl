 using UnityEngine;
using UnityEngine.UI;

public class Sound : MonoBehaviour {

	public MusicObject values;
	public AudioSource music;
	public Slider musicSliderLobby;
	public Slider volumeSlider;

	void Start () {
		Application.runInBackground = true;
		if (PlayerPrefs.HasKey ("musicValue")) {
			values.music = PlayerPrefs.GetFloat ("musicValue");
			values.volume = PlayerPrefs.GetFloat ("volumeValue");
		}

		if (name == "PlayerCanvas") {
			music = GameObject.Find ("LobbyManager").GetComponent<AudioSource> ();
			volumeSlider.value = values.volume;
			AudioListener.volume = values.volume;
		}
		musicSliderLobby.value = values.music;
		music.volume = values.music;
	}

	public void ChangeMusicLobby(){
		music.volume = musicSliderLobby.value;
		values.music = musicSliderLobby.value;
	}

	public void ChangeVolumeGame(){
		AudioListener.volume = volumeSlider.value;
		values.volume = volumeSlider.value;
	}

	void OnApplicationQuit(){
		PlayerPrefs.SetFloat ("musicValue", values.music);
		PlayerPrefs.SetFloat ("volumeValue", values.volume);
	}

}
