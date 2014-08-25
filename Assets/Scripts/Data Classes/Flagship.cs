using UnityEngine;

[System.Serializable]
public class Flagship : Unit {
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } }
	[SerializeField]
	private string id;
	public string Id { get { return id; } }
	
	public Flagship(string pId, string pName, UAbility[] pAbilities, string pText, int pCost, int pBattle, int pMultiplier, int pMove, int pCapacity, TechManager techManager) : base(UType.Flagship, techManager) {
		id = pId;
		name = pName;
		abilities = pAbilities;
		text = pText;
		cost = pCost;
		battle = pBattle;
		shots = pMultiplier;
		move = pMove;
		capacity = pCapacity;
		carries = UCarry.Anything;
		buys = 1;
		fleetSupply = UFSupply.Always;
		ship = true;
		maxQuantity = 1;
	}
}

