using UnityEngine;
using System;

[System.Serializable]
public class Board {

	[SerializeField]
	private BoardSection[] hexMap;		//Unconnected sections of the board

	public Board() {
	}

	public Board(BoardSection[] inMap) {
		hexMap = inMap;
	}

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


	private float hexSize;
	private int maxRowSize;

	public BoardSection() {
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
				hexObject.AddComponent<SystemHex>();
				hexObject.GetComponent<SystemHex>().System = inMap[i][j];
				hexObject.transform.parent = GameObject.Find ("Board").transform;
				hexObject.name = inMap[i][j].Name;
			}
			hexMap[i] = hexRow;
		}
	}

	public Vector3 GetHexLocation(int q, int r) {
		//Convert array coords to axial coords
		Vector2 axial = arrayCoordsToAxial(q, r);
		//Calculate locations local to the board section
		float boardLocalX = (3f / 2) * (hexSize / 2) * axial.x;
		float boardLocalY = (float)Math.Sqrt (3.0f) * (hexSize / 2) * (axial.y + (axial.x / 2));
		//board's local x is global x, but board's local y is global -z

		return new Vector3 (boardLocalX + origin.x, origin.y, boardLocalY + origin.z);
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