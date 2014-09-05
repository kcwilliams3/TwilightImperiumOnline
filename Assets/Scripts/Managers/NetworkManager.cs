using UnityEngine;
using System.Collections.Generic;

//Mode constants
public enum NetMode { initial, quick, full };

public class MetaPlayer {
	//currently just a wrapper for PUN's Player

	public int ID { get; private set; }
	public string name { get; set; }

	public MetaPlayer (int pID, string pName){
		this.ID = pID;
		this.name = pName;
	}
}

public class Lobby {

	public string name { get; set; }
	public int playerCount { get; set; }
	public int maxPlayers { get; set; }
	public List<Option> LobbyOptions { get; set; }
	public Scenario LobbyScenario { get; set; }
	public Expansion LobbyExpansion { get; set; }

	public Lobby(string pName, int pPlayerCount, int pMaxPlayers) {
		this.name = pName;
		this.playerCount = pPlayerCount;
		this.maxPlayers = pMaxPlayers;
		this.LobbyOptions = new List<Option>();
	}

	public Lobby(string pName) {
		this.name = pName;
		this.LobbyOptions = new List<Option>();
	}

	public void AddOption(Option option) {
		LobbyOptions.Add (option);
	}
}

public class NetworkManager : TIOMonoBehaviour {

	public NetMode mode { get; set; }
	public bool connectFailed { get; set; }

	private GameManager gameManager;
		
	//PUN wrappers
	public bool connected { 
		get { return PhotonNetwork.connected;}
	}
	public bool connecting { 
		get { return PhotonNetwork.connecting;}
	}
	public string serverAddress { 
		get { return PhotonNetwork.PhotonServerSettings.ServerAddress;}
	}
	public string appID { 
		get { return PhotonNetwork.PhotonServerSettings.AppID;}
	}
	public string PlayerName { 
		get { return PhotonNetwork.playerName;}
		set { PhotonNetwork.playerName = value;}
	} 
	public Lobby Lobby { get; set; }

	private void Awake()
	{

		mode = NetMode.initial;
		connectFailed = false;
		// Connect to the main photon server.
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("1.0");
		}	
		
		//Load our name from PlayerPrefs
		PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
		Lobby = new Lobby ("Default");
	}
	
	// Use this for initialization
	void Start () {
		//Set scene to automatically update on all clients
		PhotonNetwork.automaticallySyncScene = true;

		gameManager = (GameManager)GetComponent("GameManager");
		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (PhotonNetwork.connectionStateDetailed);
	}

	public void startNetworkedGame(Lobby lobby) {
		//Update gui state and number of players for everyone
		//photonView.RPC("RPC_setGUIStage", PhotonTargets.All, (int)GuiStage.inGame);
		photonView.RPC ("RPC_SetPlayerCount", PhotonTargets.All, PhotonNetwork.room.playerCount);

		//Update  game info for everyone
		foreach(Option option in lobby.LobbyOptions) {
			photonView.RPC("RPC_SetOption", PhotonTargets.All, (int)option, true);
		}
		photonView.RPC ("RPC_SetExpansion", PhotonTargets.All, (int)lobby.LobbyExpansion);
		photonView.RPC ("RPC_SetScenario", PhotonTargets.All, (int)lobby.LobbyScenario);

		//Start game for everyone
		PhotonNetwork.LoadLevel ("Game");
		//photonView.RPC ("RPC_StartGame", PhotonTargets.All);
	}


//	//Not needed (autosync + PhotonNetwork.LoadLevel takes care of this for us)
//	[RPC]
//	public void RPC_startGame() {
//		Debug.Log ("Loading MainMap.unity...");
//		this.mode = NetMode.full; 
//		Application.LoadLevel ("MainMap");
//	}

	public void setPlayerName(string name) {
		PhotonNetwork.playerName = name;
	}

	public bool hasLobby() {
		return PhotonNetwork.room != null;
	}
	
	public void CreateLobby(string room, RoomOptions options){
		PhotonNetwork.CreateRoom(room, options, null);
		this.Lobby = new Lobby(room);
	}

	public Lobby[] GetLobbies() {
		// Using Lobby as a wrapper for RoomInfo to 
		//  restrict PUN-specific things to this file
		RoomInfo[] rooms = PhotonNetwork.GetRoomList ();
		int numLobbies = rooms.Length;
		Lobby[] lobbies = new Lobby[numLobbies];
		for (int i = 0; i < numLobbies; i++){
			lobbies[i] = new Lobby(rooms[i].name, rooms[i].playerCount, rooms[i].maxPlayers);
		}
		
		return lobbies;
	}

	public void JoinRoom(string name) {
		PhotonNetwork.JoinRoom(name);
		this.Lobby = new Lobby(name);
	}

	public MetaPlayer[] GetMetaPlayers() {
		// Using MetaPlayer as a wrapper for Player to 
		//  restrict PUN-specific things to this file
		PhotonPlayer[] players  = PhotonNetwork.playerList;
		MetaPlayer[] metaPlayers = new MetaPlayer[players.Length];
		for (int i = 0; i < players.Length; i++){
			metaPlayers[i] = new MetaPlayer(players[i].ID, players[i].name);
		}


		return metaPlayers;
	}

	private void OnFailedToConnectToPhoton(object parameters) {
		connectFailed = true;
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
	}

}
