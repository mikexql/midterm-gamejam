using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Data")]
    public BalanceSO balance;
    public EndingRulesSO endings;
    public EventDeck deck;

    [Header("Systems")]
    public Resolver resolver;
    public EventSystem eventSystem;
    public AIController ai;
    public UIController ui;
    public DialogueManager dialogueManager;
    public Canvas dialogueCanvas;

    public PortraitHighlighter portraitHighlighter;


    // State
    int round, score;
    float aiTrust = 0.5f;
    float heat = 0f;
    Choice lastPlayer = Choice.None;
    bool allRoundsCooperated = true;

    IEnumerator Start()
    {
        if (!balance) { Debug.LogError("BalanceSO missing."); yield break; }

        // Basic slider ranges (if using Sliders)
        if (ui && ui.scoreSlider) ui.scoreSlider.maxValue = balance.targetScoreToEscape;
        if (ui && ui.trustSlider) ui.trustSlider.maxValue = 1f;
        if (ui && ui.heatSlider) ui.heatSlider.maxValue = balance.heatMax;

        score = 0;
        aiTrust = 0.5f;     // neutral trust midpoint
        heat = 0f;          // start calm

        ui?.UpdateMeters(score, aiTrust, heat, balance);

        if (dialogueManager && dialogueCanvas)
        {
            dialogueManager.OnLineSpeaker += portraitHighlighter.SetActiveSpeaker;
            portraitHighlighter.SetAllActive();
            string[] tutorial = {
                "Detective: How does it feel to be finally caught? It took us a while, but here you are, sharing one cold room.",
                "Detective: Here's this is going to work. Right now, you're both looking at 20 years each.",
                "Detective: No deals, no mercy - unless one of you talks. Give me enough dirt on your buddy here and you might be able to walk.",
                "Mafia: Don't let him play you. No one talks and he can't do anything to us. Remember the family.",
                "Detective: We'll see about that. I'm sure one of you will crack sooner or later.",
                "*Each round, choose to *COOPERATE* (stay silent) or *BETRAY* (talk). Each choice affects your TRUST and SCORE.*",
                "*TRUST - That's how much your partner still believes in you. If its higher, he is more likely to cooperate. Lose it, and he'll start talking first.*",
                //"*HEAT - the detective's patience is wearing thin. The longer the both of you stay silent, the closer he gets to just ending this and tossing the both of you in solitary.*",
                "*SCORE - That's your shot at freedom. Max it out, and you walk out of here clean.*",
                "*Think carefully each round. Betray too much, you'll lose trust. Cooperate too much, you'll never earn your freedom.*",
                "Detective: Keep your silence and you're both going to rot in here. You can't win just by playing nice."
            };
            yield return ShowDialogueSequence(tutorial);
        }

        yield return new WaitForSeconds(0.25f);

        for (round = 1; round <= balance.roundsPlayed; round++)
        {
            ui?.SetRound(round, balance.roundsPlayed);

            // Draw event
            var ev = eventSystem ? eventSystem.Draw(deck, round, balance.roundsPlayed) : null;
            ui?.ShowEvent(ev);

            // Player decides
            yield return ui.WaitForChoice(c => lastPlayer = c);
            if (lastPlayer != Choice.C)
                allRoundsCooperated = false;

            // AI decides
            var aiChoice = ai ? ai.Decide(lastPlayer, aiTrust, heat, ev) : Choice.C;

            var result = resolver.Resolve(lastPlayer, aiChoice, ev, balance);
            score += result.scoreDelta;
            aiTrust = Mathf.Clamp01(aiTrust + result.aiTrustDelta);
            heat = Mathf.Clamp(heat + result.heatDelta, 0, balance.heatMax);

            yield return ShowRoundSummary(lastPlayer, aiChoice, result);
            ui?.ShowOutcome(lastPlayer, aiChoice, result);
            ui?.UpdateMeters(score, aiTrust, heat, balance);

            if (dialogueManager && dialogueCanvas)
            {
                string[] reaction = BuildReactionLines(lastPlayer, aiChoice, aiTrust, heat);
                if (reaction != null && reaction.Length > 0)
                    yield return ShowDialogueSequence(reaction);
            }

            if (heat >= balance.heatMax) break;
            yield return new WaitForSeconds(0.35f);
        }

        // Ending
        bool secretOK = allRoundsCooperated && round > balance.roundsPlayed;

        if (secretOK)
        {
            ui?.ShowEnding("TAKE_THE_FALL", score, aiTrust, heat);
            if (dialogueManager && dialogueCanvas)
            {
                string[] endLines = {
            "Detective: So, you're really not going to say anything huh?",
            "You: I'll never talk. In fact, this entire thing is on me, take me in and let my partner go.",
            "Detective: (chuckles) Loyalty - funny thing. It's what what gets men like you buried and forgotten.",
            "You: Maybe. But our family will continue to thrive. A pity that you won't get what you want.",
            "Mafia: You didn't have to do this, brother... but you did. I'll handle everything on the outside for you.",
            "Mafia: The streets will remember who kept the code. I'll make sure of it.",
            "You: Make it count. Anything for our family.",
            "TRUE ENDING: TAKE THE FALL"
                };
                yield return ShowDialogueSequence(endLines);
            }
            ui?.SetChoiceButtonsActive(false);
            ui?.SetMainMenuButtonActive(true);
            yield break;
        }

        var endingId = resolver.DecideEnding(score, aiTrust, heat, balance, endings);
        ui?.ShowEnding(endingId, score, aiTrust, heat);

        if (dialogueManager && dialogueCanvas)
        {
            string[] endLines = BuildEndingLines(endingId);
            if (endLines != null && endLines.Length > 0)
                yield return ShowDialogueSequence(endLines);
        }
        ui?.SetChoiceButtonsActive(false);
        ui?.SetMainMenuButtonActive(true);
    }

    string[] BuildReactionLines(Choice player, Choice aiChoice, float trust, float h)
    {
        if (player == Choice.C && aiChoice == Choice.C)
        {
            string[][] options = {
            new[] { "Detective: Staying quiet huh, we'll see how long you last."},
            new[] { "Mafia: You played clean. I respect that."},
            new[] { "Detective: One of you is bound to crack sooner or later."},
            new[] { "Mafia: Guess loyalty ain't dead yet."},
            new[] { "Detective: We'll see how long both of you can keep up that act."}
        };
            return options[Random.Range(0, options.Length)];
        }

        if (player == Choice.B && aiChoice == Choice.C)
        {
            string[][] options = {
            new[] { "Mafia: You sold me out?!"},
            new[] { "Detective: Smart move. You talked."},
            new[] { "Mafia: You just made an enemy." },
            new[] { "Detective: Cold move, kid."},
            new[] { "Mafia: I should've known you'd sell me out." },
            new[] { "Detective: Oh? How interesting. Tell me more." }
        };
            return options[Random.Range(0, options.Length)];
        }

        if (player == Choice.C && aiChoice == Choice.B)
        {
            string[][] options = {
            new[] { "Detective: Tough luck. Your buddy here sold you out." },
            new[] { "Mafia: Sorry, I have to look out for myself, our family needs me." },
            new[] { "Mafia: Sorry, kid. It is what it is."},
            new[] { "Mafia: Nothing personal, you understand that right?" },
            new[] { "Detective: Should've seen that coming." }
        };
            return options[Random.Range(0, options.Length)];
        }

        string[][] bothBetray = {
        new[] { "Mafia: You rat! I thought we had a deal!" },
        new[] { "Mafia: Should've kept my mouth shut." },
        new[] { "Detective: Two snakes biting each other's tails." },
        new[] { "Detective: Now that's more like it." },
        new[] { "Detective: Hahaha, all that talk about family but you guys are always so quick to turn on each other." }
    };
        return bothBetray[Random.Range(0, bothBetray.Length)];
    }


    string[] BuildEndingLines(string id)
    {
        // if (id == endings.loyalEscapeId)
        //     return new[] {
        //         "Detective: Alright, maybe you weren't the bad guy after all.",
        //         "You: Took you long enough to see it.",
        //         "ENDING: LOYAL ESCAPE"
        //     };

        if (id == endings.dealId)
            return new[] {
                "Detective: Hmm, a deal's a deal, as thanks for all the evidence, I will see to it that you walk free.",
                "Detective: The feds will keep you breathing, new name, new city. You will be protected under witness protection as long as you stay clean.",
                "You: Better me than him, I'm done with this life.",
                "ENDING: WITNESS DEAL"
            };

        if (id == endings.heatBreakId)
            return new[] {
                "Detective: Looks like I'm not getting anywhere with you two.",
                "Detective: We'll see if some extra time rotting behind bars will soften your resolve.",
                "You: Dammit!",
                "ENDING: HEAT BREAK"
            };

        return new[] {
            "Detective: That's not going to be enough to get you out of here.",
            "Detective: I want to hear more, but I guess that's all you're willing to give me. It's back to the cells for you two.",
            "You: Dammit!",
            "BAD ENDING: STILL IMPRISONED"
        };
    }

    IEnumerator ShowDialogueSequence(string[] lines)
    {
        if (!dialogueManager || !dialogueCanvas) yield break;
        ui.SetChoiceButtonsActive(false);

        dialogueCanvas.enabled = true;

        dialogueManager.ShowDialogue(lines);

        while (dialogueManager.gameObject.activeSelf)
            yield return null;

        if (portraitHighlighter) portraitHighlighter.SetAllActive();

        dialogueCanvas.enabled = false;
        ui.SetChoiceButtonsActive(true);
    }

    IEnumerator ShowRoundSummary(Choice playerChoice, Choice aiChoice, RoundResult result)
    {
        ui.SetChoiceButtonsActive(false);

        ui.ShowRoundSummary(
            ChoiceLabel(playerChoice),
            ChoiceLabel(aiChoice),
            FormatResultDelta(result)
        );

        yield return WaitForAcknowledge();

        ui.HideRoundSummary();
    }

    IEnumerator WaitForAcknowledge()
    {
        yield return null;

        while (true)
        {
            if (Input.GetMouseButtonDown(0)) break;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) break;
            yield return null;
        }
        yield return null;
    }

    string ChoiceLabel(Choice c) => c == Choice.C ? "COOPERATE" :
                                   c == Choice.B ? "BETRAY" : "—";

    string FormatResultDelta(RoundResult r)
    {
        string scoreStr = TrendSymbols(r.scoreDelta, "Score");
        string heatStr = TrendSymbols(r.heatDelta, "Heat");
        string trustStr = TrendSymbols(r.aiTrustDelta, "Trust");
        List<string> parts = new List<string>();
        if (!string.IsNullOrEmpty(scoreStr)) parts.Add(scoreStr);
        if (!string.IsNullOrEmpty(heatStr)) parts.Add(heatStr);
        if (!string.IsNullOrEmpty(trustStr)) parts.Add(trustStr);
        return string.Join("    ", parts);
    }
    string TrendSymbols(float delta, string label)
    {
        if (Mathf.Abs(delta) < 0.001f) return "";

        int symbols = Mathf.Clamp(Mathf.CeilToInt(Mathf.Abs(delta)), 1, 3);
        string sign = delta > 0 ? "+" : "-";
        string text = new string(sign[0], symbols);
        string color = delta > 0 ? "#00FF66" : "#FF4444";
        return $"<color={color}>{label} {text}</color>";
    }
}
