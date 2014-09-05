using UnityEngine;
using System.Collections;

public class SetLobbyNameLabel : TIOMonoBehaviour {

	public GameObject Manager;
	private NetworkManager networkManager;
	private UILabel label;

	// Use this for initialization
	void Start () {
		networkManager = Manager.GetComponent<NetworkManager>();
		label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		label.text = networkManager.Lobby.name;
	}
}
