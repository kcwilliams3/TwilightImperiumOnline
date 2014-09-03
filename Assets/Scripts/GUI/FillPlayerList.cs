using UnityEngine;
using System.Collections;

public class FillPlayerList : TIOMonoBehaviour {

	public GameObject Manager;
	private NetworkManager networkManager;
	private UITextList playerList;

	// Use this for initialization
	void Start () {
		networkManager = Manager.GetComponent<NetworkManager>();
		playerList = GetComponent<UITextList>();
	}
	
	// Update is called once per frame
	void Update () {
		playerList.Clear ();
		foreach(MetaPlayer player in networkManager.GetMetaPlayers()) {
			playerList.Add(player.name);
		}
	}
}
