using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    public Vector3 point1;
    public Vector3 point2;
    public bool isMainRoad;

    public Segment(Vector3 p1,Vector3 p2)
    {
        point1 = p1;
        point2 = p2;
        isMainRoad = false;
    }

    public Segment(Vector3 p1,Vector3 p2, bool b )
    {
        point1 = p1;
        point2 = p2;
        isMainRoad =b; 
    }
}
