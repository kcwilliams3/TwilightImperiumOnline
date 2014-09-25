using UnityEngine;
using System;
using System.Collections.Generic;

public enum BoardType {Hexagon, ThreePlayer, WarpZone};
public enum HexDirection {North, South, Northeast, Southeast, Northwest, Southwest};

[System.Serializable]
public class Board {

	[SerializeField]
	private BoardSection[] hexMap;		//Unconnected sections of the board

	public Board(BoardSection[] inMap) {
		hexMap = inMap;
	}

	public void SetSystem(PlanetSystem pSystem, int sectionNumber, int row, int col) {
		hexMap[sectionNumber].SetSystem(pSystem, row, col);
	}

	public SystemHex GetSystem(int boardSection, int row, int col) {
		return hexMap[boardSection].GetSystem(row, col);
	}

	public BoardSection GetSection(int i) {
		return hexMap[i];
	}
	
//	public void Recenter(int centerRow, int centerCol) {
//		Vector3 centerLocation = GetHexLocation(centerCol, centerRow);
//		Debug.Log ("Center: (" + centerLocation.x.ToString() + ", " + centerLocation.y.ToString() + ")");
//		Debug.Log ("Origin: (" + origin.x.ToString() + ", " + origin.y.ToString() + ", " + origin.z.ToString() + ")");
//		Vector3 translation = new Vector3(centerLocation.x-origin.x, centerLocation.y-origin.y, centerLocation.z-origin.z);
//		Debug.Log ("Reposition  vector: (" + translation.x.ToString() + ", " + translation.y.ToString() + ", " + translation.z.ToString() + ")");
////		foreach (BoardSection section in hexMap) {
////			section.Recenter(centerRow, centerCol);
////		}
//	}

	public void DisplayForDebug() {
		foreach(BoardSection section in hexMap) {
			section.DisplayForDebug();
		}
	}
}

[System.Serializable]
public class BoardSection {
	
	private SystemHex[][] hexMap; // q, r: Axial coordinate system (2 axes: x, -z)
	[SerializeField]
	private int[] firstColumns;
	[SerializeField]
	private Vector3 origin;
	public Vector3 Origin { get { return origin; } }
	private int[] center;

	private FileManager fileManager;

	private float hexSize;
	private int maxRowSize;
	private int sectionNumber;

	public BoardSection(int section, int ringCount, BoardType boardType, Vector3 sectionOrigin, int[] pCenter, GameObject pHexPrefab, float pHexSize, float rotation, Color emptyColor) {
		sectionNumber = section;
		origin = sectionOrigin;
		hexSize = pHexSize;
		center = pCenter;

		// Determine what column each row starts in, and how large the middle/max length row is
		if (boardType == BoardType.ThreePlayer) {
			firstColumns = new int[7] {3, 2, 2, 1, 1, 0, 0};
			maxRowSize = firstColumns.Length-1;
		} else if (boardType == BoardType.Hexagon) {
			//Note: This assumes a maximum ring count of 4.
			firstColumns = new int[1 + (2 * ringCount)];
			for (int i=ringCount; i >= 0; i--) {
				firstColumns[ringCount - i] = i;
			}
			//All remaining rows start all the way to the left
			for (int i=ringCount; i < firstColumns.Length; i++) {
				firstColumns[i] = 0;
			}
			maxRowSize = firstColumns.Length;
		}

		// Set up the map shape/size
		hexMap = new SystemHex[firstColumns.Length][];
		int rowLength=0;
		SystemHex[] hexRow1;
		SystemHex[] hexRow2 = new SystemHex[1];
		for (int i=0; i <= hexMap.Length / 2; i++) {
			if (boardType == BoardType.ThreePlayer) {
				if (i == 0) {
					rowLength = i + ringCount - 1;
				} else {
					rowLength = i + ringCount;
				}
			} else if (boardType == BoardType.Hexagon) {
				rowLength = i + ringCount + 1;
			}
			//Work in from both sides
			hexRow1 = new SystemHex[rowLength];
			if (i != hexMap.Length / 2) {
				hexRow2 = new SystemHex[rowLength];
			}
			for (int j=0; j < rowLength; j++) {
				//Create hex object and add systemHex script
				hexRow1[j] = setEmptyHexSlot(pHexPrefab, GetHexLocation (j,i), rotation, emptyColor);
				hexRow1[j].SetSection(sectionNumber);
				//Debug.Log("<><><><>: " + i + "    ,     " + j);
				hexRow1[j].SetPosition(arrayCoordsToAxial(j, i));
				if (i != hexMap.Length / 2) {
					hexRow2[j] = setEmptyHexSlot(pHexPrefab, GetHexLocation(j,hexMap.Length - i - 1), rotation, emptyColor);
					hexRow2[j].SetSection(sectionNumber);
					//Debug.Log("<><><><>: " + (hexMap.Length - i - 1) + "    ,     " + j);
					hexRow2[j].SetPosition(arrayCoordsToAxial(j, hexMap.Length - i - 1));
				}
			}

			hexMap[i] = hexRow1;
			if (i != hexMap.Length / 2) {
				hexMap[hexMap.Length - i - 1] = hexRow2;
			}
		}
	}

