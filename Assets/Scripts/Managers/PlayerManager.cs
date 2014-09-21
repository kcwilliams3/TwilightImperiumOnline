using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : TIOMonoBehaviour {

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<string, PromissoryNote> promNotes = new Dictionary<string, PromissoryNote>();
	private Dictionary<string, Treaty> treaties = new Dictionary<string, Treaty>();
	//Do need this one
	private Dictionary<string, Race> races = new Dictionary<string, Race>();

	[SerializeField]
	private PromissoryNote[] promNotesDebug;
	[SerializeField]
	private Dictionary<string, Player> players = new Dictionary<string, Player>();
	public int PlayerCount { get { return players.Count; } }
	
	private GameManager gameManager;
	public bool ReadyToPlay;

	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public Player GetPlayer(string id) {
		return players [id];
	}

	public Player[] GetPlayers() {
		Player[] tmpPlayers = new Player[players.Count];
		players.Values.CopyTo(tmpPlayers, 0);
		return tmpPlayers;
	}

	public Race GetRace(string id) {
		return races [id];
	}

	public PlanetSystem GetHomeSystem(string systemName) {
		foreach(Race race in races.Values) {
			foreach(PlanetSystem sys in race.HomeSystems) {
				if (sys.Name == systemName) {
					return sys;
				}
			}
		} 
		return new PlanetSystem();
	}

	public void InitializePlayers() {
		if (gameManager.Scenario == Scenario.StandardGame) {
			//Standard Game: Give a number of random race choices to each player. Only the MasterClient should do this.
			//	Set race options (by id) and number of choices each player will get
			ArrayList options = new ArrayList (new string[] {"Arborec", "Gashlai", "Ghosts", "Hacan", "Jol Nar", "L1z1x", "Letnev", "Mentak", "Naalu", "Nekro", "N'orr", "Saar", "Sol", "Winnu", "Xxcha", "Yin", "Yssaril"});
			int choicesPerPlayer = options.Count / gameManager.PlayerCount;
			if (choicesPerPlayer > 4) {
				choicesPerPlayer = 4;
			}

			// 	Choose races for each player
			string[] raceChoices = new string[choicesPerPlayer];
			for (int playerIndex=0; playerIndex < gameManager.PlayerCount; playerIndex ++) {
				for (int choiceIndex=0; choiceIndex < choicesPerPlayer; choiceIndex++) {
					raceChoices[choiceIndex] = (string)options[Random.Range(0, options.Count)];
					options.Remove(raceChoices[choiceIndex]);
				}
				if (playerIndex == gameManager.PlayerCount - 1) {
					// Choices for this player: set on local client
					RPC_ReceiveRaceChoices(raceChoices);
				} else {
					// Different player: send to the appropriate player
					gameManager.NetworkMgr.RPCForPlayer("RPC_ReceiveRaceChoices", networkView, playerIndex, raceChoices);
				}
			} 
		}
		//TODO: Fall of the Empire branch will need tweaking to allow for race selection
//		if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
//			string raceID = "";
//			for (int i=0; i < gameManager.PlayerCount; i++) {
//				switch (i) {
//				case 0:
//					raceID = "Lazax";
//					break;
//				case 1:
//					raceID = "Sol";
//					break;
//				case 2:
//					raceID = "Letnev";
//					break;
//				case 3:
//					raceID = "Hacan";
//					break;
//				case 4:
//					raceID = "Jol Nar";
//					break;
//				case 5:
//					raceID = "Xxcha";
//					break;
//				case 6:
//					raceID = "N'orr";
//					break;
//				}
//				addPlayer (raceID);
//			}
//		}
	}

	public Race AddRace(string raceString) {
		//Read race data from file and add to races directory
		Race race = gameManager.FileMgr.ReadRaceFile (raceString);
		races [race.Id] = race;
		return race;
	}

	[RPC]
	public Player RPC_AddPlayer(string playerString, string raceString, float colorR, float colorG, float colorB) {
		Race race;
		if (!races.ContainsKey(raceString)) {
			race = AddRace (raceString);
		} else {
			race = races[raceString];
		}

		//Create a player instance with the newly added race
		players[playerString] = new Player (playerString, race, new Color(colorR, colorG, colorB));
		if (gameManager.PlayerMgr.PlayerCount == gameManager.PlayerCount && gameManager.NetworkMgr.IsMasterClient()) {
			gameManager.networkView.RPC ("RPC_CloseRaceSelection", PhotonTargets.All);
		}

		return players[playerString];
	}


//	//TODO: Might not need this now. Decide.
//	public System.Collections.Generic.IEnumerable<Player> Players() {
//		foreach(Player player in players){
//			yield return player;
//		}
//	}

	public void InitializePlayerComponents() {
		if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
			//Read Treaties data
			readTreatyCards ();
		}
		if (gameManager.IsActive (Option.PoliticalIntrigue)) {
			//Read Promissory Notes data
			readPromNotes();
		}
		foreach (Player player in players.Values) {
			if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
				//Distribute Treaties
				player.Treaties = getTreatyHand(player);
			}
			if (gameManager.IsActive (Option.PoliticalIntrigue)) {
				getNotesHand(player);
				player.Notes = getNotesHand(player);
			}
			player.HiddenObjectives.Add (gameManager.CardMgr.GetStartingObjective(player));
		}
	}

	private void readTreatyCards() {
		foreach (Treaty treaty in gameManager.FileMgr.ReadTreatyFile()){
			treaties[treaty.Name] = treaty;
		}
	}

	
	private Treaty[] getTreatyHand(Player player) {
		ArrayList hand = new ArrayList ();
		foreach(Treaty treaty in treaties.Values) {
			Race lazax = GetRace ("Lazax");
			if ((player.Race == lazax && player.Race == treaty.Race) || (player.Race != lazax && treaty.Race != lazax)) {
				hand.Add (treaty.DuplicateFor(player));
			} 
		}
		return (Treaty[])hand.ToArray (typeof(Treaty));
	}

	private void readPromNotes() {
		ArrayList notesList = new ArrayList ();
		foreach(PromissoryNote note in GetComponent<FileManager>().ReadPromissoryFile()) {
			promNotes[note.Name] = note;
			notesList.Add (note);
		}
		promNotesDebug = (PromissoryNote[])notesList.ToArray (typeof(PromissoryNote));
	}

	private PromissoryNote[] getNotesHand(Player player) {
		ArrayList hand = new ArrayList ();
		foreach(PromissoryNote note in promNotes.Values) {
			hand.Add (note.DuplicateFor (player));
		}
		return (PromissoryNote[])hand.ToArray (typeof(PromissoryNote));
	}

	[RPC]
	public void RPC_ReceiveRaceChoices(string[] raceChoices) {
		gameManager.UIMgr.SetRaceChoices (raceChoices);
		ReadyToPlay = true;
	}
}
