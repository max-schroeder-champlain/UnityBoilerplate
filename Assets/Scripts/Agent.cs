using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    public List<Vector2> path = new List<Vector2>();
    public abstract void GeneratePath();
    public abstract void CheckPath(); //Information Reuse


}
