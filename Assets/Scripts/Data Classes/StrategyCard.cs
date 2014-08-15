using UnityEngine;

public enum StrategySet {Vanilla, ShatteredEmpire, FallOfTheEmpire, None};

[System.Serializable]
public class StrategyAbility {
	
	// Variables for logical use
	private const int TIGameProperties = 2;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("StrategyAbility: Attempted to set locked property Name."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[1]) { text = value; isLocked[1] = true; } else { Debug.Log("StrategyAbility: Attempted to set locked property Text."); } } }
	
	public StrategyAbility() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}

[System.Serializable]
public class StrategyCard {
	
	// Variables for logical use
	private const int TIGameProperties = 6;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("StrategyCard: Attempted to set locked property Name."); } } }
	[SerializeField]
	private int initiative;
	public int Initiative { get { return initiative; } set { if (!isLocked[1]) { initiative = value; isLocked[1] = true; } else { Debug.Log("StrategyCard: Attempted to set locked property Iniative."); } } }
	[SerializeField]
	private StrategySet set;
	public StrategySet Set { get { return set; } set { if (!isLocked[2]) { set = value; isLocked[2] = true; } else { Debug.Log("StrategyCard: Attempted to set locked property Set."); } } }
	[SerializeField]
	private StrategyAbility primaryAbility;
	public StrategyAbility PrimaryAbility { get { return primaryAbility; } set { if (!isLocked[3]) { primaryAbility = value; isLocked[3] = true; } else { Debug.Log("StrategyCard: Attempted to set locked property PrimaryAbility."); } } }
	[SerializeField]
	private StrategyAbility secondaryAbility;
	public StrategyAbility SecondaryAbility { get { return secondaryAbility; } set { if (!isLocked[4]) { secondaryAbility = value; isLocked[4] = true; } else { Debug.Log("StrategyCard: Attempted to set locked property SecondaryAbility."); } } }
	[SerializeField]
	private string special;
	public string Special { get { return special; } set { if (!isLocked[5]) { special = value; isLocked[5] = true; } else { Debug.Log("StrategyCard: Attempted to set locked property Special."); } } }
	
	public StrategyCard() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
