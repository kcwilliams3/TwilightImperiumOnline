using UnityEngine;

public class Leader {
	
	// Variables for logical use
	private int TIGameProperties = 2;
	private bool[] isLocked;
	
	// TI Game Data
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Leader: Attempted to set locked property Name."); } } }
	private string leaderType;
	public string LeaderType { get { return leaderType; } set { if (!isLocked[1]) { leaderType = value; isLocked[1] = true; } else { Debug.Log("Leader: Attempted to set locked property LeaderType."); } } }
	
	public Leader() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
