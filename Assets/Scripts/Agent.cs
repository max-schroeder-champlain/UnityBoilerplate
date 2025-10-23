using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public List<Vector2> path = new List<Vector2>();
    public void GeneratePath()
    {

    }
    //Information Reuse
    public bool CheckPath()
    {
        foreach(var point in path)
        {
            //if a point is bad, return false
        }
        return true;
    }


}
