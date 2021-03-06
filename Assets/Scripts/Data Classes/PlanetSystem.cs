using UnityEngine;

public enum SType {Unattached, Fixed, Special, Home};

[System.Serializable]
public class PlanetSystem {

	// Variables for logical use
	private const int TIGameProperties = 7;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Name."); } } }
	[SerializeField]
	private SType[] sysTypes = new SType[0];
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
	[SerializeField]
	private string id;
	public string Id { get { return id; } set { if (!isLocked[6]) { id = value; isLocked[6] = true; } else { Debug.Log("PlanetSystem: Attempted to set locked property Id."); } } }

	public PlanetSystem() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}

	public bool isUnattached() {
		foreach(SType sysType in sysTypes) {
			if (sysType == SType.Unattached) {
				return true;
			}
		}
		return false;
	}

	public bool isFixed() {
		foreach(SType sysType in sysTypes) {
			if (sysType == SType.Fixed) {
				return true;
			}
		}
		return false;
	}

	public bool isSpecial() {
		foreach(SType sysType in sysTypes) {
			if (sysType == SType.Special) {
				return true;
			}
		}
		return false;
	}

	public bool isHome() {
		foreach(SType sysType in sysTypes) {
			if (sysType == SType.Home) {
				return true;
			}
		}
		return false;
	}

	public bool isEmpty() {
		return planets.Length == 0;
	}
}