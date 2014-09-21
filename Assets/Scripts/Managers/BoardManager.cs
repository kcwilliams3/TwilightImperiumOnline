using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : TIOMonoBehaviour {

	//TODO: May not need these dictionaries. Decide.
	private Dictionary<string, PlanetSystem> systems = new Dictionary<string, PlanetSystem>();

	//Maximum System Counts
	private Dictionary<PlanetSystem, int> systemCounts = new Dictionary<PlanetSystem, int>();

	//Board
	//TODO: Probably get rid of nexus
	//public PlanetSystem center;
	public PlanetSystem nexus;
	[SerializeField]
	private Board gameBoard;
	private int[] center;

	//Attributes for hex and section creation
	public GameObject HexPrefab;
	private float hexSize;
	private int sectionCount = 0;

	[SerializeField]
	private PlanetSystem[] systemsDebug;

	private bool isReady;
	public bool IsReady { get { return isReady; } }

	private GameManager gameManager;


	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager> ();
		hexSize = HexPrefab.renderer.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
		systemsDebug = new PlanetSystem[systems.Values.Count];
		systems.Values.CopyTo (systemsDebug,0);
	}

	public void Initialize() {
		prepareSystemsDirectory ();
	}

	public PlanetSystem GetSystem(string systemName) {
		if (systemName.Contains(gameManager.LanguageMgr.StringToSTag ("Home") + " " + gameManager.LanguageMgr.StringToSTag("System"))) {
			return gameManager.PlayerMgr.GetHomeSystem (systemName);
		}
		return systems [systemName];
	}

	public void LoadMap(string mapName) {
		//prepareSystemsDirectory ();
		gameBoard = gameManager.FileMgr.ReadMapFile (mapName);
		//gameBoard.DisplayForDebug ();
	}
	
	public void PrepareGalaxySetup() {
		int[] center = determineCenter();

		BoardSection[] boardSections = new BoardSection[1];
		int ringCount;
		BoardType boardType;
		if (gameManager.PlayerCount <= 3) {
			//1 & 2 aren't valid player numbers, so we assume 3 in those cases (for quicker solo testing)
			boardType = BoardType.ThreePlayer;
		} else {
			boardType = BoardType.Hexagon;
		}
		if (gameManager.PlayerCount < 7) {
			ringCount = 3;
		} else {
			ringCount = 4;
		}

		float rotation = determineBoardRotation();
		boardSections[0] = new BoardSection(ringCount, boardType, calculateNextOrigin (), center, HexPrefab, hexSize, rotation);

		gameBoard = new Board(boardSections);
		setHomeSystems();
		gameBoard.SetSystem(GetSystem("Mecatol Rex"), 0, center[0], center[1]);

		repositionBoard(rotation);
	}

	private void setHomeSystems() {
		int[][] positions = determineHomePositions();

		Player[] players = gameManager.PlayerMgr.GetPlayers();
		for (int i=0; i < players.Length; i++) {
			gameBoard.SetSystem(players[i].Race.HomeSystems[0], 0, positions[i][0], positions[i][1]);
		}
	}

	private int[][] determineHomePositions() {
		// TODO: Will need to extend this for Warp Zone options at some point
		int playerCount = gameManager.PlayerCount;
		int[][] positions = new int[playerCount][];
		if (playerCount == 3) {
			positions[0] = new int[2] { 0, 0 };
			positions[1] = new int[2] { 3, 5 };
			positions[2] = new int[2] { 6, 0 };
		} else if (playerCount == 4) {
			positions[0] = new int[2] { 0, 2 };
			positions[1] = new int[2] { 4, 5 };
			positions[2] = new int[2] { 6, 1 };
			positions[3] = new int[2] { 2, 0 };
		} else if (playerCount == 5) {
			positions[0] = new int[2] { 0, 2 };
			positions[1] = new int[2] { 3, 6 };
			positions[2] = new int[2] { 6, 3 };
			positions[3] = new int[2] { 6, 0 };
			positions[4] = new int[2] { 2, 0 };
		} else if (playerCount == 6) {
			positions[0] = new int[2] { 0, 0 };
			positions[1] = new int[2] { 0, 3 };
			positions[2] = new int[2] { 3, 6 };
			positions[3] = new int[2] { 6, 3 };
			positions[4] = new int[2] { 6, 1 };
			positions[5] = new int[2] { 3, 0 };
		} else if (playerCount == 7) {
			positions[0] = new int[2] { 0, 0 };
			positions[1] = new int[2] { 0, 3 };
			positions[2] = new int[2] { 3, 7 };
			positions[3] = new int[2] { 6, 6 };
			positions[4] = new int[2] { 8, 3 };
			positions[5] = new int[2] { 7, 0 };
			positions[6] = new int[2] { 4, 0 };
		} else if (playerCount == 8) {
			positions[0] = new int[2] { 0, 1 };
			positions[1] = new int[2] { 0, 4 };
			positions[2] = new int[2] { 3, 7 };
			positions[3] = new int[2] { 6, 6 };
			positions[4] = new int[2] { 8, 3 };
			positions[5] = new int[2] { 8, 0 };
			positions[6] = new int[2] { 5, 0 };
			positions[7] = new int[2] { 2, 0 };
		}

		return positions;
	}

	private float determineBoardRotation() {
		// TODO: Will need to extend this for Warp Zone options at some point
		int playerCount = gameManager.PlayerCount;
		float[] rotations = new float[playerCount];
		if (playerCount == 3) {
			rotations[0] = 180.0f;
			rotations[1] = 60.0f;
			rotations[2] = -60.0f;
		} else if (playerCount == 4) {
			rotations[0] = 120.0f;
			rotations[1] = 60.0f;
			rotations[2] = -60.0f;
			rotations[3] = -120.0f;
		} else if (playerCount == 5) {
			rotations[0] = 120.0f;
			rotations[1] = 60.0f;
			rotations[2] = 0.0f;
			rotations[3] = -60.0f;
			rotations[4] = -120.0f;
		} else if (playerCount == 6) {
			rotations[0] = 180.0f;
			rotations[1] = 120.0f;
			rotations[2] = 60.0f;
			rotations[3] = 0.0f;
			rotations[4] = -60.0f;
			rotations[5] = -120.0f;
		} else if (playerCount == 7) {
			rotations[0] = 180.0f;
			rotations[1] = 120.0f;
			rotations[2] = 60.0f;
			rotations[3] = 60.0f;
			rotations[4] = 0.0f;
			rotations[5] = -60.0f;
			rotations[6] = -120.0f;
		} else if (playerCount == 8) {
			rotations[0] = 180.0f;
			rotations[1] = 120.0f;
			rotations[2] = 60.0f;
			rotations[3] = 60.0f;
			rotations[4] = 0.0f;
			rotations[5] = -60.0f;
			rotations[6] = -120.0f;
			rotations[7] = -120.0f;
		}

		float chosenRotation = 0.0f;
		Player[] players = gameManager.PlayerMgr.GetPlayers();
		for (int i=0; i < players.Length; i++) {
			if (players[i].Id == gameManager.NetworkMgr.PlayerName) {
				chosenRotation = rotations[i];
			}
		}

		return chosenRotation;
	}

	private int[] determineCenter() {
		// TODO: Will need to extend this for Warp Zone options at some point
		int[] center = new int[2];
		if (gameManager.PlayerCount == 3) {
			center[0] = 3;
			center[1] = 2;
		} else if (gameManager.PlayerCount >= 7) {
			center[0] = 4;
			center[1] = 4;
		} else {
			center[0] = 3;
			center[1] = 3;
		}
		return center;
	}

	private void repositionBoard(float rotation) {
		GameObject boardObject = GameObject.Find("Board");
		boardObject.transform.RotateAround(boardObject.transform.position, boardObject.transform.up, rotation);
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
		foreach(PlanetSystem sys in gameManager.FileMgr.ReadSystemFile()) {
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
		prepareMaxSystemCounts ();
		Dictionary<string, ArrayList> availableSystems = accumulateSystems ();

		//Shuffle the system groups
		gameManager.CardMgr.ShuffleDeck<PlanetSystem> (availableSystems ["Special"]);
		gameManager.CardMgr.ShuffleDeck<PlanetSystem> (availableSystems ["Empty"]);
		gameManager.CardMgr.ShuffleDeck<PlanetSystem> (availableSystems ["Regular"]);

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
			drawnSpecialSystems.Add (gameManager.CardMgr.DrawCard<PlanetSystem>(availableSystems ["Special"]));
		}
		ArrayList drawnEmptySystems = new ArrayList();
		while (drawnEmptySystems.Count < empty) {
			drawnEmptySystems.Add (gameManager.CardMgr.DrawCard<PlanetSystem>(availableSystems ["Empty"]));
		}
		ArrayList drawnRegularSystems = new ArrayList();
		while (drawnRegularSystems.Count < regular) {
			drawnRegularSystems.Add (gameManager.CardMgr.DrawCard<PlanetSystem>(availableSystems ["Regular"]));
		}

		//Merge all drawn systems into one pile, shuffle it, and remove some as needed
		ArrayList drawnSystems = new ArrayList ();
		drawnSystems.AddRange (drawnSpecialSystems);
		drawnSystems.AddRange (drawnEmptySystems);
		drawnSystems.AddRange (drawnRegularSystems);
		gameManager.CardMgr.ShuffleDeck<PlanetSystem> (drawnSystems);
		while (remove > 0) {
			gameManager.CardMgr.DrawCard<PlanetSystem> (drawnSystems);
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
			piles[pileIndex%numberOfPlayers][systemIndex] = gameManager.CardMgr.DrawCard<PlanetSystem>(drawnSystems); 
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
					//center = sys;
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
