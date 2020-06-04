///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0 
// Date: GPL-3.0
// Created on: 04/06/2020 23:00
// Last modified: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ObjReader
{
    // Start is called before the first frame update
    public Vector3 [][] GetPolylines(string filePath)
    {
		// To store polylines
		Vector3 [][] polylines = null; // An array of arrays
		
		// Open
        StreamReader reader = new StreamReader(filePath);
		
		// To store vertices and lines
		List<Vector3> vertices = new List<Vector3>();
		List<List<int>> lines  = new List<List<int>>();
        
		// Read line by line
		string line = reader.ReadLine();
		while(line != null) {
			// Trim line (removes whitespaces at beginning and end)
			line = line.Trim();
			
			// Get words
			string[] words = line.Split(' ');
			
			// If words is empty, ignore
			if(words.Length > 0) {
				// If first word is 'v'
				if(words[0].Equals("v")) {
					// Store each vertex position
					vertices.Add(new Vector3(
						float.Parse(words[1]),
						float.Parse(words[2]),
						float.Parse(words[3])
					));
				}
				// If first word is 'l'
				else if(words[0].Equals("l")) {
					// Store each index position
					List<int> thisLine = new List<int>();
					
					for(int i=1; i<words.Length; i++) {
						thisLine.Add(int.Parse(words[i]));
					}
					
					lines.Add(thisLine);
				}
			}

			// Read next line
			line = reader.ReadLine();
		}
		
		// Close
        reader.Close();
		
		// Create polylines
		polylines = new Vector3 [lines.Count][]; // Number of lines
		
		for(int i=0; i<lines.Count; i++) {
			// Create polyline
			polylines[i] = new Vector3[lines[i].Count];
			
			// Set vertices
			for(int j=0; j<lines[i].Count; j++) {
				// Set vertex
				polylines[i][j] = vertices[lines[i][j]-1]; // We subtract 1, becase OBJ indeces start from 1, instead of 0
			}
		}
		
		// Return data
		return polylines;
    }
}
