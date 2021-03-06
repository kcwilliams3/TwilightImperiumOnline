using UnityEngine;

public enum TType {Red, Blue, Yellow, Green, Racial}
public enum TPrereqMode {SINGULARorNONE, AND, OR}

[System.Serializable]
public class Tech {
	
	// Variables for logical use
	private const int TIGameProperties = 9;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Tech: Attempted to set locked property Name."); } } }
	[SerializeField]
	private TType techType;
	public TType TechType { get { return techType; } set { if (!isLocked[1]) { techType = value; isLocked[1] = true; } else { Debug.Log("Tech: Attempted to set locked property TechType."); } } }
	[SerializeField]
	private Expansion expansion;
	public Expansion Expansion { get { return expansion; } set { if (!isLocked[2]) { expansion = value; isLocked[2] = true; } else { Debug.Log("Tech: Attempted to set locked property Expansion."); } } }
	[SerializeField]
	private Tech[] prereqs;
	public Tech[] Prereqs { get { return prereqs; } set { if (!isLocked[3]) { prereqs = value; isLocked[3] = true; } else { Debug.Log("Tech: Attempted to set locked property Prereqs."); } } }
	[SerializeField]
	private TPrereqMode prereqMode = TPrereqMode.SINGULARorNONE;
	public TPrereqMode PrereqMode { get { return prereqMode; } set { if (!isLocked[4]) { prereqMode = value; isLocked[4] = true; } else { Debug.Log("Tech: Attempted to set locked property PrereqMode."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[5]) { text = value; isLocked[5] = true; } else { Debug.Log("Tech: Attempted to set locked property Text."); } } }
	//Racial tech fields
	[SerializeField]
	private int cost;
	public int Cost { get { return cost; } set { if (!isLocked[6]) { cost = value; isLocked[6] = true; } else { Debug.Log("Tech: Attempted to set locked property Cost."); } } }
	[SerializeField]
	private Race race;
	public Race Race { get { return race; } set { if (!isLocked[7]) { race = value; isLocked[7] = true; } else { Debug.Log("Tech: Attempted to set locked property Race."); } } }
	[SerializeField]
	private string id;
	public string Id { get { return id; } set { if (!isLocked[8]) { id = value; isLocked[8] = true; } else { Debug.Log("Tech: Attempted to set locked property Id."); } } }
	
	public Tech() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
