using UnityEngine;

[System.Serializable]
public class Tech {
	
	// Variables for logical use
	private int TIGameProperties = 4;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Tech: Attempted to set locked property Name."); } } }
	[SerializeField]
	private string expansion;
	public string Expansion { get { return expansion; } set { if (!isLocked[1]) { expansion = value; isLocked[1] = true; } else { Debug.Log("Tech: Attempted to set locked property Expansion."); } } }
	[SerializeField]
	private int cost;
	public int Cost { get { return cost; } set { if (!isLocked[2]) { cost = value; isLocked[2] = true; } else { Debug.Log("Tech: Attempted to set locked property Cost."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[3]) { text = value; isLocked[2] = true; } else { Debug.Log("Tech: Attempted to set locked property Text."); } } }
	
	public Tech() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
