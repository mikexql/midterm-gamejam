using UnityEngine;

public class Resolver : MonoBehaviour
{
    public RoundResult Resolve(Choice p, Choice a, EventSO e, BalanceSO b)
    {
        var rr = new RoundResult();
        int coopBonus = e ? e.bonusScoreCoop : 0;
        int betrayBonus = e ? e.bonusScoreBetray : 0;
        float heatMult = e ? e.heatMultiplier : 1f;
        float trustAdj = e ? e.trustDelta : 0f; // events can sway AI trust too

        if (p == Choice.C && a == Choice.C)
        {
            rr.scoreDelta = b.ccScore + coopBonus;
            rr.aiTrustDelta = b.ccAiTrust + trustAdj;
            rr.heatDelta = b.ccHeat * heatMult;
        }
        if (p == Choice.B && a == Choice.C)
        {
            rr.scoreDelta = b.bcScore + betrayBonus;
            rr.aiTrustDelta = b.bcAiTrust + trustAdj;
            rr.heatDelta = b.bcHeat * heatMult;
        }
        if (p == Choice.C && a == Choice.B)
        {
            rr.scoreDelta = b.cbScore + coopBonus;
            rr.aiTrustDelta = b.cbAiTrust + trustAdj;
            rr.heatDelta = b.cbHeat * heatMult;
        }
        if (p == Choice.B && a == Choice.B)
        {
            rr.scoreDelta = b.bbScore + betrayBonus;
            rr.aiTrustDelta = b.bbAiTrust + trustAdj;
            rr.heatDelta = b.bbHeat * heatMult;
        }
        return rr;
    }

    public string DecideEnding(int score, float trust, float heat, BalanceSO b, EndingRulesSO e)
    {
        if (heat >= b.heatMax) return e.heatBreakId;
        if (score >= b.targetScore && trust >= b.trustBestThreshold) return e.loyalEscapeId;
        if (score >= b.targetScore && trust <= b.trustDealThreshold) return e.dealId;
        return e.doomId;
    }
}
