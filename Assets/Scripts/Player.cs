using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ToggleEvent: UnityEvent<bool>{}

public class Player : NetworkBehaviour {

	[SyncVar (hook = "OnNameChanged")] public string playerName;
	[SyncVar (hook = "OnColorChanged")] public Color playerColor;

	[SerializeField] ToggleEvent onToggleShared;
	[SerializeField] ToggleEvent onToggleLocal;
	[SerializeField] ToggleEvent onToggleRemote;
	[SerializeField] float respawnTime = 5f;

	public static List<Player> players = new List<Player> ();
	public static int playerReady = 0;

	public GameObject color;
	public bool playersReady = false;
	public bool gameStarted = false;

	GameObject mainCam;
	NetworkAnimator anim;
	bool canvInitialized = false;

	void Start()
	{
		anim = gameObject.transform.GetComponent<NetworkAnimator> ();
		mainCam = Camera.main.gameObject;
		canvInitialized = false;

		if (isLocalPlayer) {

		} else {
			onToggleRemote.Invoke (true);
		}

	}

	void Update(){
		if (canvInitialized == false) {
			if (PlayerCanvas.canvas != null && isLocalPlayer) {
				PlayerCanvas.canvas.Initialize (gameObject);
				canvInitialized = true;
			}
		}
	}

	public void Ready(){
		CmdPressedReady();
	}

	public void StartGame(){
		PlayerCanvas.canvas.menu.SetActive (false);
		EnablePlayer ();
	}

	[Command]
	void CmdPressedReady(){
		playerReady++;
		if (playerReady == Player.players.Count) {
			playersReady = true;
			for (int i = 0; i < Player.players.Count; i++) {
				Player.players [i].RpcSetReady (Player.players [i].netId);
			}
		}
	}

	[ClientRpc]
	void RpcSetReady(NetworkInstanceId netID){
		if (netID == netId) {
			playersReady = true;
		}
	}

	public void Unready(){
		CmdUnressedReady();
	}

	[Command]
	void CmdUnressedReady(){
		playerReady--;
	}

	[ServerCallback]
	void OnEnable(){

		if (!players.Contains (this))
			players.Add (this);
		
	}

	[ServerCallback]
	void OnDisable(){

		if (players.Contains (this))
			players.Remove (this);

	}


	void DisablePlayer(){

		if (isLocalPlayer) {
			mainCam.SetActive (true);
		}

		gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
		onToggleShared.Invoke (false);
		if (isLocalPlayer) {
			onToggleLocal.Invoke (false);
		} else {
			onToggleRemote.Invoke (false);
		}

	}

	public void EnablePlayer(){
		if (isLocalPlayer) {
			PlayerCanvas.canvas.Initialize (gameObject);
			mainCam.SetActive (false);
		}
		anim.animator.Play ("Idle");
		gameObject.GetComponent<Rigidbody2D> ().isKinematic = false;
		onToggleShared.Invoke (true);

		if (isLocalPlayer) {
			onToggleLocal.Invoke (true);
		} else {
			onToggleRemote.Invoke (true);
		}

	}

	public void Die(){

		if (isLocalPlayer) {
			PlayerCanvas.canvas.WriteGameStatusText ("You Died!");
		}

		anim.animator.Play ("Death");
		DisablePlayer ();

		Invoke ("Respawn", respawnTime);

	}

	void Respawn(){

		if (isLocalPlayer) {

			Transform spawn = NetworkManager.singleton.GetStartPosition ();
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;

		}

		EnablePlayer ();

	}

	void OnNameChanged(string value){
		playerName = value;
		gameObject.name = playerName;
		GetComponentInChildren<Text> (true).text = playerName;
	}

	void OnColorChanged(Color clr){
		playerColor = clr;
		color.GetComponent<Image> ().color = playerColor;
	}

	[Server]
	public void Won(){

		for (int i = 0; i < players.Count; i++) {
			players [i].RpcGameOver (netId, name);
		}

		Invoke ("BackToLobby", 5f);

	}

	[ClientRpc]
	void RpcGameOver(NetworkInstanceId networkID, string name){
		DisablePlayer ();

		if (isLocalPlayer) {
			if (netId == networkID) {
				PlayerCanvas.canvas.WriteGameStatusText ("You Won!");
			} else {
				PlayerCanvas.canvas.WriteGameStatusText ("Game Over!\n" + name + " Won");
			}
		}
	}

	void BackToLobby(){
		for (int i = 0; i < players.Count; i++)
			playerReady = 0;
		
		players.Clear ();
		FindObjectOfType<NetworkLobbyManager> ().SendReturnToLobby ();
	}

}
