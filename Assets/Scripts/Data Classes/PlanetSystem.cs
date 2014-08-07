using UnityEngine;

[System.Serializable]
public class PlanetSystem {

	// Variables for logical use
	private int TIGameProperties = 3;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Name."); } } }
	[SerializeField]
	private string sysType;
	public string SysType { get { return sysType; } set { if (!isLocked[1]) { sysType = value; isLocked[1] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property SysType."); } } }
	[SerializeField]
	private Planet[] planets;
	public Planet[] Planets { get { return planets; } set { if (!isLocked[2]) { planets = value; isLocked[2] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Planets."); } } }

	public PlanetSystem() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}