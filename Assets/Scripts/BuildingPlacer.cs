using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab;
    public void PlaceBuildingtown(List<Segment> segments)
    {
        foreach(var segment in segments)
        {
            PlaceBuildingOnStreet(segment);
        }
    }
    public void PlaceBuildingOnStreet(Segment segment)
    {
        Vector3 p1 = segment.point1;
        Vector3 p2 = segment.point2;
        float d = Vector3.Distance(p1,p2);
        int dInt = (int)d/3;

        for (int i = 0; i< dInt-1 ;i++)
        {
            Vector3 p = Vector3.MoveTowards(p1,p2,1.5f+d);
            Vector3 vdir = p2 - p1;
            Vector3 vOrth = Vector3.Cross(vdir,Vector3.up).normalized;
            PlaceBuilding(p+vOrth,vdir);
            PlaceBuilding(p-vOrth,vdir);
        }
    }    
    public void PlaceBuilding(Vector3 p,Vector3 vdir)
    {
        var building = Instantiate(buildingPrefab,p, Quaternion.identity);
        Quaternion rot  = Quaternion.FromToRotation(new Vector3(0,0,1) ,vdir);
		rot.x =0;
		building.transform.rotation = rot;
    }
    
}
