///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0 
// Date: GPL-3.0
// Created on: 04/06/2020 23:00
// Last modified: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	public float speed = 1.0f;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward*Input.GetAxis("Vertical")*speed*Time.deltaTime);
		transform.Translate(Vector3.right*Input.GetAxis("Horizontal")*speed*Time.deltaTime);
	}
}
