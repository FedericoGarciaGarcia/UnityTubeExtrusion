///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0
// Created on: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private Mesh mesh;
	//private Tube tube;

    // Start is called before the first frame update
    void Start()
    {
    }
	
	// Set material to mesh
	public void SetMaterial(Material material)　{
		// Set material
		meshRenderer.material = material;
	}
	
	// Set color to mesh
	public void SetColor(Color color)　{
		meshRenderer.material.color = color;
	}
	
	// Set mesh data from tube
	public void SetTube(Tube tube) {
        // Create mesh and attach
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        meshFilter = gameObject.AddComponent<MeshFilter>();
		
		// Get tube and data
		//this.tube = tube;
		
        mesh = new Mesh();
		
		// Set new data
        mesh.vertices = tube.vertices;
        mesh.triangles = tube.tris;
        mesh.uv = tube.uv;
		
		// Recalculate normals
		mesh.RecalculateNormals();
		
		// Modify
		Vector3[] normals = mesh.normals;

		for(int i=tube.resolution-1; i<normals.Length-tube.resolution*2; i+=tube.resolution) { //-tube.resolution*2 to be careful with tube endcaps
            normals[i] = normals[i-tube.resolution+1];
        }

        // assign the array of normals to the mesh
        mesh.normals = normals;
		
        meshFilter.mesh = mesh;
	}

    // Update is called once per frame
    void Update()
    {
    }
}
