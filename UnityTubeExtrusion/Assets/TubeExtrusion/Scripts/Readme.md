# UnityTubeExtrusion
Polyline extrusion for Unity.

## Features

* Extrusion from OBJ polyline file.
* Extrusion from Vector3 array.
* Coloring and texturing.
* Resolution.
* Decimation.
* Multithreading.
* Realtime.

## Installing

Simply import the package in Unity from [here](https://assetstore.unity.com/packages/slug/170643).

## How to use

### OBJ File
OBJ files do not need to be included in Unity's Asset folder; this allows the user to select any file on any location.

1. Create a GameObject and Attach an *TubeGeneratorFromObj* script to it.
2. Set the .obj's file path in *TubeGeneratorFromObj*'s *filePath* variable.
3. Tube extrusion will commence automatically.

### Vector3 Array

As an example, a simple cylinder will be generated through code from a 2-point polyline.

1. Create a new script in Unity, and make it inherit *TubeGenerator*.

```
public class MyTubeGenerator : TubeGenerator
```

2. Create an array of Vector3 arrays of size 1; make the first element (polyline) of this array a new Vector3 of size 2. Give each point a position.

```
Vector3 [][] polylines = new Vector3[1][];
polylines[0] = new Vector3[2];
polylines[0][0] = new Vector3(0, -1, 0);
polylines[0][1] = new Vector3(0, 1, 0);
```

3. Pass the array to *Generate()* to begin the tube extrusion.

```
Generate(polylines);
```

4. Any properties may be changed from Unity's Editor or through code. For example, to change the tube radius to 0.5:

```
radius = 0.5f;
```

The full code is:

```
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExampleCylinder : TubeGenerator
{
	void Start()
	{
		Vector3 [][] polylines = new Vector3[1][];
		polylines[0] = new Vector3[2];
		polylines[0][0] = new Vector3(0, -1, 0);
		polylines[0][1] = new Vector3(0, 1, 0);
		
		radius = 0.5f;
		
		Generate(polylines);
	}
}
```
## Authors

* **Federico Garcia Garcia**

## Acknowledgments

* [3D Textures](https://3dtextures.me/)
* [Texture Haven](https://texturehaven.com/textures/)
