using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	public GameObject Manager;
	private NetworkManager networkManager;
	
	// Use this for initialization
	void Start () {
		networkManager = Manager.GetComponent<NetworkManager>();
	}
	
	void OnClick () {
		networkManager.startNetworkedGame (networkManager.Lobby);
	}
}
