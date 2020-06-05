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

public class TubeGeneratorWebFromObj : TubeGeneratorWeb
{
	public string url;
	
	void Start()
	{
		// Create OBJ reader
		ObjReader objReader = new ObjReader();
		
		// Get data from OBJ reader
		Vector3[][] polylines = objReader.GetPolylinesFromURL(url);
		
		// Create tubes
		Generate(polylines);
	}
}