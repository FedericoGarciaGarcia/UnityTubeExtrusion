﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExampleWave : TubeGenerator
{
	void Start()
	{
		int n=500;
		float w = 5;
		Vector3 [][] polylines = new Vector3[1][];
		polylines[0] = new Vector3[n];
		
		for(int i=0; i<n; i++) {
			float x = (float)i/100.0f;
			polylines[0][i] = new Vector3(x, Mathf.Sin(w*x)/w, 0);
		}
		
		Generate(polylines);
	}
}