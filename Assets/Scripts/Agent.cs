using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private GridManagerScript gridManagerScript;
    private List<Vector2> path = new List<Vector2>();
    Queue<Vector2> frontier;
    Dictionary<Vector2, bool> visited = new Dictionary<Vector2, bool>();
    List<Vector2> frontierSet;
    public Coroutine move;
    private void Awake()
    {
        gridManagerScript = FindObjectOfType<GridManagerScript>();
    }
    public void GeneratePath()
    {
        gridManagerScript.ClearInPath();
        int testLog = 0;
        path = new List<Vector2>();
        visited = new Dictionary<Vector2, bool>();
        Vector2 targetPosition = GameManagerScript.Instance.currentTargetPosition;
        Vector2 agentPos = GameManagerScript.Instance.agentPosition;
        bool foundTarget = false;
        Vector2 targetNeighbor;
        if (targetPosition == agentPos)
        {
            Debug.Log("At Target");
            return;
        }
        if (!gridManagerScript.CheckHex((int)targetPosition.x, (int)targetPosition.y))
        {
            Debug.Log("No Valid Path");
            return;
        }

        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();
        frontier = new Queue<Vector2>();
        frontierSet = new List<Vector2>();
        frontier.Enqueue(agentPos);
        frontierSet.Add(agentPos);
        costSoFar.Add(agentPos, ManhattanDistance(agentPos, targetPosition));
        while (frontier.Count != 0)
        {
            Vector2 current = frontier.Dequeue();
            frontierSet.Remove(current);
            visited[current] = true;

            List<Vector2> neighbors = FindNeighbors(current, gridManagerScript.grid);
            foreach (Vector2 neighbor in neighbors)
            {
                int newCost = costSoFar[current] + 1; // Add neighbor weight here
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    frontier.Enqueue(neighbor);
                    frontierSet.Add(neighbor);
                    costSoFar.Add(neighbor, costSoFar[current] + ManhattanDistance(neighbor, targetPosition));
                    cameFrom.Add(neighbor, current);
                }
                if (neighbor == targetPosition)
                {
                    foundTarget = true;
                    targetNeighbor = neighbor;
                    path.Add(neighbor);
                    gridManagerScript.grid[(int)path.Last().x, (int)path.Last().y].GetComponent<HexScript>().SetIsInPath(true);
                }
            }
            if (foundTarget)
            {
                while(path.Last() != agentPos)
                {
                    path.Add(cameFrom[path.Last()]);
                    gridManagerScript.grid[(int)path.Last().x, (int)path.Last().y].GetComponent<HexScript>().SetIsInPath(true);
                }
                if(path.Last() == agentPos)
                {
                    gridManagerScript.grid[(int)path.Last().x, (int)path.Last().y].GetComponent<HexScript>().SetIsInPath(false);
                    path.Remove(agentPos);
                    Debug.Log(testLog);
                    return;
                }
            }
            testLog++;
        }
        Debug.Log(testLog);
    }

    private IEnumerator MoveAgent()
    {
        yield return new WaitForSeconds(GameManagerScript.Instance.StepTime);
        if(GameManagerScript.Instance.UseReuse && CheckPath()) { }
        else
            GeneratePath();
        foreach(Vector2 vector2 in path)
        {
            Debug.Log(vector2);
        }
        if(path.Count != 0)
        {
            gridManagerScript.MoveAgent(path.Last());
            path.Remove(path.Last());
           
        }
        if(GameManagerScript.Instance.isRunning)
            move = StartCoroutine(MoveAgent());
    }
    public void StartMove()
    {
        move = StartCoroutine(MoveAgent());
    }
    public void StopMove()
    {
        if(move != null)
            StopCoroutine(move);
    }
    //Information Reuse
    public bool CheckPath()
    {
        if (path.Count == 0) return false;
        foreach(var point in path)
        {
            if(!gridManagerScript.grid[(int)point.x, (int)point.y].GetComponent<HexScript>().isPassable)
            {
                return false;
            }
        }
        return true;
    }
    private int ManhattanDistance(Vector2 a, Vector2 b)
    {
        float temp = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        return (int)temp;
    }
    private List<Vector2> FindNeighbors(Vector2 current, GameObject[,] grid)
    {
        List<Vector2> neighbors = new List<Vector2>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector2 temp = new Vector2(current.x + x, current.y + y);
                int tempX = x;
                int tempY = y;
                if (current.x % 2 == 0)
                {
                    tempY++;
                }
                if (temp.x < 0 || temp.x >= gridManagerScript.gridWidth) continue;

                int oddModifier = 0;
                if (temp.x % 2 != 0)
                {
                    oddModifier++;
                }

                if (temp.y < 0 || temp.y >= gridManagerScript.gridHeight - oddModifier) continue;
                //if can't move there: return
                if (y != 0)
                {
                    if (tempY == 0 || tempY == 1) { }
                    else continue;
                }
                if (visited.ContainsKey(temp)) continue;
                if (frontierSet.Contains(temp)) continue;
                Debug.Log(temp.x + " " + temp.y);
                if (!gridManagerScript.CheckHex((int)temp.x, (int)temp.y)) continue;
                neighbors.Add(temp);
            }
        }
        return neighbors;
    }

}
