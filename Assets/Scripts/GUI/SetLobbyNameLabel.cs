using UnityEngine;
using System.Collections;

public class SetLobbyNameLabel : TIOMonoBehaviour {

	public GameObject Manager;
	private NetworkManager networkManager;
	private UILabel label;

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		if (networkManager == null) {
			networkManager = Manager.GetComponent<NetworkManager>();
		}
		label.text = networkManager.LobbyName;
	}
}
