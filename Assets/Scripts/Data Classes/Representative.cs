using UnityEngine;

public enum RType {Spy, Councilor, Bodyguard};

[System.Serializable]
public class Representative {
	
	// Variables for logical use
	protected int TIGameProperties = 4;
	protected bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Representative: Attempted to set locked property Name."); } } }
	[SerializeField]
	private int votes;
	public int Votes { get { return votes; } set { if (!isLocked[1]) { votes = value; isLocked[1] = true; } else { Debug.Log("Representative: Attempted to set locked property Votes."); } } }
	[SerializeField]
	private RType[] repTypes;
	public RType[] RepTypes { get { return repTypes; } set { if (!isLocked[2]) { repTypes = value; isLocked[2] = true; } else { Debug.Log("Representative: Attempted to set locked property RepTypes."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[3]) { text = value; isLocked[2] = true; } else { Debug.Log("Representative: Attempted to set locked property Text."); } } }
	
	public Representative() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}