using UnityEngine;
using System.Collections;

public class Wormhole {

	// Variables for logical use
	private const int TIGameProperties = 3;
	private bool[] isLocked;

	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("Wormhole: Attempted to set locked property Name."); } } }
	[SerializeField]
	private ArrayList systems = new ArrayList();
	public ArrayList Systems { get { return systems; } set { if (!isLocked[1]) { systems = value; isLocked[1] = true; } else { Debug.Log("Wormhole: Attempted to set locked property Systems."); } } }
	[SerializeField]
	private string id;
	public string Id { get { return Id; } set { if (!isLocked[2]) { Id = value; isLocked[2] = true; } else { Debug.Log("Wormhole: Attempted to set locked property Planets."); } } }


	public Wormhole() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}

}
