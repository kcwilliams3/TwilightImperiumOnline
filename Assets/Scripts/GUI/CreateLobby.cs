using UnityEngine;
using System.Collections.Generic;

public class CreateLobby : TIOMonoBehaviour {

	public GameObject Manager;

	public List<Option> DefaultOptions = new List<Option> {Option.AllObjectives, Option.RaceSpecificTechnologies, Option.Artifacts, Option.ShockTroops, Option.WormholeNexus, Option.TacticalRetreats, Option.CustodiansOfMecatolRex, Option.PreliminaryObjectives, Option.Flagships, Option.MechanizedUnits, Option.PoliticalIntrigue};
	public Scenario DefaultScenario = Scenario.StandardGame;
	public Expansion DefaultExpansion = Expansion.ShardsOfTheThrone;

	void OnClick() {
		NetworkManager networkManager = Manager.GetComponent<NetworkManager>();

		networkManager.CreateLobby(networkManager.PlayerName + "'s Quick Lobby", new RoomOptions() { maxPlayers = 8});
		foreach(Option option in DefaultOptions) {
			networkManager.Lobby.AddOption(option);
		}
		networkManager.Lobby.LobbyScenario = DefaultScenario;
		networkManager.Lobby.LobbyExpansion = DefaultExpansion;
	}
}
