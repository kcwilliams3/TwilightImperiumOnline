using UnityEngine;

public enum LType {Admiral, General, Scientist, Diplomat, Agent};

[System.Serializable]
public class Leader {
	
	// Variables for logical use
	private const int TIGameProperties = 3;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Leader: Attempted to set locked property Name."); } } }
	[SerializeField]
	private LType leaderType;
	public LType LeaderType { get { return leaderType; } set { if (!isLocked[1]) { leaderType = value; isLocked[1] = true; } else { Debug.Log("Leader: Attempted to set locked property LeaderType."); } } }
	[SerializeField]
	private string id;
	public string Id { get { return id; } set { if (!isLocked[2]) { id = value; isLocked[2] = true; } else { Debug.Log("Leader: Attempted to set locked property Id."); } } }
	
	public Leader() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
