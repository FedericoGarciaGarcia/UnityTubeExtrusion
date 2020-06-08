///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0
// Created on: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

// @TODO: implement "smart decimation": not all points are equal; spikes should be preserved

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube
{
	public static int iii = 0;
	
	public Vector3[] polyline; // Polyline vertices
	public Vector3[] vertices; // Mesh vertices
	public int[] tris;         // Triangles that make the tube mesh
	public Vector2[] uv;       // UV texture coordinates
	public Vector3 position;   // The position is the vertex in the middle
	
	private float decimation; // Decimation level, between 0 and 1
	private float scale;      // Rescaling of the vertices
    private float [] radii;   // Array of radius for each point
	public int resolution ;   // Number of points per revolution
		
	private Vector3[] circle; // Circle to make the tubes
	private int tpr;          // Number of tube trianges per revolution
	
	private float length; // Length of the polyline (sum of distances between adyacent pair of points)
	
    public void Create(Vector3 [] polyline, float decimation, float scale, float [] radii, int resolution)
    {
		// More vertices for texture seaming
		resolution ++;
		
		// Set properties
		this.decimation = decimation;
		this.scale      = scale;
		this.radii      = radii;
		this.resolution = resolution;
		
		// Create a radius=1 circle
		circle = new Vector3[resolution];
		float angleSize = 360.0f / (float)(resolution-1);
		float rad = angleSize * (Mathf.PI / 180.0f);
		
		for(int i=0; i<resolution; i++) {
            circle[i] = new Vector3(0, Mathf.Cos(i*rad), Mathf.Sin(i*rad));
        }
		
		circle[resolution-1] = circle[0];
		
		// Estimate number of polylines and vertices
		int npoints = (int)((1.0f-decimation) * (float)polyline.Length);
		
		// If there are not even 2 points, the decimation was too much
		if(npoints < 2) {
			decimation = 1.0f;
			npoints = polyline.Length;
		}
		
		int nvertices = (npoints+4) * resolution+4;
        vertices = new Vector3[nvertices];

		// Number of tris
		tpr = resolution * 2 * 3;
        tris = new int[(polyline.Length-1) * tpr + 4 * tpr ];
		
		// Generate uv data
		uv = new Vector2[nvertices];
		
		// This polyline
		this.polyline = new Vector3[npoints];
		
		// Convert to one single polyline rescaling and decimating
		float skip = (float)polyline.Length/(float)npoints;
		
		int [] skipIndices = new int[npoints];
		
		float currentSkip = 0;
		for(int i=0; i<npoints; i++) {
			skipIndices[i] = (int)currentSkip;
			currentSkip += skip;
        }
		
		skipIndices[skipIndices.Length-1] = polyline.Length-1; // Make sure last vertex is the real last vertex
		
		// Rescale with skip indices
		for(int i=0; i<npoints; i++) {
			this.polyline[i] = polyline[skipIndices[i]] * scale;
        }
			
		// Create vertices
		for(int i=0; i<vertices.Length; i++) {
            vertices[i] = new Vector3(0, 0, 0);
        }
		
		// Get polyline length
		for(int i=0; i<npoints-1; i++) {
			length += Vector3.Distance(this.polyline[i], this.polyline[i+1]);
		}
		
		// Set the position of the tube
		position = polyline[polyline.Length/2];
		
		// Generate tube
		GenerateTube();
		
		// Generate caps
		GenerateCaps();
		
		// Generate UV for tube
		GenerateUVForTube();
    }
	
	public void SetCaps(Tube cap1, Tube cap2) {
		// Copy vertex and tri data from caps
		
	}
	
	void GenerateTube() {
			
		RevolPoints(0, polyline[0], polyline[0], polyline[1]);
			
		for(int i=1; i<polyline.Length-1; i++)
			RevolPoints(i, polyline[i-1], polyline[i], polyline[i+1]);
		
		RevolPoints(polyline.Length-1, polyline[polyline.Length-2], polyline[polyline.Length-1], polyline[polyline.Length-1]);
		
		// Generate tris
		for(int i=0; i<polyline.Length-1; i++)
			GenerateTris(i);
	}
	
	void GenerateCaps() {
		// Set the first point position to the first point of the first cap
		int index = polyline.Length;
		
		for(int i=0; i<resolution; i++) {
			vertices[index*resolution+i] = polyline[0];
		}
		
		// Copy the vertices of the first circle to the second point of the first cap
		for(int i=0; i<resolution; i++) {
			vertices[(index+1)*resolution+i] = vertices[i];
		}
		
		// Generate tris
		GenerateTris(index);
		
		// Copy the vertices of the first circle to the first point of the second cap
		index = polyline.Length+2;
		
		for(int i=0; i<resolution; i++) {
			vertices[index*resolution+i] = vertices[(polyline.Length-1)*resolution+i];
		}
		
		// Set the first point position to the second point of the second cap
		for(int i=0; i<resolution; i++) {
			vertices[(index+1)*resolution+i] = polyline[polyline.Length-1];
		}
		
		// Generate tris
		GenerateTris(index);
	}
	
	// p is the first point
	// q is the middle point
	// r is the end point
	//
	// p----q----r
	//
	void RevolPoints(int index, Vector3 P, Vector3 Q, Vector3 R) {
		// If start/end point
		if(Vector3.Distance(P, Q) < 0.0001) {
			Vector3 angle = R-P;
			GenerateRevolPoints(index, 0, Q, angle);
		}
		// If middle points
		else {
			Vector3 PQ = Q-P;
			Vector3 QR = R-Q;
		
			Vector3 angle = (PQ+QR)/2.0f; // The angle is the average
			GenerateRevolPoints(index, 1, Q, angle);
		}
	}
	
	public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis)
	{
		Vector3 right = Vector3.Cross(axis, forward).normalized;
		forward = Vector3.Cross(right, axis).normalized;
		return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * (180.0f/3.14f);
	}
	
	void GenerateRevolPoints(int index, int deltaIndex, Vector3 Q, Vector3 angle) {
		// Rotate circle points
		Quaternion rotation    = Quaternion.FromToRotation(Vector3.right, angle);
		Quaternion rotationUp  = Quaternion.FromToRotation(Vector3.up, angle);
		
		// The circles that form the tube are going to be not properly rotated.
		// We need to make sure that the first vertex of each circle is the closest
		// to the first vertex of the previous circle.
		// We start from the second vertex
		if(index == 0) {
			// Generate points
			for(int i=0; i<resolution; i++) {
				Vector3 rotCirclePoint = rotation * (circle[i] * radii[index]);
				
				vertices[index*resolution+i] = new Vector3(Q.x+rotCirclePoint.x, Q.y+rotCirclePoint.y, Q.z+rotCirclePoint.z);
			}
		}
		else {

			float angleSize = 360.0f / (float)(resolution-1);
			float rad = angleSize * (Mathf.PI / 180.0f);
		
			// Previous circle local first vertex
			Vector3 v2 = vertices[(index-deltaIndex)*resolution] - polyline[index-deltaIndex];
			
			// This circle's first vertex and its perpendicular
			Vector3 v0 = rotation   * circle[0];
			Vector3 v1 = rotationUp * circle[0];
			
			// Get difference in angle
			float extraRot = AngleOffAroundAxis(v2, v0, v1);
			
			// Quaternion rotation
			Quaternion rotation2 = Quaternion.Euler(Vector3.right*extraRot);
			// Generate points
			for(int i=0; i<resolution; i++) {
				
				Vector3 rotCirclePoint = rotation * rotation2 * (circle[i] * radii[index]); // We multiply by the radius here instead of when the circle is generated
																					        // to avoid deformated tubes when the radius is too small
				
				vertices[index*resolution+i] = new Vector3(Q.x+rotCirclePoint.x, Q.y+rotCirclePoint.y, Q.z+rotCirclePoint.z);
			}
		}
	}
	
	void GenerateTris(int index) {
		// Join
		for(int i=0; i<resolution; i++) {
			// lower left triangle
			// 0, 2, 1,
			tris[index*tpr + i*6 + 0] = index*resolution + i + 0;
			tris[index*tpr + i*6 + 1] = index*resolution + i + 1;
			tris[index*tpr + i*6 + 2] = index*resolution + i + resolution+1;
			
		
			// upper right triangle
			// 2, 3, 1
			tris[index*tpr + i*6 + 3] = index*resolution + i + 0;
			tris[index*tpr + i*6 + 4] = index*resolution + i + resolution+1;
			tris[index*tpr + i*6 + 5] = index*resolution + i + resolution;
			
			
			if(i == resolution-1) {
				// lower left triangle
				// 0, 2, 1,
				tris[index*tpr + i*6 + 0] = index*resolution + resolution-1;
				tris[index*tpr + i*6 + 1] = index*resolution + 0;
				tris[index*tpr + i*6 + 2] = index*resolution + resolution;
				
				// upper right triangle
				// 2, 3, 1
				tris[index*tpr + i*6 + 3] = index*resolution + resolution-1;
				tris[index*tpr + i*6 + 4] = index*resolution + resolution;
				tris[index*tpr + i*6 + 5] = index*resolution + resolution*2-1;
			}
		}
	}
	
	void GenerateUVForTube() {
		float width = 1.0f/(float)(resolution-1);
		float currentLength = 0;
		
		for(int j=0; j<polyline.Length-1; j++) {
			float distance = Vector3.Distance(polyline[j], polyline[j+1]);
			float lengthStart = currentLength;
			float lengthEnd   = (currentLength+distance);

			for(int i=0; i<resolution-1; i++) {
				uv[j*resolution+i]              = new Vector2(lengthStart, width*i);     //bottom-left
				uv[j*resolution+i+1]            = new Vector2(lengthStart, width*(i+1)); //bottom-right
				uv[j*resolution+i+resolution]   = new Vector2(lengthEnd,   width*i);     //top-left
				uv[j*resolution+i+resolution+1] = new Vector2(lengthEnd,   width*(i+1)); //top-right
			}
			
			currentLength += distance;
		}
	}
}
