using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CardManager : TIOMonoBehaviour {

	// Card directories
	//TODO: May not need these dictionaries. Decide.
	private Dictionary<string, ActionCard> actionCards = new Dictionary<string, ActionCard>();
	private Dictionary<string, Merc> mercs = new Dictionary<string, Merc>();
	private Dictionary<string, Objective> objs = new Dictionary<string, Objective>();
	private Dictionary<string, PoliticalCard> politicalCards = new Dictionary<string, PoliticalCard>();
	private Dictionary<string, PoliticalCard> agendas = new Dictionary<string, PoliticalCard>();

	// Decks
	//TODO: After finished, get rid of debug arrays.
	[SerializeField]
	private ActionCard[] actionDeckDebug; 
	private ArrayList actionDeck = new ArrayList();
	[SerializeField]
	private Merc[] mercDeckDebug;
	private ArrayList mercDeck = new ArrayList();
	[SerializeField]
	private Objective[] pubObjDeckDebug;
	private ArrayList pubObjDeck = new ArrayList ();
	[SerializeField]
	private Objective[] prelimObjsDebug;
	private ArrayList prelimObjs = new ArrayList ();
	[SerializeField]
	private Objective[] secretObjsDebug;
	private ArrayList secretObjs = new ArrayList ();
	[SerializeField]
	private Objective[] specialObjsDebug;
	private ArrayList specialObjs = new ArrayList ();
	[SerializeField]
	private Objective[] scenarioObjsDebug;
	private ArrayList scenarioObjs = new ArrayList ();
	private Objective lazaxObj;
	[SerializeField]
	private PoliticalCard[] politicalDeckDebug; 
	private ArrayList politicalDeck = new ArrayList();

	// Discard piles
	//TODO: After finished, get rid of debug arrays.
	[SerializeField]
	private ActionCard[] actionDiscDebug; 
	private ArrayList actionDisc = new ArrayList();
	[SerializeField]
	private Merc[] mercDiscDebug;
	private ArrayList mercDisc = new ArrayList();
	[SerializeField]
	private PoliticalCard[] politicalDiscDebug;
	private ArrayList politicalDisc = new ArrayList();

	private bool isReady;
	public bool IsReady { get { return isReady; } }
	
	private GameManager gameManager;

	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		actionDeckDebug = (ActionCard[])actionDeck.ToArray (typeof(ActionCard));
		mercDeckDebug = (Merc[])mercDeck.ToArray (typeof(Merc));
		politicalDeckDebug = (PoliticalCard[])politicalDeck.ToArray (typeof(PoliticalCard));
		actionDiscDebug = (ActionCard[])actionDisc.ToArray (typeof(ActionCard));
		mercDiscDebug = (Merc[])mercDisc.ToArray (typeof(Merc));
		politicalDiscDebug = (PoliticalCard[])politicalDisc.ToArray (typeof(PoliticalCard));

		pubObjDeckDebug = (Objective[])pubObjDeck.ToArray (typeof(Objective));
		prelimObjsDebug = (Objective[])prelimObjs.ToArray (typeof(Objective));
		secretObjsDebug = (Objective[])secretObjs.ToArray (typeof(Objective));
		specialObjsDebug = (Objective[])specialObjs.ToArray (typeof(Objective));
		scenarioObjsDebug = (Objective[])scenarioObjs.ToArray (typeof(Objective));
	}

	public void Initialize() {
		//Initialize Action Cards
		readActionCards ();

		if (gameManager.IsActive(Option.Mercenaries)) {
			//Initialize Mercenaries
			readMercCards ();
		}

		//Initialize Political Cards/Agendas
		readPoliticalCards ();

		//Initialize Objectives
		readObjCards ();
	}

	public void PrepareObjectives() {
		prepObjectives ();
	}

	private void readActionCards() {
		int deckSize = 0;
		foreach (ActionCard actionCard in gameManager.FileMgr.ReadActionFile ()) {
			actionCards[actionCard.Name] = actionCard;
			deckSize += actionCard.Quantity;
		}
		actionDeckDebug = new ActionCard[deckSize];
	}

	private void readMercCards() {
		int deckSize = 0;
		foreach (Merc merc in gameManager.FileMgr.ReadMercFile()){
			mercs[merc.Name] = merc;
			deckSize += 1;
		}
		mercDeckDebug = new Merc[deckSize];
	}

	private void readObjCards(){
		foreach (Objective obj in gameManager.FileMgr.ReadObjectiveFile()){
			objs[obj.Name] = obj;
		}
	}

	private void readPoliticalCards() {
		int deckSize = 0;
		PoliticalCard[] cards = new PoliticalCard[1];
		if (gameManager.Scenario == Scenario.StandardGame) {
			cards = gameManager.FileMgr.ReadPoliticalFile();
		} else if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
			cards = gameManager.FileMgr.ReadAgendaFile();
		}
		foreach (PoliticalCard politicalCard in cards){
			if (politicalCards.ContainsKey(politicalCard.Name)) {
				politicalCards[politicalCard.Name + "/" + politicalCard.Expansion] = politicalCard;
			} else {
				politicalCards[politicalCard.Name] = politicalCard;
			}
			deckSize += 1;
		}
		politicalDeckDebug = new PoliticalCard[deckSize];
	}

	private void prepActionDeck() {
		foreach (ActionCard actionCard in actionCards.Values) {
			for (int j=0; j < actionCard.Quantity; j++) {
				actionDeck.Add (actionCard);
			}
		}
		ShuffleActionDeck ();
	}

	private void prepMercDeck() {
		foreach (Merc merc in mercs.Values){
			mercDeck.Add(merc);
		}
		ShuffleMercDeck ();
	}

	private void prepPoliticalDeck() {
		foreach (PoliticalCard politicalCard in politicalCards.Values){
			politicalDeck.Add(politicalCard);
		}
		ShufflePoliticalDeck ();
	}

	private void prepObjectives() {
		//Accumulate all valid objectives (based on game options)
		ArrayList validStageIObjectives = new ArrayList();
		ArrayList validStageIIObjectives = new ArrayList();
		Objective imperiumRexCard = new Objective();
		foreach (Objective obj in objs.Values){
			switch (obj.Type) {
				case OType.PublicStageI:
					if (validPublicObjective(obj)) {
						// Valid Stage I Public Objective
						validStageIObjectives.Add (obj);
					}
					break;
				case OType.PublicStageII:
					if (validPublicObjective(obj)) {
						// Valid Stage II Public Objective
						if (obj.Id == "Imperium Rex") {
							imperiumRexCard = obj;
						} else {
							validStageIIObjectives.Add (obj);
						}
					}
					break;
				case OType.Preliminary:
					if (gameManager.IsActive (Option.PreliminaryObjectives)) {
						// Prelim Objective & using Prelim Objectives
						prelimObjs.Add (obj);
					}
					break;
				case OType.Secret:
					// Secret Objectives (always used)
					secretObjs.Add (obj);
					break;
				case OType.Special:
					if (validSpecialObjective(obj)) {
						// Valid Special Objective
						specialObjs.Add(obj);
					}
					break;
				case OType.Scenario:
					scenarioObjs.Add (obj);
					break;
				case OType.Lazax:
					lazaxObj = obj;
					break;
			}
		}

		if (gameManager.Scenario != Scenario.FallOfTheEmpire) {
			// Build Public Objectives deck
			//		Choose Stage I objectives
			ArrayList stageIdeck = new ArrayList();
			ShuffleDeck<Objective>(validStageIObjectives);
			while (stageIdeck.Count < 6) {
				stageIdeck.Add (DrawCard<Objective>(validStageIObjectives));
			}
			ShuffleDeck<Objective>(stageIdeck);
			//		Choose Stage II objectives
			ArrayList stageIIdeck = new ArrayList();
			stageIIdeck.Add (imperiumRexCard);
			ShuffleDeck<Objective>(validStageIIObjectives);
			while (stageIIdeck.Count < 4) {
				stageIIdeck.Add (DrawCard<Objective>(validStageIIObjectives));
			}
			ShuffleDeck<Objective>(stageIIdeck);
			//		Combine into one Public Objectives deck
			pubObjDeck.AddRange(stageIdeck);
			pubObjDeck.AddRange(stageIIdeck);
		}

		ShuffleHiddenObjectives();
	}

	private bool validPublicObjective(Objective obj) {
		if (gameManager.IsActive(Option.AllObjectives)) {
			return true;
		} else if (gameManager.IsActive (Option.SEObjectives)) {
			foreach(Expansion exp in obj.Expansions) {
				if (exp == Expansion.ShatteredEmpire) {
					return true;
				}
			}
		} else {
			foreach(Expansion exp in obj.Expansions) {
				if (exp == Expansion.Vanilla) {
					return true;
				}
			}
		}
		return false;
	}

	private bool validSpecialObjective(Objective obj) {
		if (gameManager.IsActive (Option.Artifacts) && (obj.Id == "Lazax Armory" || obj.Id == "Precursor Fossil" || obj.Id == "Ancient Shipwreck" || obj.Id == "Imperial Datacache")) {
			// Artifact & using the artifacts option
			return true;
		} else if (gameManager.IsActive (Option.VoiceOfTheCouncil) && (obj.Id == "Voice of the Council")) {
			// Voice of the Council & using the VotC option
			return true;
		} else {
			return false;
		}
	}

	public void ShuffleDeck<T>(ArrayList deck) {
		for (int i=deck.Count-1; i > 0; i--) {
			int targetIndex = Random.Range(0,i+1);
			T tempCard = (T)deck[i];
			deck[i] = deck[targetIndex];
			deck[targetIndex] = tempCard;
		}
	}

	public void ShuffleActionDeck() {
		ShuffleDeck<ActionCard> (actionDeck);
	}

	public void ShuffleMercDeck() {
		ShuffleDeck<Merc> (mercDeck);
	}

	public void ShufflePoliticalDeck() {
		ShuffleDeck<PoliticalCard> (politicalDeck);
	}

	public void ShuffleHiddenObjectives() {
		if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
			ShuffleDeck<Objective>(scenarioObjs);
		} else {
			if (gameManager.IsActive (Option.PreliminaryObjectives)) {
				ShuffleDeck<Objective>(prelimObjs);
			}
			ShuffleDeck<Objective>(secretObjs);
		} 
		
	}

	public T DrawCard<T>(ArrayList deck) {
		T card = (T)deck [0];
		deck.Remove (card);
		return card;
	}

	private T DrawCard<T>(ArrayList deck, ArrayList discPile) {
		if (deck.Count > 0) {
			T card = (T)deck [0];
			deck.Remove (card);
			return card;
		} else {
			deck.AddRange(discPile);
			discPile.RemoveRange(0, discPile.Count);
			ShuffleDeck<T>(deck);
			return DrawCard<T>(deck, discPile);
		}
	}

	public ActionCard DrawActionCard() {
		return DrawCard<ActionCard>(actionDeck, actionDisc);
	}

	public Merc DrawMerc() {
		return DrawCard<Merc>(mercDeck, mercDisc);
	}

	public PoliticalCard DrawPoliticalCard() {
		return DrawCard<PoliticalCard> (politicalDeck, politicalDisc);
	}

	private T discardCard<T>(T card, ArrayList discPile) {
		discPile.Add (card);
		return card;
	}

	public ActionCard DiscardActionCard(ActionCard card) {
		return discardCard<ActionCard> (card, actionDisc);
	}

	public Merc DiscardMerc(Merc card) {
		return discardCard<Merc> (card, mercDisc);
	}

	public PoliticalCard DiscardPoliticalCard(PoliticalCard card) {
		return discardCard<PoliticalCard> (card, politicalDisc);
	}

	private T putOnBottom<T>(T card, ArrayList deck) {
		deck.Add (card);
		return card;
	}

	public Merc PutMercOnBottom(Merc card) {
		return putOnBottom<Merc>(card, mercDeck);
	}

	private T putOnTop<T>(T card, ArrayList deck) {
		deck.Insert (0, card);
		return card;
	}

	private T searchFor<T>(T card, ArrayList deck) {
		ShuffleDeck<T> (deck);
		if (deck.Contains (card)){
			deck.Remove (card);
			return card;
		} else {
			return default(T);
		}
	}

	public ActionCard SearchForAction(ActionCard action) {
		return (ActionCard)searchFor<ActionCard>(action, actionDeck);
	}

	public Objective GetStartingObjective(Player player) {
		if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
			if (player.Race.Id == "Lazax") {
				return lazaxObj;
			} else {
				Debug.Log ("SCENARIOOBJS: " + scenarioObjs.Count);
				return DrawCard<Objective> (scenarioObjs);
			}
		} else if (gameManager.IsActive (Option.PreliminaryObjectives)) {
			return DrawCard<Objective> (prelimObjs);
		} else {
			return DrawCard<Objective> (secretObjs);
		}
	}
}
