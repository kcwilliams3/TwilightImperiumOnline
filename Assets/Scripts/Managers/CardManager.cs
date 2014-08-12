using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour {

	private Dictionary<string, ActionCard> actionCards = new Dictionary<string, ActionCard>();
	private Dictionary<string, Merc> mercs = new Dictionary<string, Merc>();
	private FileManager fileManager;

	// Use this for initialization
	void Start () {
		fileManager = GetComponent<FileManager>();
		
		readActionCards ();
		readMercCards ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public ActionCard getActionCard(string actionCardName) {
		return actionCards [actionCardName];
	}

	public Merc getMerc(string mercName) {
		return mercs [mercName];
	}
	
	private void readActionCards() {
		foreach (ActionCard actionCard in fileManager.ReadActionFile ()) {
			actionCards[actionCard.Name] = actionCard;
		}
	}

	private void readMercCards() {
		foreach (Merc merc in fileManager.ReadMercFile()){
			mercs[merc.Name] = merc;
		}
	}
}
