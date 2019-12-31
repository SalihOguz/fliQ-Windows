using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	
	public GameObject levelButtons;
	public Color[] buttonColors;
	public GameObject mainMenuPanel;
	
	public GameObject[] patternPrefabs;

	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll();
		if (PlayerPrefs.GetInt("isFirst") == 0)
		{
			PlayerPrefs.SetInt("latestLevel", 0);
			PlayerPrefs.SetInt("isFirst", 1);
		}

		for (int i = 0; i < levelButtons.transform.childCount-1; i++)
		{
			if (i < PlayerPrefs.GetInt("latestLevel"))
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[0];
			}
			else if (i == PlayerPrefs.GetInt("latestLevel"))
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[1];
			}
			else
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[2];
				levelButtons.transform.GetChild(i).GetComponent<Button>().enabled = false;
			}
		}
	}

	public void LevelSelect(int levelNo)
	{
		PlayerPrefs.SetInt("clickedLevel", levelNo);
		SceneManager.LoadScene("Game");
	}

	public void MenuAnim()
	{
		LeanTween.moveLocalY(mainMenuPanel, 1800f, 1f).setEase(LeanTweenType.easeInBack);
	}

	public void UnlockAll()
	{
		PlayerPrefs.SetInt("latestLevel",  levelButtons.transform.childCount);
		for (int i = 0; i < levelButtons.transform.childCount-1; i++)
		{
			if (i < PlayerPrefs.GetInt("latestLevel"))
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[0];
				levelButtons.transform.GetChild(i).GetComponent<Button>().enabled = true;
			}
			else if (i == PlayerPrefs.GetInt("latestLevel"))
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[1];
				levelButtons.transform.GetChild(i).GetComponent<Button>().enabled = true;
			}
			else
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[2];
				levelButtons.transform.GetChild(i).GetComponent<Button>().enabled = false;
			}
		}
	}

	public void ResetAll()
	{
		PlayerPrefs.SetInt("latestLevel",  0);
		for (int i = 0; i < levelButtons.transform.childCount-1; i++)
		{
			if (i < PlayerPrefs.GetInt("latestLevel"))
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[0];
			}
			else if (i == PlayerPrefs.GetInt("latestLevel"))
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[1];
			}
			else
			{
				levelButtons.transform.GetChild(i).GetComponent<Image>().color = buttonColors[2];
				levelButtons.transform.GetChild(i).GetComponent<Button>().enabled = false;
			}
		}
	}

	public void Exit()
	{
		Application.Quit();
	}
}