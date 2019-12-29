using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Flip : MonoBehaviour {

	List<GameObject> chosens = new List<GameObject>();
	public GameObject flipEffectPrefab;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.touchCount < 2) {
			if (Input.GetKeyDown (KeyCode.Mouse0)) // 
			{
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				mousePos = new Vector3 (mousePos.x, mousePos.y, 0);
				transform.position = mousePos;
				GetComponent<BoxCollider2D> ().enabled = true;
			}
			if (Input.GetKey (KeyCode.Mouse0)) 
			{
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				mousePos = new Vector3 (mousePos.x, mousePos.y, transform.position.z);
				transform.position = mousePos;
			}
			if(Input.GetKeyUp(KeyCode.Mouse0) && chosens.Count > 0)
			{
				GameObject flipParent = new GameObject("flipParent"); // creating parent for all chosen flip pieces

				if (chosens.Count % 2 == 0) // finding mid point of even nubmer of flip piece chosen
				{
					flipParent.transform.position = new Vector3((chosens[(chosens.Count/2)-1].transform.position.x +  chosens[(chosens.Count/2)].transform.position.x)/2,
					 	(chosens[(chosens.Count/2)-1].transform.position.y +  chosens[(chosens.Count/2)].transform.position.y)/2.0f, 0);
				}
				else // finding mid point of odd nubmer of flip piece chosen
				{
					flipParent.transform.position = chosens[(chosens.Count)/2].transform.position;
				}

				foreach(GameObject obj in chosens) // take all chosen flip pieces into the same parent
				{
					obj.GetComponent<BoxCollider2D>().enabled = false;
					obj.transform.SetParent(flipParent.transform);
				}

				if(chosens.Count > 1 && Math.Abs(chosens[0].transform.position.y - chosens[1].transform.position.y) < 0.3f) // horizontal flip TODO
				{
					LeanTween.rotate(flipParent, new Vector3(0,180,0), 0.5f).setOnComplete(delegate() { FlipComplete(flipParent);});
				}
				else // vertical flip
					LeanTween.rotate(flipParent, new Vector3(180,0,0), 0.5f).setOnComplete(delegate() { FlipComplete(flipParent);});

				

				GetComponent<BoxCollider2D> ().enabled = false;
				chosens = new List<GameObject>();
			}
		}
	}

	void FlipComplete(GameObject parent) // puts flip pieces back in to their previous parent, scales it and destroys the current parent when rotation is completed
	{
		Transform[] flipChildren = parent.transform.GetComponentsInChildren<Transform>();
		foreach(Transform tr in flipChildren)
		{
			if (tr.gameObject.name != "flipParent")
			{
				tr.GetComponent<BoxCollider2D>().enabled = true;
				LeanTween.scale(tr.gameObject, tr.gameObject.GetComponent<FlipPiece>().startScale, 0.2f);
				tr.SetParent(tr.gameObject.GetComponent<FlipPiece>().parent.transform);
			}
			//olderParents.Dequeue();
		}
		Destroy(parent);
		Camera.main.GetComponent<GameManager>().VictoryCheck();
	}

	void OnTriggerEnter2D(Collider2D col) // what happends when a piece is touched
	{
		if (chosens.Count > 0 && Vector3.Distance(chosens[chosens.Count-1].transform.position, col.transform.position) <= 2 || chosens.Count == 0) // if pieces are adjecent
		{
			if (!chosens.Contains(col.gameObject)) // if piece haven't touched before
			{
				if(chosens.Count < 2 ||
				chosens[0].transform.position.x == chosens[1].transform.position.x && col.transform.position.x == chosens[0].transform.position.x
				|| chosens[0].transform.position.y == chosens[1].transform.position.y && col.transform.position.y == chosens[0].transform.position.y)
				// one line at a time, no L shape allowed
				{
					chosens.Add(col.gameObject);
					LeanTween.scale(col.gameObject, col.gameObject.transform.localScale*1.1f, 0.2f);

					// wave effect when holding the flip piece
					GameObject holdEffect = Instantiate(flipEffectPrefab) as GameObject;
					holdEffect.transform.position = col.transform.position;
					holdEffect.transform.eulerAngles = col.transform.eulerAngles;
					Color colColor = col.gameObject.GetComponent<SpriteRenderer>().color;
					holdEffect.GetComponent<SpriteRenderer>().color = new Color(colColor.r, colColor.g, colColor.b, 0.8f);
					LeanTween.scale(holdEffect, holdEffect.transform.localScale*1.35f, 0.2f);
					LeanTween.color(holdEffect, new Color(colColor.r, colColor.g, colColor.b, 0), 0.3f).setOnComplete(delegate(){Destroy(holdEffect);});
					Vibration.Vibrate(25);
				}
			}
			else if (chosens.Count > 1 && chosens[chosens.Count-2] == col.gameObject) // if piece is touched before and this is a reverse touch
			{
				LeanTween.scale(chosens[chosens.Count-1], chosens[chosens.Count-1].GetComponent<FlipPiece>().startScale, 0.2f);
				chosens.RemoveAt(chosens.Count-1);
				Vibration.Vibrate(10);
			}
			else if (chosens.Count == 1 && chosens[0] == col.gameObject) // if piece is the last piece, user doesn't want to play
			{
				LeanTween.scale(chosens[0], chosens[0].GetComponent<FlipPiece>().startScale, 0.2f);
				chosens.RemoveAt(0);
			}
		}
	}
}