# UnityTubeExtrusion
Tube extrusion for polylines in Unity.

![Cover](https://raw.githubusercontent.com/FedericoGarciaGarcia/UnityTubeExtrusion/master/Images/Tubes.png)

## Features

* Extrusion from OBJ polyline file.
* Extrusion from Vector3 array.
* Coloring and texturing.
* Resolution.
* Decimation.
* Multithreading.
* Realtime.
* End capping.

## Installing

Simply import the package in Unity from [here](https://assetstore.unity.com/packages/slug/170643) (approval pending). Alternatively, download this repository and manually import files in Unity.

## How to use

### Data

Data must be *Vector3 \[\]\[\]*; an array of polylines, where each point is a Vector3.

### OBJ File
OBJ files do not need to be included in Unity's Asset folder; this allows the user to select any file on any location.

1. Create a GameObject and Attach an *TubeGeneratorFromObj* script to it.
2. Set the .obj's file path in *TubeGeneratorFromObj*'s *filePath* variable.
3. Tube extrusion will commence automatically.

A demo scene, "*Example Obj Reader*", is included. The *.obj* file is located in the *Resources* folder.

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
A demo scene, "*Example Arrays*", is included.

### *TubeGenerator* properties

Before tube extrusion, the following properties may be specified:

* *Skip polylines:* how many polylines should be skipped. 0 by default (all polylines are read).
* *Deque size:* how many tubes are to be sent to the GPU each frame after extrusion. If value is too high, there will be a performance hit. 50 by default.
* *Decimation:* beween 0 and 1. Percentage of points to be used in each polyline. 0 means all points are used, 0.5 means only half the points are used, etc. Points are evenly skipped. Useful for dense polylines. 0 by default.
* *Scale:* rescaling of the polylines. 1 by default (no rescaling).
* *Radius:* the tube 'thickness'. 1 by default.
* *Resolution:* number of sides in each tube. 3 by default.
* *Material:* texture and shader material. None by default.
* *Color Start:* the color of the first tube. White by default.
* *Color End:* the color of the last tube; other tubes will be interpolated between *Color Start* and *Color End*. White by default.
* *Threads:* how many threads to use for tube extrusion. It is not recommended to use all available CPU threads, as Unity uses one for the Main thread. 1 by default.

## Showcase

### Extrusion from OBJ polyline file
![Obj](https://raw.githubusercontent.com/FedericoGarciaGarcia/UnityTubeExtrusion/master/Images/Corpus%20callosum.png)

### Extrusion from Vector3 array
![Array](https://raw.githubusercontent.com/FedericoGarciaGarcia/UnityTubeExtrusion/master/Images/Coil.png)

### Coloring and texturing
![Texturing](https://raw.githubusercontent.com/FedericoGarciaGarcia/UnityTubeExtrusion/master/Images/Texturing.png)

### Resolution
![Resolution](https://raw.githubusercontent.com/FedericoGarciaGarcia/UnityTubeExtrusion/master/Images/Resolution.png)

### Decimation
![Decimation](https://raw.githubusercontent.com/FedericoGarciaGarcia/UnityTubeExtrusion/master/Images/Decimation.png)

### Realtime
![Realtime](https://raw.githubusercontent.com/FedericoGarciaGarcia/UnityTubeExtrusion/master/Images/frames.png)

## Authors

* **Federico Garcia Garcia**

## Acknowledgments

* [3D Textures](https://3dtextures.me/)
* [Texture Haven](https://texturehaven.com/textures/)