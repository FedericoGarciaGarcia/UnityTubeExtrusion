///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0 
// Created on: 05/06/2020 13:27
// Last modified: 05/06/2020 13:27
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExampleWeb : TubeGenerator
{
	void Start()
	{
		Vector3 [][] polylines = new Vector3[1][];
		polylines[0] = new Vector3[2];
		polylines[0][0] = new Vector3(0, -1, 0);
		polylines[0][1] = new Vector3(0, 1, 0);
		
		Generate(polylines);
	}
}