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
    float aiTrust = 0.5f;   // 0..1, AI's trust in YOU
    float heat = 0f;
    Choice lastPlayer = Choice.None;
    bool allRoundsCooperated = true;

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
            dialogueManager.OnLineSpeaker += portraitHighlighter.SetActiveSpeaker;
            portraitHighlighter.SetAllActive();
            string[] tutorial = {
                "Detective: Took us long enough to get here. Let's make this quick.",
                "Detective: The first one to talk gets a deal. I'm not here to play any games, if I don't get what I want, you're both going to the slammer.",
                "Mafia: Don't let him play you. No one talks and he can't do anything to us.",
                "Detective: We'll see about that. My patience has limits.",
                "*TRUST - the thin wire between you and your mafia buddy. The lower it is, the more likely you are to be betrayed.*",
                "*HEAT - the cop's patience is wearing thin. The longer the both of you stay silent, the closer he gets to tossing you both in a cell.*",
                "*SCORE - the ticket out. Say just enough, and you can get out of this situation.*",
                "Each round, choose to *COOPERATE* (stay silent) or *BETRAY* (talk). Each choice affects your TRUST, HEAT, and SCORE.",
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

            if (lastPlayer != Choice.C)
                allRoundsCooperated = false;

            // AI decides
            var aiChoice = ai ? ai.Decide(lastPlayer, aiTrust, heat, ev) : Choice.C;

            // Resolve
            var result = resolver.Resolve(lastPlayer, aiChoice, ev, balance);
            score += result.scoreDelta;
            aiTrust = Mathf.Clamp01(aiTrust + result.aiTrustDelta);
            heat = Mathf.Clamp(heat + result.heatDelta, 0, balance.heatMax);

            yield return ShowRoundSummary(lastPlayer, aiChoice, result);

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
        bool secretOK = allRoundsCooperated && round > balance.rounds;

        if (secretOK)
        {
            // Secret sacrifice ending
            ui?.ShowEnding("TAKE_THE_FALL", score, aiTrust, heat);

            if (dialogueManager && dialogueCanvas)
            {
                string[] endLines = {
            "Detective: So, you're really gonna take the fall for him?",
            "You: ...",
            "Detective: (chuckles) Loyalty-funny thing. It's what buries men like you.",
            "You: But our family will continue to thrive. A pity that you won't get what you want.",
            "Mafia: You did right by the family. We'll handle things from here. Out there, the world keeps turning, but I'll make sure to keep your story straight.",
            "You: Make it count. Anything for the family.",
            "ENDING: TAKE THE FALL"
                };
                yield return ShowDialogueSequence(endLines);
            }
            ui?.SetChoiceButtonsActive(false);
            ui?.SetMainMenuButtonActive(true);
            yield break; // stop normal ending flow
        }

        var endingId = resolver.DecideEnding(score, aiTrust, heat, balance, endings);
        ui?.ShowEnding(endingId, score, aiTrust, heat);

        // --- NEW: Ending dialogue ---

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
                "Detective: Alright, maybe you weren't the bad guy after all.",
                "You: Took you long enough to see it.",
                "ENDING: LOYAL ESCAPE"
            };

        if (id == endings.dealId)
            return new[] {
                "Detective: Hmm, a deal's a deal, the feds will keep you breathing, new name, new city.",
                "You: Nothing personal.",
                "ENDING: WITNESS DEAL"
            };

        if (id == endings.heatBreakId)
            return new[] {
                "Detective: Looks like I'm not getting anything from you two. We'll see if some time behind bars will change that.",
                "You: Dammit!",
                "ENDING: HEAT BREAK"
            };

        // Default / sacrifice
        return new[] {
            "Detective: Looks like I won't be getting anything else from you two. We'll see if some time behind bars will change that.",
            "You: Dammit!",
            "ENDING: SACRIFICE"
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

        if (portraitHighlighter) portraitHighlighter.SetAllActive();

        // Disable canvas
        dialogueCanvas.enabled = false;
        ui.SetChoiceButtonsActive(true);
    }

    IEnumerator ShowRoundSummary(Choice playerChoice, Choice aiChoice, RoundResult result)
    {
        // Disable choice buttons during summary
        ui.SetChoiceButtonsActive(false);

        // Tell UI to show a small panel with both choices + result deltas
        ui.ShowRoundSummary(
            ChoiceLabel(playerChoice),
            ChoiceLabel(aiChoice),
            FormatResultDelta(result)
        );

        // Wait for player acknowledge (click or Space/Enter)
        yield return WaitForAcknowledge();

        // Hide the summary UI
        ui.HideRoundSummary();

        // (buttons will be re-enabled later by your normal flow)
    }

    IEnumerator WaitForAcknowledge()
    {
        // small debounce so an earlier click doesn't instantly skip
        yield return null;

        while (true)
        {
            if (Input.GetMouseButtonDown(0)) break;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) break;
            yield return null;
        }

        // optional small delay to avoid accidental double-skip
        yield return null;
    }

    // Tiny format helpers
    string ChoiceLabel(Choice c) => c == Choice.C ? "COOPERATE" :
                                   c == Choice.B ? "BETRAY" : "—";

    string FormatResultDelta(RoundResult r)
    {
        string scoreStr = TrendSymbols(r.scoreDelta, "Score");
        string heatStr = TrendSymbols(r.heatDelta, "Heat");
        string trustStr = TrendSymbols(r.aiTrustDelta, "Trust");

        // Only include ones that changed
        List<string> parts = new List<string>();
        if (!string.IsNullOrEmpty(scoreStr)) parts.Add(scoreStr);
        if (!string.IsNullOrEmpty(heatStr)) parts.Add(heatStr);
        if (!string.IsNullOrEmpty(trustStr)) parts.Add(trustStr);

        return string.Join("    ", parts);
    }
    string TrendSymbols(float delta, string label)
    {
        if (Mathf.Abs(delta) < 0.001f) return ""; // no visible change

        // Map magnitude to 1–3 symbols for effect
        int symbols = Mathf.Clamp(Mathf.CeilToInt(Mathf.Abs(delta) * 3f), 1, 3);
        string sign = delta > 0 ? "+" : "-";
        string text = new string(sign[0], symbols);

        // TMP color tags (green for +, red for -)
        string color = delta > 0 ? "#00FF66" : "#FF4444";

        return $"<color={color}>{label} {text}</color>";
    }
}
