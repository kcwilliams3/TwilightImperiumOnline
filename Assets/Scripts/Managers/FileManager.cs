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
	public PlanetSystem[] testSystems;

	// Directory-related Variables
	private string rawTextDir;
	[SerializeField]
	private string procTextDir;
	private string mapDir = "Assets/Maps/";

	// Managers
	private GameManager gameManager;

	// Need to check directories before Tech Manager starts
	void Awake() {
		checkDirectories ("Tools/folders.txt");
	}

	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager>();
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

	public Race ReadRaceFile(string raceID) {
		//RaceID should be the language-independent identifier. (Equivalent to english short name, currently.)
		string fullPath = procTextDir + gameManager.LanguageMgr.Language;
		if (raceID == "Lazax") {
			fullPath += "/Fall Of The Empire/";
		} else {
			fullPath += "/Races/";
		}
		fullPath += raceID + ".tirace";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readRaceFile(fullPath);
	}

	public Tech[] ReadTechFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/technologies.titechs";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readTechFile (fullPath);
	}

	public ActionCard[] ReadActionFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/actions.tiacts";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readActionFile (fullPath);
	}

	public DomainCounter[] ReadDomainFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/domainCounters.tidomain";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readDomainFile (fullPath);
	}

	public Merc[] ReadMercFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/mercenaries.timercs";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readMercFile (fullPath);
	}

	public Objective[] ReadObjectiveFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language;
		if (gameManager.Scenario == Scenario.FallOfTheEmpire) {
			fullPath += "/Fall of the Empire/scenario.tiobjs";
		} else {
			fullPath += "/objectives.tiobjs";
		}
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readObjectiveFile (fullPath);
	}

	public PlanetSystem[] ReadSystemFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/systems.tisysts";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readSystemFile (fullPath);
	}

	public PoliticalCard[] ReadPoliticalFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/politicalCards.tipols";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readPoliticalFile (fullPath);
	}

	public PromissoryNote[] ReadPromissoryFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/promissoryNotes.tiproms";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readPromissoryFile (fullPath);
	}

	public StrategyCard[] ReadStrategyFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/strategyCards.tistrats";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readStrategyFile (fullPath);
	}

	public PoliticalCard[] ReadAgendaFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/Fall of the Empire/agendas.tipols";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readPoliticalFile (fullPath);
	}

	public Treaty[] ReadTreatyFile() {
		string fullPath = procTextDir + gameManager.LanguageMgr.Language + "/Fall of the Empire/treaties.titrts";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readTreatyFile (fullPath);
	}

	public Board ReadMapFile(string mapName){
		string fullPath = mapDir + mapName + ".timap";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		return readMapFile (fullPath);
	}

	public Texture ReadSystemTexture(string sysName, string sysID, GameObject hexObject) {
		//RaceID should be the language-independent identifier. (Equivalent to english short name, currently.)
		//If it's a regular empty system, randomly choose a variant and orientation
		if (sysID == "Empty System") {
			int variant = ((int)Random.Range (1,3)); //Currently two variants. Not likely to change, so no need to make it a variable.
			sysID += " " + variant;
			sysName += " " + variant;
			hexObject.transform.Rotate(hexObject.transform.up, 60 * (int)Random.Range(0,6));
		}
		else {
			//Otherwise, change the name & ID into a valid file name
			string tempName = "";
			foreach(char c in sysID) {
				if (c != '/') {
					tempName += c; 
				}
			}
			sysID = tempName;
			tempName = "";
			foreach(char c in sysName) {
				if (c != '/') {
					tempName += c; 
				}
			}
			sysName = tempName;
		}

		//Now try to load the texture
		string directory = "Systems/" + gameManager.LanguageMgr.Language;
		Texture systemTexture;
		systemTexture = (Texture)Resources.Load (directory + "/" + sysName, typeof(Texture));


		if (systemTexture != null) {
			//If the texture exists, we're done
			return systemTexture;
		} else {
			//Otherwise, try an english asset
			directory = "Images/Systems/english/";
			systemTexture = (Texture)Resources.Load (directory + sysID, typeof(Texture));
			if (systemTexture != null) {
				//If the english texture exists, we're done
				return systemTexture;
			} else {
				//Otherwise, load the relevant system backside
				if (sysID.Contains("Home System") || sysID.Contains ("Creuss")) {
					return (Texture)Resources.Load (directory + "Home System (Back)",typeof(Texture));
				} else {
					return (Texture)Resources.Load (directory + "Regular System (Back)",typeof(Texture));
				}
			}
		} 
	}

	public void ReadLanguageFile() {
		string fullPath = procTextDir + "Localization/";
		fullPath += gameManager.LanguageMgr.Language + ".tilang";
		Debug.Log(string.Format("Reading {0}... ", fullPath));
		readLanguageFile(fullPath);
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

	private StrategyCard[] readStrategyFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				StrategyCard[] strategyCards = readStrategyBlock(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();
				
				return strategyCards;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	private Treaty[] readTreatyFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				Treaty[] treaties = readTreatiesBlock(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();
				
				return treaties;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	
	private Board readMapFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);

			using (reader) {
				Board map = readMap(fileName, reader);

				// Reading successfully finished
				reader.Close ();

				return map;
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
			return null;
		}
	}

	private void readLanguageFile(string fileName) {
		try {
			StreamReader reader = new StreamReader(fileName, Encoding.Default);
			
			using (reader) {
				readLanguage(fileName, reader);
				
				// Reading successfully finished
				reader.Close ();
			}
		}
		catch (System.Exception e) {
			Debug.Log(string.Format("{0}\n{1}\n", e.Message, e.StackTrace));
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
				string dataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string dataText = lineParts[1].Trim ();

				if (dataType == "Full Name") {
					race.FullName = readTextLine(dataType, dataText, fileName);
				} else if (dataType == "Short Name") {
					race.ShortName = readTextLine(dataType, dataText, fileName);
				} else if (dataType == "Species Name") {
					race.SpeciesName = readTextLine(dataType, dataText, fileName);
				} else if (dataType == "Expansion") {
					race.Expansion = gameManager.LanguageMgr.StringToExpansion(readTextLine(dataType, dataText, fileName));
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
				} else if (dataType == "ID") {
					race.Id = readTextLine (dataType, dataText, fileName);
				}
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of outermost block
		}

		if (race.Id == default(string)) {
			race.Id = race.ShortName;
		}

		foreach(PlanetSystem sys in race.HomeSystems) {
			sys.Expansion = race.Expansion;
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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0] = lineParts[0].Trim ());
				string newDataText = lineParts[1] = lineParts[1].Trim ();

				if (newDataType == "Name") {
					name = readTextLine(newDataType, newDataText, fileName);
					if (name != "") {
						system.HasRealName = true;
					} else {
						system.HasRealName = false;
					}
				} else if (newDataType == "Expansion") {
					system.Expansion = gameManager.LanguageMgr.StringToExpansion(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Type") {
					string typeString = readTextLine (newDataType, newDataText, fileName);
					if (typeString != gameManager.LanguageMgr.StringToSTag("Standard") && typeString != gameManager.LanguageMgr.StringToSTag("Empty")) {
						if (isHomeSystem) {
							system.SysTypes = new SType[2]{gameManager.LanguageMgr.StringToSType(typeString), SType.Home};
						} else {
							system.SysTypes = new SType[1]{gameManager.LanguageMgr.StringToSType (typeString)};
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
						if (parts.Length == 2 && parts[1] == gameManager.LanguageMgr.StringToSTag ("Wormhole")) {
							Wormhole wormhole = new Wormhole();
							wormhole.Name = planet.Name;
							wormholesForSystem.Add (wormhole);
						} else {
							planetsForSystem.Add (planet);
						}
					}
					system.Planets = (Planet[])planetsForSystem.ToArray(typeof(Planet));
					system.Wormholes = (Wormhole[])wormholesForSystem.ToArray(typeof(Wormhole));
				} else if (newDataType == "ID") {
					system.Id = readTextLine (newDataType, newDataText, fileName);
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
					name += wormhole.Name;
				}
				if (name == "") {
					name = gameManager.LanguageMgr.STagToString ("Empty") + " " + gameManager.LanguageMgr.STagToString("System");
				}
				system.Name = name;
			} else if (system.Planets.Length == 1 && system.SysTypes.Length == 1 && system.SysTypes[0] == SType.Special){
				system.Name = system.Planets[0].Name + " " + name;
			} else {
				system.Name = name;
			}

			if (system.Id == default(string)) {
				system.Id = system.Name;
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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0] = lineParts[0].Trim ());
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
				} else if (newDataType == "ID") {
					planet.Id = readTextLine (newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of block

			if (planet.Id == default(string)) {
				planet.Id = planet.Name;
			}

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
				techSpecs.Add (gameManager.LanguageMgr.StringToTType(readTextLine("", line, fileName)));
				
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
			Unit unit = new Unit(UType.SpaceDock, gameManager.TechMgr); //Need a temporary unit here for compiler
			int quantity = 0; //Need a temporary quantity here for compiler

			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Unit") {
					unit = gameManager.UnitMgr.GetUnit(gameManager.LanguageMgr.StringToUType(readTextLine (newDataType, newDataText, fileName)));
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
				Tech tech = gameManager.TechMgr.GetTech(readTextLine ("", line, fileName));
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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					leader.Name = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Type") {
					leader.LeaderType = gameManager.LanguageMgr.StringToLType(readTextLine (newDataType, newDataText, fileName));
				} else if (newDataType == "ID") {
					leader.Id = readTextLine (newDataType, newDataText, fileName);
				} 
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block

			if (leader.Id == default(string)) {
				leader.Id = leader.Name;
			}

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
					string dataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
					string dataText = lineParts[1].Trim ();

					if (dataType == "Name") {
						tech.Name = readTextLine(dataType, dataText, fileName);
					} else if (dataType == "Color") {
						tech.TechType = gameManager.LanguageMgr.StringToTType(readTextLine(dataType, dataText, fileName));
					} else if (dataType == "Expansion") {
						tech.Expansion = gameManager.LanguageMgr.StringToExpansion(readTextLine(dataType, dataText, fileName));
					} else if (dataType == "Requires") {
						// The prereq techs may not actually exist yet, so just save the name for now
						prereqs[tech] = readTextBlock (dataType, dataText, fileName, reader);
					} else if (dataType == "Text") {
						tech.Text = readTextLine(dataType, dataText, fileName);
					} else if (dataType == "Cost") {
						tech.Cost = readIntLine(dataType, dataText, fileName);
						// Only racial techs have a cost.
						tech.TechType = TType.Racial;
					} else if (dataType == "ID") {
						tech.Id = readTextLine (dataType, dataText, fileName);
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
					if (gameManager.LanguageMgr.HasTPrereqMode(techName)) {
						tech.PrereqMode = gameManager.LanguageMgr.StringToTPrereqMode(techName);
					} else if (techs.ContainsKey(techName)) {
						prereqObjects.Add(techs[techName]);
					}
				}
				tech.Prereqs = (Tech[])prereqObjects.ToArray(typeof(Tech));
			}
			if (tech.Id == default(string)) {
				tech.Id = tech.Name;
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
			int capacity = 0;
			string id = "";

			string line = reader.ReadLine().Trim ();
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

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
				} else if (newDataType == "ID") {
					id = readTextLine (newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block

			if (id == default(string)) {
				id = name;
			}

			return new Flagship(name, id, abilities, text, cost, battle, multiplier, move, capacity, gameManager.TechMgr);
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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					rep.Name = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Votes") {
					rep.Votes = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Types") {
					rep.RepTypes = readRTypesBlock (newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Text") {
					rep.Text = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "ID") {
					rep.Id = readTextLine (newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();
				
			} while (line != "<}>");
			// End of block

			if (rep.Id == default(string)) {
				rep.Id = rep.Name;
			}

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
				repTypes.Add (gameManager.LanguageMgr.StringToRType(readTextLine("", line, fileName)));

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
				uAbilities.Add (gameManager.LanguageMgr.StringToUAbility(readTextLine("", line, fileName)));
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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					action.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Quantity") {
					action.Quantity = readIntLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Expansion") {
					action.Expansion = gameManager.LanguageMgr.StringToExpansion(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Flavor Text") {
					action.FlavorText = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Rule Text A") {
					action.RulesText = readTextBlock(newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Play Text") {
					action.PlayText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Rule Text B") {
					action.DiscardText = readTextBlock(newDataType, newDataText, fileName, reader);
				} else if (newDataType == "ID") {
					action.Id = readTextLine (newDataType, newDataText, fileName);
				}
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			if (action.Id == default(string)) {
				action.Id = action.Name;
			}

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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					politicalCard.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Law") {
					politicalCard.IsLaw = gameManager.LanguageMgr.StringToBoolean(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Event") {
					bool isEvent = gameManager.LanguageMgr.StringToBoolean (readTextLine (newDataType, newDataText, fileName));
					if (isEvent) {
						voteType = VType.Event;
					}
				} else if (newDataType == "Expansion") {
					politicalCard.Expansion = gameManager.LanguageMgr.StringToExpansion(readTextLine(newDataType, newDataText, fileName));
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
					if (voteType == VType.Event) {
						politicalCard.EventText = readTextLine(newDataType, newDataText, fileName);
					} else {
						politicalCard.ElectText = readTextLine(newDataType, newDataText, fileName);
					}
				} else if (newDataType == "ID") {
					politicalCard.Id = readTextLine (newDataType, dataText, fileName);
				}
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			politicalCard.VoteType = voteType;

			if (politicalCard.Id == default(string)) {
				politicalCard.Id = politicalCard.Name;
			}

			return politicalCard;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private void readElectLine(string line, PoliticalCard politicalCard) {
		string[] parts = line.Split (" ".ToCharArray());

		//Determine election type
		politicalCard.ElectType = gameManager.LanguageMgr.ExtractEType(parts);

		//Determine election quantity
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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					domain.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Quantity") {
					domain.Quantity = readIntLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Expansion") {
					domain.Expansion = gameManager.LanguageMgr.StringToExpansion(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Qualifier") {
					domain.Qualifier = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Option") {
					domain.Option = gameManager.LanguageMgr.StringToOption(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Text") {
					domain.Text = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "ID") {
					domain.Id = readTextLine (newDataType, newDataText, fileName);
				} 
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			if (domain.Id == default(string)) {
				domain.Id = domain.Name;
			}

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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
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
				} else if (newDataType == "ID") {
					merc.Id = readTextLine (newDataType, newDataText, fileName);
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

			if (merc.Id == default(string)) {
				merc.Id = merc.Name;
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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					string nameString = readTextLine(newDataType, newDataText, fileName);
					if (nameString != "") {
						obj.Name = nameString;
						obj.HasRealName = true;
					}
				} else if (newDataType == "Type") {
					obj.Type = gameManager.LanguageMgr.StringToOType(readTextLine (newDataType, newDataText, fileName));;
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
					} else if (rewardText == "Immediate Victory") {
						obj.RewardType = OReward.INSTANTWIN;
						obj.RewardQuantity = 0;
					} else {
						obj.RewardType = OReward.VP;
						obj.RewardQuantity = System.Convert.ToInt32(rewardText);
					}
				} else if (newDataType == "ID") {
					obj.Id = readTextLine (dataType, dataText, fileName);
				}
				
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			if (obj.Name == default(string)) {
				obj.Name = obj.Text;
				obj.HasRealName = false;
			}

			if (obj.Id == default(string)) {
				obj.Id = obj.Name;
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
				expansions.Add(gameManager.LanguageMgr.StringToExpansion(readTextLine("", line, fileName)));

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
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					note.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Flavor Text") {
					note.FlavorText = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Rule Text") {
					note.RulesText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Play Text") {
					note.PlayText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "ID") {
					note.Id = readTextLine (newDataType, dataText, fileName);
				}
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of outermost block

			if (note.Id == default(string)) {
				note.Id = note.Name;
			}

			return note;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private StrategyCard[] readStrategyBlock(string fileName, StreamReader reader) {
		
		ArrayList strategyCards = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				strategyCards.Add(readStrategyCard("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (StrategyCard[])strategyCards.ToArray(typeof(StrategyCard));
	}

	private StrategyCard readStrategyCard(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			StrategyCard strategyCard = new StrategyCard();
			string line = reader.ReadLine().Trim ();
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);

				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					strategyCard.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Initiative") {
					strategyCard.Initiative = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Set") {
					strategyCard.Set = gameManager.LanguageMgr.StringToStrategySet(readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "Primary Ability") {
					strategyCard.PrimaryAbility = readStrategyAbility(newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Secondary Ability") {
					strategyCard.SecondaryAbility = readStrategyAbility(newDataType, newDataText, fileName, reader);
				} else if (newDataType == "Special") {
					strategyCard.Special = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "ID") {
					strategyCard.Id = readTextLine (dataType, dataText, fileName);
				}
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of outermost block

			if (strategyCard.Id == default(string)) {
				strategyCard.Id = strategyCard.Name;
			}

			return strategyCard;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private StrategyAbility readStrategyAbility(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			StrategyAbility strategyAbility = new StrategyAbility();
			string line = reader.ReadLine().Trim ();
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					strategyAbility.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Text") {
					strategyAbility.Text = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "ID") {
					strategyAbility.Id = readTextLine (newDataType, newDataText, fileName);
				} 
				line = reader.ReadLine().Trim ();
			} while (line != "<}>");
			// End of outermost block

			if (strategyAbility.Id == default(string)) {
				strategyAbility.Id = strategyAbility.Name;
			}

			return strategyAbility;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Treaty[] readTreatiesBlock(string fileName, StreamReader reader) {
		
		ArrayList treaties = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();
		
		do {
			if (line == "<{>") {
				// Start of an outermost block
				treaties.Add(readTreaty("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return (Treaty[])treaties.ToArray(typeof(Treaty));
	}
	
	private Treaty readTreaty(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			Treaty treaty = new Treaty();
			string line = reader.ReadLine().Trim ();
			
			do {
				string[] lineParts;
				//Split category name from data
				lineParts = line.Split(":".ToCharArray(), 2);
				
				//Remove any extra whitespace from parts & set descriptive variables
				string newDataType = gameManager.LanguageMgr.StringToDataType(lineParts[0].Trim ());
				string newDataText = lineParts[1].Trim ();

				if (newDataType == "Name") {
					treaty.Name = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Flavor Text") {
					treaty.FlavorText = readTextLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Rule Text") {
					treaty.RulesText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Suggestion") {
					treaty.SuggestionText = readTextLine(newDataType, newDataText, fileName);
				} else if (newDataType == "Rank") {
					treaty.Rank = readIntLine (newDataType, newDataText, fileName);
				} else if (newDataType == "Race") {
					treaty.Race = gameManager.PlayerMgr.GetRace (readTextLine(newDataType, newDataText, fileName));
				} else if (newDataType == "ID") {
					treaty.Id = readTextLine (newDataType, newDataText, fileName);
				}
				
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			if (treaty.Id == default(string)) {
				treaty.Id = treaty.Name;
			}

			return treaty;
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private Board readMap(string fileName, StreamReader reader) {
		ArrayList sections = new ArrayList ();
		
		string line = reader.ReadLine().Trim ();

		do {
			if (line == "<{>") {
				// Start of an outermost block
				sections.Add(readMapSection("", line, fileName, reader));
				
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);
		
		return new Board((BoardSection[])sections.ToArray(typeof(BoardSection)));
	}

	private BoardSection readMapSection(string dataType, string dataText, string fileName, StreamReader reader) {
		if (dataText == "<{>") {
			string line = reader.ReadLine().Trim ();

			// Read "first column" information
			string[] firstColumnStrings = readTextLine ("", line, fileName).Split (",".ToCharArray());
			ArrayList firstCols = new ArrayList();
			foreach(string numberString in firstColumnStrings) {
				firstCols.Add(System.Convert.ToInt32(numberString));
			}

			int[] columnArray = (int[])firstCols.ToArray (typeof(int));

			line = reader.ReadLine().Trim ();

			ArrayList mapGrid = new ArrayList();
			do {
				string[] systemNames = readTextLine ("", line, fileName).Split (",".ToCharArray());
				ArrayList systems = new ArrayList();
				foreach(string name in systemNames) {
					systems.Add(gameManager.BoardMgr.GetSystem(name));
					//Debug.Log ("Added " + name + ": " + ((PlanetSystem)systems[systems.Count-1]).Name + " , " + ((PlanetSystem)systems[systems.Count-1]).Id);
				}
				mapGrid.Add ((PlanetSystem[])systems.ToArray (typeof(PlanetSystem)));
				line = reader.ReadLine().Trim ();	
			} while (line != "<}>");
			// End of outermost block

			PlanetSystem[][] section = new PlanetSystem[mapGrid.Count][];
			for (int i=0; i<mapGrid.Count; i++) {
				section[i] = (PlanetSystem[])mapGrid[i];
			}

			return gameManager.BoardMgr.CreateSection(section, columnArray);
		} else {
			throw new System.Exception(string.Format("Error reading file {0}:: got \"{1}\" should be <{>", fileName, dataText));
		}
	}

	private void readLanguage(string fileName, StreamReader reader) {
		string line = reader.ReadLine().Trim ();
		int sectionIndex = 0;
		do {
			if (line == "<{>") {
				// Start of an outermost block
				line = reader.ReadLine ().Trim ();
				do {
					line = readTextLine("", line, fileName);
					string englishString = line;
					string languageString = line;
					if (line.Contains (",")) {
						string[] parts = line.Split(",".ToCharArray());
						englishString = parts[0];
						languageString = parts[1];
					}
					switch(sectionIndex) {
						case 0:
							gameManager.LanguageMgr.AddNumber(languageString, englishString);
							break;
						case 1:
							gameManager.LanguageMgr.AddBoolean(languageString, englishString);
							break;
						case 2:
							gameManager.LanguageMgr.AddTPrereqMode(languageString, englishString);
							break;
						case 3:
							gameManager.LanguageMgr.AddDataType(languageString, englishString);
							break;
						case 4:
							gameManager.LanguageMgr.AddExpansion(languageString, englishString);
							break;
						case 5:
							gameManager.LanguageMgr.AddScenario(languageString, englishString);
							break;
						case 6:
							gameManager.LanguageMgr.AddUType(languageString, englishString);
							break;
						case 7:
							gameManager.LanguageMgr.AddUAbility(languageString, englishString);
							break;
						case 8:
							gameManager.LanguageMgr.AddTType(languageString, englishString);
							break;
						case 9:
							gameManager.LanguageMgr.AddSTag(languageString, englishString);
							break;
						case 10:
							gameManager.LanguageMgr.AddSType(languageString, englishString);
							break;
						case 11:
							gameManager.LanguageMgr.AddOType(languageString, englishString);
							break;
						case 12:
							gameManager.LanguageMgr.AddOReward(languageString, englishString);
							break;
						case 13:
							gameManager.LanguageMgr.AddStrategySet(languageString, englishString);
							break;
						case 14:
							gameManager.LanguageMgr.AddEType(languageString, englishString);
							break;
						case 15:
							gameManager.LanguageMgr.AddRType(languageString, englishString);
							break;
						case 16:
							gameManager.LanguageMgr.AddOption(languageString, englishString);
							break;
						case 17:
							gameManager.LanguageMgr.AddLType(languageString, englishString);
							break;
					}
					line = reader.ReadLine ().Trim ();
				} while (line != "<}>");
				sectionIndex += 1;
				if (!reader.EndOfStream) {
					line = reader.ReadLine().Trim ();
				} 
			}
		} while (line == "<{>" && !reader.EndOfStream);

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


	// Conversions
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
