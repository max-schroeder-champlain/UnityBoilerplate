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
}
