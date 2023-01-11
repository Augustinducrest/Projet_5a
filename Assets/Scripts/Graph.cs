using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var graph = InitGraph();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Node> InitGraph()
    {
        var nodes = new Dictionary<string, Node>();

        // nodes.Add("Head", new Node("Head"));
        // nodes.Add("T1", new Node("T1"));
        // nodes.Add("T2", new Node("T2"));
        // // While that works, a method is nicer:
        // nodes.Add("C1");

        // // These two lines should really be factored out to a single method call
        // nodes["Head"].Successors.Add(nodes["T1"]);
        // nodes["T1"].Predecessors.Add(nodes["Head"]);
        // nodes["Head"].Successors.Add(nodes["T2"]);
        // nodes["T2"].Predecessors.Add(nodes["Head"]);

        // // Yes. Much nicer
        // nodes.Connect("Head", "C1");
        // nodes.Connect("T1", "C1");
        // nodes.Connect("T2", "C1");

        var nodelist = new List<Node>(nodes.Values);
        return nodelist;
    }
}
