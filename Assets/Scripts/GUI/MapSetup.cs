using UnityEngine;
using System.Collections.Generic;

public class MapSetup : MonoBehaviour {

	[SerializeField]
	private List<PlanetSystem> systemChoices = new List<PlanetSystem>();
	private int currentSysChoice;
	private int currentRing = 1;

	private GameManager gameManager;
	private GameObject mapSetupObject;

	public Color Valid;
	public Color Invalid;
	public Color Empty;

	// Use this for initialization
	void Start () {
		GameObject manager = GameObject.Find ("Manager");
		gameManager = manager.GetComponent<GameManager>();
		UIManager uiManager = manager.GetComponent<UIManager>();
		uiManager.MapSetupUI = this;

		mapSetupObject = gameObject.transform.FindChild("Map Setup Menu").gameObject;
	}

	public void SetSystemChoices(string[] systemNames) {
		for (int i = 0; i < systemNames.Length; i++) {
			systemChoices.Add(gameManager.BoardMgr.GetSystem(systemNames[i]));
			gameManager.FileMgr.ReadSystemTexture(systemNames[i], systemNames[i]);
		}
		currentSysChoice = 0;
		
		UpdateMapSetupMenu();
		
		mapSetupObject.SetActive(true);
	}

	public PlanetSystem GetSelected() {
		return systemChoices[currentSysChoice];
	}

	public PlanetSystem PopSelected() {
		PlanetSystem choice = systemChoices[currentSysChoice];
		systemChoices.RemoveAt(currentSysChoice);
		if (currentSysChoice == systemChoices.Count) {
			currentSysChoice -= 1;
		}
		return choice;
	}

	public void NextSystem() {
		if (currentSysChoice < systemChoices.Count-1) {
			currentSysChoice += 1;
			UpdateMapSetupMenu();
		}
	}
	
	public void PrevSystem() {
		if (currentSysChoice > 0) {
			currentSysChoice -= 1;
			UpdateMapSetupMenu();
		}
	}
	
	public void UpdateMapSetupMenu() {
		UITexture previousTex = mapSetupObject.transform.FindChild("Previous").GetChild(0).GetChild(0).GetComponent<UITexture>();
		UITexture nextTex = mapSetupObject.transform.FindChild("Next").GetChild(0).GetChild(0).GetComponent<UITexture>();
		UITexture selectedTex = mapSetupObject.transform.FindChild("Selected System").GetChild(0).GetComponent<UITexture>();
		
		string systemName;
		if (currentSysChoice <= 0) {
			systemName = "System Placeholder"; 
		} else {
			systemName = systemChoices[currentSysChoice - 1].Name;
		}
		Material mat = new Material(previousTex.material);
		mat.mainTexture = gameManager.FileMgr.ReadSystemTexture(systemName, systemName);
		previousTex.material = mat;
		if (currentSysChoice == systemChoices.Count-1) {
			systemName = "System Placeholder"; 
		} else {
			systemName = systemChoices[currentSysChoice + 1].Name;
		}
		mat = new Material(nextTex.material);
		mat.mainTexture = gameManager.FileMgr.ReadSystemTexture(systemName, systemName);
		nextTex.material = mat;
		if (systemChoices.Count == 0) {
			systemName = "System Placeholder"; 
		} else {
			systemName = systemChoices[currentSysChoice].Name;
		}
		mat = new Material(selectedTex.material);
		mat.mainTexture = gameManager.FileMgr.ReadSystemTexture(systemName, systemName);
		selectedTex.material = mat;
		//
		//		previousTex.MarkAsChanged();
		//		nextTex.MarkAsChanged();
		//		selectedTex.MarkAsChanged();
		
		mapSetupObject.transform.parent.gameObject.GetComponent<UIPanel>().alpha = 0.9f;
		mapSetupObject.transform.parent.gameObject.GetComponent<UIPanel>().alpha = 1.0f;

		updateHexesForSelected();
	}

	private void updateHexesForSelected() {
		//Update board for ring coloring
		BoardSection mainBoard = gameManager.BoardMgr.GameBoard.GetSection (0);
		Color color;
		int filledCount = 0;
		int ringSize = 0;
		foreach (SystemHex hex in mainBoard.Ring(currentRing)) {
			if (hex.System == null && systemChoices.Count > 0) {
				if (systemChoices[currentSysChoice].isSpecial() && hex.NextToSpecial) {
					//Special system - can't be placed next to other special systems (unless you have no other choice)
					color = Invalid;
					hex.IsValidPlacement = false;
				} else {
					color = Valid;
					hex.IsValidPlacement = true;
				}
				color.a = hex.renderer.material.color.a;
				hex.renderer.material.color = color;
			} else if (systemChoices.Count > 0) {
				filledCount += 1;
			} else {
				color = Empty;
				color.a = hex.renderer.material.color.a;
				hex.renderer.material.color = color;
				hex.IsValidPlacement = false;
			}
			ringSize += 1;
		}

		if (filledCount == ringSize) {
			//If this ring is filled, go to the next ring
			currentRing += 1;
			UpdateMapSetupMenu();
		}
	}
}
