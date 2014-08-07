using UnityEngine;

[System.Serializable]
public class Planet {
	
	// Variables for logical use
	private int TIGameProperties = 4;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Planet: Attempted to set locked property Name."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[1]) { text = value; isLocked[1] = true; } else { Debug.Log("Planet: Attempted to set locked property Text."); } } }
	[SerializeField]
	private int resources;
	public int Resources { get { return resources; } set { if (!isLocked[2]) { resources = value; isLocked[2] = true; } else { Debug.Log("Planet: Attempted to set locked property Resources."); } } }
	[SerializeField]
	private int influence;
	public int Influence { get { return influence; } set { if (!isLocked[3]) { influence = value; isLocked[2] = true; } else { Debug.Log("Planet: Attempted to set locked property Influence."); } } }

	public Planet() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
