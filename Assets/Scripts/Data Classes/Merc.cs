using UnityEngine;

[System.Serializable]
public class Merc {
	
	// Variables for logical use
	private const int TIGameProperties = 10;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Merc: Attempted to set locked property Name."); } } }
	[SerializeField]
	private bool sustainDamage;
	public bool SustainDamage { get { return sustainDamage; } set { if (!isLocked[1]) { sustainDamage = value; isLocked[1] = true; } else { Debug.Log("Merc: Attempted to set locked property SustainDamage."); } } }
	[SerializeField]
	private int evasion;
	public int Evasion { get { return evasion; } set { if (!isLocked[2]) { evasion = value; isLocked[2] = true; } else { Debug.Log("Merc: Attempted to set locked property Evasion."); } } }
	[SerializeField]
	private int capacity;
	public int Capacity { get { return capacity; } set { if (!isLocked[3]) { capacity = value; isLocked[3] = true; } else { Debug.Log("Merc: Attempted to set locked property Capacity."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[4]) { text = value; isLocked[4] = true; } else { Debug.Log("Merc: Attempted to set locked property Text."); } } }
	[SerializeField]
	private int spaceBattle;
	public int SpaceBattle { get { return spaceBattle; } set { if (!isLocked[5]) { spaceBattle = value; isLocked[5] = true; } else { Debug.Log("Merc: Attempted to set locked property SpaceBattle."); } } }
	[SerializeField]
	private int spaceShots;
	public int SpaceShots { get { return spaceShots; } set { if (!isLocked[6]) { spaceShots = value; isLocked[6] = true; } else { Debug.Log("Merc: Attempted to set locked property SpaceShots."); } } }
	[SerializeField]
	private int groundBattle;
	public int GroundBattle { get { return groundBattle; } set { if (!isLocked[7]) { groundBattle = value; isLocked[7] = true; } else { Debug.Log("Merc: Attempted to set locked property GroundBattle."); } } }
	[SerializeField]
	private int groundShots;
	public int GroundShots { get { return groundShots; } set { if (!isLocked[8]) { groundShots = value; isLocked[8] = true; } else { Debug.Log("Merc: Attempted to set locked property GroundShots."); } } }
	[SerializeField]
	private int movement;
	public int Movement { get { return movement; } set { if (!isLocked[9]) { movement = value; isLocked[9] = true; } else { Debug.Log("Merc: Attempted to set locked property Movement."); } } }


	public Merc() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
