using UnityEngine;
using System.Collections;

public class SystemHex : TIOMonoBehaviour {
	
	public PlanetSystem System;
	private Vector3 Location = new Vector3();

	public bool NextToSpecial = false;
	
	private MapSetup mapSetupUI;
	public bool IsValidPlacement;
	private BoardManager boardManager;

	public void Start() {
		boardManager = GameObject.Find("Manager").GetComponent<BoardManager>();
		mapSetupUI = GameObject.Find("Main Camera").transform.Find("UI Root (3D)").Find("Bottom Panel").GetComponent<MapSetup>();
	}

	public void SetPosition(Vector2 newPosition) {
		//Debug.Log ("~~~~~~~~~~" + newPosition.x + "    ,     " + newPosition.y);
		Location.y = newPosition.x;
		Location.z = newPosition.y;
	}

	public Vector2 GetPosition() {
		return new Vector2(Location.y, Location.z);
	}

	public void SetSection(int section) {
		Location.x = section;
	}

	public int GetSection() {
		return (int)Location.x;
	}

	void OnClick() {
		if (IsValidPlacement) {
			//Debug.Log (",,,,.....,,,,,,: " + Location.y + "     ,      " + Location.z);
			PlanetSystem selected = mapSetupUI.PopSelected();
			BoardSection section = boardManager.GameBoard.GetSection((int)Location.x);
			boardManager.networkView.RPC ("RPC_SetSystem", PhotonTargets.AllViaServer, selected.Id, (int)Location.y, (int)Location.z);
		}
	}
}
