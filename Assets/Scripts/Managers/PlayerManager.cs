using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : TIOMonoBehaviour {

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<string, PromissoryNote> promNotes = new Dictionary<string, PromissoryNote>();

	[SerializeField]
	private PromissoryNote[] promNotesDebug;

	private bool first = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (first) {
			first = false;
			preparePromNotes ();
		}
	}

	private void preparePromNotes() {
		ArrayList notesList = new ArrayList ();
		foreach(PromissoryNote note in GetComponent<FileManager>().ReadPromissoryFile()) {
			promNotes[note.Name] = note;
			notesList.Add (note);
		}
		promNotesDebug = (PromissoryNote[])notesList.ToArray (typeof(PromissoryNote));
	}
}
