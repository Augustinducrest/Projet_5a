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
    public GameObject spheres;

    [Header("Parameters")]

    public int angle = 90;

    public float r =3.0f;
    [Range(0,120)]
    public int tolerance = 50;
    [Range(0,45)]
    public int interv_angle = 50;
     [Range(0.0f,30.0f)]  
    public float length = 8;
    [Range(0.0f,1.0f)]
    public float proba_att_length = 1.0f;
    [Range(0.0f,2.0f)]
    public float width = 1.0f;


    //private
    //List<Vector3> positions = new List<Vector3>();
    //Vector3[] positions;
    Segment[] segments;
    int segselect = 0;
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

    public float Width
    {
        get
        {
            if (width > 0)
            {
                return width;
            }
            else
            {
                return 0.1f;
            }
        }
        set => width = value;
    }
    private void Start()
    {
        var sequence = lsystem.Generatesentence();
        int N = CalculLength(sequence);
        segments = new Segment[N];
        Debug.Log(sequence);
        Debug.Log(segments.Length);
        VisualizeSequence(sequence);
        Debug.Log(segselect);

        //BUILDINGS
        // density_map = new float[res,res];
        // //List<Vector3> ensemble = Concatenate(positions,batiments);
        // remap = MinMaxVect(positions); //on doit utiliser ensemble
        // Create_DM();
        // //CreateBuildings();
        // Texture2D tex = GetTexturefromArray(density_map,res);
        // image.GetComponent<RawImage>().texture = tex;
    }

    private void VisualizeSequence(string sequence)
    {
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        var currentPosition = Vector3.zero;
        bool success = true;

        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;

        Stack<GameObject> GO = new Stack<GameObject>();
        GameObject currentGO = new GameObject("Lines");

        //positions.Add(currentPosition);


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
                        width = Width,
                        angle = 180,
                        success = success
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
                        Width = agentParameter.width;
                        anglevar = agentParameter.angle;
                        success = agentParameter.success;
                        currentGO = GO.Pop();
                    }
                    else
                    {
                        throw new System.Exception("Dont have saved point in our stack");
                    }
                    break;

                case EncodingLetters.draw:
                    if(success){
                        //width and length
                        if (UnityEngine.Random.Range(0.0f,1.0f)<proba_att_length){
                            Length-=1;
                            Width-=0.05f;
                        }
                        GameObject line = new GameObject("line"); 
                        line.transform.parent = currentGO.transform;
                        currentGO = line;

                        tempPosition = currentPosition;
                        currentPosition += direction * length;

                        //new
                        Segment seg = new Segment(tempPosition,currentPosition);
                        // if(Detect(seg)){
                            
                        //     success = false;
                        // }
                        segments[segselect]=seg;
                        segselect+=1;
                        DrawLine(line,seg.point1, seg.point2, Color.red);
                    }
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
            for(int i=0;i<segselect;++i)
            {
                Instantiate(prefab, segments[i].point2, Quaternion.identity).transform.parent = spheres.transform;
            }
        }

    //Intersections et contacts
    private bool Detect(Segment seg1){
        Vector2 A = new Vector2(seg1.point1.x,seg1.point1.z);
        Vector2 B = new Vector2(seg1.point2.x,seg1.point2.z);
        Vector2 joint = Vector2.zero;
        float mindist = r;
        for(int i=0;i<segselect;++i)
        {
            Segment seg2 = segments[i];
            Vector2 C = new Vector2(seg2.point1.x,seg2.point1.z);
            Vector2 D = new Vector2(seg2.point2.x,seg2.point2.z);
            
            if((A!=D)&&(A!=C)){ //si pas meme points
                //intersection
                if(LineSegmentsIntersection(A,B,C,D, out Vector2 intersection)){
                    
                    //verif si pas a coté extremités
                    if((Vector2.Distance(B , D) < r)){
                        
                        seg1.point2.x= D.x;
                        seg1.point2.z= D.y;
                        return true;
                    }
                    if((Vector2.Distance(B , C) < r)){
                        
                        seg1.point2.x= C.x;
                        seg1.point2.z= C.y;
                        return true;
                    }
                    //
                    seg1.point2.x= intersection.x;
                    seg1.point2.z= intersection.y;
                    return true;
                }
                //nearest point
                float dist = Vector2.Distance(B , D);
                if((dist < mindist)){
                    joint.x= D.x;
                    joint.y= D.y;
                    mindist = dist;
                    //return true;
                }
                //nearest intersection
                float d = Vector2.Dot(C-D,B-D)/(Mathf.Pow((C.x-D.x),2) + Mathf.Pow((C.y-D.y),2));
                if((d>0) && (d<1)){
                    dist = Vector2.Distance(B,joint);
                    if((dist < mindist)){
                        joint = D + (C-D)*d;
                        mindist = dist;
                        //return true;
                    }
                }


            }
            
           
        }
        if(mindist!=r){
            seg1.point2.x= joint.x;
            seg1.point2.z= joint.y;
        }
        return false;
    }

    private int CalculLength(string sequence){
        int N = 0;
        foreach(var st in sequence){
            if(st == 'F'){
                N +=1;
            }
        }
        return N;
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
    private void CreateBuilding(float[,] density_map,Vector3 pos){
        GameObject Building =new GameObject("Building");
        Building.transform.parent = Buildings.transform;
        int posX = (int)Remap(pos.x,remap[0],remap[1],0.0f,res-1);
        int posZ = (int)Remap(pos.z,remap[2],remap[3],0.0f,res-1);
        int taille;
        if(posX>0 || posX<49 || posZ>0 || posZ<49 ){
            taille = taille_max*(int)(density_map[posX,posZ]);
        }else{
            taille = 1;
        }
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
        foreach (var position in segments)
        {
            posX = (int)Remap(position.point2.x,remap[0],remap[1],0.0f,res-1.0f);
            posZ = (int)Remap(position.point2.z,remap[2],remap[3],0.0f,res-1.0f);
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
                value = array[i,j];
                colors[j*res +i] = new Color(value,value,value,1.0f);   
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


}





