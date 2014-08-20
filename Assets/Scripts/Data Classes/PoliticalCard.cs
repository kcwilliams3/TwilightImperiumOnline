using UnityEngine;

public enum VType { ForAgainst, Elect, Event };
public enum EType { Player, Planet, PublicObjective, CurrentLaw, ASpecialSystem, TechColor };

[System.Serializable]
public class PoliticalCard {
	
	// Variables for logical use
	private const int TIGameProperties = 13;
	private bool[] isLocked;
	
	// TI Game Data
	[SerializeField]
	private string name;
	public string Name { get { return name; } set { if (!isLocked[0]) { name = value; isLocked[0] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property Name."); } } }
	[SerializeField]
	private bool isLaw;
	public bool IsLaw { get { return isLaw; } set { if (!isLocked[1]) { isLaw = value; isLocked[1] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property IsLaw."); } } }
	[SerializeField]
	private VType voteType;
	public VType VoteType { get { return voteType; } set { if (!isLocked[2]) { voteType = value; isLocked[2] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property VoteType."); } } }
	[SerializeField]
	private Expansion expansion;
	public Expansion Expansion { get { return expansion; } set { if (!isLocked[3]) { expansion = value; isLocked[3] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property Expansion."); } } }
	[SerializeField]
	private string flavorText;
	public string FlavorText { get { return flavorText; } set { if (!isLocked[4]) { flavorText = value; isLocked[4] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property FlavorText."); } } }
	[SerializeField]
	private string discardText;
	public string DiscardText { get { return discardText; } set { if (!isLocked[5]) { discardText = value; isLocked[5] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property DiscardText."); } } }
	// Attributes for For/Against vote type
	[SerializeField]
	private string forText;
	public string ForText { get { return forText; } set { if (!isLocked[6]) { forText = value; isLocked[6] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property ForText."); } } }
	[SerializeField]
	private string againstText;
	public string AgainstText { get { return againstText; } set { if (!isLocked[7]) { againstText = value; isLocked[7] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property AgainstText."); } } }
	// Attributes for Elect vote type
	[SerializeField]
	private EType electType;
	public EType ElectType { get { return electType; } set { if (!isLocked[8]) { electType = value; isLocked[8] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property ElectType."); } } }
	[SerializeField]
	private int electQuantity;
	public int ElectQuantity { get { return electQuantity; } set { if (!isLocked[9]) { electQuantity = value; isLocked[9] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property ElectQuantity."); } } }
	[SerializeField]
	private string electText;
	public string ElectText { get { return electText; } set { if (!isLocked[10]) { electText = value; isLocked[10] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property ElectText."); } } }
	[SerializeField]
	private string eventText;
	public string EventText { get { return eventText; } set { if (!isLocked[11]) { eventText = value; isLocked[11] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property EventText."); } } }
	[SerializeField]
	private string id;
	public string Id { get { return id; } set { if (!isLocked[12]) { id = value; isLocked[12] = true; } else { Debug.Log("PoliticalCard: Attempted to set locked property Id."); } } }

	public PoliticalCard() {
		isLocked = new bool[TIGameProperties];
		for (int i=0; i<TIGameProperties; i++) {
			isLocked[i] = false;
		}
	}
}