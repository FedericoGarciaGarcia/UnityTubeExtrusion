///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0 
// Created on: 05/06/2020 23:23
// Last modified: 05/06/2020 13:23
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// As it is web, no threading is allowed
public class TubeGeneratorWeb : MonoBehaviour
{  
	public int skipPolylines = 0; // How many polylines to be skipped.
	public int dequeSize = 10000; // How many generated tubes to be sent to the GPU every frame   
	public float decimation = 0;  // Decimation level, between 0 and 1. If set to 0, each polyline will have only two vertices (the endpoints)
	public float scale = 1;       // To resize the vertex data
    public float radius = 1;      // Thickness of the tube
	public int resolution = 3;    // Number of sides for each tube
	public Material material;     // Texture (can be null)
	public Color colorStart = Color.white; // Start color
	public Color colorEnd   = Color.white; // End color
	
	private Vector3 [][] polylines;   // To store polylines data
	private GameObject [] actors;     // Gameobjects that will have tubes attached
	private Tube [] tubes;            // Tubes
	private bool [] attached;         // If actors have already have their tube attached
	
	// Used polylines when created
	private Vector3 [][] allpolylines;
	
	// To dispatch coroutines
	public readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();
	
    protected void Generate(Vector3 [][] allpolylines)
    {
		// Get this lines
		this.allpolylines = allpolylines;
		
		// Skip polylines, if any
		skipPolylines++;
		polylines = new Vector3[allpolylines.Length/skipPolylines][];
		
		for(int i=0; i<polylines.Length; i++) {
			polylines[i] = allpolylines[i*skipPolylines];
		}
		
		// Generate tubes
		UpdateTubes();
	}
	
	// Only update tubes
	public void UpdateTubes() {
		// Create array of bools
		attached = new bool[polylines.Length];
		
		// Create game objects
		actors = new GameObject[polylines.Length];
		
		// Attach an actor to the game object
		for(int i=0; i<actors.Length; i++) {
			actors[i] = new GameObject();
			actors[i].name = "Tube "+(i+1);
			actors[i].AddComponent<Actor>();
			actors[i].transform.parent = transform;
		}
		
		// To store tubes
		tubes = new Tube[polylines.Length];
		
		// Decimate points
		int npoints = 0;
		int ndecimatedpoints = 0;
		
		for(int i=0; i<polylines.Length; i++) {
			// Total points
			npoints += polylines[i].Length;
			
			// Decimated point
			int npointsLine = (int)((1.0f - decimation) * (float)polylines[i].Length);
			
			if(npointsLine < 2)
				npointsLine = 2;
			ndecimatedpoints += npointsLine;
		}
		
		CreateTubes(polylines);
	}
	
	// Create tube in a thread
	public void CreateTubes(Vector3 [][] polylines) {
		
		for(int i=0; i<polylines.Length; i++) {
			CreateTube(polylines[i], i);
			AttachTubeToGameobject(i);
		}
    }
	
	// Coroutine to attach tube to actor
	IEnumerator AttachTubeToGameobject(int i) {
		yield return null;
		
		// Give tube data to gameobject's actor
		actors[i].GetComponent<Actor>().SetTube(tubes[i]);
		
		// Give it color
		float lerp = (float)i/(float)polylines.Length;
		actors[i].GetComponent<Actor>().SetMaterial(material);
		actors[i].GetComponent<Actor>().SetColor(Color.Lerp(colorStart, colorEnd, lerp));
		
		if(!attached[i])
		actors[i].transform.localPosition += transform.position;
	
		attached[i] = true;
	}
	
	// Create a tube
	void CreateTube(Vector3 [] polyline, int i) {
		// Create empty tube
		tubes[i] = new Tube();
		
		// Generate data
		tubes[i].Create(polyline, decimation, scale, radius, resolution);
	}
}