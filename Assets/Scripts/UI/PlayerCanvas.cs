using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;

/**
 * This class is responsible for all UI interactions.
 * It displays relevant information on player canvas, such as score, health, character selection and more
 */ 
public class PlayerCanvas : NetworkBehaviour {

	// Static canvas to access from other classes
	public static PlayerCanvas canvas;

	[Header("Component References")]
	// Damage image that flashes when the player takes damage
	[SerializeField] UIfade damageImage;
	// Game status (Won/Defeat)
	[SerializeField] Text gameStatusText;
	// Player health
	[SerializeField] Text healthValue;
	// How many kills did the player get
	[SerializeField] Text killsValue;
	// Used for debugging
	[SerializeField] Text logText;
	// AudioSource that plays death sound
	[SerializeField] AudioSource deathAudio;
	// Countdown mefore the match starts
	[SerializeField] Text timer;
	// Ready to play button
	[SerializeField] GameObject readyButton;
	// Unready button
	[SerializeField] GameObject unreadyButton;

	// Characters
	public ArchersSets[] archers = new ArchersSets[3];
	// Playaer health bar
	public Slider hpBar;
	// Player kills bar
	public Slider killsBar;
	// Sprite of the archer
	public Image archerImg;
	// Character stats
	public Text archerStats;
	// Menu GameObject
	public GameObject menu;
	public GameObject[] startGame = new GameObject[3];

	// The player that is currently selected
	private GameObject currentPlayer;
	// The player
	private Player player;
	// Character selection count
	private int count = 2;
	// Countdown timer
	private float readyTimer = 3.0f;

	/**
	 * Thie method assigns singleton instance
	 */ 
	void Awake()
	{
		// Check if canvas is null and assign static instance
		if (canvas == null) {
			canvas = this;
		} else if (canvas != this) {
			Destroy (gameObject);
		}
	}

	/**
	 * This method enables the timer
	 */ 
	void Start(){
		timer.gameObject.SetActive (true);
	}

	/**
	 * Called once every 0.02s
	 */ 
	void FixedUpdate(){

		// Check if we have the player reference
		if (player) {
			// check that player is ready and game has not started already
			if (player.playersReady && player.gameStarted == false) {
				
				// Start timer/countdown
				readyTimer -= Time.deltaTime;
				timer.text = Mathf.RoundToInt (readyTimer).ToString ();
			}
		}

		// If ready timer is less than 0, start the game
		if (readyTimer <= 0) {
			player.gameStarted = true;
			timer.gameObject.SetActive (false);
			CmdStartGame ();
			readyTimer = 3;
		}
	}

	/**
	 * This method is called when starting a new game
	 */ 
	[Command]
	private void CmdStartGame(){
		player.StartGame ();
		RpcStartGame ();
	}

	/**
	 * Start game on all clients
	 */ 
	[ClientRpc]
	private void RpcStartGame(){
		player.StartGame ();
	}

	/**
	 * This method gets finds relevant GameObjects and accesses components that are needed for this script
	 */ 
	private void Reset(){

		damageImage = GameObject.Find ("DamagedFlash").GetComponent<UIfade> ();
		gameStatusText = GameObject.Find ("GameStatusText").GetComponent<Text> ();
		healthValue = GameObject.Find ("HealthValue").GetComponent<Text> ();
		killsValue = GameObject.Find ("KillsValue").GetComponent<Text> ();
		logText = GameObject.Find ("LogText").GetComponent<Text> ();
		deathAudio = GameObject.Find ("DeathAudio").GetComponent<AudioSource> ();

	}

	/**
	 * This method sets timer text to time in format mm:ss
	 * 
	 * @param time to be converted to time text
	 */ 
	public void Timer(float time){
		timer.text = Mathf.Floor (time / 60).ToString () + ":" + (Mathf.RoundToInt(time%60)).ToString();
	}

	/**
	 * This method initializes player canvas
	 * 
	 * @param obj player who owns this player canvas
	 */ 
	public void Initialize(GameObject obj)
	{
		gameStatusText.text = "";
		currentPlayer = obj;
		player = currentPlayer.GetComponent<Player> ();
	}

	/**
	 * This method initiates damage flash.
	 * It is called whenever the player receives damage
	 */ 
	public void FlashDamageEffect()
	{
		damageImage.StartCoroutine ("Flash", 0.5f);
	}

	/**
	 * This method is called when the player dies and death audio is needed to be played
	 */
	public void PlayDeathAudio()
	{
		// Check that the death audio is not playing already
		if (!deathAudio.isPlaying) {
			deathAudio.Play ();
		}
	}

	/**
	 * This method updates kills slider and appropriate text to display for the player
	 * 
	 * @param amount of kills
	 */ 
	public void SetKills(int amount)
	{
		killsValue.text = amount.ToString ();
		killsBar.value = amount;
	}

	/**
	 * This method updates health slider and text that is displayed on canvas
	 * 
	 * @param amount
	 */ 
	public void SetHealth(int amount)
	{
		healthValue.text = amount.ToString ();
		hpBar.value = amount;
	}

	/**
	 * This method updates text of game status
	 * 
	 * @param text current game status
	 */ 
	public void WriteGameStatusText(string text)
	{
		gameStatusText.text = text;
	}

	/**
	 * This method updates log text that is displayed for a number of seconds
	 * 
	 * @param text of the log
	 * @param duration to be displayed for
	 */ 
	public void WriteLogText(string text, float duration)
	{
		CancelInvoke ();
		logText.text = text;
		Invoke ("ClearLogText", duration);
	}

	/**
	 * This method clears log text
	 */ 
	private void ClearLogText()
	{
		logText.text = "";
	}

	/**
	 * This method is called when the player selects next archer
	 */ 
	public void NextArcher(){

		// Increase archer index
		count++;

		// Make sure that index of not out of bounds
		if (count > 2) {
			count = 0;
		}
			
		// Change archer
		ChangePlayer ();
	}

	/**
	 * This method is called when the player selects previous archer
	 */ 
	public void PreviousArcher(){

		// Decrement index
		count--;

		// Make sure index is within bounds
		if (count < 0) {
			count = 2;
		}

		// Change archer
		ChangePlayer ();
	}

	/**
	 * This method changes archer.
	 * It updates the sprite and relevant stats
	 */ 
	private void ChangePlayer(){
		
		// Set sprite
		archerImg.sprite = archers [count].archerImg;
		// Set Stats
		archerStats.text = "Health: " + archers [count].health + "\nDamage: " + archers [count].arrowPrefab.GetComponent<Arrow> ().damage + "\nSpeed: " + archers [count].speed + "\nJump: " + archers [count].jumpForce + "\nAttack cooldown: " + archers [count].attackCd + "\nAttack speed: " + archers [count].shootDelay;
		// Update player's archer
		currentPlayer.GetComponent<CharacterSwitch> ().ChangeCharacter (count, currentPlayer);
	}

	/**
	 * This method is called when the player clicks 'ready'
	 */ 
	public void Ready(){
		player.Ready ();
		readyButton.SetActive (false);
		unreadyButton.SetActive (true);
	}

	/**
	 * This method is called when the player clicks 'unready'
	 */ 
	public void Unready(){
		player.Unready ();
		unreadyButton.SetActive (false);
		readyButton.SetActive (true);
	}

}