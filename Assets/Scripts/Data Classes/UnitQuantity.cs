using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class UnitQuantity {
	[SerializeField]
	private  Unit unitType;
	public Unit UnitType { get { return unitType; } }

	[SerializeField]
	private int quantity;
	public int Quantity { get { return quantity; } }

	public UnitQuantity(Unit pUnitType, int pQuantity) {
		unitType = pUnitType;
		quantity = pQuantity;
	}
}