	private SystemHex setEmptyHexSlot(GameObject hexPrefab, Vector3 hexLocation, float rotation, Color emptyColor) {
		GameObject hexObject = (GameObject)GameObject.Instantiate(hexPrefab, hexLocation, Quaternion.identity);
		SystemHex sysHex = hexObject.AddComponent<SystemHex>();
		hexObject.transform.parent = GameObject.Find("Board").transform;
		hexObject.transform.Rotate(0.0f, -rotation, 0.0f);
		hexObject.transform.FindChild("Top").gameObject.SetActive(false);
		fileManager = GameObject.Find("Manager").GetComponent<FileManager>();
//		Material topMaterial = hexObject.transform.FindChild ("Top").renderer.material;
//		topMaterial.mainTexture = fileManager.ReadSystemTexture("Regular System (Back)", "Regular System (Back)", hexObject);
//		topMaterial.color = new Color(topMaterial.color.r, topMaterial.color.g, topMaterial.color.b, .3f);
		Material sideMaterial = hexObject.renderer.material;
		sideMaterial.mainTexture = fileManager.ReadSystemTexture("System Placeholder", "System Placeholder", hexObject);
		emptyColor.a = 0.3f;
		sideMaterial.color = emptyColor;
		hexObject.name = "<Empty System Slot>";
		sysHex.IsValidPlacement = false;
		return sysHex;
	}

	public BoardSection(int section, PlanetSystem[][] inMap, int[] inFirstColumns, Vector3 sectionOrigin, GameObject pHexPrefab, float pHexSize) {
		sectionNumber = section;
		origin = sectionOrigin;
		maxRowSize = inMap [inMap.Length / 2].Length/2;
		hexSize = pHexSize;
		firstColumns = inFirstColumns;
		hexMap = new SystemHex[inMap.Length][];
		for(int i=0;i<inMap.Length;i++) {
			SystemHex[] hexRow = new SystemHex[inMap[i].Length];
			for(int j=0;j<inMap[i].Length;j++) {
				//Create hex object and add systemHex script
				GameObject hexObject = (GameObject)GameObject.Instantiate(pHexPrefab, GetHexLocation (j,i), Quaternion.identity);
				SystemHex sysHex = hexObject.AddComponent<SystemHex>();
				sysHex.System = inMap[i][j];
				sysHex.SetSection(sectionNumber);
				sysHex.SetPosition(arrayCoordsToAxial(i, j));
				hexObject.transform.parent = GameObject.Find("Board").transform;
				fileManager = GameObject.Find("Manager").GetComponent<FileManager>();
				hexObject.transform.FindChild("Top").renderer.material.mainTexture = fileManager.ReadSystemTexture(inMap[i][j].Name, inMap[i][j].Id, hexObject);
				hexObject.name = inMap[i][j].Name;
				hexRow[j] = sysHex;
			}
			hexMap[i] = hexRow;
		}
	}

