using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TechManager : MonoBehaviour {

	private Dictionary<TType, Tech> techs;
	private FileManager fileManager;

	// Use this for initialization
	void Start () {
		fileManager = GetComponent<FileManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Tech getTech(TType techType) {
		return techs [techType];
	}

	private void readTechs() {
		foreach (Tech tech in fileManager.ReadTechFile ());
	}
}
