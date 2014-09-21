using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Expansion {Vanilla, ShatteredEmpire, ShardsOfTheThrone};
public enum Option {DistantSuns, TheFinalFrontier, TheLongWar, AgeOfEmpire, Leaders, SabotageRuns, SEObjectives, AllObjectives, RaceSpecificTechnologies, Artifacts, ShockTroops, SpaceMines, WormholeNexus, Facilities, TacticalRetreats, TerritorialDistantSuns, CustodiansOfMecatolRex, VoiceOfTheCouncil, SimulatedEarlyTurns, PreliminaryObjectives, Flagships, MechanizedUnits, Mercenaries, PoliticalIntrigue};
public enum Scenario {StandardGame, FallOfTheEmpire};

public enum GameStage {LoadingLevel, LevelLoaded, InitializingManagers, ManagersInitialized, InitializingOther, Playing, Menus}; 

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
	
	GameStage stage = GameStage.LoadingLevel;

	//public int readyCount = 0;

	private int updateCounter = 0;
	
	public BoardManager BoardMgr;
	public CameraManager CameraMgr;
	public CardManager CardMgr;
	public ComponentManager ComponentMgr;
	public FileManager FileMgr;
	public LanguageManager LanguageMgr;
	public NetworkManager NetworkMgr;
	public PlayerManager PlayerMgr;
	public TechManager TechMgr;
	public UnitManager UnitMgr;
	public UIManager UIMgr;

	// Use this for initialization
	void Start () {
		BoardMgr = GetComponent<BoardManager> ();
		CardMgr = GetComponent<CardManager> ();
		CameraMgr = GetComponent<CameraManager> ();
		ComponentMgr = GetComponent<ComponentManager> ();
		FileMgr = GetComponent<FileManager> ();
		LanguageMgr = GetComponent<LanguageManager> ();
		NetworkMgr = GetComponent<NetworkManager> ();
		PlayerMgr = GetComponent<PlayerManager> ();
		TechMgr = GetComponent<TechManager> ();
		UnitMgr = GetComponent<UnitManager> ();
		UIMgr = GetComponent<UIManager> ();

		initializeOptions ();
	}
	
	// Update is called once per frame
	void Update () {
		ActiveOptions.Values.CopyTo(activeOptionsDebugValues,0);
		strategyCards.Values.CopyTo(strategyCardsDebug,0);
		//Debug.Log (stage);
		// Game setup, early initializations
		if (stage == GameStage.LevelLoaded) {
			stage = GameStage.InitializingManagers;
			initializeManagers();
		} else if (stage == GameStage.ManagersInitialized) {
			stage = GameStage.InitializingOther;
			if (NetworkMgr.IsMasterClient()) {
				PlayerMgr.InitializePlayers();
				CardMgr.PrepareDecks();
				ComponentMgr.PrepareComponents();
			} else {
				CardMgr.ReadyToPlay = true;
				ComponentMgr.ReadyToPlay = true;
			}
		}

		if (PlayerMgr.ReadyToPlay && CardMgr.ReadyToPlay && ComponentMgr.ReadyToPlay && stage == GameStage.InitializingOther) {
			stage = GameStage.Menus;
		}
	}

	public void SetStage(GameStage pStage) {
		stage = pStage;
	}

	public GameStage GetStage() {
		return stage;
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

	private void initializeManagers () {
		LanguageMgr.Initialize ();
		CameraMgr.Initialize ();
		TechMgr.Initialize (); //Dependency: Language file
		BoardMgr.Initialize (); //Dependency: Language file
		CardMgr.Initialize (); //Dependency: Language file
		readStrategyCards (); //Dependency: Language file
		ComponentMgr.Initialize (); //Dependency: Language file
		UnitMgr.Initialize (); //Dependency: Tech file
		stage = GameStage.ManagersInitialized;
	}

	private void gameSetup() {
		//1: Race selection --
		//2: Color selection --
		//3: Set up "common play area" (Action Cards, Political Cards, and Supplement Counters.) --
		//4: Players take their Home System planets.
		//5: Set up "Trade Supply" --
		//6: Place Strategy Cards in the "common play area"
		//7: Prepare the Objective Cards
		//8: Set up Victory Point Track
		//9: Set up the galaxy
		//10: Place setup units and receive starting techs
		//11: Place starting command counters

//		CardMgr.PrepareObjectives ();
//		PlayerMgr.InitializePlayerComponents ();
//		if (Scenario == Scenario.FallOfTheEmpire) {
//			string mapName = "fall" + playerCount.ToString() + "p";
//			BoardMgr.LoadMap (mapName);
//		} else {
//
//		}

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

//	[RPC]
//	private void RPC_RacesReady() {
//		readyCount += 1;
//		Debug.Log (readyCount);
//		if ((readyCount == playerCount) && NetworkMgr.IsMasterClient()) {
//			Debug.Log ("GOGOGOGO");
//			UIMgr.networkView.RPC ("RPC_CloseRaceSelection", PhotonTargets.All);
//		}
//	}
}
