using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton stuff
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                FindFirstObjectByType<GameManager>().Init();

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
            Init();
        else if (_instance != this)
            Destroy(gameObject);
    }

    private void Init()
    {
        _instance = this;
    }
    #endregion

    [SerializeField]
    private UIHandler uiHandler;

    [SerializeField]
    private GameObject player;

    public GameObject CurrentEnemy { get; set; }

    public GameObject Player { get { return player; } }

    private void Start()
    {
        player.GetComponent<PlayerHealth>().OnValueChanged += uiHandler.SetBarValue;
        //player.GetComponent<PlayerStamina>().OnValueChanged += uiHandler.SetBarValue;
        CurrentEnemy.GetComponent<EnemyHealth>().OnValueChangedEnemy += uiHandler.SetCurrentEnemyHealthBar;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    // Not in use. 
    public void ChangeUIBarValue(int barIdx, float currentValue, float maxValue)
    {
        uiHandler.SetBarValue(barIdx, currentValue, maxValue);
    }

    public void DisplayDeathScreen()
    {
        uiHandler.ShowDeathScreen();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(0);
    }

    //public void SetEnemyLifeBar()
    //{
    //    //uiHandler.currentEnemyLifeBar = CurrentEnemy.
    //}
}
