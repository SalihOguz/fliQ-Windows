using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPiece : MonoBehaviour {

	public GameObject parent;
	public Vector3 startScale;
	[HideInInspector]
	public Color color;
	[HideInInspector]
	public bool isRotated = false;

	// Use this for initialization
	void Start () {
		parent = gameObject.transform.parent.gameObject;
		startScale = gameObject.transform.localScale;
		
	}
}
