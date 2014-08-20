using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Expansion {Vanilla, ShatteredEmpire, ShardsOfTheThrone};
public enum Option {DistantSuns, TheFinalFrontier, TheLongWar, AgeOfEmpire, Leaders, SabotageRuns, SEObjectives, AllObjectives, RaceSpecificTechnologies, Artifacts, ShockTroops, SpaceMines, WormholeNexus, Facilities, TacticalRetreats, TerritorialDistantSuns, CustodiansOfMecatolRex, VoiceOfTheCouncil, SimulatedEarlyTurns, PreliminaryObjectives, Flagships, MechanizedUnits, Mercenaries, PoliticalIntrigue};
public enum Scenario {StandardGame, FallOfTheEmpire};

public class GameManager : TIOMonoBehaviour {

	//TODO: A: Move language dependency to language manager
	private string language = "English";
	public string Language { get { return language; } }

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<string, StrategyCard> strats = new Dictionary<string, StrategyCard> ();

	//TODO: After finished, get rid of debug arrays.
	[SerializeField]
	private Option[] activeOptionsDebug;
	private ArrayList activeOptions = new ArrayList ();
	[SerializeField]
	private StrategyCard[] strategyCardsDebug;
	private Dictionary<int, StrategyCard> strategyCards = new Dictionary<int, StrategyCard> ();
	[SerializeField]
	private Scenario scenario;
	public Scenario Scenario { get { return scenario; } }
	[SerializeField]
	private int playerCount;
	public int PlayerCount { get { return playerCount; } }

	private bool first = true;

	private PlayerManager playerManager;
	private CardManager cardManager;
	private BoardManager boardManager;

	// Use this for initialization
	void Start () {
		Activate (Option.AllObjectives);
		Activate (Option.PreliminaryObjectives);
		Activate (Option.Artifacts);
		Activate (Option.WormholeNexus);
		Activate (Option.PoliticalIntrigue);
		scenario = Scenario.FallOfTheEmpire;
		playerCount = 7;
		playerManager = GetComponent<PlayerManager> ();
		cardManager = GetComponent<CardManager> ();
		boardManager = GetComponent<BoardManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (first) {
			first = false;
			readStrategyCards ();
			StrategyCard[] replacements = new StrategyCard[2]{strats["Technology II"],strats["Trade III"]};
			prepStrategyCards (StrategySet.FallOfTheEmpire, replacements);

			InitializeGame();
		}

		activeOptionsDebug = (Option[])activeOptions.ToArray (typeof(Option));
		strategyCards.Values.CopyTo(strategyCardsDebug,0);
	}

	public void Activate(Option option) {
		if (!Active (option)){
			activeOptions.Add (option);
		}
	}

	public bool Active(Option option) {
		return activeOptions.Contains(option);
	}

	public void Deactivate(Option option) {
		if (Active (option)){
			activeOptions.Remove(option);
		}
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
		if (scenario == Scenario.FallOfTheEmpire) {
			string mapName = "fall" + playerCount.ToString() + "p";
			boardManager.LoadMap (mapName);
		}
	}
}
