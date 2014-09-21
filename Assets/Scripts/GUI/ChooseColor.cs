using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChooseColor : TIOMonoBehaviour {

	public Color[] colors = new Color[8];
	public Color available;
	public Color unavailable;
	private int current = 0;
	private UISprite availability;
	private UISprite colorSwatch;
	private Dictionary<string, int> selectedColors = new Dictionary<string, int>();
	private NetworkManager networkManager;

	void Awake() {
		availability = gameObject.transform.FindChild("Availability").GetComponent<UISprite>();
		colorSwatch = gameObject.transform.FindChild("Color Swatch").GetComponent<UISprite>();
		networkManager = GameObject.Find ("Manager").GetComponent<NetworkManager>();

		foreach (MetaPlayer player in networkManager.GetMetaPlayers()) {
			if (player.name != networkManager.PlayerName && !selectedColors.ContainsKey(player.name)) {
				selectedColors[player.name] = current;
			}
		}

		colorSwatch.color = colors[current];
	}

	// Use this for initialization
	void Start () {
		updateAvailability();
	}
	
	void OnClick () {
		current = (current + 1) % 8;
		colorSwatch.color = colors[current];
		updateAvailability();
		networkView.RPC("RPC_UpdateSelectedColors", PhotonTargets.Others, networkManager.PlayerName, current);
	}

	[RPC]
	public void RPC_UpdateSelectedColors(string playerName, int colorIndex) {
		selectedColors [playerName] = colorIndex;
		updateAvailability();
	}

	private void updateAvailability() {
		if (selectedColors.ContainsValue(current)) {
			availability.color = unavailable;
		} else {
			availability.color = available;
		}
	}
}
