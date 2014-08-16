using UnityEngine;

[System.Serializable]
public class Treaty {

	// Variables for logical use
	private const int TIGameProperties = 7;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Treaty: Attempted to set locked property Name."); } } }
	[SerializeField]
	private string flavorText;
	public string FlavorText { get { return flavorText; } set { if (!isLocked[1]) { flavorText = value; isLocked[1] = true; } else { Debug.Log("Treaty: Attempted to set locked property FlavorText."); } } }
	[SerializeField]
	private string rulesText;
	public string RulesText { get { return rulesText; } set { if (!isLocked[2]) { rulesText = value; isLocked[2] = true; } else { Debug.Log("Treaty: Attempted to set locked property RulesText."); } } }
	[SerializeField]
	private string suggestionText;
	public string SuggestionText { get { return suggestionText; } set { if (!isLocked[3]) { suggestionText = value; isLocked[3] = true; } else { Debug.Log("Treaty: Attempted to set locked property SuggestionText."); } } }
	[SerializeField]
	private int rank;
	public int Rank { get { return rank; } set { if (!isLocked[4]) { rank = value; isLocked[4] = true; } else { Debug.Log("Treaty: Attempted to set locked property Rank."); } } }
	[SerializeField]
	private Race race;
	public Race Race { get { return race; } set { if (!isLocked[5]) { race = value; isLocked[5] = true; } else { Debug.Log("Treaty: Attempted to set locked property Race."); } } }
	private Player owner;
	public Player Owner { get { return owner; } set { if (!isLocked[6]) { owner = value; isLocked[6] = true; } else { Debug.Log("Treaty: Attempted to set locked property Owner."); } } }

	public Treaty() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}

	public Treaty DuplicateFor(Player pOwner) {
		Treaty treaty = new Treaty ();
		treaty.Name = name;
		treaty.FlavorText = flavorText;
		treaty.rulesText = rulesText.Replace ("<%race%>", pOwner.Race.ShortName);
		treaty.SuggestionText = suggestionText.Replace ("<%race%>", pOwner.Race.ShortName);
		treaty.Rank = rank;
		treaty.Race = pOwner.Race;
		treaty.Owner = pOwner;
		return treaty;
	}
	
}
