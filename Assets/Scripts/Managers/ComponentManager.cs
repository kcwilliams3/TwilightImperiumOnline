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
	
	private GameManager gameManager;

	public bool ReadyToPlay;


	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Initialize() {
		if (gameManager.IsActive (Option.DistantSuns) || gameManager.IsActive (Option.TheFinalFrontier)) {
			readDomains ();
		}
	}

	public void PrepareComponents() {
		setInitialCounts();
		ReadyToPlay = true;
	}

	private void setInitialCounts(){
		commandCounters = maxCommandCounters;
		controlMarkers = maxControlMarkers;
		bonusCounters = maxBonusCounters;
		tradeGoods = maxTradeGoods;
		if (gameManager.IsActive (Option.Facilities)) {
			colonies = maxColonies;
			refineries = maxRefineries;
		}
		if (gameManager.IsActive (Option.ShockTroops)) {
			shockTroops = maxShockTroops;
		}
		if (gameManager.IsActive (Option.SpaceMines)) {
			spaceMines = maxSpaceMines;
		}
	}

	private void readDomains() {
		foreach (DomainCounter domain in gameManager.FileMgr.ReadDomainFile ()) {
			if (gameManager.IsActive (domain.Option)) {
				maxDomainCounters[domain] = domain.Quantity;
			}
		};
	}
}
