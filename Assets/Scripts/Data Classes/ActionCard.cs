using UnityEngine;

[System.Serializable]
public class ActionCard {
	
	// Variables for logical use
	private const int TIGameProperties = 7;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("ActionCard: Attempted to set locked property Name."); } } }
	[SerializeField]
	private int quantity;
	public int Quantity { get { return quantity; } set { if (!isLocked[1]) { quantity = value; isLocked[1] = true; } else { Debug.Log("ActionCard: Attempted to set locked property Quantity."); } } }
	[SerializeField]
	private Expansion expansion;
	public Expansion Expansion { get { return expansion; } set { if (!isLocked[2]) { expansion = value; isLocked[2] = true; } else { Debug.Log("ActionCard: Attempted to set locked property Expansion."); } } }
	[SerializeField]
	private string flavorText;
	public string FlavorText { get { return flavorText; } set { if (!isLocked[3]) { flavorText = value; isLocked[3] = true; } else { Debug.Log("ActionCard: Attempted to set locked property FlavorText."); } } }
	[SerializeField]
	private string[] rulesText;
	public string[] RulesText { get { return rulesText; } set { if (!isLocked[4]) { rulesText = value; isLocked[4] = true; } else { Debug.Log("ActionCard: Attempted to set locked property RulesText."); } } }
	[SerializeField]
	private string playText;
	public string PlayText { get { return playText; } set { if (!isLocked[5]) { playText = value; isLocked[5] = true; } else { Debug.Log("ActionCard: Attempted to set locked property PlayText."); } } }
	[SerializeField]
	private string[] discardText;
	public string[] DiscardText { get { return discardText; } set { if (!isLocked[6]) { discardText = value; isLocked[6] = true; } else { Debug.Log("ActionCard: Attempted to set locked property DiscardText."); } } }
	
	public ActionCard() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
