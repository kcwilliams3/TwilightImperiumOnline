using UnityEngine;
using System.Collections.Generic;

public class Race {

	// Variables for logical use
	private int TIGameProperties = 14;
	private bool[] isLocked;

	// TI Game Data
	private string fullName;
	public string FullName { get { return fullName; } set { if (!isLocked[0]) { fullName = value; isLocked[0] = true; } else { Debug.Log("Race: Attempted to set locked property FullName."); } } }
	private string shortName;
	public string ShortName { get { return shortName; } set { if (!isLocked[1]) { shortName = value; isLocked[1] = true; } else { Debug.Log("Race: Attempted to set locked property ShortName."); } } }
	private string speciesName;
	public string SpeciesName { get { return speciesName; } set { if (!isLocked[2]) { speciesName = value; isLocked[2] = true; } else { Debug.Log("Race: Attempted to set locked property SpeciesName."); } } }
	private string expansion;
	public string Expansion { get { return expansion; } set { if (!isLocked[3]) { expansion = value; isLocked[3] = true; } else { Debug.Log("Race: Attempted to set locked property Expansion."); } } }
	private string[] history;
	public string[] History { get { return history; } set { if (!isLocked[4]) { history = value; isLocked[4] = true; } else { Debug.Log("Race: Attempted to set locked property History."); } } }
	private string[] specialAbilities;
	public string[] SpecialAbilities { get { return specialAbilities; } set { if (!isLocked[5]) { specialAbilities = value; isLocked[5] = true; } else { Debug.Log("Race: Attempted to set locked property SpecialAbilities."); } } }
	private int[] tradeContracts;
	public int[] TradeContracts { get { return tradeContracts; } set { if (!isLocked[6]) { tradeContracts = value; isLocked[6] = true; } else { Debug.Log("Race: Attempted to set locked property TradeContracts."); } } }
	private PlanetSystem[] homeSystems;
	public PlanetSystem[] HomeSystems { get { return homeSystems; } set { if (!isLocked[7]) { homeSystems = value; isLocked[7] = true; } else { Debug.Log("Race: Attempted to set locked property HomeSystems."); } } }
	private Dictionary<Unit, int> startingUnits;
	public Dictionary<Unit, int> StartingUnits { get { return startingUnits; } set { if (!isLocked[8]) { startingUnits = value; isLocked[8] = true; } else { Debug.Log("Race: Attempted to set locked property StartingUnits."); } } } //key temporarily a string. Should change to Unit once units have been implemented
	private string[] startingTechs;
	public string[] StartingTechs { get { return startingTechs; } set { if (!isLocked[9]) { startingTechs = value; isLocked[9] = true; } else { Debug.Log("Race: Attempted to set locked property StartingTechs."); } } } //temporarily a string. Should change to Tech once tech reading has been implemented
	private Leader[] leaders;
	public Leader[] Leaders { get { return leaders; } set { if (!isLocked[10]) { leaders = value; isLocked[10] = true; } else { Debug.Log("Race: Attempted to set locked property Leaders."); } } }
	private Tech[] racialTechs;
	public Tech[] RacialTechs { get { return racialTechs; } set { if (!isLocked[11]) { racialTechs = value; isLocked[11] = true; } else { Debug.Log("Race: Attempted to set locked property RacialTechs."); } } }
	private Unit flagship;
	public Unit Flagship { get { return flagship; } set { if (!isLocked[12]) { flagship = value; isLocked[12] = true; } else { Debug.Log("Race: Attempted to set locked property Flagship."); } } }
	private Representative[] representatives;
	public Representative[] Representatives { get { return representatives; } set { if (!isLocked[13]) { representatives = value; isLocked[13] = true; } else { Debug.Log("Race: Attempted to set locked property Representatives."); } } }

	public Race() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
