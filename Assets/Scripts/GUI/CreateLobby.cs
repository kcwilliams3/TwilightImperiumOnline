using UnityEngine;
using System.Collections;

public class CreateLobby : TIOMonoBehaviour {

	public GameObject Manager;

	void OnClick() {
		NetworkManager networkManager = Manager.GetComponent<NetworkManager>();

		networkManager.LobbyName = networkManager.PlayerName + "'s Quick Lobby";
		networkManager.CreateLobby(networkManager.LobbyName, new RoomOptions() { maxPlayers = 8});
	}
}
