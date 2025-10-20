using NUnit.Framework;
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

    //Shows range in which new units can be spawned.
    public void CreateSetUp()
    {
        //Get range limit
        int setUpLimit = GetComponent<GameManagerScript>().setUpRange;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                //Checks if in bottom section of map
                if (GetComponent<GameManagerScript>().activePlayer == 0 && i < setUpLimit)
                {
                    AssignSetUpRange(i, j);
                }
                //Checks in top section, needs a -1 since the 0 adds an extra row on the bottom
                else if (GetComponent<GameManagerScript>().activePlayer == 1 && i > grid.GetLength(0) - setUpLimit - 1)
                {
                    AssignSetUpRange(i, j);
                }
            }
        }
    }

    void AssignSetUpRange(int x, int y)
    {
        //Checks for null due to uneven column height
        if (grid[x, y] != null)
        {
            if (grid[x, y].GetComponent<HexScript>().type != HexType.BaseCamp)//Ignores base camp hex when making range
            {
                grid[x, y].GetComponent<HexScript>().inReach = true;
                grid[x, y].GetComponent<SpriteRenderer>().color = Color.grey;
            }
        }
    }

    public void EndSetUp()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null && grid[i, j].GetComponent<HexScript>().inReach)
                    grid[i, j].GetComponent<HexScript>().ResetHex();
            }
        }
    }

    //Finds the total range of a unit, calls DisplayRange to show it
    public void FindReach(Vector2 pos, int remainingReach)
    {
        //Starts reach list so it can be used in a for loop
        SetReach(pos);
        remainingReach--;

        //Finds rest of reach
        for(int i = 0; i < remainingReach; i++)
        {
            int reachLength = reach.Count;

            for (int j = 0; j < reachLength; j++)
            {
                SetReach(reach[j].GetComponent<HexScript>().ID);
            }
        }

        //Displays reach in game
        DisplayRange();
    }

    //Finds a tile's adjacent tile array and calls AddHexToReach for each
    void SetReach(Vector2 pos)
    {
        int xID = (int)pos.x, yID = (int)pos.y;

        for (int i = 0; i < 6; i++)
        {
            Vector2 adjID = grid[xID, yID].GetComponent<HexScript>().adjacent[i];
            AddHexToReach(adjID, pos, i);
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

    //Checks if adjacent tile exists and if it is not already in list, adds if both true
    void AddHexToReach(Vector2 pos, Vector2 prevPos, int direction)
    {
        //Checks for nonexistent tiles
        if (pos != new Vector2(-1, -1))
        {
            bool inList = false;
            
            //Checks if hex exists in reach list
            for (int i = 0; i < reach.Count; i++)
            {
                if (reach[i].GetComponent<HexScript>().ID == pos)
                    inList = true;
            }
            
            //Adds to list if not
            if (!inList)
            {
                reach.Add(grid[(int)pos.x, (int)pos.y]);
                grid[(int)pos.x, (int)pos.y].GetComponent<HexScript>().inReach = true;//no check for odd top values

                //Sets distance from starting position
                for (int i = 0; i < grid[(int)pos.x, (int)pos.y].GetComponent<HexScript>().distance.Length; i++)
                {
                    grid[(int)pos.x, (int)pos.y].GetComponent<HexScript>().distance[i] = grid[(int)prevPos.x, (int)prevPos.y].GetComponent<HexScript>().distance[i];
                }
                grid[(int)pos.x, (int)pos.y].GetComponent<HexScript>().distance[direction] += 1;
            }
        }
    }

    //Handles increasing rifleman range with movement
    public void AddRiflemanReach(Vector2 unitID, int range)//pointer here?
    {
        //Create temporary array for easy of writing
        int[] tempArray = new int[6];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = GetComponent<UnitManagerScript>().GetUnit(unitID).GetComponent<RiflemanScript>().rangeIncrease[i];
        }

        //Searches each hex in reach for an adjacency that is at max range
        //If a hex is found, it checks whether the rifleman has a range increase in that direction and applies it
        for (int i = 0; i < reach.Count; i++)
        {
            for (int j = 0; j < tempArray.Length; j++)
            {
                if (reach[i].GetComponent<HexScript>().distance[j] == range)
                {
                    Vector2 newID = new Vector2();
                    newID = reach[i].GetComponent<HexScript>().adjacent[j];

                    while (tempArray[j] > 0)
                    {
                        AddHexToReach(newID, unitID, j);

                        tempArray[j] -= 1;

                        newID = reach[reach.Count - 1].GetComponent<HexScript>().adjacent[j];
                    }
                }
            }
        }

        DisplayRange();
    }

    public void IncrementBaseStats()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j]?.GetComponent<HexScript>().type == HexType.BaseCamp)
                {
                    grid[i, j].GetComponent<BaseHexScript>().AddResources();
                }
            }
        }
    }

    //Checks if a base has enough resources to create a new unit
    public bool CheckBaseStats(int neededRecruits, int neededAmmo)
    {
        return GetBaseHex(GetComponent<GameManagerScript>().activePlayer).GetComponent<BaseHexScript>().CheckBuildUnit(neededRecruits, neededAmmo);
    }

    //Resets neededVars when a unit is removed from build
    public void RemoveBuild(int neededRecruits, int neededAmmo)
    {
        GetBaseHex(GetComponent<GameManagerScript>().activePlayer)?.GetComponent<BaseHexScript>().RemoveBuild(neededRecruits, neededAmmo);
    }

    //Finds the base hex matching the activePlayer
    public GameObject GetBaseHex(int player)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j]?.GetComponent<HexScript>().type == HexType.BaseCamp && grid[i, j]?.GetComponent<BaseHexScript>().player == GetComponent<GameManagerScript>().activePlayer)
                {
                    return grid[i, j];
                }
            }
        }

        return null;
    }

    //Visualizes range
    void DisplayRange()
    {
        for (int i = 0; i < reach.Count; i++)
        {
            reach[i].GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    //Resets hexs to original state and clears range list
    public void ResetRange()
    {
        for(int i = 0; i < reach.Count; i++)
        {
            reach[i].GetComponent<HexScript>().ResetHex();
        }

        reach.Clear();
    }
}