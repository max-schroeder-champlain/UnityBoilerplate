using UnityEngine;

public class InitiateGridScript : MonoBehaviour
{
    //Hex types
    public GameObject basicHexPrefab, baseHexPrefab;

    //Position in world space
    Vector3 gridTransform = new Vector3(0, 0, 0);

    //Grid parameters
    int gridHeight, gridWidth;

    //Counters for place in grid
    int rowCounter = 0, colCounter = 0;

    //Counts which base is being made, likely can be repclaced
    int playerCount = 0;

    //Fetches grid stats
    private void Awake()
    {
        gridHeight = GetComponent<GameManagerScript>().gridHeight;
        gridWidth = GetComponent<GameManagerScript>().gridWidth;
    }

    private void Start()
    {
        InstantiateGrid();
    }

    //Creates hexs for grid and calls InstantiateHex to assign variables, assigns adjaceny arrays to each
    void InstantiateGrid()
    {
        while(rowCounter < gridHeight)
        {
            //Spawns at 0,0
            InstantiateHex();

            gridTransform.y += 1;

            //Checks if colCounter has reached the end of the width and if rowCounter is at max
            //gridWidth and gridHeight are subtracted by 1 since they are set at the accurate number of hex starting at 1, not 0
            if (colCounter < gridWidth - 1 && rowCounter == gridHeight - 1)
            {
                //Starts new column
                colCounter++;
                gridTransform.x = colCounter;

                //Sets y position to 0 on even, 0.5 on odd
                //RowCounter set to -1 on even to allow for symetrical grid since 1 more is needed on even columns
                if (colCounter % 2 == 0)
                {
                    gridTransform.y = 0;
                    rowCounter = -1;
                }
                else
                {
                    gridTransform.y = 0.5f;
                    rowCounter = 0;
                }
            }

            rowCounter++;
        }

        GetComponent<GridManagerScript>().MakeAdjacencyArrays();
    }

    //Creates new hex
    void InstantiateHex()
    {
        //Subtracts 1 from odd as they start at 1 instead of 0
        int newCol = rowCounter;
        if (colCounter % 2 == 1)
            newCol--;

        //Finds and creates correct hex prefab
        GameObject newHex = AssignHexType();

        //Makes child of Gridmanager, names prefab
        newHex.transform.SetParent(GameObject.Find("GridManager").transform, false);
        newHex.name = newCol + "," + colCounter;

        //Sets ID
        newHex.GetComponent<HexScript>().AssignVars(newCol, colCounter);

        //Adds to grid in GridManagerScript
        GetComponent<GridManagerScript>().AddHex(newCol, colCounter, newHex);
    }

    //Sets prefab according to position in array
    GameObject AssignHexType()
    {
        GameObject temp;

        //If on top or bottom edge and in the middle column creates a base
        if (colCounter == gridWidth / 2 && (rowCounter == 1 || rowCounter == gridHeight - 1))
        {
            temp = Instantiate(baseHexPrefab, gridTransform, transform.rotation);
            temp.GetComponent<BaseHexScript>().player = playerCount;
            playerCount++;
        }
        else
        {
            temp = Instantiate(basicHexPrefab, gridTransform, transform.rotation);
        }

        return temp;
    }
}