using Ink.Parsed;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField]
    private UIHandler uiHandler;

    [SerializeField]
    private GameObject player;
    
    public GameObject CurrentEnemy { get; set; }

    public GameObject Player { get { return player; } }

    public float SavedPlayerHealth { get; set; }
    public Vector3 SavedPlayerPosition { get; set; }
    public List<string> ClearedEventsIds { get; set; } = new();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferences();

        if (player != null && SavedPlayerPosition != Vector3.zero)
        {
            player.transform.position = SavedPlayerPosition;
        }
    }

    private void FindReferences()
    {
        uiHandler = FindFirstObjectByType<UIHandler>();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
