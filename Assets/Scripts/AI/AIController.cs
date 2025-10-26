using UnityEngine;

public class AIController : MonoBehaviour
{
    public BalanceSO balance;

    // Player choice is currently ignored but could be used next time
    public Choice Decide(Choice _ignored, float aiTrust, float heat, EventSO ev)
        => Decide(aiTrust, heat, ev);

    public Choice Decide(float aiTrust, float heat, EventSO ev)
    {
        if (!balance)
        {
            Debug.LogWarning("AIController: BalanceSO missing. Using defaults.");
            return (Random.value < 0.5f) ? Choice.B : Choice.C;
        }

        float p = balance.aiBaseBetray;

        // TRUST (0.5 = neutral), higher trust lowers betrayal
        float t = Mathf.Clamp((aiTrust - 0.5f) / 0.5f, -1f, 1f);
        p += -balance.aiTrustWeight * t;

        // HEAT, higher heat also raises betrayal of AI
        float neutralFrac = balance.heatBetrayStartingPoint;
        float heatFrac = (balance.heatMax > 0f) ? Mathf.Clamp01(heat / balance.heatMax) : 0.5f;
        float h = Mathf.Clamp((heatFrac - neutralFrac) / 0.5f, -1f, 1f);
        p += balance.aiHeatWeight * h;

        // Event bias
        if (ev) p += ev.aiBetrayBias;

        p = Mathf.Clamp(p, 0.1f, 0.9f);

        return (Random.value < p) ? Choice.B : Choice.C;
    }
}

