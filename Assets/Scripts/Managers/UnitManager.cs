using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {

	private Dictionary<UType, Unit> baseStats;

	// Use this for initialization
	void Start () {
		setBaseStats ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void setBaseStats(){
		baseStats = new Dictionary<UType, Unit>();
		baseStats.Add(UType.GroundForce, new Unit(UType.GroundForce));
		baseStats.Add(UType.SpaceDock, new Unit(UType.SpaceDock)); 
		baseStats.Add(UType.Carrier, new Unit(UType.Carrier));
		baseStats.Add(UType.PDS, new Unit(UType.PDS));
		baseStats.Add(UType.Fighter, new Unit(UType.Fighter));
		baseStats.Add(UType.Cruiser, new Unit(UType.Cruiser));
		baseStats.Add(UType.Destroyer, new Unit(UType.Destroyer));
		baseStats.Add(UType.Dreadnought, new Unit(UType.Dreadnought));
		baseStats.Add(UType.WarSun, new Unit(UType.WarSun));
		baseStats.Add(UType.MechanizedUnit, new Unit(UType.MechanizedUnit));
	}

	public Unit getUnit(UType unitType) {
		return baseStats [unitType];
	}
}
