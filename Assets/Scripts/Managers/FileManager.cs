using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

public class FileManager : TIOMonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool ReadFile(string fileName) {
		string extension = fileName.Split ('.') [1];
		if (extension == ".tirace") {
			return ReadRaceFile(fileName);
		}
		return false;
	}

	private bool ReadRaceFile(string fileName) {
		try {
			string line;
			StreamReader reader = new StreamReader(fileName, Encoding.Default);

			using (reader) {
				do {
					line = reader.ReadLine ();
//
//					if (line.StartsWith("<{>")) {
//						// Start of an outermost block
//						do {
//							line = reader.ReadLine ();
//							string[] lineParts;
//							//Split category name from data
//							lineParts = line.Split(":",2);
//
//							//Remove any extra whitespace from data
//							lineParts[1] = lineParts[1].Trim ();
//
//							//Allocate FileData struct and insert into file array
//
//
//
//						} while (line != "<}>");
//						// End of outermost block
//					}
				} while (line != null);

				// Reading successfully finished
				reader.Close ();
				return true;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n", e.Message));
			return false;
		}
	}
}
