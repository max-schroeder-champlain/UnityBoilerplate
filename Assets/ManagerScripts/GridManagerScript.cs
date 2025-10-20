using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.EventSystems.EventTrigger;

public class GridManagerScript : MonoBehaviour
{
    //Grid
    public GameObject[,] grid;

    //Grid stats
    int gridHeight, gridWidth;
    int xEvenUpLimit, yRightLimit;

    //Holds tiles in range
    public List<GameObject> reach = new List<GameObject>();

    //Hex types
    public enum HexType { Basic, BaseCamp, Mana }

    //Sets grid paramaters
    private void Awake()
    {
        gridHeight = GetComponent<GameManagerScript>().gridHeight;
        gridWidth = GetComponent<GameManagerScript>().gridWidth;

        xEvenUpLimit = gridHeight - 1;
        yRightLimit = gridWidth - 1;

        grid = new GameObject[gridHeight, gridWidth];
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
}