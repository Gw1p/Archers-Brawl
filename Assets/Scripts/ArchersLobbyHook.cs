using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

/**
 * This class is used as a lobby hook.
 * It determines when and who joins a lobby.
 */ 
public class ArchersLobbyHook : LobbyHook {

	/**
	 * This method is called when a player loads into the lobby
	 * 
	 * @param manager that controls network interactions
	 * @param lobbyPlayer LobbyPlayer that joined the lobby
	 * @param gamePlayer Player that joined the lobby
	 */ 
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer){

		LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer> ();
		Player gPlayer = gamePlayer.GetComponent<Player> ();

		gPlayer.playerName = lPlayer.playerName;
		gPlayer.playerColor = lPlayer.playerColor;

	}
}
