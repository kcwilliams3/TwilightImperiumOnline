using UnityEngine;
using System.Collections;

public class NextSystem : MonoBehaviour {

	public GameObject BotPanelObject;
	private MapSetup mapSetupUI;

	// Use this for initialization
	void Start () {
		mapSetupUI = BotPanelObject.GetComponent<MapSetup>();
	}
	
	void OnClick() {
		mapSetupUI.NextSystem();
	}
}
