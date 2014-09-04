using UnityEngine;
using System.Collections;

public class JoinLobby : TIOMonoBehaviour {

	public GameObject Manager;
	private NetworkManager networkManager;
	public Lobby Lobby;
	
	// Use this for initialization
	void Start () {
		networkManager = Manager.GetComponent<NetworkManager>();
	}

	void OnClick () {
		networkManager.LobbyName = Lobby.name;
		networkManager.JoinRoom (Lobby.name);
	}
}
