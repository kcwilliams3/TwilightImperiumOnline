using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class FileManager : TIOMonoBehaviour {

	public Race testRace;
	public ActionCard testActionCard;
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
			testRace = ReadRaceFile ("Gashlai");
			testActionCard = GetComponent<CardManager>().getActionCard("The Hand That Takes");
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

	public ActionCard[] ReadActionFile() {
		string fullPath = procTextDir + language + "/actions.tiacts";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readActionFile (fullPath);
	}


	/*
	 * Specific File Readers
	 */

	private Race readRaceFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);

			using (reader) {
				Race race = readRace(fileName, reader);

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

	private ActionCard[] readActionFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				ActionCard[] actions = readActionsBlock(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();

				return actions;
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

	private Race readRace(string fileName, StreamReader reader) {

		Race race = new Race ();

		string line = reader.ReadLine().Trim ();
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
					race.Expansion = stringToExpansion(readTextLine(dataType, dataText, fileName));
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
					race.Flagship = readFlagship(dataType, dataText, fileName, reader);
				} else if (dataType == "Representatives") {
					race.Representatives = readRepsBlock(dataType, dataText, fileName, reader);
				}
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of outermost block
		}

		return race;
	}

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
			Unit unit = new Unit(UType.SpaceDock, unitManager); //Need a temporary unit here for compiler
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
					unit = unitManager.GetUnit(stringToUType(readTextLine (newDataType, newDataText, fileName)));
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
				Tech tech = techManager.GetTech(readTextLine ("", line, fileName));
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
						tech.Expansion = stringToExpansion(readTextLine(dataType, dataText, fileName));
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

	private Flagship readFlagship(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			string name = "";
			string text = "";
			UAbility[] abilities = {};
			int cost = 0;
			int battle = 0;
			int multiplier = 0;
			int move = 0;
			int capacity = 0;;

			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0] = lineParts[0].Trim ();
				string newDataText = lineParts[1] = lineParts[1].Trim ();
				
				if (newDataType == "Name") {
					name = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Abilities") {
					abilities = readUAbilitiesBlock (newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Text") {
					text = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Cost") {
					cost = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Battle") {
					battle = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Multiplier") {
					multiplier = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Move") {
					move = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Capacity") {
					capacity = readIntLine(newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block
			
			return new Flagship(name, abilities, text, cost, battle, multiplier, move, capacity, unitManager);
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Representative[] readRepsBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			ArrayList reps = new ArrayList();
			
			string line = reader.ReadLine().Trim ();
			do {
				reps.Add (readRep("", line, fileName, reader));
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of block
			
			return (Representative[])reps.ToArray(typeof(Representative));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Representative readRep(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			Representative rep = new Representative();
			
			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0] = lineParts[0].Trim ();
				string newDataText = lineParts[1] = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					rep.Name = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Votes") {
					rep.Votes = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Types") {
					rep.RepTypes = readRTypesBlock (newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Text") {
					rep.Text = readTextLine (newDataType, newDataText, fileName);
				} 
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block
			
			return rep;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private RType[] readRTypesBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			ArrayList repTypes = new ArrayList();
			
			string line = reader.ReadLine().Trim ();
			do {
				repTypes.Add (stringToRType(readTextLine("", line, fileName)));

				line = reader.ReadLine ().Trim ();
				
			} while (line != "<}>");
			// End of block
			
			return (RType[])repTypes.ToArray(typeof(RType));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private UAbility[] readUAbilitiesBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			ArrayList uAbilities = new ArrayList();
			
			string line = reader.ReadLine().Trim ();

			do {
				uAbilities.Add (stringToUAbility(readTextLine("", line, fileName)));
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of block
			
			return (UAbility[])uAbilities.ToArray(typeof(UAbility));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private ActionCard[] readActionsBlock(string fileName, StreamReader reader) {

		ArrayList actions = new ArrayList ();

		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				actions.Add(readAction("", line, fileName, reader));

				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (ActionCard[])actions.ToArray(typeof(ActionCard));
	}

	private ActionCard readAction(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			ActionCard action = new ActionCard();
			string line = reader.ReadLine().Trim ();
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0].Trim ();
				string newDataText = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					action.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Quantity") {
					action.Quantity = readIntLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Expansion") {
					action.Expansion = stringToExpansion(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Flavor Text") {
					action.FlavorText = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Rule Text A") {
					action.RulesText = readTextBlock(newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Play Text") {
					action.PlayText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Rule Text B") {
					action.DiscardText = readTextBlock(newDataType, newDataText, fileName, reader);
				}
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			return action;
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

	private RType stringToRType(string repType) {
		if (repType == "Spy") {
			return RType.Spy;
		} else if (repType == "Councilor") {
			return RType.Councilor;
		} else {
			return RType.Bodyguard;
		}
	}

	private UAbility stringToUAbility(string unitAbility) {
		if (unitAbility == "Sustain Damage") {
			return UAbility.SustainDamage;
		} else if (unitAbility == "Anti-Fighter Barrage") {
			return UAbility.AntiFighterBarrage;
		} else if (unitAbility == "Bombardment") {
			return UAbility.Bombardment;
		} else if (unitAbility == "Planetary Shield") {
			return UAbility.PlanetaryShield;
		} else {
			return UAbility.Production;
		}
	}

	private Expansion stringToExpansion(string expansionName) {
		if (expansionName == "Shattered Empire") {
			return Expansion.ShatteredEmpire;
		} else if (expansionName == "Shards of the Throne") {
			return Expansion.ShardsOfTheThrone;
		} else {
			return Expansion.Vanilla;
		}
	}
}
