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
    public Vector2 agentStartPosition;
    public bool UseReuse = false;
    public bool UseHierachy = false;
    public Slider StepSlider;
    public TMP_Text StepText;
    public float StepTime = 1f;
    public bool isRunning;

    public bool SettingTarget;
    public bool SettingAgent;
    public TMP_Text TargetText;
    public TMP_Text AgentText;


    void Awake()
    {
        StepSlider.value = StepTime;
        StepText.text = StepTime.ToString("F2");
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
        gameObject.AddComponent<InitiateGridScript>().basicHexPrefab = HexPrefab;;
    }

    public void SetReuse()
    {
        UseReuse = !UseReuse;
    }

    public void SetHierarch()
    {
        UseHierachy = !UseHierachy;
    }
    public void SetStepTime()
    {
        StepTime = StepSlider.value;
        StepText.text = StepTime.ToString("F2");
    }
    public void SetAgent()
    {
        if (SettingTarget) return;
        SettingAgent = !SettingAgent;
    }
    public void Run(GameObject button)
    {
        TMP_Text text = button.GetComponent<TMP_Text>();
        if (isRunning)
        {
            text.text = "Run";
            isRunning = false;
            FindObjectOfType<Agent>().StopMove();
        }
        else
        {
            text.text = "Stop";
            isRunning = true;
            FindObjectOfType<Agent>().StartMove();
        }
    }
    private void Update()
    {
        if (!SettingTarget)
        {
            TargetText.text = "Set Target";
        }
        else
        {
            TargetText.text = "Setting Target";
        }
        if (!SettingAgent)
        {
            AgentText.text = "Set Agent";
        }
        else
        {
            AgentText.text = "Setting Agent";
        }
    }
}
