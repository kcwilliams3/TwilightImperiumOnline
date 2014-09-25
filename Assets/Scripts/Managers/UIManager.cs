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

	public MapSetup MapSetupUI;

	// Use this for initialization
	void Start () {
		GameObject manager = GameObject.Find ("Manager");
		Object.DontDestroyOnLoad (manager);

		networkManager = (NetworkManager)GetComponent("NetworkManager");
		languageManager = (LanguageManager)GetComponent("LanguageManager");
		gameManager = (GameManager)GetComponent("GameManager");
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
		GameObject bottomBackground = GameObject.Find ("Main Camera").transform.FindChild ("UI Root (3D)").FindChild ("Bottom Panel").FindChild("Background").gameObject;

		raceMenu.SetActive(false);
		bottomBackground.SetActive(true);
		gameManager.SetStage (GameStage.Playing);
	}
}