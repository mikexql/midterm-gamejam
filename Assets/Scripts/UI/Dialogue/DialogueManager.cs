using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public GameObject continueIcon;          // set inactive by default

    [Header("Typing")]
    public string[] lines;
    [Tooltip("Base delay per character (seconds).")]
    public float typingSpeed = 0.03f;
    [Tooltip("Extra delay after punctuation (seconds).")]
    public float punctuationPause = 0.06f;
    [Tooltip("Characters that trigger an extra pause.")]
    public string punctuationChars = ".,!?;:";

    [Header("Audio (blip only)")]
    public AudioSource blipSource;           // optional: tiny “pip”
    public AudioClip blipClip;               // assign a very short clip
    public bool blipOnlyOnLetters = true;    // play blip only for letters/digits

    private int index;
    private bool isTyping;
    private Coroutine typingRoutine;

    private string currentTypedLine = string.Empty;
    private string currentPrefix = string.Empty;   // e.g., "Detective: "
    public bool IsPlaying => gameObject.activeSelf;
    public Action<string> OnLineSpeaker; // subscribe from GameManager/Highlighter

    void Start()
    {
        if (continueIcon) continueIcon.SetActive(false);
        dialogueText.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (lines == null || lines.Length == 0) return;

            // If currently typing, finish instantly; else advance
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        if (lines == null || lines.Length == 0)
        {
            dialogueText.text = string.Empty;
            gameObject.SetActive(false);
            return;
        }

        index = 0;
        StartTypingCurrentLine();
    }

    public void ShowDialogue(string[] newLines)
    {
        lines = newLines;
        gameObject.SetActive(true);
        dialogueText.text = string.Empty;
        StartDialogue();
    }

    string ExtractSpeaker(ref string line)
    {
        // Expect "Name: text"; returns "Name" and removes the "Name: " prefix.
        int colon = line.IndexOf(':');
        if (colon > 0)
        {
            string sp = line.Substring(0, colon).Trim();
            line = line.Substring(colon + 1).TrimStart();
            return sp;
        }
        return string.Empty;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartTypingCurrentLine();
        }
        else
        {
            dialogueText.text = string.Empty;
            if (continueIcon) continueIcon.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    void StartTypingCurrentLine()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        isTyping = true;
        if (continueIcon) continueIcon.SetActive(false);

        string raw = lines[index];
        string speaker = ExtractSpeaker(ref raw); // removes "Name: "
        currentTypedLine = raw;                   // stripped content to type
        currentPrefix = string.IsNullOrEmpty(speaker) ? "" : (speaker + ": ");
        if (!string.IsNullOrEmpty(speaker))
            OnLineSpeaker?.Invoke(speaker);             // 🔔 notify portraits

        dialogueText.text = string.Empty;
        typingRoutine = StartCoroutine(TypeLine(lines[index]));
    }

    void SkipTyping()
    {
        if (!isTyping) return;
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        isTyping = false;

        // Instantly reveal the full line
        dialogueText.text = currentPrefix + currentTypedLine;
        if (continueIcon) continueIcon.SetActive(true);
    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            dialogueText.text += c;

            // Blip only (optional)
            if (blipSource && blipClip)
            {
                bool shouldBlip = !blipOnlyOnLetters || char.IsLetterOrDigit(c);
                if (shouldBlip) blipSource.PlayOneShot(blipClip, 1f);
            }

            // Base delay + punctuation pause
            float delay = typingSpeed;
            if (punctuationChars.IndexOf(c) >= 0) delay += punctuationPause;
            yield return new WaitForSeconds(delay);

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        if (continueIcon) continueIcon.SetActive(true);
    }
}
