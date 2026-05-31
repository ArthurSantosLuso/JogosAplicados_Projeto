using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{

    #region Singleton stuff
    private static CombatManager _instance;

    public static CombatManager Instance
    {
        get
        {
            if (_instance == null)
                FindFirstObjectByType<CombatManager>().Init();

            return _instance;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferences();
    }
    private void FindReferences()
    {
        if (uiHandler == null)
            uiHandler = FindFirstObjectByType<UIHandler>();

        if (player == null)
            player = FindFirstObjectByType<GameObject>();

        if (enemy == null)
            enemy = FindFirstObjectByType<GameObject>();
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

    [SerializeField]
    private GameObject enemy;

    public GameObject CurrentEnemy { get { return enemy; } }

    public GameObject Player { get { return player; } }

    public float SavedPlayerHealth { get; set; }
    public Vector3 SavedPlayerPosition { get; set; }
    public List<string> ClearedEventsIds { get; set; } = new();

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = player.GetComponent<PlayerStats>();
        if (uiHandler != null)
        {
            player.GetComponent<PlayerStats>().OnValueChanged += uiHandler.SetBarValue;
            //player.GetComponent<PlayerStamina>().OnValueChanged += uiHandler.SetBarValue;
            enemy.GetComponent<EnemyHealth>().OnValueChangedEnemy += uiHandler.SetCurrentEnemyHealthBar;
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
    public void ReturnToMap()
    {
        GameManager.Instance.SavedPlayerHealth = playerStats.CurrentValue;
        SceneManager.LoadScene(0);
    }
}
