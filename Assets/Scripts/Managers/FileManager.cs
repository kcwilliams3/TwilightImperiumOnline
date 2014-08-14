using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class FileManager : TIOMonoBehaviour {

	//TODO: After project is finished, get rid of these test/debug variables.
	public Race[] testRaces;
	public ActionCard testActionCard;
	public Merc testMerc;
	public bool read;
	public PlanetSystem[] testSystems;

	// Directory-related Variables
	private string rawTextDir;
	private string procTextDir;

	// Managers
	private GameManager gameManager;
	private UnitManager unitManager;
	private TechManager techManager;

	// Need to check directories before Tech Manager starts
	void Awake() {
		checkDirectories ("Tools/folders.txt");
	}

	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager>();
		unitManager = GetComponent<UnitManager>();
		techManager = GetComponent<TechManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!read) {
			testRaces = new Race[17];
			testRaces[0] = ReadRaceFile ("Arborec");
			testRaces[1] = ReadRaceFile ("Gashlai");
			testRaces[2] = ReadRaceFile ("Ghosts");
			testRaces[3] = ReadRaceFile ("Hacan");
			testRaces[4] = ReadRaceFile ("Jol Nar");
			testRaces[5] = ReadRaceFile ("L1z1x");
			testRaces[6] = ReadRaceFile ("Letnev");
			testRaces[7] = ReadRaceFile ("Mentak");
			testRaces[8] = ReadRaceFile ("Naalu");
			testRaces[9] = ReadRaceFile ("Nekro");
			testRaces[10] = ReadRaceFile ("N'orr");
			testRaces[11] = ReadRaceFile ("Saar");
			testRaces[12] = ReadRaceFile ("Sol");
			testRaces[13] = ReadRaceFile ("Winnu");
			testRaces[14] = ReadRaceFile ("Xxcha");
			testRaces[15] = ReadRaceFile ("Yin");
			testRaces[16] = ReadRaceFile ("Yssaril");
			//testActionCard = GetComponent<CardManager>().getActionCard("The Hand That Takes");
			//testMerc = GetComponent<CardManager>().getMerc ("52N6");
			read = true;
			//testSystems = ReadSystemFile();
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
		string fullPath = procTextDir + gameManager.Language + "/Races/" + raceName + ".tirace";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readRaceFile(fullPath);
	}

	public Tech[] ReadTechFile() {
		string fullPath = procTextDir + gameManager.Language + "/technologies.titechs";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readTechFile (fullPath);
	}

	public ActionCard[] ReadActionFile() {
		string fullPath = procTextDir + gameManager.Language + "/actions.tiacts";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readActionFile (fullPath);
	}

	public DomainCounter[] ReadDomainFile() {
		string fullPath = procTextDir + gameManager.Language + "/domainCounters.tidomain";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readDomainFile (fullPath);
	}

	public Merc[] ReadMercFile() {
		string fullPath = procTextDir + gameManager.Language + "/mercenaries.timercs";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readMercFile (fullPath);
	}

	public Objective[] ReadObjectiveFile() {
		string fullPath = procTextDir + gameManager.Language + "/objectives.tiobjs";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readObjectiveFile (fullPath);
	}

	public PlanetSystem[] ReadSystemFile() {
		string fullPath = procTextDir + gameManager.Language + "/systems.tisysts";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readSystemFile (fullPath);
	}

	public PoliticalCard[] ReadPoliticalFile() {
		string fullPath = procTextDir + gameManager.Language + "/politicalCards.tipols";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readPoliticalFile (fullPath);
	}

	public PromissoryNote[] ReadPromissoryFile() {
		string fullPath = procTextDir + gameManager.Language + "/promissoryNotes.tiproms";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readPromissoryFile (fullPath);
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

	private DomainCounter[] readDomainFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				DomainCounter[] actions = readDomainsBlock(fileName, reader);
				
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

	private Merc[] readMercFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				Merc[] mercs = readMercsBlock(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();
				
				return mercs;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	private Objective[] readObjectiveFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				Objective[] objs = readObjectivesBlock(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();
				
				return objs;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	private PlanetSystem[] readSystemFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				PlanetSystem[] systs = readSystemsBlock("", "", fileName, reader, false, true);
				
				// Reading successfully finished
				reader.Close ();
				
				return systs;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	private PoliticalCard[] readPoliticalFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				PoliticalCard[] politicalCards = readPoliticalCardsBlock(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();
				
				return politicalCards;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	private PromissoryNote[] readPromissoryFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				PromissoryNote[] promissoryNotes = readPromissoryBlock(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();
				
				return promissoryNotes;
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
					race.HomeSystems = readSystemsBlock (dataType, dataText, fileName, reader, true, false);
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

	private PlanetSystem[] readSystemsBlock(string dataType, string dataText, string fileName, StreamReader reader, bool areHomeSystems, bool isFile) {
		if (isFile || dataText.EndsWith ("<{>")) {
			ArrayList systems = new ArrayList();
			// Start of inner block
			string line = reader.ReadLine().Trim ();
			while (line != "<}>" && !reader.EndOfStream) {
				systems.Add (readSystem(line, fileName, reader, areHomeSystems));
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				}
			} 
			// End of inner block
			return (PlanetSystem[])systems.ToArray (typeof(PlanetSystem));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: \"{1}: {2}\" should end with <{>", fileName, dataType, dataText));
		}
	}
	
	private PlanetSystem readSystem(string dataText, string fileName, StreamReader reader, bool isHomeSystem) {
		if (dataText == "<{>") {
			// Start of block
			PlanetSystem system = new PlanetSystem();
			string name = "";

			string line = reader.ReadLine().Trim ();
			while (line != "<}>") {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0] = lineParts[0].Trim ();
				string newDataText = lineParts[1] = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					name = readTextLine(newDataType, newDataText, fileName);
					if (name != "") {
						system.HasRealName = true;
					} else {
						system.HasRealName = false;
					}
				} else if (newDataType == "Expansion") {
					system.Expansion = stringToExpansion(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Type") {
					string typeString = readTextLine (newDataType, newDataText, fileName);
					if (typeString != "Standard" && typeString != "Empty") {
						if (isHomeSystem) {
							system.SysTypes = new SType[2]{stringToSType(typeString), SType.Home};
						} else {
							system.SysTypes = new SType[1]{stringToSType (typeString)};
						}
					} else if (isHomeSystem) {
						system.SysTypes = new SType[1]{SType.Home};
					}
				} else if (newDataType == "Planets") {
					Planet[] planets = readPlanetsBlock(newDataType, newDataText, fileName, reader);
					ArrayList planetsForSystem = new ArrayList();
					ArrayList wormholesForSystem = new ArrayList();
					foreach(Planet planet in planets){
						string[] parts = planet.Name.Split(' ');
						if (parts.Length == 2 && parts[1] == "Wormhole") {
							wormholesForSystem.Add (stringToWormhole(planet.Name));
						} else {
							planetsForSystem.Add (planet);
						}
					}
					system.Planets = (Planet[])planetsForSystem.ToArray(typeof(Planet));
					system.Wormholes = (Wormhole[])wormholesForSystem.ToArray(typeof(Wormhole));
				}
				line = reader.ReadLine().Trim ();
			}
			// End of block

			if (system.HasRealName == false) {
				foreach(Planet planet in system.Planets){
					if (name != "") {
						name += "/";
					}
					name += planet.Name;
				}
				foreach(Wormhole wormhole in system.Wormholes) {
					if (name != "") {
						name += "/";
					}
					name += wormhole + " Wormhole";
				}
				if (name == "") {
					name = "Empty System";
				}
				system.Name = name;
			} else if (system.Planets.Length == 1 && system.SysTypes.Length == 1 && system.SysTypes[0] == SType.Special){
				system.Name = system.Planets[0].Name + " " + name;
			} else {
				system.Name = name;
			}

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
			while (line != "<}>") {
				planets.Add (readPlanet(line, fileName, reader));
				line = reader.ReadLine().Trim ();
			} 
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
				} else if (newDataType == "Refresh Ability") {
					planet.Refresh = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Tech Specialties") {
					planet.TechSpecialties = readTechSpecsBlock( newDataType, newDataText, fileName, reader);
				}
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of block
			return planet;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private TType[] readTechSpecsBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			// Start of block
			ArrayList techSpecs = new ArrayList();
			
			string line = reader.ReadLine().Trim ();
			while (line != "<}>") {
				techSpecs.Add (stringToTType(readTextLine("", line, fileName)));
				
				line = reader.ReadLine ().Trim ();
				
			}
			// End of block
			
			return (TType[])techSpecs.ToArray(typeof(TType));
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
					string multiString = readTextLine (newDataType, newDataText, fileName);
					if (multiString == "?") {
						multiplier = -1; //represents a special case
					} else {
						multiplier = System.Convert.ToInt32(multiString);
					}
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

	private PoliticalCard[] readPoliticalCardsBlock(string fileName, StreamReader reader) {

		ArrayList politicalCards = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				politicalCards.Add(readPoliticalCard("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (PoliticalCard[])politicalCards.ToArray(typeof(PoliticalCard));
	}

	private PoliticalCard readPoliticalCard(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			PoliticalCard politicalCard = new PoliticalCard();
			string line = reader.ReadLine().Trim ();

			VType voteType = VType.ForAgainst;
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0].Trim ();
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					politicalCard.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Law") {
					politicalCard.IsLaw = stringToBool(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Expansion") {
					politicalCard.Expansion = stringToExpansion(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Flavor Text") {
					politicalCard.FlavorText = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "For") {
					politicalCard.ForText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Against") {
					politicalCard.AgainstText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Discard") {
					politicalCard.DiscardText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Elect") {
					voteType = VType.Elect;
					readElectLine(readTextLine (newDataType, newDataText, fileName), politicalCard);
				} else if (newDataType == "Effect") {
					politicalCard.ElectText = readTextLine(newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			politicalCard.VoteType = voteType;

			return politicalCard;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private void readElectLine(string line, PoliticalCard politicalCard) {
		//Determine election type
		if (line.Contains ("Player")) {
			politicalCard.ElectType = EType.Player;
		} else if (line.Contains ("Planet")) {
			politicalCard.ElectType = EType.Planet;
		} else if (line.Contains ("Public Objective")) {
			politicalCard.ElectType = EType.PublicObjective;
		} else if (line.Contains ("Current Law")) {
			politicalCard.ElectType = EType.CurrentLaw;
		} else if (line.Contains ("a Special System")) {
			politicalCard.ElectType = EType.ASpecialSystem;
		} else if (line.Contains ("Technology Color")) {
			politicalCard.ElectType = EType.TechColor;
		}

		//Determine election quantity
		string[] parts = line.Split (" ".ToCharArray());
		int quantity = 1;
		foreach(string part in parts) {
			bool isANumber;
			int conversion = stringToInt(part, out isANumber);
			if (isANumber) {
				quantity = conversion;
			}
		}
		politicalCard.ElectQuantity = quantity;
	}

	private DomainCounter[] readDomainsBlock(string fileName, StreamReader reader) {
		
		ArrayList domains = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				domains.Add(readDomain("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (DomainCounter[])domains.ToArray(typeof(DomainCounter));
	}

	private DomainCounter readDomain(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			DomainCounter domain = new DomainCounter();
			string line = reader.ReadLine().Trim ();
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0].Trim ();
				string newDataText = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					domain.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Quantity") {
					domain.Quantity = readIntLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Expansion") {
					domain.Expansion = stringToExpansion(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Qualifier") {
					domain.Qualifier = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Option") {
					domain.Option = stringToOption(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Text") {
					domain.Text = readTextLine(newDataType, newDataText, fileName);
				} 
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block
			
			return domain;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Merc[] readMercsBlock(string fileName, StreamReader reader) {
		
		ArrayList mercs = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				mercs.Add(readMerc("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (Merc[])mercs.ToArray(typeof(Merc));
	}

	private Merc readMerc(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			Merc merc = new Merc();
			string line = reader.ReadLine().Trim ();

			bool sustain = false;
			bool evasion = false;
			bool capacity = false;
			bool groundShots = false;
			bool spaceShots = false;
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0].Trim ();
				string newDataText = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					merc.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Sustain Damage") {
					sustain = merc.SustainDamage = true;
				} else if (newDataType == "Evasion") {
					merc.Evasion = readIntLine(newDataType, newDataText, fileName);
					evasion = true;
				} else if (newDataType == "Capacity") {
					merc.Capacity = readIntLine(newDataType, newDataText, fileName);
					capacity = true;
				} else if (newDataType == "Text") {
					merc.Text = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Space Battle") {
					merc.SpaceBattle = readIntLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Space Shots") {
					merc.SpaceShots = readIntLine(newDataType, newDataText, fileName);
					spaceShots = true;
				} else if (newDataType == "Ground Battle") {
					merc.GroundBattle = readIntLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Ground Shots") {
					merc.GroundShots = readIntLine(newDataType, newDataText, fileName);
					groundShots = true;
				} else if (newDataType == "Movement") {
					merc.Movement = readIntLine(newDataType, newDataText, fileName);
				}

				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			if (!sustain) {
				merc.SustainDamage = false;
			}
			if (!evasion) {
				merc.Evasion = 0;
			}
			if (!capacity) {
				merc.Capacity = 0;
			}
			if (!groundShots) {
				merc.GroundShots = 1;
			}
			if (!spaceShots) {
				merc.SpaceShots = 1;
			}
			
			return merc;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Objective[] readObjectivesBlock(string fileName, StreamReader reader) {
		
		ArrayList objs = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				objs.Add(readObjective("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (Objective[])objs.ToArray(typeof(Objective));
	}

	private Objective readObjective(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			Objective obj = new Objective();
			string line = reader.ReadLine().Trim ();
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0].Trim ();
				string newDataText = lineParts[1].Trim ();
				
				
				if (newDataType == "Name") {
					string nameString = readTextLine(newDataType, newDataText, fileName);
					if (nameString != "") {
						obj.Name = nameString;
						obj.HasRealName = true;
					}
				} else if (newDataType == "Type") {
					obj.Type = stringToOType(readTextLine (newDataType, newDataText, fileName));;
				} else if (newDataType == "Expansions") {
					obj.Expansions = readExpansionsBlock(newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Text") {
					obj.Text = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Reward") {
					string rewardText = readTextLine (newDataType, newDataText, fileName);
					if (rewardText == "I WIN THE GAME") {
						obj.RewardType = OReward.WIN;
						obj.RewardQuantity = 0;
					} else if (rewardText == "GAME OVER") {
						obj.RewardType = OReward.GAMEOVER;
						obj.RewardQuantity = 0;
					} else {
						obj.RewardType = OReward.VP;
						obj.RewardQuantity = System.Convert.ToInt32(rewardText);
					}
				} 
				
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			if (obj.Name == default(string)) {
				obj.Name = obj.Text;
				obj.HasRealName = false;
			}
			
			return obj;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Expansion[] readExpansionsBlock(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			ArrayList expansions = new ArrayList ();
			string line = reader.ReadLine().Trim ();

			do {
				// Start of an outermost block
				expansions.Add(stringToExpansion(readTextLine("", line, fileName)));

				line = reader.ReadLine ().Trim ();
			} while (line != "<}>");
		
			return (Expansion[])expansions.ToArray(typeof(Expansion));
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private PromissoryNote[] readPromissoryBlock(string fileName, StreamReader reader) {
		
		ArrayList notes = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				notes.Add(readPNote("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (PromissoryNote[])notes.ToArray(typeof(PromissoryNote));
	}
	
	private PromissoryNote readPNote(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			PromissoryNote note = new PromissoryNote();
			string line = reader.ReadLine().Trim ();
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = lineParts[0].Trim ();
				string newDataText = lineParts[1].Trim ();
				
				if (newDataType == "Name") {
					note.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Flavor Text") {
					note.FlavorText = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Rule Text") {
					note.RulesText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Play Text") {
					note.PlayText = readTextLine(newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of outermost block
			
			return note;
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

	private Option stringToOption(string optionName) {
		if (optionName == "Distant Suns") {
			return Option.DistantSuns;
		} else {
			return Option.TheFinalFrontier;
		}
	}

	private OType stringToOType(string objType) {
		if (objType == "Public Stage I") {
			return OType.PublicStageI;
		} else if (objType == "Public Stage II") {
			return OType.PublicStageII;
		} else if (objType == "Preliminary") {
			return OType.Preliminary;
		} else if (objType == "Secret") {
			return OType.Secret;
		} else {
			return OType.Special;
		}
	}

	private SType stringToSType(string sysType) {
		if (sysType == "Unattached") {
			return SType.Unattached;
		} else if (sysType == "Home") {
			return SType.Home;
		} else if (sysType == "Special") {
			return SType.Special;
		} else {
			return SType.Fixed;
		}
	}

	private Wormhole stringToWormhole(string wormhole) {
		if (wormhole == "Alpha Wormhole") {
			return Wormhole.Alpha;
		} else if (wormhole == "Beta Wormhole") {
			return Wormhole.Beta;
		} else if (wormhole == "C Wormhole") {
			return Wormhole.C;
		} else {
			return Wormhole.Delta;
		}
	}

	private bool stringToBool(string boolString) {
		if (boolString.ToLower() == "true") {
			return true;
		} else if (boolString.ToLower () == "false") {
			return false;
		} else {
			throw new System.Exception(string.Format("Error converting string {0} to bool:: no equivalent bool value", boolString));
		}
	}

	private int stringToInt(string numberString, out bool isAnInt) {
		int number = 0;
		if (numberString.ToLower()== "two") {
			isAnInt = true;
			return 2;
		} else {
			isAnInt = int.TryParse(numberString, out number);
			return number;
		}
	}
}
