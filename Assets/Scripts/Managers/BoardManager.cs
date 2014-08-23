using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : TIOMonoBehaviour {

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<string, PlanetSystem> systems = new Dictionary<string, PlanetSystem>();

	//Maximum System Counts
	private Dictionary<PlanetSystem, int> systemCounts = new Dictionary<PlanetSystem, int>();

	//Board
	//TODO: Probably get rid of center and nexus
	public PlanetSystem center;
	public PlanetSystem nexus;
	[SerializeField]
	private Board gameBoard;

	//Attributes for hex and section creation
	public GameObject HexPrefab;
	private float hexSize;
	private int sectionCount = 0;

	[SerializeField]
	private PlanetSystem[] systemsDebug;

	private bool isReady;
	public bool IsReady { get { return isReady; } }

	private GameManager gameManager;
	private FileManager fileManager;
	private PlayerManager playerManager;
	private LanguageManager languageManager;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		systemsDebug = new PlanetSystem[systems.Values.Count];
		systems.Values.CopyTo (systemsDebug,0);
	}

	public void Initialize() {
		fileManager = GetComponent<FileManager> ();
		gameManager = GetComponent<GameManager> ();
		playerManager = GetComponent<PlayerManager> ();
		languageManager = GetComponent<LanguageManager> ();
		HexPrefab = gameManager.HexPrefab;
		hexSize = HexPrefab.renderer.bounds.size.x;
	}

	public PlanetSystem GetSystem(string systemName) {
		if (systemName.Contains(languageManager.StringToSTag ("Home") + " " + languageManager.StringToSTag("System"))) {
			return playerManager.GetHomeSystem (systemName);
		}
		return systems [systemName];
	}

	public void LoadMap(string mapName) {
		prepareSystemsDirectory ();
		gameBoard = fileManager.ReadMapFile (mapName);
		//gameBoard.DisplayForDebug ();
	}

	public BoardSection CreateSection(PlanetSystem[][] sectionArrays, int[] columnArray) {
		BoardSection section = new BoardSection (sectionArrays, columnArray, calculateNextOrigin (), HexPrefab, hexSize);
		return section;
	}

	private Vector3 calculateNextOrigin() {
		sectionCount += 1;
		if (sectionCount == 1) {
			return Vector3.zero;
		} else {
			return new Vector3(2 * hexSize, 0, 3 * hexSize);
		}
	}

	private void prepareSystemsDirectory() {
		foreach(PlanetSystem sys in fileManager.ReadSystemFile()) {
			systems[sys.Name] = sys;
		}
	}

	private void prepareMaxSystemCounts() {
		foreach(PlanetSystem sys in systems.Values) {
			if (sys.Id == "Asteroid Field") {
				systemCounts[sys] = 4;
			} else if (sys.Id == "Supernova") {
				systemCounts[sys] = 2;
			} else if (sys.Id == "Nebula") {
				systemCounts[sys] = 2;
			} else if (sys.Id == "Ion Storm") {
				systemCounts[sys] = 2;
			} else if (sys.Id == "Empty System") {
				systemCounts[sys] = 12;
			} else {
				systemCounts[sys] = 1;
			}
		}
	}

	public PlanetSystem[][] DealSystems(int numberOfPlayers) {
		return DealSystems (numberOfPlayers, false);
	}

	public PlanetSystem[][] DealSystems(int numberOfPlayers, bool largerGalaxy) {
		//Prepare all systems
		prepareSystemsDirectory ();
		prepareMaxSystemCounts ();
		Dictionary<string, ArrayList> availableSystems = accumulateSystems ();

		//Shuffle the system groups
		CardManager cardManager = GetComponent<CardManager>();
		cardManager.ShuffleDeck<PlanetSystem> (availableSystems ["Special"]);
		cardManager.ShuffleDeck<PlanetSystem> (availableSystems ["Empty"]);
		cardManager.ShuffleDeck<PlanetSystem> (availableSystems ["Regular"]);

		//Determine the correct number of systems
		int special = 0;
		int empty = 0;
		int regular = 0;
		int remove = 0;
		switch(numberOfPlayers) {
			case 3:
				special = 3;
				empty = 5;
				regular = 16;
				break;
			case 4:
				special = 4;
				empty = 8;
				regular = 20;
				break;
			case 5:
			if (largerGalaxy) {
				special = 9;
				empty = 12;
				regular = 34;
			} else {
				special = 4;
				empty = 8;
				regular = 20;
				remove = 1;
			}
			break;
			case 6:
				if (largerGalaxy) {
					special = 9;
					empty = 12;
					regular = 34;
					remove = 1;
				} else {
					special = 4;
					empty = 8;
					regular = 20;
					remove = 2;
				}
				break;
			case 7:
				special = 9;
				empty = 12;
				regular = 34;
				remove = 4;
				break;
			case 8:
				special = 9;
				empty = 12;
				regular = 34;
				remove = 5;
				break;
		}

		//Draw the correct number of systems
		ArrayList drawnSpecialSystems = new ArrayList ();
		while (drawnSpecialSystems.Count < special) {
			drawnSpecialSystems.Add (cardManager.DrawCard<PlanetSystem>(availableSystems ["Special"]));
		}
		ArrayList drawnEmptySystems = new ArrayList();
		while (drawnEmptySystems.Count < empty) {
			drawnEmptySystems.Add (cardManager.DrawCard<PlanetSystem>(availableSystems ["Empty"]));
		}
		ArrayList drawnRegularSystems = new ArrayList();
		while (drawnRegularSystems.Count < regular) {
			drawnRegularSystems.Add (cardManager.DrawCard<PlanetSystem>(availableSystems ["Regular"]));
		}

		//Merge all drawn systems into one pile, shuffle it, and remove some as needed
		ArrayList drawnSystems = new ArrayList ();
		drawnSystems.AddRange (drawnSpecialSystems);
		drawnSystems.AddRange (drawnEmptySystems);
		drawnSystems.AddRange (drawnRegularSystems);
		cardManager.ShuffleDeck<PlanetSystem> (drawnSystems);
		while (remove > 0) {
			cardManager.DrawCard<PlanetSystem> (drawnSystems);
			remove--;
		}

		//Deal into separate piles
		PlanetSystem[][] piles = new PlanetSystem[numberOfPlayers][];
		int systemsPerPile = drawnSystems.Count / numberOfPlayers;
		int remainder = drawnSystems.Count - (systemsPerPile * numberOfPlayers);
		for(int pIndex=0; pIndex < piles.Length; pIndex++) {
			if (remainder > 0) {
				piles[pIndex] = new PlanetSystem[systemsPerPile + 1];
				remainder--;
			} else {
				piles[pIndex] = new PlanetSystem[systemsPerPile];
			}
		}
		int pileIndex = 0;
		int systemIndex = 0;
		while (drawnSystems.Count > 0) {
			piles[pileIndex%numberOfPlayers][systemIndex] = cardManager.DrawCard<PlanetSystem>(drawnSystems); 
			pileIndex++;
			if (pileIndex != 0 && pileIndex % numberOfPlayers == 0) {
				//Looping back around to first pile, so go to next systemIndex
				systemIndex++;
			}
		}

		return piles;
	}

	private Dictionary<string, ArrayList> accumulateSystems() {
		Dictionary<string, ArrayList> availableSystems = new Dictionary<string, ArrayList>();

		ArrayList specialSystems = new ArrayList ();
		ArrayList emptySystems = new ArrayList ();
		ArrayList regularSystems = new ArrayList ();

		foreach(PlanetSystem sys in systems.Values) {
			for (int i=0; i < systemCounts[sys]; i++) {
				if (sys.isFixed() && sys.Id == "Mecatol Rex") {
					center = sys;
				} else if (sys.isUnattached() && gameManager.IsActive (Option.WormholeNexus) && sys.Id == "Wormhole Nexus"){
					nexus = sys;
				} else if (sys.isSpecial()) {
					specialSystems.Add (sys);
				} else if (sys.isEmpty()) {
					emptySystems.Add (sys);
				} else {
					regularSystems.Add (sys);
				}
			}
		}

		availableSystems["Special"] = specialSystems;
		availableSystems["Empty"] = emptySystems;
		availableSystems["Regular"] = regularSystems;

		return availableSystems;
	}
}
