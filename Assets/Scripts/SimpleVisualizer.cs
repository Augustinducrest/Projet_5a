/*
	Made by Sunny Valle Studio
	(https://svstudio.itch.io)
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleVisualizer : MonoBehaviour
{
    //public
    [Header("Objects")]
    public LSystemGenerator lsystem;
    public GameObject prefab;
    public Material lineMaterial;
    public RoadHelper roadHelper;
    public GameObject image;
    public GameObject Buildings;

    [Header("Parameters")]

    public int angle = 90;

    public float r =3.0f;
    [Range(0,120)]
    public int tolerance = 50;
    [Range(0,45)]
    public int interv_angle = 50;
     [Range(0.0f,30.0f)]  
    public float length = 8;
    [Range(0.0f,2.0f)]
    public float att_length = 1.0f;
    [Range(0.0f,2.0f)]
    public float width = 1.0f;
    [Range(0.0f,2.0f)]
    public float att_width = 1.0f;


    //private
    List<Vector3> positions = new List<Vector3>();
    List<Segment> segments = new List<Segment>();
    List<Vector3> batiments;
    private int anglevar = 180;
    private int res = 20;
    private int taille_max = 10;
    private float[,] density_map;
    private float[] remap;


    public float Length
    {
        get
        {
            if (length > 0)
            {
                return length;
            }
            else
            {
                return 1;
            }
        }
        set => length = value;
    }

    private void Start()
    {
        var sequence = lsystem.Generatesentence();
        VisualizeSequence(sequence);

        //BUILDINGS
        density_map = new float[res,res];
        //List<Vector3> ensemble = Concatenate(positions,batiments);
        remap = MinMaxVect(positions); //on doit utiliser ensemble
        Create_DM();
        //CreateBuildings();
        Texture2D tex = GetTexturefromArray(density_map,res);
        image.GetComponent<RawImage>().texture = tex;
    }

    private void VisualizeSequence(string sequence)
    {
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        var currentPosition = Vector3.zero;

        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;

        Stack<GameObject> GO = new Stack<GameObject>();
        GameObject currentGO = new GameObject("Lines");
        GameObject spheres = new GameObject("Spheres");

        positions.Add(currentPosition);


        foreach (var letter in sequence)
        {
            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding)
            {
                case EncodingLetters.save:

                    savePoints.Push(new AgentParameters
                    {
                        position = currentPosition,
                        direction = direction,
                        length = Length,
                        angle = 180
                    });
                    GO.Push(currentGO);
                    break;

                case EncodingLetters.load:

                    if (savePoints.Count > 0)
                    {
                        var agentParameter = savePoints.Pop();
                        currentPosition = agentParameter.position;
                        direction = agentParameter.direction;
                        Length = agentParameter.length;
                        anglevar = agentParameter.angle;
                        currentGO = GO.Pop();
                    }
                    else
                    {
                        throw new System.Exception("Dont have saved point in our stack");
                    }
                    break;

                case EncodingLetters.draw:
                    length = Length;
                    tempPosition = currentPosition;
                    currentPosition += direction * length;
                    //r = length/4;

                    Segment seg = new Segment(currentPosition,tempPosition);
                    Vector3 pInterseg =DetectIntersection(seg,tempPosition);

                    GameObject line = new GameObject("line"); 
                    if (pInterseg != new Vector3(0,0,0) && pInterseg !=  tempPosition )
                    {
                        currentPosition = pInterseg;
                    } 
                    Vector3 detectedpoint = DetectClosestPoint(currentPosition);
                    int nbrClosePoint = CountNumberClosePoint(currentPosition);
                    if (nbrClosePoint <4 )
                    {
                        if ( currentPosition == detectedpoint)
                        {
                            DrawLine(line,tempPosition, currentPosition, Color.red);
                            //roadHelper.PlaceStreetPositions(tempPosition,currentPosition);
                            positions.Add(currentPosition);
                        }
                        else
                        {
                            currentPosition = detectedpoint;
                            DrawLine(line,tempPosition, currentPosition, Color.red);
                            //roadHelper.PlaceStreetPositions(tempPosition,currentPosition);
                        }
                    }
                    
                    width*=att_width;
                    Length-=2;
                    line.transform.parent = currentGO.transform;
                    currentGO = line;
                    break;

                case EncodingLetters.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    break;

                case EncodingLetters.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    break;
                    
                case EncodingLetters.turnRightAngle:
                    
                    direction = Quaternion.AngleAxis(UnityEngine.Random.Range(90-interv_angle,135+interv_angle), Vector3.up) * direction;
                    break;

                case EncodingLetters.turnLeftAngle:

                    direction = Quaternion.AngleAxis(-UnityEngine.Random.Range(90-interv_angle,135+interv_angle), Vector3.up) * direction;
                    break;

                case EncodingLetters.turn:
                    int rot = 0;
                    if (anglevar == 180){ //aucune branche présente
                        rot = RandomValueFromRanges(new Range(-180+tolerance,180-tolerance));
                    }
                    else // branche présente
                    {
                        rot = GiveAngle(anglevar,tolerance);
                    }
                    var agent = savePoints.Pop();
                    agent.angle = rot;
                    savePoints.Push(agent);
                    direction = Quaternion.AngleAxis(rot, Vector3.up) * direction;
                    break;

                    default:
                        break;
                }
            }
            foreach (var position in positions)
            {
                Instantiate(prefab, position, Quaternion.identity).transform.parent = spheres.transform;
            }
        }

    //Intersections et contacts
    Vector3 DetectIntersection(Segment seg1,Vector3 p1)
    {
        Vector3 A = seg1.point1;
        Vector3 B = seg1.point2;
        Vector3 pointInter = new Vector3(0,0,0) ;
        float minpointInterDist = 1000f;
        foreach(var seg2 in segments)
        {
            Vector3 C = seg2.point1;
            Vector3 D = seg2.point2;
            Vector3 p =  CalculeIntersection(A,B,C,D);
            if( p != new Vector3(0,0,0))
            {
                float d = Vector3.Distance(p,p1);
                if(d < minpointInterDist && d > 0.001f)
                {
                    minpointInterDist = d;
                    pointInter = p;
                }
            }   
        }
        return pointInter;
    }

    Vector3 CalculeIntersection(Vector3 A,Vector3 B, Vector3 C,Vector3 D)
    {
        float xA = A.x; float yA = A.z;
        float xB = B.x; float yB = B.z;
        float xC = C.x; float yC = C.z;
        float xD = D.x; float yD = D.z;

        float det = ((xB-xA)*(yC-yD)-(xC-xD)*(yB-yA));

        if (det == 0)
        {
            return new Vector3(0,0,0);
        }
        else 
        {
            float t1 = ((xC-xA)*(yC-yD)-(xC-xD)*(yC-yA))/det;
            float t2 = ((xB-xA)*(yC-yA)-(xC-xA)*(yB-yA))/det;
            if(t1 >1 || t1<0 || t2>1 || t2<0 )
            {
                return new Vector3(0,0,0);
            }
            else
            {
                if(t1== 0)        {return A;}
                else if (t1 == 1) {return B;}
                else if (t2 == 0) {return C;}
                else if (t2 == 1) {return D;}
                else
                {
                    return new Vector3 (xA+t1*(xB-xA) ,0 , yA+t1*(yB-yA));
                }
            }
        }
    }

    Vector3 DetectClosestPoint(Vector3 pos)
        {
            Vector3 closestPoint =new Vector3(0,0,0);
            float closestdist = 100f;

            foreach( var point in positions)
            {
                float distToPoint = Vector3.Distance(pos , point);
                if ( distToPoint < closestdist)
                {
                    closestPoint = point;
                    closestdist =distToPoint;
                }
            }
            if (closestdist < r)
            {
                return closestPoint;
            }
            else
            {
                return pos;
            }
        }
    private int CountNumberClosePoint(Vector3 pos)
    {
        int n = 0;
        foreach( var point in positions)
        {
            float dist = Vector3.Distance(pos , point);
            if( dist < r )
            {
                n += 1;
            }
        }
        return n; 
    }
    private void DrawLine(GameObject line, Vector3 start, Vector3 end, Color color)
        {
            line.transform.position = start;
            var lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.SetPosition(0,end);
            lineRenderer.SetPosition(1,start);
            Segment seg = new Segment(start,end);
            segments.Add(seg);
        }

    //Encoding
    public enum EncodingLetters
    {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnRight = '+',
        turnLeft = '-',
        turn = 'R',
        turnRightAngle = '<',
        turnLeftAngle = '>'
    }

    //Range Angles
    public struct Range
    {
        public int min;
        public int max;
        public int range {get {return max-min + 1;}}
        public Range(int aMin, int aMax)
        {
            min = aMin; max = aMax;
        }
    }
    public static int RandomValueFromRanges(params Range[] ranges)
    {
        if (ranges.Length == 0)
            return 0;
        int count = 0;
        foreach(Range r in ranges)
            count += r.range;
        int sel = UnityEngine.Random.Range(0,count);
        foreach(Range r in ranges)
        {
            if (sel < r.range)
            {
                return r.min + sel;
            }
            sel -= r.range;
        }
        throw new Exception("This should never happen");
    }

    private int GiveAngle(int anglem, int t){

        if (anglem< -180 + 2*t){
            return RandomValueFromRanges(new Range[]{new Range(anglem+t,180-t)});
        }
        else if (anglem > 180 - 2*t){
            return RandomValueFromRanges(new Range[]{new Range(-180+t,anglem-t)});
        }
        else
        {
            return RandomValueFromRanges(new Range[]{new Range(anglem+t,180-t),new Range(anglem+t,180-t)});
        }
        
    }


//TEST NEW INTERSECTION

    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }
        

//BUILDINGS

    private void CreateBuilding(float taille,Vector3 pos){
        GameObject Building =new GameObject("Building");
        Building.transform.parent = Buildings.transform;
        for (int i =0;i< (int)taille;i++){
            Instantiate(prefab, pos + Vector3.up*0.5f*i, Quaternion.identity).transform.parent = Building.transform;
        }
        
    }

    private void CreateBuildings(){
        int posX=0;
        int posZ=0;
        foreach (var batiment in batiments)
        {
            posX = (int)Remap(batiment.x,remap[0],remap[1],0.0f,res-1);
            posZ = (int)Remap(batiment.z,remap[2],remap[3],0.0f,res-1);
            int taille;
            if(posX>0 || posX<49 || posZ>0 || posZ<49 ){
                taille = taille_max*(int)(density_map[posX,posZ]);
            }else{
                taille = 1;
            }

            CreateBuilding(taille,batiment);
        }
        
    }

   
    private void Create_DM(){
        int posX=0;
        int posZ=0;
        float max=0;
        foreach (var position in positions)
        {
            posX = (int)Remap(position.x,remap[0],remap[1],0.0f,res-1.0f);
            posZ = (int)Remap(position.z,remap[2],remap[3],0.0f,res-1.0f);
            density_map[posX,posZ] +=1;
            if(posX<res-1){
                density_map[posX+1,posZ] +=0.8f;
            }
            if(posX>0){
                density_map[posX-1,posZ] +=0.8f;
            }
            if(posZ<res-1){
                density_map[posX,posZ+1] +=0.8f;
            }
            if(posZ>0){
                density_map[posX,posZ-1] +=0.8f;
            }
            if(density_map[posX,posZ]>max){
                max = density_map[posX,posZ];
            }

        }
        //normalisation
        for(int i=0;i<res;++i){
            for(int j=0;j<res;++j){
                density_map[j,i] = density_map[j,i]/max;
            }
        }
    }

    //retourne min/max respectivement en x et z
    private static float[] MinMaxVect(List<Vector3> list){
        float min_x = list[0].x;
        float max_x = list[0].x;
        float min_z = list[0].z;
        float max_z = list[0].z;
        float current_x = 0;
        float current_z = 0;
        for(int i=1;i<list.Count;i++)
        {
            current_x = list[i].x;
            current_z = list[i].z;
            if(min_x>current_x){
                min_x = current_x;
            }
            if(max_x<current_x){
                max_x = current_x;
            }
            if(min_z>current_z){
                min_z = current_z;
            }
            if(max_z<current_z){
                max_z = current_z;
            }
        }
        float[] values = new float[4] {min_x,max_x,min_z,max_z};
        return values;
    }

    public static Texture2D GetTexturefromArray(float[,] array,int res){
        Texture2D tex = new Texture2D(res,res);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        Color[] colors = new Color[res*res];
        float value = 0;
        for(int i=0;i<res;++i){
            for(int j=0;j<res;++j){
                value = array[j,i];
                colors[i*res +j] = new Color(value,value,value,1.0f);   
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }


 //remap entre 2 ranges de nombres
    private static float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
    {
        return targetFrom + (source-sourceFrom)*(targetTo-targetFrom)/(sourceTo-sourceFrom);
    }

    public List<Vector3> Concatenate(List<Vector3> firstList, List<Vector3> secondlist)
    {
        var result = new List<Vector3>(firstList.Count + secondlist.Count);
        result.AddRange(firstList);
        result.AddRange(secondlist);
        return result;
    }

}





