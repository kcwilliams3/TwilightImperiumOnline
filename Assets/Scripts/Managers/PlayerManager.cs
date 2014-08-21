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
	private Player[] players;
	private int playerCount = 0;

	private bool first = true;
	
	private FileManager fileManager;
	private GameManager gameManager;
	private CardManager cardManager;

	// Use this for initialization
	void Start () {
		fileManager = GetComponent<FileManager>();
		gameManager = GetComponent<GameManager>();
		cardManager = GetComponent<CardManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (first) {
			first = false;
		}
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
		players = new Player[gameManager.PlayerCount];
		if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
			string raceID = "";
			for (int i=0; i < gameManager.PlayerCount; i++) {
				switch (i) {
				case 0:
					raceID = "Lazax";
					break;
				case 1:
					raceID = "Sol";
					break;
				case 2:
					raceID = "Letnev";
					break;
				case 3:
					raceID = "Hacan";
					break;
				case 4:
					raceID = "Jol Nar";
					break;
				case 5:
					raceID = "Xxcha";
					break;
				case 6:
					raceID = "N'orr";
					break;
				}
				addPlayer (raceID);
			}
		}
	}

	private Player addPlayer(string raceString) {
		//Read race data from file and add to races directory
		Race race = fileManager.ReadRaceFile (raceString);
		races [race.Id] = race;

		//Create a player instance with the newly added race
		Player player = new Player (race);
		players [playerCount] = player;
		playerCount += 1;

		return player;
	}


	//TODO: Might not need this now. Decide.
	public System.Collections.Generic.IEnumerable<Player> Players() {
		foreach(Player player in players){
			yield return player;
		}
	}

	public void InitializePlayerComponents() {
		if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
			//Read Treaties data
			readTreatyCards ();
		}
		if (gameManager.Active (Option.PoliticalIntrigue)) {
			//Read Promissory Notes data
			readPromNotes();
		}
		foreach (Player player in players) {
			if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
				//Distribute Treaties
				player.Treaties = getTreatyHand(player);
			}
			if (gameManager.Active (Option.PoliticalIntrigue)) {
				player.Notes = getNotesHand(player);
			}
			player.HiddenObjectives.Add (cardManager.GetStartingObjective(player));
		}
	}

	private void readTreatyCards() {
		foreach (Treaty treaty in fileManager.ReadTreatyFile()){
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
}
