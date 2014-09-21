using UnityEngine;
using System.Collections;

public class ChooseRace : TIOMonoBehaviour {
	
	private GameObject manager;
	private GameManager gameManager;
	public string RaceID;
	public GameObject ColorSelection;
	private UISprite colorSwatch;
	private UISprite availability;
	private ChooseColor	colorScript;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find("Manager");
		gameManager = manager.GetComponent<GameManager>();

		colorSwatch = ColorSelection.gameObject.transform.FindChild("Color Swatch").GetComponent<UISprite>();
		availability = ColorSelection.gameObject.transform.FindChild("Availability").GetComponent<UISprite>();
		colorScript = ColorSelection.GetComponent<ChooseColor>();
	}
	
	void OnClick () {
		if (availability.color == colorScript.available) {
			gameManager.PlayerMgr.networkView.RPC("RPC_AddPlayer", PhotonTargets.All, gameManager.NetworkMgr.PlayerName, RaceID, colorSwatch.color.r, colorSwatch.color.g, colorSwatch.color.b);
		}
	}
}

