using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;

public class PlayerCanvas : NetworkBehaviour {

	public static PlayerCanvas canvas;

	[Header("Component References")]
	[SerializeField] UIfade damageImage;
	[SerializeField] Text gameStatusText;
	[SerializeField] Text healthValue;
	[SerializeField] Text killsValue;
	[SerializeField] Text logText;
	[SerializeField] AudioSource deathAudio;
	[SerializeField] Text timer;
	[SerializeField] GameObject readyButton;
	[SerializeField] GameObject unreadyButton;

	public ArchersSets[] archers = new ArchersSets[3];
	public Slider hpBar;
	public Slider killsBar;
	public Image archerImg;
	public Text archerStats;
	public GameObject menu;
	public GameObject[] startGame = new GameObject[3];

	GameObject currentPlayer;
	Player player;
	int count = 2;

	float readyTimer = 3.0f;

	void Awake()
	{
		if (canvas == null)
			canvas = this;
		else if (canvas != this)
			Destroy (gameObject);

	}

	void Start(){
		timer.gameObject.SetActive (true);
	}

	void FixedUpdate(){
		if (player) {
			if (player.playersReady && player.gameStarted == false) {
				readyTimer -= Time.deltaTime;
				timer.text = Mathf.RoundToInt (readyTimer).ToString ();
			}
		}

		if (readyTimer <= 0) {
			player.gameStarted = true;
			timer.gameObject.SetActive (false);
			CmdStartGame ();
			readyTimer = 3;
		}
	}

	[Command]
	void CmdStartGame(){
		player.StartGame ();
		RpcStartGame ();
	}

	[ClientRpc]
	void RpcStartGame(){
		player.StartGame ();
	}

	void Reset(){

		damageImage = GameObject.Find ("DamagedFlash").GetComponent<UIfade> ();
		gameStatusText = GameObject.Find ("GameStatusText").GetComponent<Text> ();
		healthValue = GameObject.Find ("HealthValue").GetComponent<Text> ();
		killsValue = GameObject.Find ("KillsValue").GetComponent<Text> ();
		logText = GameObject.Find ("LogText").GetComponent<Text> ();
		deathAudio = GameObject.Find ("DeathAudio").GetComponent<AudioSource> ();

	}

	public void Timer(float time){
		timer.text = Mathf.Floor (time / 60).ToString () + ":" + (Mathf.RoundToInt(time%60)).ToString();
	}

	public void Initialize(GameObject obj)
	{
		gameStatusText.text = "";
		currentPlayer = obj;
		player = currentPlayer.GetComponent<Player> ();
	}

	public void FlashDamageEffect()
	{
		damageImage.StartCoroutine ("Flash", 0.5f);
	}

	public void PlayDeathAudio()
	{
		if (!deathAudio.isPlaying)
			deathAudio.Play ();
	}

	public void SetKills(int amount)
	{
		killsValue.text = amount.ToString ();
		killsBar.value = amount;
	}

	public void SetHealth(int amount)
	{
		healthValue.text = amount.ToString ();
		hpBar.value = amount;
	}

	public void WriteGameStatusText(string text)
	{
		gameStatusText.text = text;
	}

	public void WriteLogText(string text, float duration)
	{
		CancelInvoke ();
		logText.text = text;
		Invoke ("ClearLogText", duration);
	}

	void ClearLogText()
	{
		logText.text = "";
	}

	public void NextArcher(){
		count++;
		if (count > 2)
			count = 0;
			
		ChangePlayer ();
	}

	public void PreviousArcher(){
		count--;
		if (count < 0)
			count = 2;

		ChangePlayer ();
	}

	void ChangePlayer(){
		archerImg.sprite = archers [count].archerImg;
		archerStats.text = "Health: " + archers [count].health + "\nDamage: " + archers [count].arrowPrefab.GetComponent<Arrow> ().damage + "\nSpeed: " + archers [count].speed + "\nJump: " + archers [count].jumpForce + "\nAttack cooldown: " + archers [count].attackCd + "\nAttack speed: " + archers [count].shootDelay;
		currentPlayer.GetComponent<CharacterSwitch> ().ChangeCharacter (count, currentPlayer);
	}

	public void Ready(){
		player.Ready ();
		readyButton.SetActive (false);
		unreadyButton.SetActive (true);
	}

	public void Unready(){
		player.Unready ();
		unreadyButton.SetActive (false);
		readyButton.SetActive (true);
	}

}