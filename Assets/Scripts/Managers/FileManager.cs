using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class FileManager : TIOMonoBehaviour {

	public Race testRace;
	public bool read;

	// Directory-related Variables
	public string language = "English";
	private string rawTextDir;
	private string procTextDir;

	// Managers
	private UnitManager unitManager;
	private TechManager techManager;

	// Need to check directories before Tech Manager starts
	void Awake() {
		checkDirectories ("Tools/folders.txt");
	}

	// Use this for initialization
	void Start () {
		unitManager = GetComponent<UnitManager>();
		techManager = GetComponent<TechManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!read) {
			testRace = ReadRaceFile ("Nekro");
			read = true;
		}
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

	public Race ReadRaceFile(string raceName) {
		string fullPath = procTextDir + language + "/Races/" + raceName + ".tirace";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readRaceFile(fullPath);
	}

	public Tech[] ReadTechFile() {
		string fullPath = procTextDir + language + "/technologies.titechs";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readTechFile (fullPath);
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
						string[] lineParts;
						//Split category name from data
						lineParts = line.Split(":".ToCharArray(), 2);

						//Remove any extra whitespace from parts & set descriptive variables
						string dataType = lineParts[0].Trim ();
						string dataText = lineParts[1].Trim ();

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
						} else if (dataType == "Starting Units") {
							race.StartingUnits = readUnitsBlock (dataType, dataText, fileName, reader);
						} else if (dataType == "Starting Techs") {
							race.StartingTechs = readTechsBlock(dataType, dataText, fileName, reader);
						} else if (dataType == "Leaders") {
							race.Leaders = readLeadersBlock(dataType, dataText, fileName, reader);
						} else if (dataType == "Racial Techs") {
							race.RacialTechs = readFullTechsBlock (fileName, reader);
							foreach(Tech tech in race.RacialTechs) {
								tech.Race = race;
							}
						} else if (dataType == "Flagship") {
							break;
						}
						line = reader.ReadLine().Trim ();
					} while (line != "<}>");
					// End of outermost block
				}

				// Reading successfully finished
				reader.Close ();
				return race;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	private Tech[] readTechFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				Tech[] tech = readFullTechsBlock(fileName, reader);

				// Reading successfully finished
				reader.Close ();

				return tech;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
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
			return planet;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private UnitQuantity[] readUnitsBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText.EndsWith ("<{>")) {
			ArrayList units = new ArrayList();
			// Start of inner block
			string line = reader.ReadLine().Trim ();
			do {
				UnitQuantity unit = readUnit(dataText, fileName, reader);
				units.Add (unit);

				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of inner block
			return (UnitQuantity[])units.ToArray (typeof(UnitQuantity));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}

	private UnitQuantity readUnit(string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			Unit unit = new Unit(UType.SpaceDock); //Need a temporary unit here for compiler
			int quantity = 0; //Need a temporary quantity here for compiler

			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0] = lineParts[0].Trim ();
				string newDataText = lineParts[1] = lineParts[1].Trim ();
				
				
				if (newDataType == "Unit") {
					unit = unitManager.getUnit(stringToUType(readTextLine (newDataType, newDataText, fileName)));
				} else if (newDataType == "Quantity") {
					quantity = readIntLine (newDataType, newDataText, fileName);
				} 
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block

			return new UnitQuantity(unit, quantity);
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Tech[] readTechsBlock (string dataType, string dataText, string fileName, StreamReader reader){
		if (dataText.EndsWith ("<{>")) {
			ArrayList techs = new ArrayList();
			// Start of inner block
			string line = reader.ReadLine().Trim ();
			while (line != "<}>") {
				Tech tech = techManager.getTech(readTextLine ("", line, fileName));
				techs.Add (tech);
				
				line = reader.ReadLine().Trim ();
			}
			// End of inner block
			return (Tech[])techs.ToArray (typeof(Tech));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}

	private Leader[] readLeadersBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText.EndsWith ("<{>")) {
			ArrayList leaders = new ArrayList();
			// Start of inner block
			string line = reader.ReadLine().Trim ();
			while (line != "<}>") {
				Leader leader = readLeader(dataType, dataText, fileName, reader);
				leaders.Add (leader);
				
				line = reader.ReadLine().Trim ();
			}
			// End of inner block
			return (Leader[])leaders.ToArray (typeof(Leader));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}

	private Leader readLeader(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			Leader leader = new Leader();
			
			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0] = lineParts[0].Trim ();
				string newDataText = lineParts[1] = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					leader.Name = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Type") {
					leader.LeaderType = stringToLType(readTextLine (newDataType, newDataText, fileName));
				} 
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block

			return leader;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Tech[] readFullTechsBlock(string fileName, StreamReader reader) {

		Dictionary<string, Tech> techs = new Dictionary<string, Tech>();
		Dictionary<Tech, string[]> prereqs = new Dictionary<Tech, string[]>();
		
		string line = reader.ReadLine().Trim ();

		do {
			if (line == "<{>") {
				// Start of an outermost block
				line = reader.ReadLine().Trim ();
				Tech tech = new Tech ();
				do {
					string[] lineParts;
					//Split category name from data
					lineParts = line.Split(":".ToCharArray(), 2);
					
					//Remove any extra whitespace from parts & set descriptive variables
					string dataType = lineParts[0].Trim ();
					string dataText = lineParts[1].Trim ();
					
					
					if (dataType == "Name") {
						tech.Name = readTextLine(dataType, dataText, fileName);
					} else if (dataType == "Color") {
						tech.TechType = stringToTType(readTextLine(dataType, dataText, fileName));
					} else if (dataType == "Expansion") {
						tech.Expansion = readTextLine(dataType, dataText, fileName);
					} else if (dataType == "Requires") {
						// The prereq techs may not actually exist yet, so just save the name for now
						prereqs[tech] = readTextBlock (dataType, dataText, fileName, reader);
					} else if (dataType == "Text") {
						tech.Text = readTextLine(dataType, dataText, fileName);
					} else if (dataType == "Cost") {
						tech.Cost = readIntLine(dataType, dataText, fileName);
						// Only racial techs have a cost.
						tech.TechType = TType.Racial;
					}
					line = reader.ReadLine().Trim ();	
				} while (line != "<}>");
				// End of outermost block

				if (!techs.ContainsKey(tech.Name)) {
					techs[tech.Name] = tech;
				} else {
					techs[tech.Name + "/" + tech.Expansion] = tech;
				}

				if (!reader.EndOfStream) {
					line = reader.ReadLine ().Trim ();
				}
			}
		} while (line == "<{>");

		// All techs have been created, so fill in prereqs (if any)
		foreach (Tech tech in techs.Values) {
			ArrayList prereqObjects = new ArrayList();

			if (prereqs.Keys.Count > 0) {
				foreach (string techName in prereqs[tech]) {
					if (techName == "AND") {
						tech.PrereqMode = TPrereqMode.AND;
					} else if (techName == "OR") {
						tech.PrereqMode = TPrereqMode.OR;
					} else if (techs.ContainsKey(techName)) {
						prereqObjects.Add(techs[techName]);
					}
				}
				tech.Prereqs = (Tech[])prereqObjects.ToArray(typeof(Tech));
			}
		}
		
		Tech[] techsArray = new Tech[techs.Values.Count];
		techs.Values.CopyTo(techsArray, 0);

		return techsArray;
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
			while (line != "<}>") {
				lines += line;
				line = reader.ReadLine().Trim ();
			} 
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


	/*
	 * Conversions
	 */

	private UType stringToUType(string unitName) {
		if (unitName != "PDS") {
			unitName = unitName.TrimEnd ('s');
		}
		if (unitName == "Ground Force") {
			return UType.GroundForce;
		} else if (unitName == "Space Dock") {
			return UType.SpaceDock;
		} else if (unitName == "Carrier") {
			return UType.Carrier;
		} else if (unitName == "PDS") {
			return UType.PDS;
		} else if (unitName == "Fighter") {
			return UType.Fighter;
		} else if (unitName == "Cruiser") {
			return UType.Cruiser;
		} else if (unitName == "Destroyer") {
			return UType.Destroyer;
		} else if (unitName == "Dreadnought") {
			return UType.Dreadnought;
		} else if (unitName == "War Sun") {
			return UType.WarSun;
		} else {
			return UType.MechanizedUnit;
		}
	}

	private TType stringToTType(string techType) {
		if (techType == "Blue") {
			return TType.Blue;
		} else if (techType == "Red") {
			return TType.Red;
		} else if (techType == "Yellow") {
			return TType.Yellow;
		} else if (techType == "Green") {
			return TType.Green;
		} else {
			return TType.Racial;
		}
	}

	private LType stringToLType(string leaderType) {
		if (leaderType == "Admiral") {
			return LType.Admiral;
		} else if (leaderType == "Agent") {
			return LType.Agent;
		} else if (leaderType == "Diplomat") {
			return LType.Diplomat;
		} else if (leaderType == "General") {
			return LType.General;
		} else {
			return LType.Scientist;
		}
	}
}
