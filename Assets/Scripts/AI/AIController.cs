using UnityEngine;

public class AIController : MonoBehaviour
{
    int consecBetray = 0;

    public Choice Decide(Choice lastPlayer, float aiTrust, float heat, EventSO ev)
    {
        float p = 0.3f;                       // base betray
        if (lastPlayer == Choice.B) p += 0.25f;  // you burned them
        if (aiTrust > 0.75f) p -= 0.15f;  // high trust = less likely to betray
        if (aiTrust < 0.30f) p += 0.20f;  // they don't trust you
        if (heat > 5f) p += 0.10f;  // outside pressure
        p += ev ? ev.aiBetrayBias : 0f;         // event sway
        if (consecBetray >= 2) p -= 0.10f;  // remorse flicker

        p = Mathf.Clamp(p, 0.1f, 0.9f);
        bool betray = Random.value < p;
        consecBetray = betray ? consecBetray + 1 : 0;
        return betray ? Choice.B : Choice.C;
    }
}
