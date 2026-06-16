using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Window Controllers")]
    [Tooltip("The ActionWindowController configured for attacking.")]
    [SerializeField] private ActionWindowController _attackWindow;

    [Tooltip("The ActionWindowController configured for defending")]
    [SerializeField] private ActionWindowController _defenseWindow;
    private PlayerStats playerStats;

    [Header("Input Keys")]
    private KeyCode _attackKey = KeyCode.F;
    private KeyCode _defendKey = KeyCode.J;

    public ActionWindowController _AttackWindow { get { return _attackWindow; } }
    public ActionWindowController _DefenseWindow { get { return _defenseWindow; } }

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ValidateReferences();
        SubscribeToWindowEvents();
        playerStats = GetComponent<PlayerStats>(); // ← was: GameManager.Instance.Player.GetComponent<PlayerStats>();
    }

    private void OnDestroy()
    {
        UnsubscribeFromWindowEvents();
    }

    private void Update()
    {
        if (_attackWindow == null)
        {
            Debug.Log($"o attack window ficou null... tempo: {Time.time}");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerStats.Potions > 0)
            {
                playerStats.AddHP(50);
                playerStats.AddPotions(-1);
            }
        }


        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(_attackKey))
        {
            if (_attackWindow.IsActive)
            {
                _attackWindow.TryConsumeWindow();
            }
        }

        if (Input.GetKeyDown(_defendKey))
        {
            if (_defenseWindow.IsActive)
            {
                _defenseWindow.TryConsumeWindow();
            }
        }
    }

    private void OnAttackWindowOpened(ActionWindowSnapshot snapshot)
    {
        Debug.Log("[Combat] Attack window opened — press attack to strike!");
        // add attack prepartion visual/audio
    }

    private void OnAttackWindowConsumed(ActionWindowSnapshot snapshot)
    {
        Debug.Log($"[Combat] Attack LANDED! ({snapshot.Progress * 100f:F0}% through window)");
        // add attack landded visual/audio
        animator.SetTrigger("Attack");
        GameManager.Instance.CurrentEnemy.GetComponent<EnemyHealth>().Damage(playerStats.AttackDamage);
    }

    private void OnAttackWindowExpired(ActionWindowSnapshot snapshot)
    {
        Debug.Log("[Combat] Attack MISSED — window expired with no input.");
        // add miss attack visual/audio
    }

    private void OnDefenseWindowOpened(ActionWindowSnapshot snapshot)
    {
        Debug.Log("[Combat] Defense window opened — press defend to parry!");
        // add defence start animation/audio
    }

    private void OnDefenseWindowConsumed(ActionWindowSnapshot snapshot)
    {
        Debug.Log($"[Combat] Defense SUCCESSFUL! ({snapshot.Progress * 100f:F0}% through window)");
        // add parry defence visual/audio
    }

    private void OnDefenseWindowExpired(ActionWindowSnapshot snapshot)
    {
        Debug.Log("[Combat] Defense FAILED — window expired, guard is down.");
        // add take damage visual/audio
        playerStats.Damage(20);
        animator.SetTrigger("Hurt");
    }

    private void SubscribeToWindowEvents()
    {
        _attackWindow.OnWindowOpened += OnAttackWindowOpened;
        _attackWindow.OnWindowConsumed += OnAttackWindowConsumed;
        _attackWindow.OnWindowExpired += OnAttackWindowExpired;

        _defenseWindow.OnWindowOpened += OnDefenseWindowOpened;
        _defenseWindow.OnWindowConsumed += OnDefenseWindowConsumed;
        _defenseWindow.OnWindowExpired += OnDefenseWindowExpired;
    }

    private void UnsubscribeFromWindowEvents()
    {
        if (_attackWindow != null)
        {
            _attackWindow.OnWindowOpened -= OnAttackWindowOpened;
            _attackWindow.OnWindowConsumed -= OnAttackWindowConsumed;
            _attackWindow.OnWindowExpired -= OnAttackWindowExpired;
        }

        if (_defenseWindow != null)
        {
            _defenseWindow.OnWindowOpened -= OnDefenseWindowOpened;
            _defenseWindow.OnWindowConsumed -= OnDefenseWindowConsumed;
            _defenseWindow.OnWindowExpired -= OnDefenseWindowExpired;
        }
    }

    private void ValidateReferences()
    {
        if (_attackWindow == null)
            Debug.LogError("[PlayerCombatController] _attackWindow is not assigned!", this);

        if (_defenseWindow == null)
            Debug.LogError("[PlayerCombatController] _defenseWindow is not assigned!", this);
    }

    /// <summary>Called by animation event</summary>
    public void TriggerAttackWindow() => _attackWindow.OpenWindow();

    /// <summary>Called by animation event</summary>
    public void TriggerDefenseWindow() => _defenseWindow.OpenWindow();

    public void TriggerAudio(AudioClip audioClip) => AudioManager.Instance.PlaySound(audioClip);
}