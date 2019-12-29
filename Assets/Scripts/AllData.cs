using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
	public List<PieceData> piecesData =  new List<PieceData>();
	public int rowCount;
	public int columnCount;
	public int colorThemeIndex;
}