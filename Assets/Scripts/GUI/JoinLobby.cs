using UnityEngine;
using System.Collections;

public class JoinLobby : TIOMonoBehaviour {

	public GameObject Manager;
	private NetworkManager networkManager;
	public Lobby Lobby;
	
	// Use this for initialization
	void Start () {
	}

	void OnClick () {
		if (networkManager == null) {
			networkManager = Manager.GetComponent<NetworkManager>();
		}
		networkManager.LobbyName = Lobby.name;
		networkManager.JoinRoom (Lobby.name);
	}
}
