﻿using UnityEngine;
using System.Collections;

public class AnnounceLoad : TIOMonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject manager = GameObject.Find ("Manager");
		manager.GetComponent<GameManager>().SetStage(GameStage.LevelLoaded);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
