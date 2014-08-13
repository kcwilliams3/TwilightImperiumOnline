using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Expansion {Vanilla, ShatteredEmpire, ShardsOfTheThrone};
public enum Option {DistantSuns, TheFinalFrontier, TheLongWar, AgeOfEmpire, Leaders, SabotageRuns, SEObjectives, AllObjectives, RaceSpecificTechnologies, Artifacts, ShockTroops, SpaceMines, WormholeNexus, Facilities, TacticalRetreats, TerritorialDistantSuns, CustodiansOfMecatolRex, VoiceOfTheCouncil, SimulatedEarlyTurns, PreliminaryObjectives, Flagships, MechanizedUnits, Mercenaries, PoliticalIntrigue, FallOfTheEmpire};

public class GameManager : MonoBehaviour {

	private string language = "English";
	public string Language { get { return language; } }

	[SerializeField]
	//TODO: After finished, get rid of debug arrays.
	private Option[] activeOptionsDebug;
	private ArrayList activeOptions = new ArrayList ();

	// Use this for initialization
	void Start () {
		Activate (Option.AllObjectives);
		Activate (Option.PreliminaryObjectives);
		Activate (Option.Artifacts);
		Activate (Option.VoiceOfTheCouncil);
	}
	
	// Update is called once per frame
	void Update () {
		activeOptionsDebug = (Option[])activeOptions.ToArray (typeof(Option));
	}

	public void Activate(Option option) {
		if (!Active (option)){
			activeOptions.Add (option);
		}
	}

	public bool Active(Option option) {
		return activeOptions.Contains(option);
	}

	public void Deactivate(Option option) {
		if (Active (option)){
			activeOptions.Remove(option);
		}
	}
}
