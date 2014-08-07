using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

public class FileManager : TIOMonoBehaviour {

	public Race testRace;
	public string fullName;
	public string shortName;
	public string speciesName;
	public string expansion;
	public string[] history;
	public string[] specialAbilities;
	public int[] tradeContracts;

	// Use this for initialization
	void Start () {
		testRace = readRaceFile ("Assets/Text/Processed/English/Races/Arborec.tirace");
		fullName = testRace.FullName;
		shortName = testRace.ShortName;
		speciesName = testRace.SpeciesName;
		expansion = testRace.Expansion;
		history = testRace.History;
		specialAbilities = testRace.SpecialAbilities;
		tradeContracts = testRace.TradeContracts;
	}
	
	// Update is called once per frame
	void Update () {
	
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

	private Race readRaceFile(string fileName) {
		try {
			string line;
			StreamReader reader = new StreamReader(fileName, Encoding.Default);

			using (reader) {
				Race race = new Race();

				//do {
					line = reader.ReadLine().Trim ();
					//Debug.Log (line);
					if (line == "<{>") {
						// Start of an outermost block
						line = reader.ReadLine().Trim ();
						do {
							//Debug.Log (line);
							string[] lineParts;
							//Split category name from data
							lineParts = line.Split(":".ToCharArray(), 2);

							//Remove any extra whitespace from parts & set descriptive variables
							string dataType = lineParts[0] = lineParts[0].Trim ();
							string dataText = lineParts[1] = lineParts[1].Trim ();


							if (dataType == "Full Name") {
								race.FullName = readTextLine(dataType, dataText, fileName);
								//Debug.Log (race.FullName);
							} else if (dataType == "Short Name") {
								race.ShortName = readTextLine(dataType, dataText, fileName);
								//Debug.Log (race.ShortName);
							} else if (dataType == "Species Name") {
								race.SpeciesName = readTextLine(dataType, dataText, fileName);
								//Debug.Log (race.SpeciesName);
							} else if (dataType == "Expansion") {
								race.Expansion = readTextLine(dataType, dataText, fileName);
								//Debug.Log (race.Expansion);
							} else if (dataType == "History") {
								race.History = readTextBlock (dataType, dataText, fileName, reader);
								//Debug.Log (race.History[0]);
							} else if (dataType == "Special Abilities") {
								race.SpecialAbilities = readTextBlock (dataType, dataText, fileName, reader);
								//Debug.Log (race.SpecialAbilities[0]);
							} else if (dataType == "Trade Contracts") {
								race.TradeContracts = readIntBlock (dataType, dataText, fileName, reader);
								//Debug.Log (race.TradeContracts[0]);
							} else {
								//Debug.Log ("NONE!");
							}
							line = reader.ReadLine().Trim ();
							//Debug.Log(line == "<{>");

						} while (line != "<{>"); //should probably be "<}>" once race file reader is finished
						// End of outermost block
					}
				//} while (line != null); 

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

//	private void readHomeSystems(string dataText, string fileName, Race race, StreamReader reader) {
//		if (dataText.EndsWith ("<{>")) {
//			ArrayList systems = new ArrayList();
//			// Start of inner block
//			string line = reader.ReadLine().Trim ();
//			do {
//				systems.Add (readSystem(line, fileName, reader));
//				line = reader.ReadLine().Trim ();
//			} while (line != "<}>");
//			// End of inner block
//			race.HomeSystems = systems.ToArray ();
//		} else {
//			throw new System.Exception(string.Format("Error reading file {0}:: \"Home Systems: {1}\" should end with <{>", fileName, dataText));
//		}
//	}
//	
//	private PlanetSystem readSystem(string dataText, string fileName, StreamReader reader) {
//		if (dataText.EndsWith ("<{>")) {
//			// Start of inner block
//			PlanetSystem system = new PlanetSystem();
//			system.Name = reader.ReadLine().Trim().TrimEnd('>').TrimEnd(';').TrimEnd('<');
//			system.SysType = reader.ReadLine().Trim().TrimEnd('>').TrimEnd(';').TrimEnd('<');
//			
//			ArrayList planets = new ArrayList();
//			string line = reader.ReadLine ().Trim ();
//			do {
//				readPlanet(line, fileName, reader);
//				line = reader.ReadLine ().Trim ();
//			} while (line != "<}>");
//			// End of inner block
//			system.Planets = planets.ToArray ();
//		} else {
//			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
//		}
//	}
//	private Planet readPlanet(string data

	/* 
	 * Basic Section Readers 
	 */

	private string readTextLine(string dataType, string dataText, string fileName) {
		if (dataText.EndsWith ("<;>")) {
			return dataText.TrimEnd ('>').TrimEnd (';').TrimEnd('<');
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
//			Debug.Log (lines);
//			Debug.Log (lines.TrimEnd('\n'));
//			Debug.Log (lines.TrimEnd ('\n').TrimEnd ('>'));
//			Debug.Log (lines.TrimEnd ('\n').TrimEnd ('>').TrimEnd (';'));
//			Debug.Log (lines.TrimEnd ('\n').TrimEnd ('>').TrimEnd (';').TrimEnd('<'));
			string[] textSeparators = new string[] {"<;>"};
//			Debug.Log (lines.TrimEnd ('\n').TrimEnd ('>').TrimEnd (';').TrimEnd('<').Split(textSeparators, System.StringSplitOptions.RemoveEmptyEntries)[1]);
			return lines.TrimEnd ('\n').TrimEnd ('>').TrimEnd (';').TrimEnd('<').Split(textSeparators, System.StringSplitOptions.RemoveEmptyEntries);
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
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
//			Debug.Log (numberStrings[0] + numberStrings[1]);
//			Debug.Log (numberStrings.GetLength (0));
			int[] numbers = new int[numberStrings.GetLength(0)];
			for (int i=0; i < numberStrings.GetLength(0); i++) {
				numbers[i] = System.Convert.ToInt32(numberStrings[i]);
//				Debug.Log(numbers[i]);
			}
			return numbers;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}
}