	public void SetSystem(PlanetSystem sys, int row, int col) {
		//Debug.Log (row + "   ,   " + col + " :::::: " + hexMap.Length + " ::::::::::: " + hexMap[row].Length);
		Vector2 arrayCoords = axialToArrayCoords(col, row);
		//Debug.Log (arrayCoords.x + "   ,   " + arrayCoords.y);
		SystemHex hex = hexMap[(int)arrayCoords.x][(int)arrayCoords.y];
		hex.System = sys;
		hex.SetSection(sectionNumber);
		hex.SetPosition(new Vector2(row, col));
		fileManager = GameObject.Find("Manager").GetComponent<FileManager>();
		Material topMaterial = hex.gameObject.transform.FindChild ("Top").renderer.material;
		topMaterial.mainTexture = fileManager.ReadSystemTexture(sys.Name, sys.Id, hex.gameObject);
		topMaterial.color = new Color(topMaterial.color.r, topMaterial.color.g, topMaterial.color.b, 1.0f);
		Material sideMaterial = hex.gameObject.renderer.material;
		sideMaterial.mainTexture = fileManager.ReadSystemTexture("Empty System 1", "Empty System 1", hex.gameObject);
		sideMaterial.color = new Color(0.035f, 0.075f, 0.212f, 1.0f);
		hex.gameObject.transform.FindChild("Top").gameObject.SetActive(true);
		hex.gameObject.name = hex.System.Name;

		if (sys.isSpecial()) {
			foreach (SystemHex iterHex in HexesInRadius(hex.GetPosition(), 1)) {
				iterHex.NextToSpecial = true;
			}
		}
		hex.IsValidPlacement = false;
	}

	public SystemHex GetSystem(int row, int col) {
		return hexMap [row] [col];
	}
	
//	public void Recenter(Vector3 translation) {
//		Vector3 centerLocation = GetHexLocation(centerCol, centerRow);
//		Debug.Log ("Center: (" + centerLocation.x.ToString() + ", " + centerLocation.y.ToString() + ")");
//		Debug.Log ("Origin: (" + origin.x.ToString() + ", " + origin.y.ToString() + ", " + origin.z.ToString() + ")");
//		Vector3 translation = new Vector3(centerLocation.x-origin.x, centerLocation.y-origin.y, centerLocation.z-origin.z);
//		Debug.Log ("Reposition  vector: (" + translation.x.ToString() + ", " + translation.y.ToString() + ", " + translation.z.ToString() + ")");
//		foreach(SystemHex[] row in hexMap) {
//			foreach(SystemHex hex in row) {
//				hex.gameObject.transform.Translate(translation);
//			}
//		}
//		origin = translation;
//	}

	public Vector3 GetHexLocation(int q, int r) {
		//Convert array coords to axial coords
		Vector2 axial = arrayCoordsToAxial(q, r);
		Vector2 centerAxial = arrayCoordsToAxial(center[0], center[1]);
		//Calculate locations local to the board section
		float boardLocalX = (3f / 2) * (hexSize / 2) * axial.x;
		float boardLocalY = (float)Math.Sqrt (3.0f) * (hexSize / 2) * (axial.y + (axial.x / 2));
		float boardCenterX = (3f / 2) * (hexSize / 2) * centerAxial.x;
		float boardCenterY = (float)Math.Sqrt (3.0f) * (hexSize / 2) * (centerAxial.y + (centerAxial.x / 2));
		//board's local x is global x, but board's local y is global -z

		return new Vector3 (boardLocalX + origin.x - boardCenterX, origin.y, -boardLocalY + origin.z + boardCenterY);
	}

	public void DisplayForDebug() {
		string colIndString = "";
		foreach(int colIndex in firstColumns) {
			if (colIndString != "") {
				colIndString += ", ";
			}
			colIndString += colIndex;
		}
		Debug.Log ("First Columns:" + colIndString);
		foreach(SystemHex[] row in hexMap) {
			string rowString = "";
			foreach(SystemHex sys in row) {
				if (rowString != "") {
					rowString += ", ";
				}
				rowString += sys.System.Name;
			}
			Debug.Log(rowString);
		}
	}

