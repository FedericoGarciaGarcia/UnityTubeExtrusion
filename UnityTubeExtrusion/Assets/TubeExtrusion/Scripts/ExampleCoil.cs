///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0
// Created on: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExampleCoil : TubeGenerator
{
	void Start()
	{
		int n=500;
		float w = 10;
		Vector3 [][] polylines = new Vector3[1][];
		polylines[0] = new Vector3[n];
		
		for(int i=0; i<n; i++) {
			float x = (float)i/100.0f;
			polylines[0][i] = new Vector3(x, Mathf.Sin(w*x), Mathf.Cos(w*x));
		}
		
		StartCoroutine(Generate(polylines));
	}
}