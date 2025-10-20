using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    var path = new Queue.<Vector2>();
    public virtual void GeneratePath();
    public virtual void CheckPath(); //Information Reuse


}
