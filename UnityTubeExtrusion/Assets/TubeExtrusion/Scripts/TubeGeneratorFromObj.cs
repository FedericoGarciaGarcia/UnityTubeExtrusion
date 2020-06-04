using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TubeGeneratorFromObj : TubeGenerator
{
	public string filePath;
	
	void Start()
	{
		// Create OBJ reader
		ObjReader objReader = new ObjReader();
		
		// Get data from OBJ reader
		Vector3[][] polylines = objReader.GetPolylines(filePath);
		
		// Create tubes
		Generate(polylines);
	}
}