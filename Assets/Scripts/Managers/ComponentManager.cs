using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComponentManager : MonoBehaviour {

	//Maximum Component Counts
	private int maxCommandCounters = 16;
	private int maxControlMarkers = 17;
	private int maxBonusCounters = 8;
	private int maxTradeGoods = 88; // 52 * 1 + 12 * 3
	private int maxColonies = 8;
	private int maxRefineries = 8;
	private int maxShockTroops = 12;
	private int maxSpaceMines = 12;

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

	// Use this for initialization
	void Start () {
		setInitialCounts ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void setInitialCounts(){
		commandCounters = maxCommandCounters;
		controlMarkers = maxControlMarkers;
		bonusCounters = maxBonusCounters;
		tradeGoods = maxTradeGoods;
		colonies = maxColonies;
		refineries = maxRefineries;
		shockTroops = maxShockTroops;
		spaceMines = maxSpaceMines;
	}
}
