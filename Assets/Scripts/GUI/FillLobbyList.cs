using UnityEngine;
using System.Collections;

public class FillLobbyList : TIOMonoBehaviour {

	public GameObject Manager;
	public GameObject LobbyPrefab;
	private NetworkManager networkManager;
	private UITable lobbyTable;

	// Use this for initialization
	void Start () {
		networkManager = Manager.GetComponent<NetworkManager>();
		lobbyTable = GetComponent<UITable>();
	}
	
	// Update is called once per frame
	void Update () {
//		foreach(Transform child in lobbyTable.children) {
//			GameObject.Destroy(child.transform.gameObject);
//		}
		lobbyTable.children.Clear();
		foreach(Lobby lobby in networkManager.GetLobbies ()) {
			GameObject lobbyObject = NGUITools.AddChild (gameObject, LobbyPrefab);
			UILabel lobbyLabel = lobbyObject.transform.FindChild ("LobbyName Label").GetComponent<UILabel>();
			SetLobbyNameLabel labelScript = lobbyLabel.GetComponent<SetLobbyNameLabel>();
			GameObject joinButton = lobbyObject.transform.FindChild("Join Button").gameObject;
			JoinLobby joinScript = joinButton.GetComponent<JoinLobby>();

			lobbyLabel.text = lobby.name + " " + lobby.playerCount + "/" + lobby.maxPlayers;
			joinScript.Lobby = lobby;
			joinScript.Manager = Manager;
			labelScript.Manager = Manager;
			lobbyTable.children.Add(lobbyObject.transform);
		}
		lobbyTable.Reposition ();
	}
}
