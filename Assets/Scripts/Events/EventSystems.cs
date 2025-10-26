using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public EventSO Draw(EventDeck deck, int currentRound, int totalRounds)
    {
        if (deck == null || deck.pool == null || deck.pool.Count == 0) return null;

        float t = Mathf.Clamp01((currentRound - 1) / Mathf.Max(1f, totalRounds - 1f));
        float weight = deck.byRoundWeight.Evaluate(t);

        int idx = Mathf.FloorToInt(Random.value * deck.pool.Count);

        return deck.pool[Mathf.Clamp(idx, 0, deck.pool.Count - 1)];
    }
}
