using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;
    public GameObject HexPrefab;
    public GameObject camera;
    //Grid parameters
    public int gridWidth, gridHeight;
    public List<Vector2> currentTargetPosition;
    public Vector2 agentPosition;

    //Keeps track of which player's turn it is
    public enum PhaseType { SetUp, Combat, Win }
    public PhaseType phase = PhaseType.SetUp;

    public bool settingTarget;
    public bool settingAgent;

    void Awake()
    {
    }

    //Called to switch who starts new build/attack phase
    public void ChangePhase(PhaseType newPhase)
    {
        phase = newPhase;
    }

    //Used to check phase in different scripts
    public bool CheckPhase(PhaseType phaseCheck)
    {
        if (phase == phaseCheck)
            return true;

        return false;
    }

    private void OnEnable() //Make instance
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void OnDisable() //Destroy instance
    {
        if (Instance != null && Instance == this)
        {
            Instance = null;
        }
    }
    public void Reset()
    {
        InitiateGridScript grid = GetComponent<InitiateGridScript>();
        GridManagerScript gridManager = GetComponent<GridManagerScript>();
        gridManager.Clear(gridWidth, gridHeight);
        Destroy(grid);
        gameObject.AddComponent<InitiateGridScript>().basicHexPrefab = HexPrefab;
    }
}
