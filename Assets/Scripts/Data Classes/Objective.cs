using UnityEngine;

public enum OType {Preliminary, Secret, PublicStageI, PublicStageII, Special, Lazax, Scenario}
public enum OReward {NONE, VP, WIN, GAMEOVER, INSTANTWIN}

[System.Serializable]
public class Objective {
	
	// Variables for logical use
	private const int TIGameProperties = 8;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Objective: Attempted to set locked property Name."); } } }
	[SerializeField]
	private OType type;
	public OType Type { get { return type; } set { if (!isLocked[1]) { type = value; isLocked[1] = true; } else { Debug.Log("Objective: Attempted to set locked property Type."); } } }
	[SerializeField]
	private Expansion[] expansions;
	public Expansion[] Expansions { get { return expansions; } set { if (!isLocked[2]) { expansions = value; isLocked[2] = true; } else { Debug.Log("Objective: Attempted to set locked property Expansions."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[3]) { text = value; isLocked[3] = true; } else { Debug.Log("Objective: Attempted to set locked property Text."); } } }
	[SerializeField]
	private OReward rewardType;
	public OReward RewardType { get { return rewardType; } set { if (!isLocked[4]) { rewardType = value; isLocked[4] = true; } else { Debug.Log("Objective: Attempted to set locked property RewardType."); } } }
	[SerializeField]
	private int rewardQuantity;
	public int RewardQuantity { get { return rewardQuantity; } set { if (!isLocked[5]) { rewardQuantity = value; isLocked[5] = true; } else { Debug.Log("Objective: Attempted to set locked property RewardQuantity."); } } }
	[SerializeField]
	private bool hasRealName;
	public bool HasRealName { get { return hasRealName; } set { if (!isLocked[6]) { hasRealName = value; isLocked[6] = true; } else { Debug.Log("Objective: Attempted to set locked property HasRealName."); } } }
	[SerializeField]
	private string id;
	public string Id { get { return id; } set { if (!isLocked[7]) { id = value; isLocked[7] = true; } else { Debug.Log("Objective: Attempted to set locked property Id."); } } }

	
	public Objective() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}