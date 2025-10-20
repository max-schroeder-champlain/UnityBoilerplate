using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    public GameObject camera;

    //Grid parameters
    public int gridWidth, gridHeight;

    //Keeps track of which player's turn it is
    public enum PhaseType { SetUp, Combat, Win }
    public PhaseType phase = PhaseType.SetUp;
    public int startingPlayer = 0, activePlayer = 0;
    public bool unitInUse = false;

    //Sets set up range
    public int setUpRange = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        GetComponent<UIManagerScript>().UpdateSetUpPopUp();
    }

    //Called to switch who starts new build/attack phase
    public void SwitchStartingPlayer()
    {
        if (startingPlayer == 0)
            startingPlayer = 1;
        else
            startingPlayer = 0;

        SwitchActivePlayer();
    }

    //Called to switch player turn, also changes camera to activePlayer
    public void SwitchActivePlayer()
    {
        if (activePlayer == 0)
            activePlayer = 1;
        else
            activePlayer = 0;

        camera.GetComponent<CameraScript>().UpdatePlayerCam(activePlayer);
    }

    void EndSetUpPhase()
    {
        GetComponent<GridManagerScript>().EndSetUp();
        phase = PhaseType.Combat;
        GetComponent<UIManagerScript>().SetUI(phase);
    }

    public void EndAttackPhase()
    {
        SwitchStartingPlayer();
        GetComponent<UnitManagerScript>().ResetExhaustion();
        GetComponent<GridManagerScript>().IncrementBaseStats();
        GetComponent<UIManagerScript>().SetUI(phase);
    }

    //Limits how many units each player can spawn
    public void HandleSetUp()
    {
        if (GetComponent<UnitManagerScript>().newUnits.Count <= 0)
        {
            if (activePlayer == 0)
            {
                GetComponent<GridManagerScript>().EndSetUp();
                GetComponent<UIManagerScript>().SwitchPlayerSetUp();
                activePlayer = 1;
            }
            else
            {
                activePlayer = 0;

                EndSetUpPhase();
            }

            camera.GetComponent<CameraScript>().UpdatePlayerCam(activePlayer);
        }
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
}
