using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GuiStage {
	mainMenu,
	multiplayerMenu,
	createGameMenu,
	inGame,
	joiningLobby
};

public class UIManager : TIOMonoBehaviour {
	private GuiStage guiStage = GuiStage.mainMenu; 
	private NetworkManager networkManager;
	private LanguageManager languageManager;
	private GameManager gameManager;
	
	private Vector2 scrollPos = Vector2.zero;
	private string lobbyName = "default";

	private int expansionIndex = 2;
	private string[] expansionStrings;
	private int scenarioIndex = 0;
	private string[] scenarioStrings;
	private Option[] possibleOptions;

	Vector2 scrollPosition = Vector2.zero;
	
	// Use this for initialization
	void Start () {
		GameObject manager = GameObject.Find ("Manager");
		Object.DontDestroyOnLoad (manager);

		networkManager = (NetworkManager)GetComponent("NetworkManager");
		languageManager = (LanguageManager)GetComponent("LanguageManager");
		gameManager = (GameManager)GetComponent ("GameManager");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	[RPC]
	public void RPC_setGUIStage(int stage) { 
		this.guiStage = (GuiStage) stage;
	}
	
	private void OnGUI () {
//		if (networkManager.hasLobby() && guiStage != GuiStage.inGame) {
//			gui_InLobby();
//		} else if (guiStage == GuiStage.mainMenu) {
//			gui_MainMenu();
//		} else if (guiStage == GuiStage.multiplayerMenu) {
//			gui_MultiplayerMenu();
//		} else if (guiStage == GuiStage.createGameMenu) {
//			gui_CreateGameMenu();
//		} else if (guiStage == GuiStage.joiningLobby) {
//			gui_JoiningLobby();
//		}
		GUILayout.BeginArea(new Rect(0, Screen.height-20, 400, 300));
		GUILayout.Label("Ping to server: " + PhotonNetwork.GetPing());
		GUILayout.EndArea ();
	}

	public void SetRaceChoices(string[] raceChoices) {
		GameObject raceChoicesWindow = GameObject.Find ("Main Camera").transform.Find ("UI Root (3D)").transform.Find ("Race Menu").transform.Find("Race Choices").gameObject;

		//Enable the correct UI object according to number of choices
		GameObject raceChoicesObject = raceChoicesWindow.transform.Find(raceChoices.Length.ToString() + " Choices").gameObject;
		for (int choiceIndex = 0; choiceIndex < raceChoices.Length; choiceIndex++) {
			GameObject indChoiceObject = raceChoicesObject.transform.FindChild("Choice " + (choiceIndex + 1).ToString()).gameObject;
			//Set Material/Texture
			UITexture indChoiceTexture = indChoiceObject.transform.GetChild (0).transform.GetChild(0).GetComponent<UITexture>();
			Material mat = new Material(indChoiceTexture.material);
			mat.mainTexture = Resources.Load("Images/Races/Banners/" + raceChoices[choiceIndex].ToString()) as Texture;
			indChoiceTexture.material = mat;
			//Set Name
			Race race = gameManager.PlayerMgr.AddRace(raceChoices[choiceIndex].ToString());
			UILabel indChoiceLabel = indChoiceObject.transform.GetChild (0).transform.GetChild(1).GetComponent<UILabel>();
			indChoiceLabel.text = race.FullName;
			indChoiceObject.GetComponent<ChooseRace>().RaceID = race.Id;
		}
		raceChoicesObject.SetActive(true);
		raceChoicesWindow.transform.parent.gameObject.SetActive(true);
	}

	[RPC]
	public void RPC_CloseRaceSelection() {
		gameManager.BoardMgr.PrepareGalaxySetup();
		GameObject raceMenu = GameObject.Find ("Main Camera").transform.FindChild ("UI Root (3D)").FindChild ("Race Menu").gameObject;
		raceMenu.SetActive(false);
		gameManager.SetStage (GameStage.Playing);
	}
	
//	private void gui_MainMenu () {
//		int buttonWidth = 250;
//		int buttonHeight = 100;
//		int space = 10;
//
//		if (!networkManager.connected) {
//			gui_Disconnected();
//		} else if (GUI.Button (new Rect ((Screen.width-buttonWidth)/2, ((Screen.height-buttonHeight)/2), 250, 100), "Multiplayer")) {
//			languageManager.Initialize();
//			guiStage = GuiStage.multiplayerMenu;
//		}
//	}
//	
//	private void gui_MultiplayerMenu() {
//		int menuWidth = 400;
//		int menuHeight = 300;
//		
//		GUILayout.BeginArea(new Rect((Screen.width - menuWidth) / 2, (Screen.height - menuHeight) / 2, menuWidth, menuHeight));
//		
//		// Player name
//		GUILayout.BeginHorizontal();
//		GUILayout.Label("Player name:", GUILayout.Width((2 * menuWidth) / 5));
//		networkManager.playerName = (GUILayout.TextField (networkManager.playerName));
//		if (GUI.changed) {
//			PlayerPrefs.SetString("playerName", networkManager.playerName);
//		}
//		GUILayout.EndHorizontal();
//		GUILayout.Space(15);
//		
//		if (GUILayout.Button("NEW GAME LOBBY")) {
//			expansionStrings = new string[3];
//			expansionStrings[0] = languageManager.ExpansionToString(Expansion.Vanilla);
//			expansionStrings[1] = languageManager.ExpansionToString(Expansion.ShatteredEmpire);
//			expansionStrings[2] = languageManager.ExpansionToString(Expansion.ShardsOfTheThrone);
//			scenarioStrings = new string[2];
//			scenarioStrings[0] = languageManager.ScenarioToString(Scenario.StandardGame);
//			scenarioStrings[1] = languageManager.ScenarioToString(Scenario.FallOfTheEmpire);
//			possibleOptions = (Option[])System.Enum.GetValues(typeof(Option));
//			guiStage = GuiStage.createGameMenu;
////			networkManager.lobbyName = this.lobbyName;
////			networkManager.CreateLobby(lobbyName, new RoomOptions() { maxPlayers = 4});
//		}
//		
//		//Show a list of all current lobbies
//		GUILayout.Label("Available Lobbies:");
//		if (networkManager.GetLobbies().Length == 0) {
//			GUILayout.Label("(none)");
//		}
//		else
//		{
//			// Room listing: simply call GetRoomList: no need to fetch/poll whatever!
//			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
//			foreach (Lobby lobby in networkManager.GetLobbies ())
//			{
//				GUILayout.BeginHorizontal();
//				GUILayout.Label(lobby.name + " " + lobby.playerCount + "/" + lobby.maxPlayers);
//				if (GUILayout.Button("JOIN"))
//				{
//					guiStage = GuiStage.joiningLobby;
//					networkManager.lobbyName = lobby.name;
//					networkManager.JoinRoom(lobby.name);
//				}
//				GUILayout.EndHorizontal();
//			}
//			GUILayout.EndScrollView();
//		}
//		GUILayout.EndArea();
//	}
//
//	private void gui_CreateGameMenu() {
//		int menuWidth = 450;
//		int menuHeight = 400;
//
//		GUILayout.BeginArea(new Rect((Screen.width - menuWidth) / 2, (Screen.height - menuHeight) / 2, menuWidth, menuHeight));
//		GUILayout.BeginHorizontal ();
//		GUILayout.Label("Lobby Name:", GUILayout.Width(menuWidth/4));
//		this.lobbyName = GUILayout.TextField (this.lobbyName);
//		GUILayout.EndHorizontal ();
//
//		GUILayout.Label("Expansion:", GUILayout.Width(menuWidth));
//		expansionIndex = GUILayout.SelectionGrid (expansionIndex, expansionStrings, 3);
//		gameManager.Expansion = languageManager.StringToExpansion (expansionStrings [expansionIndex]);
//		
//		GUILayout.Label("Scenario:", GUILayout.Width(menuWidth));
//		scenarioIndex = GUILayout.SelectionGrid (scenarioIndex, scenarioStrings, 2);
//		gameManager.Scenario = languageManager.StringToScenario (scenarioStrings [scenarioIndex]);
//
//		GUILayout.Label("Game Options:", GUILayout.Width(menuWidth));
//		//Scrollable options selection
//		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (menuWidth), GUILayout.Height (menuHeight/3));
//		int row = 0;
//		foreach(Option option in possibleOptions) {
//			GUILayout.BeginHorizontal ();
//			gameManager.ActiveOptions[option] = GUI.Toggle(new Rect(0, 5+row*25, 20, 20), gameManager.ActiveOptions[option], "");
//			GUILayout.Space(20); //Space for the checkbox area
//			GUILayout.Label (languageManager.OptionToString(option), GUILayout.Width (menuWidth-100));
//			GUILayout.EndHorizontal ();
//			row += 1;
//		}
//		GUILayout.EndScrollView ();
//
//		if (GUILayout.Button("NEW GAME LOBBY")) {
//			networkManager.lobbyName = this.lobbyName;
//			networkManager.CreateLobby(lobbyName, new RoomOptions() { maxPlayers = 8});
//		}
//
//		GUILayout.EndArea();
//	}
//	
//	private void gui_InLobby() {
//		GUILayout.Label("We are connected to room: "+networkManager.lobbyName);
//		GUILayout.Label("Players: ");
//		foreach (MetaPlayer player in networkManager.GetMetaPlayers ())
//		{
//			GUILayout.Label("ID: "+player.ID+" Name: "+player.name);
//		}
//		
//		if (GUILayout.Button("Start game"))
//		{
//			networkManager.startNetworkedGame();
//		}
//		
//		if (GUILayout.Button("Leave room"))
//		{
//			guiStage = GuiStage.multiplayerMenu;
//			PhotonNetwork.LeaveRoom();
//		}
//	}
//	
//	private void gui_JoiningLobby() {
//		GUILayout.Label ("Connecting to " + networkManager.lobbyName + "...");
//	}
//	
//	private void gui_Disconnected() {
//		if (networkManager.connecting) {
//			GUILayout.Label("Connecting...");
//		}
//		else {
//			GUILayout.Label("Not connected.");
//		} 
//		
//		if (networkManager.connectFailed){
//			GUILayout.Label("Connection failed.");
//			GUILayout.Label(string.Format("Server: {0}", networkManager.serverAddress));
//			GUILayout.Label(string.Format("AppId: {0}", networkManager.appID));
//			
//			if (GUILayout.Button("Try Again", GUILayout.Width(100)))
//			{
//				networkManager.connectFailed = false;
//				PhotonNetwork.ConnectUsingSettings("1.0");
//			}
//		}
//	}
}
