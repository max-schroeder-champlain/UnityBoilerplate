using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.EventSystems.EventTrigger;

public class GridManagerScript : MonoBehaviour
{
    //Grid
    public GameObject[,] grid;
    public GameObject Agent;
    public GameObject Target;
    private GameObject agent;
    private GameObject target;
    //Grid stats
    public int gridHeight, gridWidth;
    int xEvenUpLimit, yRightLimit;

    //Holds tiles in range
    public List<GameObject> reach = new List<GameObject>();

    //Hex types
    public enum HexType { Basic }

    //Sets grid paramaters
    private void Awake()
    {
        gridHeight = GetComponent<GameManagerScript>().gridHeight;
        gridWidth = GetComponent<GameManagerScript>().gridWidth;

        xEvenUpLimit = gridHeight - 1;
        yRightLimit = gridWidth - 1;

        grid = new GameObject[gridHeight, gridWidth];
    }
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        agent = Instantiate(Agent);
        agent.transform.position = grid[0, 0].transform.position;
        GameManagerScript.Instance.agentPosition = new Vector2(0, 0);
        target = Instantiate(Target);
        target.transform.position = grid[10,10].transform.position;
        GameManagerScript.Instance.currentTargetPosition = new Vector2(10, 10);
    }
    //Adds hex to grid
    public void AddHex(int xPos, int yPos, GameObject newHex)
    {
        grid[xPos, yPos] = newHex;
    }

    //Sets adjacent tiles for a hex unless null, leaves -1,-1
    public void MakeAdjacencyArrays()
    {
        Vector2 temp;
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int j = 0; j < grid.GetLength(1); j++)
            {
                if(grid[i, j] != null)
                {
                    temp = new Vector2(i, j);
                    GetAdjacent(temp);
                }
            }
        }
    }

    //Finds hexs in each direction around a starting hex
    public void GetAdjacent(Vector2 pos)
    {
        int xPos = (int)pos.x, yPos = (int)pos.y;
        //Marks pos in array
        int counter = 0;

        //Down
        if (xPos != 0)
            AssignAdjacent(pos, -1, 0, counter);
        counter++;

        //Even rows
        if (yPos % 2 == 0)
        {

            //Left
            if (yPos != 0)
            {
                if (xPos != 0)
                    AssignAdjacent(pos, -1, -1, counter);
                counter++;

                if (xPos != xEvenUpLimit)
                    AssignAdjacent(pos, 0, -1, counter);
                counter++;
            }

            //Up
            if (xPos != xEvenUpLimit)
                AssignAdjacent(pos, 1, 0, counter);
            counter++;

            //Right
            if (yPos != yRightLimit)
            {
                if (xPos != xEvenUpLimit)
                    AssignAdjacent(pos, 0, 1, counter);
                counter++;

                if (xPos != 0)
                    AssignAdjacent(pos, -1, 1, counter);
                counter++;
            }
        }

        //Odd rows
        if (yPos % 2 == 1)
        {
            //Left
            if (yPos != 0)
            {
                if (xPos != xEvenUpLimit)
                    AssignAdjacent(pos, 0, -1, counter);
                counter++;

                AssignAdjacent(pos, 1, -1, counter);
                counter++;

            }

            //Up
            if (xPos != xEvenUpLimit - 1)
                AssignAdjacent(pos, 1, 0, counter);
            counter++;

            //Right
            if (yPos != yRightLimit)
            {
                AssignAdjacent(pos, 1, 1, counter);
                counter++;

                if (xPos != xEvenUpLimit)
                    AssignAdjacent(pos, 0, 1, counter);
                counter++;
            }
        }
    }

    //Changes centre to match adjacent hex, adds to adjacent array
    void AssignAdjacent(Vector2 centre, int xChange, int yChange, int counter)
    {
        Vector2 tempPos = centre;

        tempPos.x += xChange;
        tempPos.y += yChange;

        grid[(int)centre.x, (int)centre.y].GetComponent<HexScript>().adjacent[counter] = tempPos;
    }
    public void Clear(int width, int height)
    {
        gridHeight = height;
        gridWidth = width;
        for(int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
               if(grid[i, j] != null)
                Destroy(grid[i,j]);
               grid[i,j] = null;
            } 
        }
        grid = new GameObject[gridWidth, gridHeight];

    }

    public bool CheckHex(int x, int y)
    {
        Debug.Log(grid[x, y].GetComponent<HexScript>() != null);
        return grid[x, y].GetComponent<HexScript>().isPassable;
    }

    public void MoveAgent(Vector2 newPos)
    {
        agent.transform.position = grid[(int)newPos.x, (int)newPos.y].transform.position;
        GameManagerScript.Instance.agentPosition = newPos;
    }
}