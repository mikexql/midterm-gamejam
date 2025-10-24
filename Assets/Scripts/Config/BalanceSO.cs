using UnityEngine;

[CreateAssetMenu(menuName = "CodeOfSilence/Balance")]
public class BalanceSO : ScriptableObject
{
    [Header("Rounds & Targets")]
    public int rounds = 10;
    public int targetScore = 15;

    [Header("Meters")]
    public float heatMax = 10f;
    public float trustBestThreshold = 0.6f;
    public float trustDealThreshold = 0.4f;

    [Header("Base Payoffs (Score / Trust / Heat)")]
    public int   ccScore = 2;   public float ccTrust = +0.2f; public float ccHeat = +1f;
    public int   bcScore = 3;   public float bcTrust = -0.3f; public float bcHeat = -1f;
    public int   cbScore = 0;   public float cbTrust = -0.4f; public float cbHeat = +1f;
    public int   bbScore = 1;   public float bbTrust = -0.1f; public float bbHeat = -0.2f;

    [Header("Escalation")]
    public int midgameRound = 6;     // start pressure later
    public float heatDriftMidLate = 0.5f; // passive heat per round after midgame
}
