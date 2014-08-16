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
	
	private PlanetSystem[][] hexMap; // q, r: Axial coordinate system (2 axes: x, -z)
	[SerializeField]
	private int[] firstColumns;

	public BoardSection() {
	}

	public BoardSection(PlanetSystem[][] inMap, int[] inFirstColumns) {
		hexMap = inMap;
		firstColumns = inFirstColumns;
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
		foreach(PlanetSystem[] row in hexMap) {
			string rowString = "";
			foreach(PlanetSystem sys in row) {
				if (rowString != "") {
					rowString += ", ";
				}
				rowString += sys.Name;
			}
			Debug.Log(rowString);
		}
	}
}