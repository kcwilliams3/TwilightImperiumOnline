using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Expansion {Vanilla, ShatteredEmpire, ShardsOfTheThrone};
public enum Option {DistantSuns, TheFinalFrontier, TheLongWar, AgeOfEmpire, Leaders, SabotageRuns, SEObjectives, AllObjectives, RaceSpecificTechnologies, Artifacts, ShockTroops, SpaceMines, WormholeNexus, Facilities, TacticalRetreats, TerritorialDistantSuns, CustodiansOfMecatolRex, VoiceOfTheCouncil, SimulatedEarlyTurns, PreliminaryObjectives, Flagships, MechanizedUnits, Mercenaries, PoliticalIntrigue};
public enum Scenario {StandardGame, FallOfTheEmpire};

public class GameManager : TIOMonoBehaviour {

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<string, StrategyCard> strats = new Dictionary<string, StrategyCard> ();

	//TODO: After finished, get rid of debug arrays.
	//[SerializeField]
	//private Option[] activeOptionsDebugKeys;
	[SerializeField]
	private bool[] activeOptionsDebugValues;
	public Dictionary<Option,bool> ActiveOptions = new Dictionary<Option, bool> ();
	[SerializeField]
	private StrategyCard[] strategyCardsDebug;
	private Dictionary<int, StrategyCard> strategyCards = new Dictionary<int, StrategyCard> ();
	public Expansion Expansion;
	public Scenario Scenario;
	private int playerCount;
	public int PlayerCount { get { return playerCount; } }

	public GameObject HexPrefab;

	private int updateCounter = 0;

	private PlayerManager playerManager;
	private CardManager cardManager;
	private BoardManager boardManager;

	// Use this for initialization
	void Start () {
//		Activate (Option.AllObjectives);
//		Activate (Option.PreliminaryObjectives);
//		Activate (Option.Artifacts);
//		Activate (Option.WormholeNexus);
//		Activate (Option.PoliticalIntrigue);
//		scenario = Scenario.FallOfTheEmpire;
//		playerCount = 7;
//		playerManager = GetComponent<PlayerManager> ();
//		cardManager = GetComponent<CardManager> ();
//		boardManager = GetComponent<BoardManager> ();

		initializeOptions ();
	}
	
	// Update is called once per frame
	void Update () {
//		if (updateCounter == 2) {
//			readStrategyCards ();
//			StrategyCard[] replacements = new StrategyCard[2]{strats["Technology II"],strats["Trade III"]};
//			prepStrategyCards (StrategySet.FallOfTheEmpire, replacements);
//
//			InitializeGame();
//		}
//		updateCounter++;

		//ActiveOptions.Keys.CopyTo (activeOptionsDebugKeys,0);
		ActiveOptions.Values.CopyTo (activeOptionsDebugValues,0);
		strategyCards.Values.CopyTo(strategyCardsDebug,0);
	}

	private void initializeOptions() {
		int count = 0;
		foreach(Option option in (Option[])System.Enum.GetValues(typeof(Option))) {
			ActiveOptions[option] = false;
			count += 1;
		}
		//activeOptionsDebugKeys = new Option[count];
		activeOptionsDebugValues = new bool[count];
	}

	public bool IsActive(Option option) {
		return ActiveOptions[option];
	}

	private void readStrategyCards(){
		foreach (StrategyCard strat in GetComponent<FileManager>().ReadStrategyFile()) {
			strats[strat.Name] = strat;
		}
	}

	private void prepStrategyCards(StrategySet set) {
		foreach (StrategyCard strategyCard in strats.Values){
			if (strategyCard.Set == set) {
				strategyCards[strategyCard.Initiative] = strategyCard;
			}
		}

		if (strategyCards.Values.Count < 8) {
			//Set is incomplete (Fall of the Empire). Fill in with original strategy cards
			foreach (StrategyCard strategyCard in strats.Values){
				if (strategyCard.Set == StrategySet.Vanilla && !strategyCards.ContainsKey (strategyCard.Initiative)) {
					strategyCards[strategyCard.Initiative] = strategyCard;
				}
			}
		}

		strategyCardsDebug = new StrategyCard[8];
	}

	private void prepStrategyCards(StrategySet set, StrategyCard[] replacements) {
		//Prepare set
		prepStrategyCards (set);

		foreach (StrategyCard replacementCard in replacements) {
			strategyCards[replacementCard.Initiative] = replacementCard;
		}
	}

	public StrategyCard ChooseStrategyCard(int initiative) {
		return strategyCards [initiative];
	}

	public void InitializeGame() {
		playerManager.InitializePlayers();
		cardManager.InitializeCards();
		playerManager.InitializePlayerComponents ();
		if (Scenario == Scenario.FallOfTheEmpire) {
			string mapName = "fall" + playerCount.ToString() + "p";
			boardManager.LoadMap (mapName);
		}
	}

	// RPC functions

	[RPC]
	private void RPC_SetPlayerCount(int players) {
		playerCount = players;
	}

	[RPC]
	private void RPC_SetExpansion(int expansion) { //can't pass Expansion via RPC, so it's been cast to an int
		this.Expansion = (Expansion)expansion;
	}

	[RPC]
	private void RPC_SetScenario(int scenario) { //can't pass Scenario via RPC, so it's been cast to an int
		this.Scenario = (Scenario)scenario;
	}

	[RPC]
	private void RPC_SetOption(int option, bool boolean) { //can't pass Option via RPC, so it's been cast to an int
		ActiveOptions [(Option)option] = boolean;
	}

	[RPC]
	private void RPC_StartGame() {
		((TechManager)this.gameObject.AddComponent ("TechManager")).Initialize();
		((UnitManager)this.gameObject.AddComponent ("UnitManager")).Initialize();
		cardManager = (CardManager)this.gameObject.AddComponent ("CardManager");
		playerManager = (PlayerManager)this.gameObject.AddComponent ("PlayerManager");
		boardManager = (BoardManager)this.gameObject.AddComponent ("BoardManager");
		boardManager.Initialize ();
		this.gameObject.AddComponent ("ComponentManager");
		((CameraManager)GameObject.Find("Main Camera").GetComponent<CameraManager>()).SetInGameBackground ();
		InitializeGame ();
	}
}
