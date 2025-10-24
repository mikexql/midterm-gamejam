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
        yield return new WaitForSeconds(0.25f);

        for (round = 1; round <= balance.rounds; round++)
        {
            ui?.SetRound(round, balance.rounds);

            // Mid/late passive heat drift
            if (round >= balance.midgameRound) heat = Mathf.Min(balance.heatMax, heat + balance.heatDriftMidLate);

            var ev = eventSystem ? eventSystem.Draw(deck, round, balance.rounds) : null;
            ui?.ShowEvent(ev);

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

            if (heat >= balance.heatMax) break;
            yield return new WaitForSeconds(0.35f);
        }

        // Ending
        var endingId = resolver.DecideEnding(score, aiTrust, heat, balance, endings);
        ui?.ShowEnding(endingId, score, aiTrust, heat);
    }
}
