/*
	Made by Sunny Valle Studio
	(https://svstudio.itch.io)
*/
using System;
using System.Collections.Generic;
using UnityEngine;


public class SimpleVisualizer : MonoBehaviour
{
    public LSystemGenerator lsystem;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    public Material lineMaterial;

    private int length = 8;
    private float angle = 90;

    public float minAngle =-90.0f;
    public float maxAngle = 90.0f;

    public int minDist = 5;
    public int maxDist = 8;

    private float r =3.0f;
    public int tolerance = 50;
    int anglevar = 180;

    public int Length
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
                    int dist = UnityEngine.Random.Range(minDist,maxDist);
                    length = dist;
                    tempPosition = currentPosition;
                    currentPosition += direction * length;

                   //Graph
                    GameObject line =  DrawLine(tempPosition, currentPosition, Color.red);
                    line.transform.parent = currentGO.transform;
                    currentGO = line;


                    positions.Add(currentPosition);
                    break;

                case EncodingLetters.turnRight:

                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    break;

                case EncodingLetters.turnLeft:

                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
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
            GameObject sph = Instantiate(prefab, position, Quaternion.identity);
            sph.transform.parent = spheres.transform;
        }

    }

    void DetectIntersection(Vector3 pos)
    {
        //Physics.CheckSphere(pos, r);
    }

    private GameObject DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject line = new GameObject("line");
        line.transform.position = start;
        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.SetPosition(0, end);
        lineRenderer.SetPosition(1, start);

        return line;
    }

    public enum EncodingLetters
    {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnRight = '+',
        turnLeft = '-',
        turn = 'R'
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

    private int GiveAngle(int angle, int t){

        if (angle< -180 + 2*t){
            Range[] rg = new Range[]{new Range(angle+t,180-t)};
            return RandomValueFromRanges(rg);
        }
        else if (angle > 180 - 2*t){
            Range[] rg = new Range[]{new Range(-180+t,angle-t)};
            return RandomValueFromRanges(rg);
        }
        else
        {
            Range[] rg = new Range[]{new Range(angle+t,180-t),new Range(angle+t,180-t)};
            return RandomValueFromRanges(rg);
        }
        
    }
}





