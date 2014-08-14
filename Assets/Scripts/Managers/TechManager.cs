using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TechManager : TIOMonoBehaviour {

	private Dictionary<string, Tech> techs = new Dictionary<string, Tech>();
	private FileManager fileManager;

	// Use this for initialization
	void Start () {
		fileManager = GetComponent<FileManager>();

		readTechs ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Tech GetTech(string techName) {
		return techs [techName];
	}

	private void readTechs() {
		foreach (Tech tech in fileManager.ReadTechFile ()) {
			techs[tech.Name] = tech;
		};
	}

	public void AddTech(Tech tech) {
		int i = 0;
		string name = tech.Name + System.Convert.ToInt32(i);
		while (techs.ContainsKey(name)) {
			i++;
			name = tech.Name + System.Convert.ToInt32 (i);
		}
	}
}
