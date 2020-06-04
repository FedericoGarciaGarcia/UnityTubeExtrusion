///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0 
// Created on: 04/06/2020 23:00
// Last modified: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TubeGenerator : MonoBehaviour
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
		
	public int numberOfThreads = 1; // Number of threads used to generate tube.
	
	private Vector3 [][] polylines;   // To store polylines data
	private GameObject [] actors;     // Gameobjects that will have tubes attached
	private Tube [] tubes;            // Tubes
	
	private int ncpus; // How many CPU cores are available
	
	// For safe multithreading
	private int nextLine;
	private readonly object _lock  = new object();
	private readonly object _enque = new object();
	
	// To dispatch coroutines
	public readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();
	
    protected void Generate(Vector3 [][] allpolylines)
    {
		nextLine = 0;
		
		// Number of CPUS to use for tubing
		ncpus = SystemInfo.processorCount;
		
		// If user wants less threads, set it to that
		ncpus = numberOfThreads < ncpus ? numberOfThreads : ncpus;

		// Avoid negative number of cpus
		if(ncpus < 0)
			ncpus = 1;
		
		// Skip polylines, if any
		skipPolylines++;
		polylines = new Vector3[allpolylines.Length/skipPolylines][];
		
		for(int i=0; i<polylines.Length; i++) {
			polylines[i] = allpolylines[i*skipPolylines];
		}
		
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
		
		// Create tubes using specified number of threads
		Thread [] threads = new Thread[ncpus];
		
		// Lines per thread
		int lpt = polylines.Length/ncpus;
		
		for(int i=0; i<ncpus; i++) {
			threads[i] = new Thread(()=>ThreadCreateTubes(polylines));
		}
		
		// Start threads
		for(int i=0; i<ncpus; i++) {
			threads[i].Start();
		}
	}
	
	// Waiting to dispatch coroutines if there are any.
	// In this case, coroutines are meant to attach tube data to gameobjects
	// (send graphical data to GPU)
	public virtual void Update()
	{
		// dispatch stuff on main thread
		int deque = 0;
		while (deque < dequeSize && ExecuteOnMainThread.Count > 0)
		{
			deque++;
			lock(_enque) {
				ExecuteOnMainThread.Dequeue().Invoke();
			}
		}
	}
	
	// Create tube in a thread
	public void ThreadCreateTubes(Vector3 [][] polylines) {
		
        while(nextLine < polylines.Length) {
		    // If we directly use i instead of x "AttachTubeToGameobject(i)" we have a concurrency problem;
		    // The thread modifies i so when the coroutine starts it may have strange i values
		    // By copying it to a local int that will be different each iteration, we avoid this problem
		   
			int x;
			lock(_lock) {
				x = nextLine;
				nextLine++; // For the next thread
			}
			
			if(x < polylines.Length) {
				// Run coroutine on the main thread that adds the tube data to the gameobject	   
				CreateTube(polylines[x], x);
				
				// Make sure to lock to avoid multithreading problems
				lock(_enque) {
					ExecuteOnMainThread.Enqueue(() => {  StartCoroutine(AttachTubeToGameobject(x)); } );
				}
			}
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
		actors[i].transform.localPosition += transform.position;
	}
	
	// Create a tube
	void CreateTube(Vector3 [] polyline, int i) {
		// Create empty tube
		tubes[i] = new Tube();
		
		// Generate data
		tubes[i].Create(polyline, decimation, scale, radius, resolution);
	}
}