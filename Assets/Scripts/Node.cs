using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string Name { get; set; }
    public AgentParameters agent { get; set; }
    public List<Node> Predecessors { get; set; }
    public List<Node> Successors { get; set; }


    public Node(AgentParameters agentParameters)
    {
        this.agent = agentParameters;
    }
}

// public static class NodeHelper
// {        
//     public static void Add(this Dictionary<string, Node> dict, string nodename)
//     {
//         dict.Add(nodename, new Node());
//     }
//     public static void Connect(this Dictionary<string, Node> dict, string from, string to)
//     {
//         dict[ from ].Successors.Add(dict[ to ]);
//         dict[ to ].Predecessors.Add(dict[ from ]);
//     }
// }