	private Vector2 axialToArrayCoords(int q, int r) {
		Debug.Log (r);
		return new Vector2 (r, q - firstColumns [r]);
	}

	private Vector2 arrayCoordsToAxial(int q, int r) {
//		foreach(int col in firstColumns) {
//			Debug.Log (col);
//		}
		//return new Vector2 (r - maxRowSize, q + firstColumns [r]);
		//Debug.Log (firstColumns [r] + " ::: " + maxRowSize + " ::: ");
		//Debug.Log (r + " ::: " + q + firstColumns [r] + " ::: ");
		return new Vector2 (r, q + firstColumns [r]);
	}

	public IEnumerable<SystemHex> Ring(int radius) {
		foreach(SystemHex hex in HexesInRadius(new Vector2(center[0], center[1]), radius)) {
			yield return hex;
		}
	}

	public IEnumerable<SystemHex> HexesInRadius(Vector2 coords, int radius) {
//		List<SystemHex> hexes = new List<SystemHex>();

		//Start by getting the hex cardinal directions. These form the "corners" of the radius
		Vector2 north = GetCoordsAtDistance(coords, radius, HexDirection.North);
		Vector2 south = GetCoordsAtDistance(coords, radius, HexDirection.South);
		Vector2 northeast = GetCoordsAtDistance(coords, radius, HexDirection.Northeast);
		Vector2 northwest = GetCoordsAtDistance(coords, radius, HexDirection.Northwest);
		Vector2 southeast = GetCoordsAtDistance(coords, radius, HexDirection.Southeast);
		Vector2 southwest = GetCoordsAtDistance(coords, radius, HexDirection.Southwest);

//		Vector2[] corners = new Vector2[6] {north, south, northeast, northwest, southeast, southwest};
//
////		foreach(Vector2 corner in corners) {
////			if ((
////		}

		//Now traverse the "ring", corner to corner, adding each hex as we go
		//	South to Southeast
		Vector2 check = new Vector2(south.x, south.y);
		Vector2 next;
		while(!((check.x == southeast.x) && (check.y == southeast.y))) {
			if (IsInBounds(check)) {
				Vector2 arrayCoords = axialToArrayCoords((int)check.y, (int)check.x);
				//Debug.Log (arrayCoords.x + " , " + arrayCoords.y);
				yield return hexMap[(int)arrayCoords.x][(int)arrayCoords.y];
			}
			next = GetNeighbor(check, HexDirection.Northeast);
			check.x = next.x;
			check.y = next.y;
		}
		//	Southeast to Northeast
		while(!((check.x == northeast.x) && (check.y == northeast.y))) {
			//Debug.Log (check.y + " ,,, " + check.x);
			if (IsInBounds(check)) {
				Vector2 arrayCoords = axialToArrayCoords((int)check.y, (int)check.x);
				//Debug.Log (arrayCoords.x + " , " + arrayCoords.y);
				yield return hexMap[(int)arrayCoords.x][(int)arrayCoords.y];
			}
			next = GetNeighbor(check, HexDirection.North);
			check.x = next.x;
			check.y = next.y;
		}
		//	Northeast to North
		while(!((check.x == north.x) && (check.y == north.y))) {
			if (IsInBounds(check)) {
				Vector2 arrayCoords = axialToArrayCoords((int)check.y, (int)check.x);
				//Debug.Log (arrayCoords.x + " , " + arrayCoords.y);
				yield return hexMap[(int)arrayCoords.x][(int)arrayCoords.y];
			}
			next = GetNeighbor(check, HexDirection.Northwest);
			check.x = next.x;
			check.y = next.y;
		}
		//	North to Southwest
		while(!((check.x == northwest.x) && (check.y == northwest.y))) {
			if (IsInBounds(check)) {
				Vector2 arrayCoords = axialToArrayCoords((int)check.y, (int)check.x);
				//Debug.Log (arrayCoords.x + " , " + arrayCoords.y);
				yield return hexMap[(int)arrayCoords.x][(int)arrayCoords.y];
			}
			next = GetNeighbor(check, HexDirection.Southwest);
			check.x = next.x;
			check.y = next.y;
		}
		//	Southwest to Southeast
		while(!((check.x == southwest.x) && (check.y == southwest.y))) {
			if (IsInBounds(check)) {
				Vector2 arrayCoords = axialToArrayCoords((int)check.y, (int)check.x);
				//Debug.Log (arrayCoords.x + " , " + arrayCoords.y);
				yield return hexMap[(int)arrayCoords.x][(int)arrayCoords.y];
			}
			next = GetNeighbor(check, HexDirection.South);
			check.x = next.x;
			check.y = next.y;
		}
		//	Southwest to South
		while(!((check.x == south.x) && (check.y == south.y))) {
			if (IsInBounds(check)) {
				Vector2 arrayCoords = axialToArrayCoords((int)check.y, (int)check.x);
				//Debug.Log (arrayCoords.x + " , " + arrayCoords.y);
				yield return hexMap[(int)arrayCoords.x][(int)arrayCoords.y];
			}
			next = GetNeighbor(check, HexDirection.Southeast);
			check.x = next.x;
			check.y = next.y;
		}

//		SystemHex[] hexArray = new SystemHex[hexes.Count];
//		hexes.CopyTo(hexArray, 0);
//		return hexArray;
	}

//	private void addHexIfExists(Vector2 axialCoords, List<SystemHex> hexes) {
//		Vector2 arrayCoords = axialToArrayCoords((int)axialCoords.y, (int)axialCoords.x);
//		if (IsInBounds(axialCoords)) {
//			hexes.Add(hexMap[(int)arrayCoords.x][(int)arrayCoords.y]);
//		}
//	}

