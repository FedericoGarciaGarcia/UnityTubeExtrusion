using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExampleRing : TubeGenerator
{
	void Start()
	{
		int n=60;
		float w = 10;
		float angle = Mathf.PI/(float)n;
		Vector3 [][] polylines = new Vector3[1][];
		polylines[0] = new Vector3[n];
		
		for(int i=0; i<n; i++) {
			float x = (float)i*angle;
			polylines[0][i] = new Vector3(0, Mathf.Sin(w*x), Mathf.Cos(w*x));
		}
		
		Generate(polylines);
	}
}