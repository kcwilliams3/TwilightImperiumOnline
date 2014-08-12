using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CardManager : MonoBehaviour {

	// Card directories
	private Dictionary<string, ActionCard> actionCards = new Dictionary<string, ActionCard>();
	private Dictionary<string, Merc> mercs = new Dictionary<string, Merc>();

	// Decks
	[SerializeField]
	private ActionCard[] actionDeckDebug; 
	private ArrayList actionDeck = new ArrayList();
	[SerializeField]
	private Merc[] mercDeckDebug;
	private ArrayList mercDeck = new ArrayList();

	// Discard piles
	[SerializeField]
	private ActionCard[] actionDiscDebug; 
	private ArrayList actionDisc = new ArrayList();
	[SerializeField]
	private Merc[] mercDiscDebug;
	private ArrayList mercDisc = new ArrayList();

	ActionCard testcard = new ActionCard ();


	private int updateCounter = 0;

	private FileManager fileManager;

	// Use this for initialization
	void Start () {
		fileManager = GetComponent<FileManager>();

		readActionCards ();
		readMercCards ();
		prepActionDeck ();
		prepMercDeck ();
	}
	
	// Update is called once per frame
	void Update () {
		if (updateCounter == 0) {
			testcard = (ActionCard)actionDeck[0];
			Debug.Log ("Searching for " + testcard.Name);
		} else if (updateCounter % 100 == 0) {
			ActionCard card = SearchForAction (testcard);
			if (card != default(ActionCard)) {
				Debug.Log (card.Name);
			}
			else {
				Debug.Log ("Not found!");
			}
		} 
		updateCounter ++;

		actionDeckDebug = (ActionCard[])actionDeck.ToArray (typeof(ActionCard));
		mercDeckDebug = (Merc[])mercDeck.ToArray (typeof(Merc));
		actionDiscDebug = (ActionCard[])actionDisc.ToArray (typeof(ActionCard));
		mercDiscDebug = (Merc[])mercDisc.ToArray (typeof(Merc));
	}

	public ActionCard getActionCard(string actionCardName) {
		return actionCards [actionCardName];
	}

	public Merc getMerc(string mercName) {
		return mercs [mercName];
	}
	
	private void readActionCards() {
		int deckSize = 0;
		foreach (ActionCard actionCard in fileManager.ReadActionFile ()) {
			actionCards[actionCard.Name] = actionCard;
			deckSize += actionCard.Quantity;
		}
		actionDeckDebug = new ActionCard[deckSize];
	}

	private void readMercCards() {
		int deckSize = 0;
		foreach (Merc merc in fileManager.ReadMercFile()){
			mercs[merc.Name] = merc;
			deckSize += 1;
		}
		mercDeckDebug = new Merc[deckSize];
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

	private void shuffleDeck<T>(ArrayList deck) {
		for (int i=deck.Count-1; i > 0; i--) {
			int targetIndex = Random.Range(0,i+1);
			T tempCard = (T)deck[i];
			deck[i] = deck[targetIndex];
			deck[targetIndex] = tempCard;
		}
	}

	public void ShuffleActionDeck() {
		shuffleDeck<ActionCard> (actionDeck);
	}

	public void ShuffleMercDeck() {
		shuffleDeck<Merc> (mercDeck);
	}

	private T drawCard<T>(ArrayList deck, ArrayList discPile) {
		if (deck.Count > 0) {
			T card = (T)deck [0];
			deck.Remove (card);
			return card;
		} else {
			deck.AddRange(discPile);
			discPile.RemoveRange(0, discPile.Count);
			shuffleDeck<T>(deck);
			return drawCard<T>(deck, discPile);
		}
	}

	public ActionCard DrawActionCard() {
		return drawCard<ActionCard>(actionDeck, actionDisc);
	}

	public Merc DrawMerc() {
		return drawCard<Merc>(mercDeck, mercDisc);
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
}
