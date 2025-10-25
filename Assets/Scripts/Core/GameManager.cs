using System.Collections;
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


    // State
    int round, score;
    float aiTrust = 0.5f;   // 0..1, AI's trust in YOU
    float heat = 0f;
    Choice lastPlayer = Choice.None;

    IEnumerator Start()
    {
        if (!balance) { Debug.LogError("BalanceSO missing."); yield break; }

        // Basic slider ranges (if using Sliders)
        if (ui && ui.scoreSlider) ui.scoreSlider.maxValue = balance.targetScore;
        if (ui && ui.trustSlider) ui.trustSlider.maxValue = 1f;
        if (ui && ui.heatSlider) ui.heatSlider.maxValue = balance.heatMax;

        score = 0;
        aiTrust = 0.5f;     // neutral trust midpoint
        heat = 0f;          // start calm

        ui?.UpdateMeters(score, aiTrust, heat, balance);

        if (dialogueManager && dialogueCanvas)
        {
            string[] tutorial = {
                "Detective: Alright, listen up. Two of you in this room: the mobster, and the messenger.",
                "Detective: One of you talks, the other walks. Both stay quiet? You might both walk free... or rot together.",
                "Mafia: Don't let him play you. No one talks and he can't do anything to us.",
                "Detective: We'll see about that. My patience has limits.",
                "*TRUST - the thin wire between you and your mafia buddy. Cut it, and you're on your own.*",
                "*HEAT - the cop's patience is wearing thin. The longer the both of you stall, the closer he gets to tossing you both in a cell.*",
                "*SCORE - the ticket out. Say just enough, and you can get out of this situation.*",
                "Detective: Now, start talking-I don't have all day."
            };
            yield return ShowDialogueSequence(tutorial);
        }

        yield return new WaitForSeconds(0.25f);

        for (round = 1; round <= balance.rounds; round++)
        {
            ui?.SetRound(round, balance.rounds);

            // Mid/late passive heat drift
            if (round >= balance.midgameRound) heat = Mathf.Min(balance.heatMax, heat + balance.heatDriftMidLate);

            var ev = eventSystem ? eventSystem.Draw(deck, round, balance.rounds) : null;
            ui?.ShowEvent(ev);

            // --- NEW: Optional event flavor dialogue at certain rounds ---
            if (dialogueManager && dialogueCanvas && (round == 3 || round == 7))
            {
                string[] eventLines = {
                    "Detective: The lights flicker. Thin walls, thicker lies.",
                    "You: You first."
                };
                yield return ShowDialogueSequence(eventLines);
            }

            // Wait for player choice
            yield return ui.WaitForChoice(c => lastPlayer = c);

            // AI decides
            var aiChoice = ai ? ai.Decide(lastPlayer, aiTrust, heat, ev) : Choice.C;

            // Resolve
            var result = resolver.Resolve(lastPlayer, aiChoice, ev, balance);
            score += result.scoreDelta;
            aiTrust = Mathf.Clamp01(aiTrust + result.aiTrustDelta);
            heat = Mathf.Clamp(heat + result.heatDelta, 0, balance.heatMax);

            ui?.ShowOutcome(lastPlayer, aiChoice, result);
            ui?.UpdateMeters(score, aiTrust, heat, balance);

            // --- NEW: Quick reaction dialogue based on outcome ---
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
        var endingId = resolver.DecideEnding(score, aiTrust, heat, balance, endings);
        ui?.ShowEnding(endingId, score, aiTrust, heat);

        // --- NEW: Ending dialogue ---
        if (dialogueManager && dialogueCanvas)
        {
            string[] endLines = BuildEndingLines(endingId);
            if (endLines != null && endLines.Length > 0)
                yield return ShowDialogueSequence(endLines);
        }
    }

    string[] BuildReactionLines(Choice player, Choice aiChoice, float trust, float h)
    {
        if (player == Choice.C && aiChoice == Choice.C)
        {
            string[][] options = {
            new[] { "Detective: Staying quiet huh, we'll see how long you last."},
            new[] { "Mafia: You played clean. I respect that."},
            new[] { "Detective: Cooperation... unexpected."},
            new[] { "Mafia: Guess loyalty ain't dead yet."},
            new[] { "Detective: Two saints in a sinner's game."}
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
            new[] { "Mafia: I should've known you'd sell me out."}
        };
            return options[Random.Range(0, options.Length)];
        }

        if (player == Choice.C && aiChoice == Choice.B)
        {
            string[][] options = {
            new[] { "Detective: Tough luck. You kept quiet-he didn't." },
            new[] { "Mafia: Sorry, kid. Had to look out for myself." },
            new[] { "Mafia: Sorry, kid. Survival's ugly."},
            new[] { "Mafia: Nothing personal." },
            new[] { "Detective: Should've seen that coming." }
        };
            return options[Random.Range(0, options.Length)];
        }

        // Both betray
        string[][] bothBetray = {
        new[] { "Mafia: You rat! Thought we had a deal!" },
        new[] { "Mafia: Should've kept my mouth shut." },
        new[] { "Detective: Two snakes biting each other's tails." },
        new[] { "Detective: You both sang. Loud and clear." }
    };
        return bothBetray[Random.Range(0, bothBetray.Length)];
    }


    string[] BuildEndingLines(string id)
    {
        if (id == endings.loyalEscapeId)
            return new[] {
                "Detective: You kept your word.",
                "You: Honor still breathes.",
                "ENDING: LOYAL ESCAPE"
            };

        if (id == endings.dealId)
            return new[] {
                "Detective: You win. Alone.",
                "You: Someone had to.",
                "ENDING: DEAL"
            };

        if (id == endings.heatBreakId)
            return new[] {
                "Detective: Sirens. Doors. It's over.",
                "You: We ran too hot.",
                "ENDING: HEAT BREAK"
            };

        // Default / sacrifice
        return new[] {
            "Detective: They'll take one of us.",
            "You: Let it be me.",
            "ENDING: SACRIFICE HONOR"
        };
    }

    // Show a dialogue sequence using the dedicated canvas with fades
    IEnumerator ShowDialogueSequence(string[] lines)
    {
        if (!dialogueManager || !dialogueCanvas) yield break;
        ui.SetChoiceButtonsActive(false);
        // Enable canvas
        dialogueCanvas.enabled = true;

        dialogueManager.ShowDialogue(lines);

        // Wait for dialogue to finish
        while (dialogueManager.gameObject.activeSelf)
            yield return null;

        // Disable canvas
        dialogueCanvas.enabled = false;
        ui.SetChoiceButtonsActive(true);
    }
}
