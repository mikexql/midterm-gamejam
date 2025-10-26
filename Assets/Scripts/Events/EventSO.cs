using UnityEngine;

public enum EventTag { None, Brotherhood, Lawyer, FakeEvidence, PowerOutage}

[CreateAssetMenu(menuName = "CodeOfSilence/Event")]
public class EventSO : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public EventTag tag = EventTag.None;

    [Header("Round Modifiers")]
    public int bonusScoreCoop = 0;
    public int bonusScoreBetray = 0;
    public float trustDelta = 0f;
    public float heatMultiplier = 1f;
    public float aiBetrayBias = 0f;
    public bool delayReveal = false;
}
