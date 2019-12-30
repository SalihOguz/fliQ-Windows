using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class GameManager : MonoBehaviour {

	List<GameObject> gamePieces;
	List<GameObject> answerPieces;
	int totalPieceCount;
	public int moveCount;
	public Text moveCountText;
	int currentLevel;
	public GameObject trueBar;
	public GameObject winPanel;
	public GameObject losePanel;
	public GameObject settingsPanel;
	public Text levelText;
	public GenerateLevel generator;
	public BGColors bgcolors;

	public Image bgPattern1;
	public Image bgPattern2;
	public Image bgPattern3;

	private static GameManager _instance;
    public static GameManager instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager> ();
            return _instance;
        }
    }

	void Awake()
	{
		 _instance = this;
		currentLevel = PlayerPrefs.GetInt("clickedLevel");
	}

	void Start () 
	{
		levelText.text = "LEVEL "+currentLevel; //"STAGE "+((currentLevel/5)+1) +" | LEVEL " + ((currentLevel%5)+1);
		generator.GenerateStart(currentLevel);
		
		if ((currentLevel - 1) / 5 == 0)
		{
			bgPattern1.gameObject.SetActive(true);
			trueBar.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = bgPattern1.color;
		}
		else if ((currentLevel - 1) / 5 == 1)
		{
			bgPattern2.gameObject.SetActive(true);
			trueBar.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = bgPattern2.color;
		}
		else if ((currentLevel - 1) / 5 == 2)
		{
			bgPattern3.gameObject.SetActive(true);
			trueBar.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = bgPattern3.color;
		}
	}

	public void Init() {
		gamePieces = PiecesInParent(generator.parentObject);
		answerPieces = PiecesInParent(generator.parentAnswersObject);

		totalPieceCount = gamePieces.Count;


		moveCount = JsonUtility.FromJson<MoveCounts>(Resources.Load<TextAsset>("MoveCountsData").text).moveCounts[currentLevel];
		moveCountText.text = moveCount.ToString();
		Camera.main.GetComponent<Camera>().backgroundColor = bgcolors.BGColor[currentLevel - 1];
		GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Rules/Rule" + currentLevel), Vector3.zero, Quaternion.identity, GameObject.Find("UI_header").transform);
		go.transform.localPosition = Vector3.zero;

		// to start with partialy filled bar
		trueBar.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = bgcolors.BGColor[currentLevel - 1];
		VictoryCheck();
		moveCount++;
		moveCountText.text = moveCount.ToString();
	}

	public void VictoryCheck()
	{
		int trueCount = 0;

		foreach (GameObject gamePiece in gamePieces)
		{
			foreach (GameObject answerPiece in answerPieces)
			{
				if (Vector3.Distance(gamePiece.transform.localPosition, answerPiece.transform.localPosition) <= 1.8f   // if their local positions are same
					&& Math.Abs(gamePiece.transform.localEulerAngles.z - answerPiece.transform.localEulerAngles.z) < 10f // if their local rotations are same
						&& gamePiece.GetComponent<SpriteRenderer>().color == answerPiece.GetComponent<SpriteRenderer>().color) // if their colors are same
				{
					trueCount++;
					break;
				}
			}
		}


		trueBar.GetComponent<Scrollbar>().size = (float)trueCount / totalPieceCount; // TODO animation, slowly increasing bar
		if (trueCount == totalPieceCount)
		{
			Win();
		}
		else
		{
			moveCount--;
			moveCountText.text = moveCount.ToString();
			if (moveCount == 0)
			{
				Lose();
			}
		}
	}

	void Lose()
	{
		print("You Lose!");
		losePanel.SetActive(true);
	}

	List<GameObject> PiecesInParent(GameObject parent)
	{
		List<GameObject> pieces = new List<GameObject>();
		foreach (Transform i in parent.transform)
		{
			if (i.gameObject.GetComponent<FlipPiece>())
			{
				pieces.Add(i.gameObject);
			}
		}
		return pieces;
	}

	void Win()
	{
		winPanel.SetActive(true);
		print("You Win!");
		if(currentLevel-1 >= PlayerPrefs.GetInt("latestLevel"))
		{
			PlayerPrefs.SetInt("latestLevel", currentLevel);
		}
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void NextLevel()
	{
		PlayerPrefs.SetInt("clickedLevel", currentLevel+1);
		LoadScene("Game");
	}
}