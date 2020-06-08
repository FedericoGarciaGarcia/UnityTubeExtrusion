///////////////////////////////////////////////////////////////////////////////
// Author: Federico Garcia Garcia
// License: GPL-3.0
// Created on: 04/06/2020 23:00
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text;

public class TubeGeneratorFromObj : TubeGenerator
{
	public string path; // Path or URL to file
	public GameObject loading; // A GameObject that is disabled after the data is generated
	
	void Start()
	{
		// If its URL, download data in a coroutine, then in
		if(path.StartsWith("https:") || path.StartsWith("http:")) {
			StartCoroutine(LoadFromURL(path));
		}
		// If its file, load data in a thread
		else {
			Thread thread = new Thread(()=>LoadFromFile());
			thread.Start();
		}
	}
	
	void LoadFromFile() {
		// Create OBJ reader
		ObjReader objReader = new ObjReader();
		
		Vector3 [][] polylines = objReader.GetPolylinesFromFilePath(path);
	
		// Make sure to lock to avoid multithreading problems
		lock(_enque) {
			// Run the generation of polylines in the Main Thread
			ExecuteOnMainThread.Enqueue(() => {  StartCoroutine(AfterLoading()); } );
			ExecuteOnMainThread.Enqueue(() => {  StartCoroutine(Generate(polylines)); } );
		};
	}
	
	IEnumerator LoadFromURL(string url) {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
			// Or retrieve results as binary data
			//byte[] byteArray = www.downloadHandler.data;
		
			// Get bytes from file
			byte[] byteArray = Encoding.UTF8.GetBytes(www.downloadHandler.text);
			
			// Create memory stream
			MemoryStream stream = new MemoryStream(byteArray);

			// Convert MemoryStream to StreamReader
			StreamReader reader = new StreamReader(stream);
			
			// Run this in a thread
			Thread thread = new Thread(()=>LoadFromStream(reader));
			thread.Start();
            
        }
    }
	
	void LoadFromStream(StreamReader reader) {
		
		// Create OBJ reader
		ObjReader objReader = new ObjReader();
	
		Vector3 [][] polylines = objReader.GetPolylinesFromStreamReader(reader);
	
		// Make sure to lock to avoid multithreading problems
		lock(_enque) {
			// Run the generation of polylines in the Main Thread
			ExecuteOnMainThread.Enqueue(() => {  StartCoroutine(AfterLoading()); } );
			ExecuteOnMainThread.Enqueue(() => {  StartCoroutine(Generate(polylines)); } );
		};
	}
	
	IEnumerator AfterLoading() {
		if(loading != null)
		loading.SetActive(false);
		
		yield return null;
	}
}