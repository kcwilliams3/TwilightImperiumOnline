using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : TIOMonoBehaviour {

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<UType, Unit> baseStats = new Dictionary<UType, Unit>();

	private GameManager gameManager;

	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Initialize() {
		setBaseStats ();
	}

	private void setBaseStats(){
		baseStats.Add(UType.GroundForce, new Unit(UType.GroundForce, gameManager.TechMgr));
		baseStats.Add(UType.SpaceDock, new Unit(UType.SpaceDock, gameManager.TechMgr)); 
		baseStats.Add(UType.Carrier, new Unit(UType.Carrier, gameManager.TechMgr));
		baseStats.Add(UType.PDS, new Unit(UType.PDS, gameManager.TechMgr));
		baseStats.Add(UType.Fighter, new Unit(UType.Fighter, gameManager.TechMgr));
		baseStats.Add(UType.Cruiser, new Unit(UType.Cruiser, gameManager.TechMgr));
		baseStats.Add(UType.Destroyer, new Unit(UType.Destroyer, gameManager.TechMgr));
		baseStats.Add(UType.Dreadnought, new Unit(UType.Dreadnought, gameManager.TechMgr));
		baseStats.Add(UType.WarSun, new Unit(UType.WarSun, gameManager.TechMgr));
		baseStats.Add(UType.MechanizedUnit, new Unit(UType.MechanizedUnit, gameManager.TechMgr));
	}

	public Unit GetUnit(UType unitType) {
		return baseStats [unitType];
	}
}
