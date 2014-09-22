using UnityEngine;
using System.Collections;

public class PreviousSystem : MonoBehaviour {

	private UIManager uiManager;
	
	// Use this for initialization
	void Start () {
		GameObject manager = GameObject.Find("Manager");
		uiManager = manager.GetComponent<UIManager>();
	}
	
	void OnClick() {
		uiManager.PrevSystem();
	}
}
