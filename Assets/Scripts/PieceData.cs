using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PieceData {
	public bool isActive;
	public bool isPiece;
	public bool isRotated;
	public int colorIndex;
	public int pieceKind = 0; // 0 = standart, 1 = stationary, 2 = joker, 3 = locked
}