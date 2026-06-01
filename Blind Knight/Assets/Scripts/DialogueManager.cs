using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image profileImage;
    [SerializeField] private GameObject continueIcon;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<VoiceMappingEntry> voiceMappingsList;

    private Story currentStory;
    public bool IsDialoguePlaying { get; private set; }
    private Coroutine displayLineCoroutine;
    private Dictionary<string, AudioClip> voiceMap = new Dictionary<string, AudioClip>();
    private string pendingVoiceKey = null;
    private int currentConversationID = -1;

    public static DialogueManager Instance { get; private set; }

    [System.Serializable]
    public class VoiceMappingEntry
    {
        public string key;        // e.g., "1_1", "2_3"
        public AudioClip clip;
    }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        dialoguePanel?.SetActive(false);

        // Build dictionary from inspector list
        foreach (var entry in voiceMappingsList)
        {
            if (!voiceMap.ContainsKey(entry.key))
                voiceMap.Add(entry.key, entry.clip);
            else
                Debug.LogWarning($"Duplicate voice mapping key: {entry.key}");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && IsDialoguePlaying)
        {
            RequestNextLine();
        }
    }

    /// <summary>
    /// Call this to start a dialogue. conversationID is used to build voice keys.
    /// </summary>
    public void EnterDialogueMode(TextAsset inkJSON, int conversationID, Sprite whosTalkingImage = null)
    {
        currentConversationID = conversationID;

        if (profileImage != null)
        {
            if (whosTalkingImage != null)
            {
                profileImage.gameObject.SetActive(true);
                profileImage.sprite = whosTalkingImage;
            }
            else
            {
                profileImage.gameObject.SetActive(false);
            }
        }

        currentStory = new Story(inkJSON.text);
        IsDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    public void RequestNextLine()
    {
        if (displayLineCoroutine != null)
        {
            // Optional: fast-forward to end of line
            return;
        }

        if (currentStory.canContinue)
        {
            ContinueStory();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void ContinueStory()
    {
        string nextLine = currentStory.Continue();
        HandleTags(currentStory.currentTags);

        // Play voiceover if a tag was set
        if (!string.IsNullOrEmpty(pendingVoiceKey) && voiceMap.TryGetValue(pendingVoiceKey, out AudioClip clip))
        {
            if (audioSource != null && clip != null)
                audioSource.PlayOneShot(clip);
            else
                Debug.LogWarning($"Missing AudioSource or clip for key: {pendingVoiceKey}");
            pendingVoiceKey = null;
        }

        if (displayLineCoroutine != null) StopCoroutine(displayLineCoroutine);
        displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = "";
        continueIcon.SetActive(false);

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        continueIcon.SetActive(true);
        displayLineCoroutine = null;
    }

    private void ExitDialogueMode()
    {
        if (profileImage != null)
            profileImage.gameObject.SetActive(false);

        IsDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        currentConversationID = -1;
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2) continue;

            string key = splitTag[0].Trim().ToLower();
            string value = splitTag[1].Trim();

            switch (key)
            {
                case "voice":
                    if (value.Contains("_"))
                        pendingVoiceKey = value;
                    else
                        pendingVoiceKey = $"{currentConversationID}_{value}";
                    break;

                default:
                    Debug.Log($"Unhandled tag: {key}");
                    break;
            }
        }
    }
}