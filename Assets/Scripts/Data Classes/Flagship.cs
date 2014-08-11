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
	
	public Flagship(string pName, string pText) : base(UType.Flagship) {
	}
}

