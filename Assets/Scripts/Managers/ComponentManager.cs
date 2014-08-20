using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComponentManager : TIOMonoBehaviour {

	//Maximum Component Counts
	private int maxCommandCounters = 16;
	private int maxControlMarkers = 17;
	private int maxBonusCounters = 8;
	private int maxTradeGoods = 88; // 52 * 1 + 12 * 3
	private int maxColonies = 8;
	private int maxRefineries = 8;
	private int maxShockTroops = 12;
	private int maxSpaceMines = 12;
	private Dictionary<DomainCounter, int> maxDomainCounters = new Dictionary<DomainCounter, int>();

	//Available Component Counts
	[SerializeField]
	private int commandCounters;
	public int CommandCounters { get { return commandCounters; } }
	[SerializeField]
	private int controlMarkers;
	public int ControlMarkers { get { return controlMarkers; } }
	[SerializeField]
	private int bonusCounters;
	public int BonusCounters { get { return bonusCounters; } }
	[SerializeField]
	private int tradeGoods;
	public int TradeGoods { get { return tradeGoods; } }
	[SerializeField]
	private int colonies;
	public int Colonies { get { return colonies; } }
	[SerializeField]
	private int refineries;
	public int Refineries { get { return refineries; } }
	[SerializeField]
	private int shockTroops;
	public int ShockTroops { get { return shockTroops; } }
	[SerializeField]
	private int spaceMines;
	public int SpaceMines { get { return spaceMines; } }

	private FileManager fileManager;
	private GameManager gameManager;


	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager> ();
		fileManager = GetComponent<FileManager> ();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	private void setInitialCounts(){
		commandCounters = maxCommandCounters;
		controlMarkers = maxControlMarkers;
		bonusCounters = maxBonusCounters;
		tradeGoods = maxTradeGoods;
		if (gameManager.Active (Option.Facilities)) {
			colonies = maxColonies;
			refineries = maxRefineries;
		}
		if (gameManager.Active (Option.ShockTroops)) {
			shockTroops = maxShockTroops;
		}
		if (gameManager.Active (Option.SpaceMines)) {
			spaceMines = maxSpaceMines;
		}
		if (gameManager.Active (Option.DistantSuns) || gameManager.Active (Option.TheFinalFrontier)) {
			readDomains ();
		}
	}

	public void readDomains() {
		foreach (DomainCounter domain in fileManager.ReadDomainFile ()) {
			if (gameManager.Active (domain.Option)) {
				maxDomainCounters[domain] = domain.Quantity;
			}
		};
	}
}
