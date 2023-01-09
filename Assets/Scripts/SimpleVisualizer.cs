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
        List<Segment> segments = new List<Segment>();
        public GameObject prefab;
        public Material lineMaterial;

        private int length = 8;
        public float angle = 90;

        public float minAngle =-90.0f;
        public float maxAngle = 90.0f;

        public int minDist = 5;
        public int maxDist = 8;

        public float r =3.0f;

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
            var sequence = lsystem.GenerateSentence();
            VisualizeSequence(sequence);
        }

        private void VisualizeSequence(string sequence)
        {
            Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
            var currentPosition = Vector3.zero;

            Vector3 direction = Vector3.forward;
            Vector3 tempPosition = Vector3.zero;

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
                            length = Length
                        });
                        break;

                    case EncodingLetters.load:

                        if (savePoints.Count > 0)
                        {
                            var agentParameter = savePoints.Pop();
                            currentPosition = agentParameter.position;
                            direction = agentParameter.direction;
                            Length = agentParameter.length;
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

                        Segment seg = new Segment(currentPosition,tempPosition);
                        Vector3 pInterseg =DetectIntersection(seg,tempPosition);
                        
                        if (pInterseg != new Vector3(0,0,0) && pInterseg !=  tempPosition )
                        {
                            //print(pInterseg +"seg:" + seg.point1 + seg.point2);
                            currentPosition = pInterseg;
                        } 
                        Vector3 detectedpoint = DetectClosestPoint(currentPosition);
                        if ( currentPosition == detectedpoint)
                        {
                            DrawLine(tempPosition, currentPosition, Color.red);
                            positions.Add(currentPosition);
                        }
                        else
                        {
                            currentPosition = detectedpoint;
                            DrawLine(tempPosition, currentPosition, Color.red);
                        }
                        break;

                    case EncodingLetters.turnRight:

                        direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                        break;

                    case EncodingLetters.turnLeft:

                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                        break;

                    case EncodingLetters.turn:

                        float rot = UnityEngine.Random.Range(minAngle,maxAngle);
                        direction = Quaternion.AngleAxis(rot, Vector3.up) * direction;
                        break;

                    default:
                        break;
                }
            }
            foreach (var position in positions)
            {
                Instantiate(prefab, position, Quaternion.identity);
            }
        }

        Vector3 DetectIntersection(Segment seg1,Vector3 p1)
        {
            print("seg:"+ seg1.point1 + seg1.point2);
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
                        print(d + "p" + pointInter );
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
                        //print (new Vector3 (xA+t1*(xB-xA) ,0 , yA+t1*(yB-yA)));
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

        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            GameObject line = new GameObject("line");
            line.transform.position = start;
            var lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.SetPosition(0,end);
            lineRenderer.SetPosition(1,start);
            Segment seg = new Segment(start,end);
            segments.Add(seg);
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
    }



