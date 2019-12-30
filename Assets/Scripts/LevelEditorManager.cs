using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LevelEditorManager : MonoBehaviour {

	public GameObject gridObject;
	public GameObject grid;
	ColorTheme[] themes;
	public int levelNo;

	public int rowCount;
	public int columnCount;
	public int colorThemeIndex;
	bool isLoading = false;

	void Start()
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
			for (int j = 0; j < 7; j++)
			{
				themes[i].colors[j] = themeObject.transform.GetChild(j).GetComponent<Image>().color;
			}
		}
	}

	public void CreateGrid()
	{
		StartCoroutine(ClearGrid());
		grid.GetComponent<GridLayoutGroup>().constraintCount = rowCount;
		for (int i = 0; i < rowCount*columnCount; i++)
		{
			GameObject gm = Instantiate(gridObject, Vector3.zero, Quaternion.identity, grid.transform) as GameObject;
			gm.GetComponent<PieceDataComponent>().colorIndex = 5;
			for (int j = 0; j < 5; j++)
			{
				gm.transform.GetChild(3).GetChild(j).GetComponent<Image>().color = themes[colorThemeIndex].colors[j];
			}
		}
	}

	IEnumerator ClearGrid()
	{
		int n =grid.transform.childCount;
		for (int i = 0; i < n; i++)
		{
			DestroyImmediate(grid.transform.GetChild(0).gameObject);
		}
		yield return new WaitForSeconds(1f);
	}

	public void ClearButton()
	{
		StartCoroutine(ClearGrid());
	}

	public void ColorButton(int no)
	{
		if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.parent.GetComponent<PieceDataComponent>())
		{
			UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.parent.GetComponent<PieceDataComponent>().colorIndex = no;
			UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.parent.GetComponent<Image>().color = themes[colorThemeIndex].colors[no];
		}
		else
		{
			UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<PieceDataComponent>().colorIndex = no;
			UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Image>().color = themes[colorThemeIndex].colors[no];
		}
		
	}

	public void ActiveChanged()
	{
		if (!isLoading)
		{
			UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<PieceDataComponent>().isActive = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<PieceDataComponent>().isActive)
			{
				//UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Image>().color = Color.white;
				ColorButton(5);
			}
			else
			{
				//UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Image>().color = Color.black;
				ColorButton(6);
			}
		}
		
	}

	public void PieceChange()
	{
		if (!isLoading)
		UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<PieceDataComponent>().isPiece = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
	}

	public void RotationChange()
	{
		if (!isLoading)
		UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<PieceDataComponent>().isRotated = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
	}

	public void SaveLevel()
	{
		LevelData levelData = new LevelData();
		foreach (Transform i in grid.transform)
		{
			PieceData piecesData = new PieceData();
			piecesData.isActive = i.GetComponent<PieceDataComponent>().isActive;
			piecesData.isPiece = i.GetComponent<PieceDataComponent>().isPiece;
			piecesData.isRotated = i.GetComponent<PieceDataComponent>().isRotated;
			piecesData.colorIndex = i.GetComponent<PieceDataComponent>().colorIndex;
			piecesData.pieceKind = i.GetComponent<PieceDataComponent>().pieceKind;

			levelData.piecesData.Add(piecesData);
		}
		levelData.rowCount = rowCount;
		levelData.columnCount = columnCount;
		levelData.colorThemeIndex = colorThemeIndex;

		File.WriteAllText("Assets/Resources/Levels/level" + levelNo + ".json", JsonUtility.ToJson(levelData));
	}

	public void SaveLevelAnswer()
	{
		LevelData levelData = new LevelData();
		foreach (Transform i in grid.transform)
		{
			PieceData piecesData = new PieceData();
			piecesData.isActive = i.GetComponent<PieceDataComponent>().isActive;
			piecesData.isPiece = i.GetComponent<PieceDataComponent>().isPiece;
			piecesData.isRotated = i.GetComponent<PieceDataComponent>().isRotated;
			piecesData.colorIndex = i.GetComponent<PieceDataComponent>().colorIndex;
			piecesData.pieceKind = i.GetComponent<PieceDataComponent>().pieceKind;

			levelData.piecesData.Add(piecesData);
		}
		levelData.rowCount = rowCount;
		levelData.columnCount = columnCount;
		levelData.colorThemeIndex = colorThemeIndex;

		File.WriteAllText("Assets/Resources/Levels/level" + levelNo + "_answer.json", JsonUtility.ToJson(levelData));
	}

	public void LoadLevel()
	{
		isLoading = true;
		if (File.Exists("Assets/Resources/Levels/level" + levelNo + ".json")) 
		{
			LevelData levelData = JsonUtility.FromJson<LevelData>(File.ReadAllText("Assets/Resources/Levels/level" + levelNo + ".json"));

			List<PieceData> piecesData = levelData.piecesData;
			rowCount = levelData.rowCount;
			columnCount = levelData.columnCount;
			colorThemeIndex = levelData.colorThemeIndex;

			// Create Grid
			StartCoroutine(ClearGrid());
			grid.GetComponent<GridLayoutGroup>().constraintCount = rowCount;
			for (int i = 0; i < rowCount*columnCount; i++)
			{
				GameObject gm = Instantiate(gridObject, Vector3.zero, Quaternion.identity, grid.transform) as GameObject;
				
				gm.GetComponent<PieceDataComponent>().isActive = piecesData[i].isActive;
				gm.GetComponent<PieceDataComponent>().isPiece = piecesData[i].isPiece;
				gm.GetComponent<PieceDataComponent>().isRotated = piecesData[i].isRotated;
				gm.GetComponent<PieceDataComponent>().colorIndex = piecesData[i].colorIndex;

				gm.GetComponent<Image>().color = themes[colorThemeIndex].colors[piecesData[i].colorIndex];

				gm.transform.GetChild(0).GetComponent<Toggle>().interactable = false;
				gm.transform.GetChild(1).GetComponent<Toggle>().interactable = false;
				gm.transform.GetChild(2).GetComponent<Toggle>().interactable = false;

				gm.transform.GetChild(0).GetComponent<Toggle>().isOn = piecesData[i].isActive;
				gm.transform.GetChild(1).GetComponent<Toggle>().isOn = piecesData[i].isPiece;
				gm.transform.GetChild(2).GetComponent<Toggle>().isOn = piecesData[i].isRotated;

				gm.transform.GetChild(0).GetComponent<Toggle>().interactable = true;
				gm.transform.GetChild(1).GetComponent<Toggle>().interactable = true;
				gm.transform.GetChild(2).GetComponent<Toggle>().interactable = true;
				for (int j = 0; j < 5; j++)
				{
					gm.transform.GetChild(3).GetChild(j).GetComponent<Image>().color = themes[colorThemeIndex].colors[j];
				}
			}
		}
		isLoading = false;
	}

	public void LoadLevelAnswer()
	{
		isLoading = true;
		if (File.Exists("Assets/Resources/Levels/level" + levelNo + "_answer.json")) 
		{
			LevelData levelData = JsonUtility.FromJson<LevelData>(File.ReadAllText("Assets/Resources/Levels/level" + levelNo + "_answer.json"));

			List<PieceData> piecesData = levelData.piecesData;
			rowCount = levelData.rowCount;
			columnCount = levelData.columnCount;
			colorThemeIndex = levelData.colorThemeIndex;

			// Create Grid
			StartCoroutine(ClearGrid());
			grid.GetComponent<GridLayoutGroup>().constraintCount = rowCount;
			for (int i = 0; i < rowCount*columnCount; i++)
			{
				GameObject gm = Instantiate(gridObject, Vector3.zero, Quaternion.identity, grid.transform) as GameObject;
				
				gm.GetComponent<PieceDataComponent>().isActive = piecesData[i].isActive;
				gm.GetComponent<PieceDataComponent>().isPiece = piecesData[i].isPiece;
				gm.GetComponent<PieceDataComponent>().isRotated = piecesData[i].isRotated;
				gm.GetComponent<PieceDataComponent>().colorIndex = piecesData[i].colorIndex;

				gm.GetComponent<Image>().color = themes[colorThemeIndex].colors[piecesData[i].colorIndex];

				gm.transform.GetChild(0).GetComponent<Toggle>().interactable = false;
				gm.transform.GetChild(1).GetComponent<Toggle>().interactable = false;
				gm.transform.GetChild(2).GetComponent<Toggle>().interactable = false;

				gm.transform.GetChild(0).GetComponent<Toggle>().isOn = piecesData[i].isActive;
				gm.transform.GetChild(1).GetComponent<Toggle>().isOn = piecesData[i].isPiece;
				gm.transform.GetChild(2).GetComponent<Toggle>().isOn = piecesData[i].isRotated;

				gm.transform.GetChild(0).GetComponent<Toggle>().interactable = true;
				gm.transform.GetChild(1).GetComponent<Toggle>().interactable = true;
				gm.transform.GetChild(2).GetComponent<Toggle>().interactable = true;
				for (int j = 0; j < 6; j++)
				{
					gm.transform.GetChild(3).GetChild(j).GetComponent<Image>().color = themes[colorThemeIndex].colors[j];
				}
			}
		}
		isLoading = false;
	}
}

[System.Serializable]
public class ColorTheme
{
	public Color[] colors;
}

/*[System.Serializable]
public class AllData
{
	public List<PieceData> piecesData =  new List<PieceData>();
	public int rowCount;
	public int columnCount;
	public int colorThemeIndex;
}

[System.Serializable]
public class PieceData {

	public bool isActive;
	public bool isPiece;
	public bool isRotated;
	public int colorIndex;
}*/