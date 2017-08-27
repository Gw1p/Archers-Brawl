using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class ArchersLobbyHook : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer){

		LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer> ();
		Player gPlayer = gamePlayer.GetComponent<Player> ();

		gPlayer.playerName = lPlayer.playerName;
		gPlayer.playerColor = lPlayer.playerColor;

	}
}
