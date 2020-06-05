///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0 
// Created on: 05/06/2020 9:00
// Last modified: 05/06/2020 9:00
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	public TubeGenerator tubeGenerator;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
	
    // Update is called once per frame
    void Update()
    {
		
		// Change resolution
        if (Input.GetKeyDown("i"))
        {
			if(Input.GetKey("left shift")) {
				tubeGenerator.resolution--;
				
				if(tubeGenerator.resolution < 2)
					tubeGenerator.resolution = 2;
			}
			else {
				tubeGenerator.resolution++;
			}
			
			tubeGenerator.UpdateTubes();
        }
		
		// Change radius
        if (Input.GetKey("r"))
        {
			if(Input.GetKey("left shift")) {
				tubeGenerator.radius -= 0.1f * Time.deltaTime;
			}
			else {
				tubeGenerator.radius += 0.1f * Time.deltaTime;
			}
			
			tubeGenerator.UpdateTubes();
        }
    }
}
