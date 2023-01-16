using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab;
    public SimpleVisualizer SV;

    
    public void PlaceBuildingtown(List<Segment> segments,float[,] dens,float[] rm, int r)
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
        int dInt = (int)d;
        Vector3 vdir = p1 - p2;
        Vector3 vOrth = Vector3.Cross(vdir,Vector3.up).normalized;
        if (segment.isMainRoad)
        {
            PlaceBuilding(p1 +vdir.normalized/2 +vOrth,vdir);
            PlaceBuilding(p1 +vdir.normalized/2 -vOrth,vdir);
            PlaceBuilding(p2 -vdir.normalized/2 +vOrth,vdir);
            PlaceBuilding(p2 -vdir.normalized/2 -vOrth,vdir);
            vOrth *= 1.5f;
            

        }
        PlaceBuilding(p1 +vdir.normalized/2,vdir);
        PlaceBuilding(p2 -vdir.normalized/2,vdir);
        for (int i = -1; i< dInt+2;i++)
        {
            Vector3 p = Vector3.MoveTowards(p1,p2,i);
            PlaceBuilding(p+vOrth,vdir);
            PlaceBuilding(p-vOrth,vdir);
        }
       


    }    
    public void PlaceBuilding(Vector3 p,Vector3 vdir)
    {
        LayerMask buildingMask = LayerMask.GetMask("building");
        LayerMask raodMask = LayerMask.GetMask("road");
        LayerMask mainRaodMask = LayerMask.GetMask("mainRoad");
    
        Quaternion rot  = Quaternion.FromToRotation(new Vector3(0,0,1) ,vdir);

        Collider[] builingCollider = Physics.OverlapBox(p, buildingPrefab.transform.localScale/2 -new Vector3(0.01f,0.01f,0.01f) ,rot , buildingMask);
        Collider[] roadCollider = Physics.OverlapBox(p,buildingPrefab.transform.localScale/2 - new Vector3 (0.1f,0.1f,0.1f), rot , raodMask);
        Collider[] mainRoadCollider = Physics.OverlapBox(p,buildingPrefab.transform.localScale/2 - new Vector3 (0.1f,0.1f,0.1f), rot , mainRaodMask);
        

        if(roadCollider.Length == 0 && mainRoadCollider.Length == 0)
        {
            SV.CreateBuilding( p, rot);
            //var building = Instantiate(buildingPrefab,p, rot);
            //building.transform.parent = BuildingParent.transform ;
        }

        /*else
        {
            foreach (var coll in roadCollider)
            {
                Vector3 vOrth = Vector3.Cross(vdir,Vector3.up).normalized;
                
                Vector3 p1 = p - vdir/2 + vOrth/2;
                Vector3 p2 = p - vdir/2 - vOrth/2;
                Vector3 p3 = coll.ClosestPoint(p);
                Vector3 dirCollider = Vector3.Cross(p1-p2,Vector3.up).normalized;
                Vector3 p4 = SV.CalculeIntersection(p1,p1+vdir, p2,p2+dirCollider);
                
                GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere1.transform.position = p1;
                sphere1.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere2.transform.position = p2;
                sphere2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere3.transform.position = p3;
                sphere3.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                GameObject sphere4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere4.transform.position = p4;
                sphere4.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                //var building = Instantiate(buildingPrefab,p, rot);
            } 
        }*/
        
    }
}
