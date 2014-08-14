using UnityEngine;
using System.Collections;

[System.Serializable]
public class DomainCounter {

	// Variables for logical use
	private const int TIGameProperties = 6;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("DomainCounter: Attempted to set locked property Name."); } } }
	[SerializeField]
	private int quantity;
	public int Quantity { get { return quantity; } set { if (!isLocked[1]) { quantity = value; isLocked[1] = true; } else { Debug.Log("DomainCounter: Attempted to set locked property Quantity."); } } }
	[SerializeField]
	private Expansion expansion;
	public Expansion Expansion { get { return expansion; } set { if (!isLocked[2]) { expansion = value; isLocked[2] = true; } else { Debug.Log("DomainCounter: Attempted to set locked property Expansion."); } } }
	[SerializeField]
	private string qualifier;
	public string Qualifier { get { return qualifier; } set { if (!isLocked[3]) { qualifier = value; isLocked[3] = true; } else { Debug.Log("DomainCounter: Attempted to set locked property Qualifier."); } } }
	[SerializeField]
	private Option option;
	public Option Option { get { return option; } set { if (!isLocked[4]) { option = value; isLocked[4] = true; } else { Debug.Log("DomainCounter: Attempted to set locked property Option."); } } }
	[SerializeField]
	private string text;
	public string Text { get { return text; } set { if (!isLocked[5]) { text = value; isLocked[5] = true; } else { Debug.Log("DomainCounter: Attempted to set locked property Text."); } } }
	
	public DomainCounter() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}
