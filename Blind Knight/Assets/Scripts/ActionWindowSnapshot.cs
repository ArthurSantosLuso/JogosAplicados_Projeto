using UnityEngine;
using System;

/// <summary>
/// Lifecycle of a combat action window
/// </summary>
public enum ActionWindowState
{
    Idle,       // No window is active
    Active,     // Window is open
    Expired,    // Window closed without a successful input
    Consumed,   // Input was successful within the window
}


public readonly struct ActionWindowSnapshot
{
    public readonly ActionWindowState State;
    public readonly float ElapsedTime;
    public readonly float Duration;

    /// <summary>
    /// Normalized progress (0, 1) through the active window
    /// </summary>
    public float Progress => Duration > 0f ? Mathf.Clamp01(ElapsedTime /  Duration) : 0f;
    /// <summary>
    /// Time remaning in the active window
    /// </summary>
    public float TimeRemaining => Mathf.Max(0f, Duration - ElapsedTime);

    public bool IsActive => State == ActionWindowState.Active;

    public ActionWindowSnapshot(ActionWindowState state, float elapsed, float duration)
    {
        State = state;
        ElapsedTime = elapsed;
        Duration = duration;
    }
}
