using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class LSystemGenerator : MonoBehaviour
{

    public rule[] rules; 
    public string rootSentence;
    [Range(0,10)]
    public int iterationLimit = 1 ;


    private void Start()
    {
        Debug.Log(Generatesentence());
    }

    public string Generatesentence(string word = null)
    {
        if(word ==null)
        {
            word = rootSentence;
        }
        return GrowRecursive(word);
        
    }

    private string  GrowRecursive(string word,int interationIndex = 0)
    {
        if(interationIndex >= iterationLimit)
        {
            return word; 
        }
        StringBuilder newWord = new StringBuilder();

        foreach ( var c in word)
        {
            newWord.Append(c);
            processRulesRecursivelly(newWord,c,interationIndex);
        }
        return newWord.ToString();

    }

    private void processRulesRecursivelly(StringBuilder newWord,char c, int interationIndex)
    {
        foreach (var rule in rules)
        {
            if( rule.letter == c.ToString())
            {
                newWord.Append(GrowRecursive(rule.GetResult(), interationIndex + 1));
            }
        }
    }



}
