using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

public class FileManager : TIOMonoBehaviour {

	public Race testRace;

	public string language = "English";
	private string rawTextDir;
	private string procTextDir;

	// Use this for initialization
	void Start () {
		checkDirectories ("Tools/folders.txt");
		testRace = readRaceFile (procTextDir + language + "/Races/Arborec.tirace");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private bool checkDirectories(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			string[] lines = reader.ReadToEnd().Trim ().Split('\n');
			rawTextDir = lines[0].TrimStart ("./".ToCharArray());
			procTextDir = lines[1].TrimStart ("./".ToCharArray());
			return true;
		} catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n", e.Message));
			return false;
		}
	}

	public bool ReadFile(string fileName) {
		Debug.Log(string.Format("Reading {0}... ", fileName));
		string extension = fileName.Split ('.') [1];
		if (extension == ".tirace") {
			readRaceFile(fileName);
			Debug.Log("Succeeded!\n");
			return true;
		}
		Debug.Log("Failed!\n");
		return false;
	}


	/*
	 * Specific File Readers
	 */

	private Race readRaceFile(string fileName) {
		try {
			string line;
			StreamReader reader = new StreamReader(fileName, Encoding.Default);

			using (reader) {
				Race race = new Race();

				line = reader.ReadLine().Trim ();
				if (line == "<{>") {
					// Start of an outermost block
					line = reader.ReadLine().Trim ();
					do {
						Debug.Log (line);
						string[] lineParts;
						//Split category name from data
						lineParts = line.Split(":".ToCharArray(), 2);

						//Remove any extra whitespace from parts & set descriptive variables
						string dataType = lineParts[0] = lineParts[0].Trim ();
						string dataText = lineParts[1] = lineParts[1].Trim ();


						if (dataType == "Full Name") {
							race.FullName = readTextLine(dataType, dataText, fileName);
						} else if (dataType == "Short Name") {
							race.ShortName = readTextLine(dataType, dataText, fileName);
						} else if (dataType == "Species Name") {
							race.SpeciesName = readTextLine(dataType, dataText, fileName);
						} else if (dataType == "Expansion") {
							race.Expansion = readTextLine(dataType, dataText, fileName);
						} else if (dataType == "History") {
							race.History = readTextBlock (dataType, dataText, fileName, reader);
						} else if (dataType == "Special Abilities") {
							race.SpecialAbilities = readTextBlock (dataType, dataText, fileName, reader);
						} else if (dataType == "Trade Contracts") {
							race.TradeContracts = readIntBlock (dataType, dataText, fileName, reader);
						} else if (dataType == "Home Systems") {
							race.HomeSystems = readSystemsBlock (dataType, dataText, fileName, reader);
						}
						line = reader.ReadLine().Trim ();

					} while (line != "<{>"); //should probably be "<}>" once race file reader is finished
					// End of outermost block
				}

				// Reading successfully finished
				reader.Close ();
				return race;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n", e.Message));
			return null;
		}
	}


	/*
	 * Complex Section Readers
	 */

	private PlanetSystem[] readSystemsBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText.EndsWith ("<{>")) {
			ArrayList systems = new ArrayList();
			// Start of inner block
			string line = reader.ReadLine().Trim ();
			do {
				systems.Add (readSystem(line, fileName, reader));
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of inner block
			Debug.Log ("System block finished");
			return (PlanetSystem[])systems.ToArray (typeof(PlanetSystem));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}
	
	private PlanetSystem readSystem(string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			PlanetSystem system = new PlanetSystem();

			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0] = lineParts[0].Trim ();
				string newDataText = lineParts[1] = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					system.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Type") {
					system.SysType = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Planets") {
					system.Planets = readPlanetsBlock(newDataType, newDataText, fileName, reader);
				}
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block
			Debug.Log (system.Name);
			return system;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Planet[] readPlanetsBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText.EndsWith ("<{>")) {
			ArrayList planets = new ArrayList();
			// Start of inner block
			string line = reader.ReadLine().Trim ();
			do {
				planets.Add (readPlanet(line, fileName, reader));
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of inner block
			Debug.Log ("Before Planet[] cast");
			return (Planet[])planets.ToArray (typeof(Planet));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}

	private Planet readPlanet(string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			Planet planet = new Planet();
			
			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0] = lineParts[0].Trim ();
				string newDataText = lineParts[1] = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					planet.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Text") {
					planet.Text = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Resources") {
					planet.Resources = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Influence") {
					planet.Influence = readIntLine (newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block
			Debug.Log (planet.Name);
			return planet;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}


	/* 
	 * Basic Section Readers 
	 */
	
	private string readTextLine(string dataType, string dataText, string fileName) {
		if (dataText.EndsWith ("<;>")) {
			return dataText.Trim().TrimEnd ('>').TrimEnd (';').TrimEnd('<');
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <;>", fileName, dataType, dataText));
		}
	}

	private string[] readTextBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText.EndsWith ("<{>")) {
			// Start of inner block
			string lines = "";
			
			string line = reader.ReadLine().Trim ();
			do {
				lines += line;
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of inner block
			string[] textSeparators = new string[] {"<;>"};
			return lines.TrimEnd ('\n').TrimEnd ('>').TrimEnd (';').TrimEnd('<').Split(textSeparators, System.StringSplitOptions.RemoveEmptyEntries);
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}

	private int readIntLine(string dataType, string dataText, string fileName) {
		return System.Convert.ToInt32(readTextLine (dataType, dataText, fileName));
	}

	private int[] readIntBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText.EndsWith ("<{>")) {
			// Start of inner block
			string lines = "";
			
			string line = reader.ReadLine().Trim ();
			do {
				lines += line + "\n";
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of inner block
			string[] numberStrings = lines.TrimEnd ('\n').TrimEnd ('>').TrimEnd (';').TrimEnd('<').Split("<;>\n".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
			int[] numbers = new int[numberStrings.GetLength(0)];
			for (int i=0; i < numberStrings.GetLength(0); i++) {
				numbers[i] = System.Convert.ToInt32(numberStrings[i]);
			}
			return numbers;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}
}
