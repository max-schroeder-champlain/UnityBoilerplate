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
            if (gameManager.GetComponent<GameManagerScript>().CheckPhase(GameManagerScript.PhaseType.SetUp))
                HandleSetUp();
            else
                HandleCombat();
        }
    }

    //Overridden for each type's set up phase
    public virtual void HandleSetUp()
    {
        if (!isOccupied && inReach)
        {
            if (gameManager.GetComponent<GameManagerScript>().activePlayer == 0 && ID.x < 10)//ten chosen arbitrarily, could be improved
            {
                gameManager.GetComponent<UnitManagerScript>().AddUnit(transform, ID);
                gameManager.GetComponent<GameManagerScript>().HandleSetUp();
            }
            else if (gameManager.GetComponent<GameManagerScript>().activePlayer == 1 && ID.x > 10)
            {
                gameManager.GetComponent<UnitManagerScript>().AddUnit(transform, ID);
                gameManager.GetComponent<GameManagerScript>().HandleSetUp();
            }
        }
    }

    //Overridden for each type's build phase
    public virtual void HandleBuild()
    {
        /*if (inReach && gameManager.GetComponent<UnitManagerScript>().newUnits.Count > 0)
        {
            gameManager.GetComponent<UnitManagerScript>().AddUnit(transform, ID);
        }*/
    }

    //Overridden for each type's attack phase
    public virtual void HandleCombat()
    {

    }
}