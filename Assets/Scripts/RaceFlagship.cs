using UnityEngine;

public class Flagship : Unit {
	
	// TI Game Data
	private string name;
	public string Name { get { return name; } set { if (!isLocked[TIGameProperties]) { name = value; isLocked[TIGameProperties] = true; } else { Debug.Log("Flagship: Attempted to set locked property Name."); } } }
	private string text;
	public string Text { get { return text; } set { if (!isLocked[TIGameProperties+1]) { text = value; isLocked[TIGameProperties+1] = true; } else { Debug.Log("Flagship: Attempted to set locked property Text."); } } }
	private int multiplier;
	public int Multiplier { get { return multiplier; } set { if (!isLocked[TIGameProperties+2]) { multiplier = value; isLocked[TIGameProperties+1] = true; } else { Debug.Log("Flagship: Attempted to set locked property Multiplier."); } } }
	
	public Flagship() {
		TIGameProperties += 3;
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}

