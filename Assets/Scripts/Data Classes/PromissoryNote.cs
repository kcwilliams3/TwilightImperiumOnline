using UnityEngine;

[System.Serializable]
public class PromissoryNote {
	
	// Variables for logical use
	private const int TIGameProperties = 6;
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
	private Player owner;
	public Player Owner { get { return owner; } set { if (!isLocked[4]) { owner = value; isLocked[4] = true; } else { Debug.Log("PromissoryNote: Attempted to set locked property Owner."); } } }
	[SerializeField]
	private string id;
	public string Id { get { return id; } set { if (!isLocked[5]) { id = value; isLocked[5] = true; } else { Debug.Log("PromissoryNote: Attempted to set locked property Id."); } } }
	
	public PromissoryNote() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}

	public PromissoryNote DuplicateFor(Player pOwner) {
		PromissoryNote note = new PromissoryNote ();
		note.Name = name;
		note.FlavorText = flavorText;
		note.rulesText = rulesText;
		note.playText = playText;
		note.owner = pOwner;
		return note;
	}
}
