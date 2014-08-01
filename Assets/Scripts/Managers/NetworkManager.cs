using UnityEngine;
using System.Collections;

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
	//currently just a wrapper for PUN's RoomInfo
	
	public string name { get; set; }
	public int playerCount { get; set; }
	public int maxPlayers { get; set; }
	
	public Lobby(string pName, int pPlayerCount, int pMaxPlayers) {
		this.name = pName;
		this.playerCount = pPlayerCount;
		this.maxPlayers = pMaxPlayers;
	}
}

public class NetworkManager : TIOMonoBehaviour {
	
	public NetMode mode { get; set; }
	public bool connectFailed { get; set; }
	
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
	public string playerName { 
		get { return PhotonNetwork.playerName;}
		set { PhotonNetwork.playerName = value;}
	} 
	public string lobbyName { get; set; }
	
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
	}
	
	// Use this for initialization
	void Start () {
		//Set scene to automatically update on all clients
		PhotonNetwork.automaticallySyncScene = true; 
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void startNetworkedGame(string scene) {
		photonView.RPC("RPC_setGUIStage", PhotonTargets.All, (int)GuiStage.inGame);
		PhotonNetwork.LoadLevel (scene);
		
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
	
	void OnJoinedRoom() {
		if (mode == NetMode.quick) {
			startNetworkedGame ("MainMap");
		}
	}
}
