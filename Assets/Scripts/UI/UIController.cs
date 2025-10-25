using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Buttons")]
    public Button btnCooperate;
    public Button btnBetray;
    public GameObject btnGroup;
    public GameObject mainMenuButton;

    [Header("Meters")]
    public Slider scoreSlider;    // or TMP text if you prefer numbers
    public Slider trustSlider;    // 0..1
    public Slider heatSlider;     // 0..heatMax
    public TextMeshProUGUI roundLabel;
    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI logText;
    public GameObject roundSummaryRoot;                 // panel root

    [Header("Choices")]
    public TMPro.TextMeshProUGUI playerChoiceText;
    public TMPro.TextMeshProUGUI aiChoiceText;
    public TMPro.TextMeshProUGUI outcomeText;


    Action<Choice> _choiceCallback;

    void Awake()
    {
        if (btnCooperate) btnCooperate.onClick.AddListener(() => Choose(Choice.C));
        if (btnBetray) btnBetray.onClick.AddListener(() => Choose(Choice.B));
    }

    void Choose(Choice c)
    {
        if (_choiceCallback != null)
        {
            var cb = _choiceCallback; _choiceCallback = null;
            cb.Invoke(c);
        }
    }

    public IEnumerator WaitForChoice(Action<Choice> onChosen)
    {
        _choiceCallback = onChosen;
        btnCooperate.interactable = true;
        btnBetray.interactable = true;

        while (_choiceCallback != null) yield return null;

        btnCooperate.interactable = false;
        btnBetray.interactable = false;
    }

    public void SetRound(int round, int total)
    {
        if (roundLabel) roundLabel.text = $"Round {round}/{total}";
    }

    public void ShowEvent(EventSO ev)
    {
        if (eventTitle) eventTitle.text = ev ? ev.title : "";
    }

    public void UpdateMeters(int score, float aiTrust, float heat, BalanceSO b)
    {
        if (scoreSlider) scoreSlider.value = score;
        if (trustSlider) trustSlider.value = Mathf.Clamp01(aiTrust);
        if (heatSlider) heatSlider.value = Mathf.Clamp(heat, 0, b.heatMax);
    }

    public void AppendLog(string s)
    {
        if (!logText) return;
        logText.text += (logText.text.Length > 0 ? "\n" : "") + s;
    }

    public void ShowOutcome(Choice p, Choice a, RoundResult r)
    {
        AppendLog($"You: {p} | Partner: {a}  =>  +{r.scoreDelta} / AItrust {r.aiTrustDelta:+0.0;-0.0} / Heat {r.heatDelta:+0.0;-0.0}");
    }


    public void ShowEnding(string endingId, int score, float trust, float heat)
    {
        AppendLog($"\n== END: {endingId} ==\nScore {score}  Trust {trust:0.00}  Heat {heat:0.0}");
    }

    public void SetChoiceButtonsActive(bool state)
    {
        btnGroup.SetActive(state);
    }

    public void SetMainMenuButtonActive(bool show)
    {
        if (mainMenuButton)
            mainMenuButton.SetActive(show);
    }

    public void ShowRoundSummary(string playerChoice, string aiChoice, string outcome)
    {
        if (roundSummaryRoot) roundSummaryRoot.SetActive(true);
        if (playerChoiceText) playerChoiceText.text = $"YOU: {playerChoice}";
        if (aiChoiceText) aiChoiceText.text = $"PARTNER: {aiChoice}";
        if (outcomeText) outcomeText.text = outcome;
    }

    public void HideRoundSummary()
    {
        if (roundSummaryRoot) roundSummaryRoot.SetActive(false);
    }
}
