using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour {

	private Dictionary<string, ActionCard> actionCards;
	private FileManager fileManager;

	// Use this for initialization
	void Start () {
		fileManager = GetComponent<FileManager>();
		
		readActionCards ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public ActionCard getActionCard(string actionCardName) {
		return actionCards [actionCardName];
	}
	
	private void readActionCards() {
		actionCards = new Dictionary<string, ActionCard>();
		foreach (ActionCard actionCard in fileManager.ReadActionFile ()) {
			actionCards[actionCard.Name] = actionCard;
		};
	}
}
