using UnityEngine;

public enum SType {Unattached, Fixed, Special, Home};
public enum Wormhole {Alpha, Beta, Delta, C}

[System.Serializable]
public class PlanetSystem {

	// Variables for logical use
	private int TIGameProperties = 6;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Name."); } } }
	[SerializeField]
	private SType[] sysTypes;
	public SType[] SysTypes { get { return sysTypes; } set { if (!isLocked[1]) { sysTypes = value; isLocked[1] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property SysTypes."); } } }
	[SerializeField]
	private Planet[] planets;
	public Planet[] Planets { get { return planets; } set { if (!isLocked[2]) { planets = value; isLocked[2] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Planets."); } } }
	[SerializeField]
	private bool hasRealName;
	public bool HasRealName { get { return hasRealName; } set { if (!isLocked[3]) { hasRealName = value; isLocked[3] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property hasRealName."); } } }
	[SerializeField]
	private Wormhole[] wormholes;
	public Wormhole[] Wormholes { get { return wormholes; } set { if (!isLocked[4]) { wormholes = value; isLocked[4] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Wormholes."); } } }
	[SerializeField]
	private Expansion expansion;
	public Expansion Expansion { get { return expansion; } set { if (!isLocked[5]) { expansion = value; isLocked[5] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Expansion."); } } }

	public PlanetSystem() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}