using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class vertex : MonoBehaviour {
	public List<vertex> connection = new List<vertex>();
	private List<vertex> oldConnection = new List<vertex>();
	public List<path> paths_connection = new List<path>();
	public vertex pass;
	public bool visited;
	public int[] canOpenDoors;

	public Dictionary<vertex,int> door_connection = new Dictionary<vertex, int>();
	//for door_connection input
	public vertex[] input;
	public int[] inputd;

    // for door
    public room parent;
	public void setup(vertex[] A, int[] a)
	{
		for (int i = 0; i < A.Length; i++)
		{
			door_connection.Add(A[i], a[i]);
			oldConnection.Add(A[i]);
		}
	}

	public void Reset()
	{
		connection = new List<vertex>();
		foreach (vertex v in oldConnection)
		{
			connection.Add(v);
		}
		pass = null;
	}

	public List<vertex> getOldConnection()
	{
		return oldConnection;
	}
	
}
