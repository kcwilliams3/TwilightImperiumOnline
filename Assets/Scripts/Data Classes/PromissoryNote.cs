using UnityEngine;

[System.Serializable]
public class PromissoryNote {
	
	// Variables for logical use
	private const int TIGameProperties = 4;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("PromissoryNote: Attempted to set locked property Name."); } } }
	[SerializeField]
	private string flavorText;
	public string FlavorText { get { return flavorText; } set { if (!isLocked[1]) { flavorText = value; isLocked[1] = true; } else { Debug.Log("PromissoryNote: Attempted to set locked property FlavorText."); } } }
	[SerializeField]
	private string playText;
	public string PlayText { get { return playText; } set { if (!isLocked[2]) { playText = value; isLocked[2] = true; } else { Debug.Log("PromissoryNote: Attempted to set locked property PlayText."); } } }
	[SerializeField]
	private string rulesText;
	public string RulesText { get { return rulesText; } set { if (!isLocked[3]) { rulesText = value; isLocked[3] = true; } else { Debug.Log("PromissoryNote: Attempted to set locked property RulesText."); } } }
	
	public PromissoryNote() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
