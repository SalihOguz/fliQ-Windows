using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;

public class GenerateLevel : MonoBehaviour {

	public GameObject parentObject;
	public GameObject parentAnswersObject;
	
	int rowCount;
	int columnCount;
	int colorThemeIndex;
	public GameObject headPrefab;
	public GameObject piecePrefab;
	public float startX, startY, paddingX, paddingY;
	ColorTheme[] themes;

	public void GenerateStart(int levelNo)
	{
		themes = new ColorTheme[10]; // 10 may be changed if not enough
		for (int i = 0; i < themes.Length; i++)
		{
			GameObject themeObject = Resources.Load<GameObject>("Prefabs/ColorThemes/Theme"+ i);
			if (!themeObject)
			{
				break;
			} 
			themes[i] = new ColorTheme();
			themes[i].colors = new Color[7];
			for (int j = 0; j < 6; j++)
			{
				themes[i].colors[j] = themeObject.transform.GetChild(j).GetComponent<Image>().color;
			}
		}
		LoadLevel(levelNo);
		GenerateAnswers(levelNo);
		parentAnswersObject.transform.position = new Vector3(30,30,0);
	}

	public void GenerateAnswers(int levelNo)
	{
		LoadLevel(levelNo, "_answer");
	}

	public void LoadLevel(int levelNo, string answer = "")
	{
		LevelData levelData = JsonUtility.FromJson<LevelData>(Resources.Load<TextAsset>("Levels/level" + levelNo + answer).text);

		List<PieceData> piecesData = levelData.piecesData;
		rowCount = levelData.rowCount;
		columnCount = levelData.columnCount;
		colorThemeIndex = levelData.colorThemeIndex;

		startX = 0;//startX-(paddingX/(columnCount/2)); //startX - ((columnCount-1)/2)*paddingX; 
		startY = 0;//startY-(paddingY/(rowCount/2));

		for (int j = 0; j < columnCount; j++)
		{
			GameObject currentHead = null;
			//List<GameObject> childFliqs = new List<GameObject>(); // child of currentHead
			for (int i = 0; i < rowCount; i++)
			{
				int indexNo = i+(rowCount*j);
				if (piecesData[indexNo].isActive)
				{
					Quaternion rot = new Quaternion(0,0,0,1);
					if (piecesData[indexNo].isRotated)
					{
						rot = new Quaternion(0,0,1,0);
					}

					GameObject gm = null;
					if (piecesData[indexNo].isPiece)
					{
						if (answer == "")
						{
							gm = Instantiate(piecePrefab, new Vector3(startX + paddingX*(j), startY - paddingY*(i)), rot, parentObject.transform) as GameObject;

						}
						else
						{
							gm = Instantiate(piecePrefab, new Vector3(startX + paddingX*(j), startY - paddingY*(i)), rot, parentAnswersObject.transform) as GameObject;
						}
						while(!gm)
						{
							print("wait");
						}
						gm.GetComponent<FlipPiece>().isRotated = piecesData[indexNo].isRotated;

						if (piecesData[indexNo].pieceKind == 0)
						{
							gm.GetComponent<SpriteRenderer>().color = themes[colorThemeIndex].colors[piecesData[indexNo].colorIndex];
							gm.GetComponent<FlipPiece>().color = themes[colorThemeIndex].colors[piecesData[indexNo].colorIndex];
						}
						else
						{
							// TODO what to do if piece kind is different
						}
						// if(currentHead)
						// {
						// 	if(currentHead.transform.rotation == gm.transform.rotation)
						// 	{
						// 		childFliqs.Add(gm);
						// 	}
						// 	else
						// 	{
						// 		currentHead.GetComponent<HeadItem>().childFliqs = childFliqs;
						// 		Vector2 size = currentHead.transform.GetChild(2).GetComponent<SpriteRenderer>().size;
						// 		size = new Vector2(size.x, size.y + childFliqs.Count * 1.85f); // 1.85 fits perfect. If fliq size changes, number must change
						// 		childFliqs = new List<GameObject>();
						// 		childFliqs.Add(gm);
						// 	}
						// }
						// else
						// {
						// 	print("ERROR: fliq without parent head !!!");
						// }
					}
					else
					{
						if (answer == "")
						{
							gm = Instantiate(headPrefab, new Vector3(startX + paddingX*(j), startY - paddingY*(i)), rot, parentObject.transform) as GameObject;
						}
						else
						{
							gm = Instantiate(headPrefab, new Vector3(startX + paddingX*(j), startY - paddingY*(i)), rot, parentAnswersObject.transform) as GameObject;
						}
						// while(!gm)
						// {
						// 	print("wait");
						// }
						// print(gm);
						// gm.GetComponent<FlipPiece>().isRotated = piecesData[indexNo].isRotated;

						if (piecesData[indexNo].pieceKind == 0)
						{
							gm.transform.GetChild(0).GetComponent<SpriteRenderer>().color = themes[colorThemeIndex].colors[piecesData[indexNo].colorIndex];
							gm.transform.GetChild(1).GetComponent<SpriteRenderer>().color = themes[colorThemeIndex].colors[piecesData[indexNo].colorIndex];
							//gm.GetComponent<FlipPiece>().color = themes[colorThemeIndex].colors[piecesData[indexNo].colorIndex];
						}
						else
						{
							// TODO what to do if piece kind is different
						}
						if (currentHead == null)
						{
							Vector2 size = gm.transform.GetChild(2).GetComponent<SpriteRenderer>().size;
							gm.transform.GetChild(2).GetComponent<SpriteRenderer>().size = new Vector2(size.x, size.y + GetActiveRowCount(j, piecesData) * 1.75f); // 1.85 fits perfect. If fliq size changes, number must change
						}
						currentHead = gm;
					}
				}
			}
			//currentHead.GetComponent<HeadItem>().childFliqs = childFliqs;
		}
		parentObject.transform.position = new Vector3(0f-(((float)columnCount/2f)*paddingX)+(paddingX/2f), 0f+(((float)rowCount/2f)*paddingY)-(paddingY/2f), 0);
		GameManager.instance.Init();
	}

	int GetActiveRowCount(int columnIndex, List<PieceData> piecesData)
	{
		int activeRowCount = 0;
		for(int i = 0; i < rowCount; i++)
		{		
			PieceData piece = piecesData[i+(rowCount*columnIndex)];
			if (piece.isActive && piece.isPiece)
			{
				activeRowCount ++;
			}
		}
		return activeRowCount;
	}
}