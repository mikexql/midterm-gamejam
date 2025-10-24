using UnityEngine;

public class AIController : MonoBehaviour
{
    int consecBetray = 0;
    Choice lastPlayerPrev = Choice.None;

    public Choice Decide(Choice lastPlayer, float trust, float heat, EventSO ev)
    {
        // base probability to betray
        float p = 0.3f;
        if (lastPlayer == Choice.B) p += 0.25f;   // tit-for-tat sting
        if (trust < 0.3f)          p += 0.20f;   // low trust → self-preserve
        if (heat > 5f)             p += 0.10f;   // pressure → talk
        p += ev ? ev.aiBetrayBias : 0f;          // event bias
        if (consecBetray >= 2)     p -= 0.10f;   // remorse flicker

        p = Mathf.Clamp(p, 0.1f, 0.9f);

        bool betray = Random.value < p;
        consecBetray = betray ? consecBetray + 1 : 0;
        lastPlayerPrev = lastPlayer;

        return betray ? Choice.B : Choice.C;
    }
}
