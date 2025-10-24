using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CodeOfSilence/EventDeck")]
public class EventDeck : ScriptableObject
{
    public List<EventSO> pool;
    [Tooltip("0..1 (X=round/rounds, Y=weight scale)")] public AnimationCurve byRoundWeight =
        AnimationCurve.Linear(0, 0.5f, 1, 1f);
}
