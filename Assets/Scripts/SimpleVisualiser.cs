using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVisualiser : MonoBehaviour
{
    public LSystemGenerator lsystem;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    public Material linemat;
    private int length = 8;
    private float angle = 90;

    public int Length 
    {
        get
        {
            if(length >0){
                return length;
            }
            else
            {
                return 1;
            }
        }
        set => length = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        var seq = lsystem.Generatesentence();
        VisualizeSeq(seq);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///Functions
    private void VisualizeSeq(string sequence){
        
    }
    private void DrawLine(Vector3 start,Vector3 end,Color red){
        
    }
}
