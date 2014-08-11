using UnityEngine;

public enum UType {GroundForce, SpaceDock, Carrier, PDS, Fighter, Cruiser, Destroyer, Dreadnought, WarSun, MechanizedUnit, Flagship};
public enum UFSupply {Never, Always, WhenExcess};
public enum UCarry {Nothing, Anything, GroundForces, Fighters, Leaders};
public enum UAbility {Production,PlanetaryShield,AntiFighterBarrage,Bombardment,SustainDamage};

[System.Serializable]
public class Unit {

	// TI Game Data
	[SerializeField]
	private UType unitType;
	public UType UnitType { get { return unitType; } }
	[SerializeField]
	private UAbility[] abilities;
	public UAbility[] Abilities { get { return abilities; } }
	[SerializeField]
	private int cost;
	public int Cost { get { return cost; } }
	[SerializeField]
	private int buys;
	public int Buys { get { return buys; } }
	[SerializeField]
	private int battle;
	public int Battle { get { return battle; } }
	[SerializeField]
	private int shots;
	public int Shots { get { return shots; } }
	[SerializeField]
	private int capacity;
	public int Capacity { get { return capacity; } }
	[SerializeField]
	private UCarry carries;
	public UCarry Carries { get { return carries; } }
	[SerializeField]
	private UFSupply fleetSupply;
	public UFSupply FleetSupply { get { return fleetSupply; } }
	[SerializeField]
	private bool ship;
	public bool Ship { get { return ship; } }
	[SerializeField]
	private int move;
	public int Move { get { return move; } }
	[SerializeField]
	private Tech prereq;
	public Tech Prereq { get { return prereq; } }
	
	public Unit(UType pUnitType) {
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
				abilities = new UAbility[2]{UAbility.SustainDamage, UAbility.Bombardment};
				//TODO: prereq = war sun tech
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
}

