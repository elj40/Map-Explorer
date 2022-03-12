using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry {
	public string type;
	public float[][][][] coordinates;

	//Coordinates: [](nothing) [](nothing) [](array of pairs) [](digits)
}

public class Feature
{
	public string type;
	public IDictionary<string,string> properties;
	public Geometry geometry;
}

public class PolygonData
{
	public string type;
	public string name;
	public List<Feature> features;
	public int totalCoordinates = 0;


	//Counts the total coordinates of the shape for use in length of vertices array in Polygon_drawer
	public void countCoordinates() {
		foreach (Feature f in features) {
			totalCoordinates += f.geometry.coordinates[0][0].Length;
			Debug.Log(f.properties["name"]);
		}
	}
}



