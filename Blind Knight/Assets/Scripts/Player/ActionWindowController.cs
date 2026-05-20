using UnityEngine;
using System.Collections;
using System;

public class ActionWindowController : MonoBehaviour
{
    [Header("Window Settings")]
    [Tooltip("Label used in debug messages to identify this window")]
    [SerializeField] private string _windowLabel = "Action";

    [Tooltip("How long, in seconds, the window stays active waiting for input")]
    [SerializeField][Min(0.05f)] private float _windowDuration = 0.5f;

    [Tooltip("Optional startup delay before the window becomes active")]
    [SerializeField][Min(0f)] private float _startupDelay = 0f;


    /// <summary>Fired when the active window opens</summary>
    public event Action<ActionWindowSnapshot> OnWindowOpened;

    /// <summary>Fired when input is accepted inside the window</summary>
    public event Action<ActionWindowSnapshot> OnWindowConsumed;

    /// <summary>Fired when the window closes without any input</summary>
    public event Action<ActionWindowSnapshot> OnWindowExpired;

    /// <summary>Fired every frame while the window is active</summary>
    public event Action<ActionWindowSnapshot> OnWindowTick;

    private ActionWindowState _state = ActionWindowState.Idle;
    private float _elapsed = 0f;
    private Coroutine _routine = null;

    /// <summary>A live snapshot of the current window state.</summary>
    public ActionWindowSnapshot Snapshot => new ActionWindowSnapshot(_state, _elapsed, _windowDuration);

    public ActionWindowState State => _state;
    public bool IsActive => _state == ActionWindowState.Active;
    public string Label => _windowLabel;

    public float WindowDuration
    {
        get => _windowDuration;
        set => _windowDuration = Mathf.Max(0.05f, value);
    }

    public float StartupDelay
    {
        get => _startupDelay;
        set => _startupDelay = Mathf.Max(0f, value);
    }

    public void OpenWindow()
    {
        CancelWindow();
        _routine = StartCoroutine(WindowRoutine());
    }

    /// <summary>
    /// Attempts to consume the window (input)
    /// </summary>
    /// <returns>Returns true if the window was active and successfully consumed</returns>
    public bool TryConsumeWindow()
    {
        if (_state != ActionWindowState.Active)
        {
            Debug.Log($"[{_windowLabel}] Input received but window is {_state}. Missed.");
            return false;
        }

        SetState(ActionWindowState.Consumed);
        StopActiveRoutine();

        ActionWindowSnapshot snapshot = new ActionWindowSnapshot(ActionWindowState.Consumed, _elapsed, _windowDuration);
        Debug.Log($"[{_windowLabel}] Window CONSUMED at {_elapsed:F3}s / {_windowDuration:F3}s " +
                  $"({snapshot.Progress * 100f:F1}% through window).");

        OnWindowConsumed?.Invoke(snapshot);
        return true;
    }

    /// <summary>
    /// Immediately cancels any running window and resets to Idle.
    /// </summary>
    public void CancelWindow()
    {
        StopActiveRoutine();
        SetState(ActionWindowState.Idle);
        _elapsed = 0f;
    }

    
    private IEnumerator WindowRoutine()
    {
        SetState(ActionWindowState.Idle);
        _elapsed = 0f;

        // Startup delay
        if (_startupDelay > 0f)
        {
            Debug.Log($"[{_windowLabel}] Startup delay: {_startupDelay:F3}s.");
            yield return new WaitForSeconds(_startupDelay);
        }

        // Open the window
        SetState(ActionWindowState.Active);
        Debug.Log($"[{_windowLabel}] Window OPENED. You have {_windowDuration:F3}s to act.");
        OnWindowOpened?.Invoke(Snapshot);

        // Tick while active
        while (_elapsed < _windowDuration)
        {
            _elapsed += Time.deltaTime;
            OnWindowTick?.Invoke(Snapshot);
            yield return null;
        }

        // Window expired without input
        _elapsed = _windowDuration;
        SetState(ActionWindowState.Expired);

        Debug.Log($"[{_windowLabel}] Window EXPIRED. No input was given in time.");
        OnWindowExpired?.Invoke(Snapshot);
    }

    private void StopActiveRoutine()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private void SetState(ActionWindowState newState)
    {
        _state = newState;
    }

    private void OnDisable()
    {
        CancelWindow();
    }
}
