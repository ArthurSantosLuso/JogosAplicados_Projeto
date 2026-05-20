using Unity.VectorGraphics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventType : MonoBehaviour
{
    [SerializeField] private string eventId;
    public enum Event { Combat, Rest, Dialogue, Tresure, Boss}

    [SerializeField] private Event _event;

    [SerializeField] private int combatSceneIdx;
    [SerializeField] private TextAsset dialogue;

    [HideInInspector]
    public bool hasHappened = false;

    public void StartEvent()
    {
        hasHappened = true;
        GameManager.Instance.ClearedEventsIds.Add(eventId);

        if (_event == Event.Combat) StartCombat();
        if (_event == Event.Rest) RestPlayer();
        if (_event == Event.Dialogue) Dialogue();
        if (_event == Event.Tresure) GetTresure();
        if (_event == Event.Boss) BossFight();
            
    }

    private void Start()
    {
        if (GameManager.Instance.ClearedEventsIds.Contains(eventId))
        {
            hasHappened = true;
        }
    }

    private void StartCombat()
    {
        var player = GameManager.Instance.Player;
        GameManager.Instance.SavedPlayerHealth = player.GetComponent<PlayerHealth>().CurrentValue;
        GameManager.Instance.SavedPlayerPosition = player.transform.position;
        
        SceneManager.LoadScene(combatSceneIdx);
    }

    private void RestPlayer()
    {
        
    }

    private void Dialogue()
    {
        DialogueManager.Instance.EnterDialogueMode(dialogue);
    }

    private void GetTresure()
    {

    }

    private void BossFight()
    {

    }
}
