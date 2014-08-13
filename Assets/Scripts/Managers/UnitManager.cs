using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<UType, Unit> baseStats = new Dictionary<UType, Unit>();

	private bool firstUpdate = true;

	// Unit manager needs to be ready before file manager
	void Awake() {
	}

	// Use this for initialization
	void Start () {
		setBaseStats ();
	}
	
	// Update is called once per frame
	void Update () {
		if (firstUpdate) {
			setBasePrereqs();
			firstUpdate = false;
		}
	}

	private void setBaseStats(){
		baseStats.Add(UType.GroundForce, new Unit(UType.GroundForce, this));
		baseStats.Add(UType.SpaceDock, new Unit(UType.SpaceDock, this)); 
		baseStats.Add(UType.Carrier, new Unit(UType.Carrier, this));
		baseStats.Add(UType.PDS, new Unit(UType.PDS, this));
		baseStats.Add(UType.Fighter, new Unit(UType.Fighter, this));
		baseStats.Add(UType.Cruiser, new Unit(UType.Cruiser, this));
		baseStats.Add(UType.Destroyer, new Unit(UType.Destroyer, this));
		baseStats.Add(UType.Dreadnought, new Unit(UType.Dreadnought, this));
		baseStats.Add(UType.WarSun, new Unit(UType.WarSun, this));
		baseStats.Add(UType.MechanizedUnit, new Unit(UType.MechanizedUnit, this));
	}

	private void setBasePrereqs(){
		foreach(Unit unit in baseStats.Values){
			unit.setPrereqs();
		}
	}

	public Unit GetUnit(UType unitType) {
		return baseStats [unitType];
	}
}
