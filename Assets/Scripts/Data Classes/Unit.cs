using UnityEngine;
using System;

public enum UType {GroundForce, SpaceDock, Carrier, PDS, Fighter, Cruiser, Destroyer, Dreadnought, WarSun, MechanizedUnit, Flagship};
public enum UFSupply {Never, Always, WhenExcess};
public enum UCarry {Nothing, Anything, GroundForces, Fighters, Leaders};
public enum UAbility {Production,PlanetaryShield,AntiFighterBarrage,Bombardment,SustainDamage};

[System.Serializable]
public class Unit {

	// TI Game Data
	[SerializeField]
	protected UType unitType;
	public UType UnitType { get { return unitType; } }
	[SerializeField]
	protected UAbility[] abilities;
	public UAbility[] Abilities { get { return abilities; } }
	[SerializeField]
	protected int cost;
	public int Cost { get { return cost; } }
	[SerializeField]
	protected int buys;
	public int Buys { get { return buys; } }
	[SerializeField]
	protected int battle;
	public int Battle { get { return battle; } }
	[SerializeField]
	protected int shots; // A value of -1 represents a special values ("?" for the Winnu flagship)
	public int Shots { get { return shots; } }
	[SerializeField]
	protected int capacity;
	public int Capacity { get { return capacity; } }
	[SerializeField]
	protected UCarry carries;
	public UCarry Carries { get { return carries; } }
	[SerializeField]
	protected UFSupply fleetSupply;
	public UFSupply FleetSupply { get { return fleetSupply; } }
	[SerializeField]
	protected bool ship;
	public bool Ship { get { return ship; } }
	[SerializeField]
	protected int move;
	public int Move { get { return move; } }
	[SerializeField]
	protected Tech[] prereqs;
	public Tech[] Prereqs { get { return prereqs; } set { prereqs = value; } }
	[SerializeField]
	protected int maxQuantity;
	public int MaxQuantity { get { return maxQuantity; } } 

	protected UnitManager unitManager;
	
	public Unit(UType pUnitType, UnitManager pUnitManager) {
		unitManager = pUnitManager;

		unitType = pUnitType;
		switch (unitType) {
			case UType.GroundForce:
				cost = 1;
				buys = 2;
				battle = 8;
				shots = 1;
				fleetSupply = UFSupply.Never;
				ship = false;
				carries = UCarry.Nothing;
				break;
			case UType.SpaceDock:
				cost = 4;
				buys = 1;
				fleetSupply = UFSupply.Never;
				ship = false;
				carries = UCarry.Fighters;
				capacity = 3;
				abilities = new UAbility[1]{UAbility.Production};
				maxQuantity = 3;
				break;
			case UType.Carrier:
				cost = 3;
				buys = 1;
				move = 1;
				battle = 9;
				shots = 1;
				fleetSupply = UFSupply.Always;
				ship = true;
				carries = UCarry.Anything;
				capacity = 6;
				maxQuantity = 4;
				break;
			case UType.PDS:
				cost = 2;
				buys = 1;
				battle = 6;
				shots = 0;
				fleetSupply = UFSupply.Never;
				ship = false;
				carries = UCarry.Nothing;
				abilities = new UAbility[1]{UAbility.PlanetaryShield};
				maxQuantity = 6;
				break;
			case UType.Fighter:
				cost = 1;
				buys = 2;
				battle = 9;
				shots = 1;
				fleetSupply = UFSupply.Never;
				ship = true;
				carries = UCarry.Leaders;
				break;
			case UType.Cruiser:
				cost = 2;
				buys = 1;
				battle = 7;
				shots = 1;
				fleetSupply = UFSupply.Always;
				ship = true;
				carries = UCarry.Leaders;
				maxQuantity = 8;
				break;
			case UType.Destroyer:
				cost = 1;
				buys = 1;
				move = 1;
				battle = 9;
				shots = 1;
				fleetSupply = UFSupply.Always;
				ship = true;
				carries = UCarry.Leaders;
				maxQuantity = 8;
				abilities = new UAbility[1]{UAbility.AntiFighterBarrage};
				break;
			case UType.Dreadnought:
				cost = 5;
				buys = 1;
				move = 1;
				battle = 5;
				shots = 1;
				fleetSupply = UFSupply.Always;
				ship = true;
				carries = UCarry.Leaders;
				abilities = new UAbility[2]{UAbility.SustainDamage, UAbility.Bombardment};
				maxQuantity = 5;
				break;
		case UType.WarSun:
				cost = 12;
				buys = 1;
				move = 2;
				battle = 3;
				shots = 3;
				fleetSupply = UFSupply.Always;
				ship = true;
				carries = UCarry.Anything;
				maxQuantity = 2;
				break;
			case UType.MechanizedUnit:
				cost = 2;
				buys = 1;
				battle = 6;
				shots = 2;
				fleetSupply = UFSupply.Never;
				ship = false;
				carries = UCarry.Nothing;
				abilities = new UAbility[1]{UAbility.SustainDamage};
				maxQuantity = 4;
				break;
		}
	}

	public int adjustBattle(int adjustment) {
		if (battle != default(int)) {
			battle -= adjustment;
		} else {
			battle = adjustment;
		}
		return battle;
	}

	public void setPrereqs() {
		if (unitType == UType.WarSun) {
			prereqs = new Tech[1];
			prereqs[0] = unitManager.GetComponent<TechManager>().GetTech ("War Sun");
		}
	}
}

