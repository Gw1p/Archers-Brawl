using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ToggleEvent: UnityEvent<bool>{}

/**
 * This class controls player game status (won/lost, name, color, etc).
 */ 
public class Player : NetworkBehaviour {

	// Player name
	[SyncVar (hook = "OnNameChanged")] public string playerName;
	// Player color
	[SyncVar (hook = "OnColorChanged")] public Color playerColor;

	// Toggle shared components
	[SerializeField] ToggleEvent onToggleShared;
	// Toggle local components (only on a local player)
	[SerializeField] ToggleEvent onToggleLocal;
	// Toggle remove components (on other players)
	[SerializeField] ToggleEvent onToggleRemote;
	// Respawn time of a player
	[SerializeField] float respawnTime = 5f;

	// List of all players
	public static List<Player> players = new List<Player> ();
	// Number of players that are ready
	public static int playerReady = 0;

	// Color of a player
	public GameObject color;
	// Are all players ready?
	public bool playersReady = false;
	// Has the game started?
	public bool gameStarted = false;

	// Main camera in the scene
	private GameObject mainCam;
	// Animator to play animations
	private NetworkAnimator anim;
	// Has canvas been initialized?
	private bool canvInitialized = false;

	void Start()
	{
		// Get NetworkAnimator
		anim = gameObject.transform.GetComponent<NetworkAnimator> ();
		// Access main camera
		mainCam = Camera.main.gameObject;
		// Set canvas initialized to false
		canvInitialized = false;

		if (isLocalPlayer) {

		} else {
			// Turn on remote components
			onToggleRemote.Invoke (true);
		}

	}

	/**
	 * This method is called once a frame
	 */ 
	void Update(){
		// Check if canvas has not yet been initialized
		if (canvInitialized == false) {
			// If canvas is not null and this is a local player
			if (PlayerCanvas.canvas != null && isLocalPlayer) {
				// Initialize player canvas
				PlayerCanvas.canvas.Initialize (gameObject);
				canvInitialized = true;
			}
		}
	}

	/**
	 * This method is called when a player presses ready
	 */ 
	public void Ready(){
		CmdPressedReady();
	}

	/**
	 * This method starts a new game
	 */ 
	public void StartGame(){
		PlayerCanvas.canvas.menu.SetActive (false);
		EnablePlayer ();
	}

	/**
	 * This method is called when a player presses ready
	 */ 
	[Command]
	private void CmdPressedReady(){
		
		// Increase player ready count
		playerReady++;

		// Check if number of players ready equals number of players
		if (playerReady == Player.players.Count) {
			playersReady = true;

			// Set ready on all players
			for (int i = 0; i < Player.players.Count; i++) {
				Player.players [i].RpcSetReady (Player.players [i].netId);
			}
		}
	}

	/**
	 * This method sets ready on a client
	 */ 
	[ClientRpc]
	private void RpcSetReady(NetworkInstanceId netID){
		
		// Check that netIds are equal
		if (netID == netId) {
			playersReady = true;
		}
	}

	/**
	 * This method is called when a player presses unready
	 */ 
	public void Unready(){
		CmdUnressedReady();
	}

	/**
	 * This method is called when a player presses unready
	 */ 
	[Command]
	private void CmdUnressedReady(){
		playerReady--;
	}

	/**
	 * This method is called when this component is enabled
	 */ 
	[ServerCallback]
	private void OnEnable(){
		// If players list does not contain this, add it
		if (!players.Contains (this)) {
			players.Add (this);
		}
	}

	/**
	 * This method is called when this component is disabled
	 */ 
	[ServerCallback]
	private void OnDisable(){
		// Remove this from players list
		if (players.Contains (this)) {
			players.Remove (this);
		}
	}

	/**
	 * This method disables player
	 */ 
	private void DisablePlayer(){

		// If this is a local player, enable main camera
		if (isLocalPlayer) {
			mainCam.SetActive (true);
		}

		// Set rigidbody to kinematic to stop physics
		gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
		// Disable shared components
		onToggleShared.Invoke (false);

		// Disable components on local and remote players
		if (isLocalPlayer) {
			onToggleLocal.Invoke (false);
		} else {
			onToggleRemote.Invoke (false);
		}

	}

	/**
	 * This method enables player
	 */ 
	public void EnablePlayer(){

		// If this is a local player, initialise canvas and disable camera
		if (isLocalPlayer) {
			PlayerCanvas.canvas.Initialize (gameObject);
			mainCam.SetActive (false);
		}

		// Play idle animation
		anim.animator.Play ("Idle");
		// Set rigidbody kinematic to false to enable physics
		gameObject.GetComponent<Rigidbody2D> ().isKinematic = false;
		// Enable shared components
		onToggleShared.Invoke (true);

		// Enable local components on local players and remote components on remote players
		if (isLocalPlayer) {
			onToggleLocal.Invoke (true);
		} else {
			onToggleRemote.Invoke (true);
		}

	}

	/**
	 * This method is called when a player dies
	 */ 
	public void Die(){

		// If this is a local player, update text
		if (isLocalPlayer) {
			PlayerCanvas.canvas.WriteGameStatusText ("You Died!");
		}

		// Play death animator
		anim.animator.Play ("Death");
		// Disable the player
		DisablePlayer ();

		// Respawn after delay
		Invoke ("Respawn", respawnTime);
	}

	/**
	 * This method respawns a player
	 */ 
	private void Respawn(){

		// If this is a local player, update location and rotation
		if (isLocalPlayer) {
			Transform spawn = NetworkManager.singleton.GetStartPosition ();
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;
		}

		// Enable the player
		EnablePlayer ();
	}

	/**
	 * This method is used as a hook for when name changes
	 * 
	 * @param value new name
	 */ 
	private void OnNameChanged(string value){
		playerName = value;
		gameObject.name = playerName;
		GetComponentInChildren<Text> (true).text = playerName;
	}

	/**
	 * This method is used as a hook for when color changes
	 * 
	 * @param clr new color
	 */ 
	private void OnColorChanged(Color clr){
		playerColor = clr;
		color.GetComponent<Image> ().color = playerColor;
	}

	/**
	 * This method is called when the game is finished and a player has won
	 */ 
	[Server]
	public void Won(){

		// Loop through all players and call GameOver
		for (int i = 0; i < players.Count; i++) {
			players [i].RpcGameOver (netId, name);
		}

		// Go back to lobby after 5 seconds
		Invoke ("BackToLobby", 5f);
	}

	/**
	 * This method is called when the game finishes
	 * 
	 * @param networkID id of a player who won the game
	 * @param name of a player
	 */ 
	[ClientRpc]
	private void RpcGameOver(NetworkInstanceId networkID, string name){
		// Disable the player
		DisablePlayer ();

		// Check if the player is local
		if (isLocalPlayer) {
			// Display appropriate text on canvas
			if (netId == networkID) {
				PlayerCanvas.canvas.WriteGameStatusText ("You Won!");
			} else {
				PlayerCanvas.canvas.WriteGameStatusText ("Game Over!\n" + name + " Won");
			}
		}
	}

	/**
	 * Return player back to the lobby
	 */ 
	private void BackToLobby(){

		// Set playerReady to 0 on all players
		for (int i = 0; i < players.Count; i++) {
			playerReady = 0;
		}

		// Clear player list
		players.Clear ();
		// Return to lobby
		FindObjectOfType<NetworkLobbyManager> ().SendReturnToLobby ();
	}

}
