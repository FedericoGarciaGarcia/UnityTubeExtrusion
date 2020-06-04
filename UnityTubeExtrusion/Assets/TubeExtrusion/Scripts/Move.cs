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
