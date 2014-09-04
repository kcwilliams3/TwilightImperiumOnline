using UnityEngine;
using System.Collections;

public class FillLobbyList : TIOMonoBehaviour {

	public GameObject Manager;
	public GameObject LobbyPrefab;
	private NetworkManager networkManager;

	// Use this for initialization
	void Start () {
		networkManager = Manager.GetComponent<NetworkManager>();
	}

	// Update is called once per frame
	void Update () {
//		foreach(Transform child in lobbyTable.children) {
//			GameObject.Destroy(child.transform.gameObject);
//		}
		int i = 0;
		foreach(Lobby lobby in networkManager.GetLobbies ()) {
			GameObject row = transform.GetChild(i).gameObject;
			UILabel lobbyLabel = row.transform.FindChild ("LobbyName Label").GetComponent<UILabel>();
			GameObject joinButton = row.transform.FindChild("Join Button").gameObject;
			JoinLobby joinScript = joinButton.GetComponent<JoinLobby>();

			row.active = true;
			lobbyLabel.text = lobby.name + " " + lobby.playerCount + "/" + lobby.maxPlayers;
			joinScript.Lobby = lobby;

			i++;
		}
		//Disable unused children
		while (i < transform.childCount) {
			GameObject row = transform.GetChild(i).gameObject;
			row.active = false;
			i++;
		}
	}
}
