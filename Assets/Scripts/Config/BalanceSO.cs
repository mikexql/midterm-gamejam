using UnityEngine;

[CreateAssetMenu(menuName = "CodeOfSilence/GameBalance")]
public class BalanceSO : ScriptableObject
{
    [Header("Rounds & Targets")]
    public int roundsPlayed = 10;
    public int targetScoreToEscape = 15;

    [Header("Meters")]
    public float heatMax = 10f;
    [Header("Ending Config")]
    [Range(0f, 1f)]public float trustBestThreshold = 0.6f;
    [Tooltip("Numbers affect how easy it is to reach certain endings")]

    [Header("Base Payoffs (Score / Trust / Heat)")]
    public int ccScore = 2; public float ccAiTrust = +0.20f; public float ccHeat = +1f;
    public int bcScore = 3; public float bcAiTrust = -0.40f; public float bcHeat = -1f;
    public int cbScore = 0; public float cbAiTrust = +0.10f; public float cbHeat = +1f;
    public int bbScore = 1; public float bbAiTrust = -0.20f; public float bbHeat = -0.2f;

    [Header("AI Config")]
    [Range(0f, 1f)] public float aiBaseBetray = 0.30f;  // baseline P(betray)
    [Range(0f, 1f)] public float aiTrustWeight = 0.30f; // trust impact strength
    [Range(0f, 1f)] public float aiHeatWeight = 0.20f; // heat impact strength
    [Range(0f, 1f)] public float heatBetrayStartingPoint = 0.5f; // starting point for heat effect

}
