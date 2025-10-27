using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HexScript : MonoBehaviour
{
    public GameObject gameManager;

    public Vector2 ID;
    public GridManagerScript.HexType type;
    public bool inReach = false, isOccupied = false;
    public bool isPassable = true, isInPath;
    public Vector2[] adjacent = new Vector2[6];
    public int[] distance = new int[6];

    public HexScript()
    {

    }

    public HexScript(GridManagerScript.HexType newType)
    {
        type = newType;
    }

    //Sets -1, -1, accurate adjacency set in InitiateGridScript
    private void Awake()
    {
        for(int i = 0; i < adjacent.Length; i++)
        {
            adjacent[i] = new Vector2(-1, -1);
        }
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    //Sets ID and hex color
    public void AssignVars(int x, int y)
    {
        ID = new Vector2(x, y);

        SetHexColour();
    }

    //Resets hex for reach checks
    public void ResetHex()
    {
        SetHexColour();
        inReach = false;

        for (int i = 0; i < distance.Length; i++)
        {
            distance[i] = 0;
        }
    }

    //Parent version of function, children set color
    public virtual void SetHexColour()
    {
        if (!isPassable) GetComponent<SpriteRenderer>().color = Color.black;
        else if (isInPath) GetComponent<SpriteRenderer>().color = Color.red;
        else GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickEvent();
        }
    }

    //Parent version of function, children set use
    public void ClickEvent()
    {
        if (Input.GetMouseButton(0))
        {
            if (GameManagerScript.Instance.SettingAgent)
            {
                FindObjectOfType<GridManagerScript>().CreateAgent((int)ID.x, (int)ID.y);
                return;
            }

            if (GameManagerScript.Instance.SettingTarget)
            {
                FindObjectOfType<GridManagerScript>().CreateTarget((int)ID.x, (int)ID.y);
                return;
            }

            if (isPassable) isPassable = false;
            else isPassable = true;

            SetHexColour();
        }
    }

    public void SetIsInPath(bool inPath)
    {
        isInPath = inPath;

        SetHexColour();
    }
}