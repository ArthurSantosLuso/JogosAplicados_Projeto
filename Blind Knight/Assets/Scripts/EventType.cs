using UnityEngine;
using UnityEngine.SceneManagement;

public class EventType : MonoBehaviour
{
    [SerializeField] private string eventId;
    public enum Event { Combat, Rest, Dialogue, Tresure, Boss }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip restClip;
    [SerializeField] private AudioClip potionClip;
    [SerializeField] private AudioClip damageUpClip;
    [SerializeField] private AudioClip combatClip;

    [SerializeField] private Event _event;
    private int rng;

    [SerializeField] private int combatSceneIdx;
    [SerializeField] private TextAsset dialogue;
    [SerializeField] private int dialogueConversationID = 1;

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
        GameManager.Instance.SavedPlayerHealth = player.GetComponent<PlayerStats>().CurrentValue;
        GameManager.Instance.SavedPlayerPosition = player.transform.position;
        audioSource.PlayOneShot(combatClip);
        SceneManager.LoadScene(combatSceneIdx);
    }

    private void RestPlayer()
    {
        GameManager.Instance.Player.GetComponent<PlayerStats>().AddHP(20);
        audioSource.PlayOneShot(restClip);
    }

    private void Dialogue()
    {
        DialogueManager.Instance.EnterDialogueMode(dialogue, dialogueConversationID);
    }

    private void GetTresure()
    {
        rng = Random.Range(0, 100);
        if (rng < 50)
        {
            GameManager.Instance.Player.GetComponent<PlayerStats>().AddPotions(1);
            audioSource.PlayOneShot(potionClip);
        }
        else
        {
            GameManager.Instance.Player.GetComponent<PlayerStats>().AddDamage(25);
            audioSource.PlayOneShot(damageUpClip);
        }
    }

    private void BossFight()
    {
        // Implement boss fight
    }
}