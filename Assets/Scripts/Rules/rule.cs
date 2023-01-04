using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Projet_5a/Rule")]
public class rule : ScriptableObject
{
    public string letter; 
    [SerializeField]
    private string[] results = null;

    public string GetResult()
    {
        return results[0];
    }
   
}
