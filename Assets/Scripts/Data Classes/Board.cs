using UnityEngine;
using System;

public enum BoardType {Hexagon, ThreePlayer, WarpZone};

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

	public BoardSection(int ringCount, BoardType boardType, Vector3 sectionOrigin, int[] pCenter, GameObject pHexPrefab, float pHexSize, float rotation) {
		origin = sectionOrigin;
		hexSize = pHexSize;
		center = pCenter;

		// Determine what column each row starts in, and how large the middle/max length row is
		if (boardType == BoardType.ThreePlayer) {
			firstColumns = new int[7] {0, -1, -1, -2, -2, -3, -3};
			maxRowSize = firstColumns.Length-1;
		} else if (boardType == BoardType.Hexagon) {
			//Note: This assumes a maximum ring count of 4.
			firstColumns = new int[1 + (2 * ringCount)];
			for (int i=0; i < ringCount; i++) {
				firstColumns[i] = -i;
			}
			//All remaining rows start all the way to the left
			for (int i=ringCount; i < firstColumns.Length; i++) {
				firstColumns[i] = -ringCount;
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
				hexRow1[j] = setEmptyHexSlot(pHexPrefab, GetHexLocation (j,i), rotation);
				if (i != hexMap.Length / 2) {
					hexRow2[j] = setEmptyHexSlot(pHexPrefab, GetHexLocation(j,hexMap.Length - i - 1), rotation);
				}
			}

			hexMap[i] = hexRow1;
			if (i != hexMap.Length / 2) {
				hexMap[hexMap.Length - i - 1] = hexRow2;
			}
		}
	}

	private SystemHex setEmptyHexSlot(GameObject hexPrefab, Vector3 hexLocation, float rotation) {
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
		sideMaterial.color = new Color(1.0f, 0.0f, 0.0f, .3f);
		hexObject.name = "<Empty System Slot>";
		return sysHex;
	}

	public BoardSection(PlanetSystem[][] inMap, int[] inFirstColumns, Vector3 sectionOrigin, GameObject pHexPrefab, float pHexSize) {
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
		SystemHex hex = hexMap[row][col];
		hex.System = sys;
		fileManager = GameObject.Find("Manager").GetComponent<FileManager>();
		Material topMaterial = hex.gameObject.transform.FindChild ("Top").renderer.material;
		topMaterial.mainTexture = fileManager.ReadSystemTexture(sys.Name, sys.Id, hex.gameObject);
		topMaterial.color = new Color(topMaterial.color.r, topMaterial.color.g, topMaterial.color.b, 1.0f);
		Material sideMaterial = hex.gameObject.renderer.material;
		sideMaterial.mainTexture = fileManager.ReadSystemTexture("Empty System 1", "Empty System 1", hex.gameObject);
		sideMaterial.color = new Color(0.035f, 0.075f, 0.212f, 1.0f);
		hex.gameObject.transform.FindChild("Top").gameObject.SetActive(true);
		hex.gameObject.name = hex.System.Name;
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
		Vector2 centerAxial = arrayCoordsToAxial(center[1], center[0]);
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
		return new Vector2 (q - firstColumns [r], r);
	}

	private Vector2 arrayCoordsToAxial(int q, int r) {
		return new Vector2 (q + firstColumns [r], r - maxRowSize);
	}
}