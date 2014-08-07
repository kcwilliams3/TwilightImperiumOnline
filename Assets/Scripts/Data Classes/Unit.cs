using UnityEngine;

[System.Serializable]
public class Unit {
	
	// Variables for logical use
	protected int TIGameProperties = 4;
	protected bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string unitType;
	public string UnitType {get { return unitType; } set { if (!isLocked[0]) { unitType = value; isLocked[0] = true; } else { Debug.Log("Unit: Attempted to set locked property UnitType."); } } }
	[SerializeField]
	private string[] abilities;
	public string[] Abilities { get { return abilities; } set { if (!isLocked[1]) { abilities = value; isLocked[1] = true; } else { Debug.Log("Unit: Attempted to set locked property Abilities."); } } }
	[SerializeField]
	private int cost;
	public int Cost { get { return cost; } set { if (!isLocked[2]) { cost = value; isLocked[2] = true; } else { Debug.Log("Unit: Attempted to set locked property Cost."); } } }
	[SerializeField]
	private int battle;
	public int Battle { get { return battle; } set { if (!isLocked[3]) { battle = value; isLocked[2] = true; } else { Debug.Log("Unit: Attempted to set locked property Battle."); } } }
	
	public Unit() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}

