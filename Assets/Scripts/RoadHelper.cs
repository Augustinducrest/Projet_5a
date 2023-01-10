/*
	Made by Sunny Valle Studio
	(https://svstudio.itch.io)
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


	public class RoadHelper : MonoBehaviour
	{

		public GameObject roadStraight;

		public void PlaceStreetPositions(Vector3 startPosition, Vector3 endPosition)
		{
			float  d = Vector3.Distance(startPosition,endPosition);
			Vector3 middlePosition = Vector3.MoveTowards(startPosition,endPosition,d/2);

			//float angle =Vector3.Angle(new Vector3(0,0,1),startPosition - endPosition);
			//Quaternion direction = Quaternion.AngleAxis(angle,new Vector3(0,1,0));
			float r = UnityEngine.Random.Range(-0.01f,0.01f);
			var road = Instantiate(roadStraight, middlePosition + new Vector3(0,r,0) , Quaternion.identity);
			road.transform.localScale = new Vector3(0.1f,1.0f,d/10);
			road.transform.rotation = Quaternion.FromToRotation(new Vector3(0,0,1) ,  endPosition - startPosition);
			road.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1,d); 
			//print("dist: " +d);
		}
	}
