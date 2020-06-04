///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0 
// Date: GPL-3.0
// Created on: 04/06/2020 23:00
// Last modified: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
	public Vector3 speed;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
    }
}