	public bool IsInBounds(Vector2 axialCoords) {
		Vector2 arrayCoords = axialToArrayCoords((int)axialCoords.y, (int)axialCoords.x);
		return ((0 <= arrayCoords.x) && (arrayCoords.x < hexMap.Length) && (0 <= arrayCoords.y) && (arrayCoords.y < hexMap [(int)arrayCoords.x].Length));
	}

	public static Vector2 GetNeighbor(Vector2 coords, HexDirection dir) {
		return GetCoordsAtDistance(coords, 1, dir);
	}

	public static Vector2 GetCoordsAtDistance(Vector2 coords, int distance, HexDirection dir) {
		switch(dir) {
			case HexDirection.North:
				return getNorth(coords, distance);
			case HexDirection.South:
				return getSouth(coords, distance);
			case HexDirection.Northeast:
				return getNortheast(coords, distance);
			case HexDirection.Northwest:
				return getNorthwest(coords, distance);
			case HexDirection.Southwest:
				return getSouthwest(coords, distance);
			case HexDirection.Southeast:
				return getSoutheast(coords, distance);
			default:
				return new Vector2();
		}
	}

	private static Vector2 getNorth(Vector2 coords, int distance) {
		return new Vector2(coords.x - distance, coords.y);
	}

	private static Vector2 getSouth(Vector2 coords, int distance) {
		return new Vector2(coords.x + distance, coords.y);
	}

	private static Vector2 getNorthwest(Vector2 coords, int distance) {
		return new Vector2(coords.x, coords.y - distance);
	}

	private static Vector2 getSoutheast(Vector2 coords, int distance) {
		return new Vector2(coords.x, coords.y + distance);
	}

	private static Vector2 getNortheast(Vector2 coords, int distance) {
		return new Vector2(coords.x - distance, coords.y + distance);
	}

	private static Vector2 getSouthwest(Vector2 coords, int distance) {
		return new Vector2(coords.x + distance, coords.y - distance);
	}

//	public bool IsNeighbor(SystemHex origin, SystemHex neighborCheck) {
//		SystemHex[] neighbors = GetHexesInRadius(origin.GetPosition(), 1);
//		foreach(SystemHex hex in neighbors){
//			if (hex == neighborCheck) {
//
//			}
//		}
//	}
}