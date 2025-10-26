using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortraitHighlighter : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public string speaker;
        public List<SpriteRenderer> sprites;
    }

    public List<Slot> slots = new();
    public Color activeTint = Color.white;
    public Color inactiveTint = new Color(0.35f, 0.35f, 0.45f);
    public float fadeTime = 0.12f;

    Dictionary<string, List<SpriteRenderer>> map;

    void Awake()
    {
        map = new Dictionary<string, List<SpriteRenderer>>();
        foreach (var s in slots)
        {
            if (string.IsNullOrEmpty(s.speaker) || s.sprites == null) continue;
            if (!map.ContainsKey(s.speaker)) map[s.speaker] = new List<SpriteRenderer>();
            foreach (var sr in s.sprites)
            {
                if (!sr) continue;
                map[s.speaker].Add(sr);
                sr.color = inactiveTint;
            }
        }
    }

    public void SetActiveSpeaker(string speaker)
    {
        // Dim all groups
        foreach (var kv in map)
            StartCoroutine(FadeMany(kv.Value, inactiveTint, fadeTime));

        // Light up the chosen group
        if (!string.IsNullOrEmpty(speaker) && map.TryGetValue(speaker, out var group))
            StartCoroutine(FadeMany(group, activeTint, fadeTime));
    }

    public void SetAllActive()
    {
        foreach (var kv in map)
            StartCoroutine(FadeMany(kv.Value, activeTint, fadeTime));
    }

    IEnumerator FadeMany(List<SpriteRenderer> grp, Color target, float t)
    {
        var froms = new Color[grp.Count];
        for (int i = 0; i < grp.Count; i++) froms[i] = grp[i].color;

        float time = 0f;
        while (time < t)
        {
            time += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(time / t);
            for (int i = 0; i < grp.Count; i++)
                if (grp[i]) grp[i].color = Color.Lerp(froms[i], target, k);
            yield return null;
        }
        for (int i = 0; i < grp.Count; i++)
            if (grp[i]) grp[i].color = target;
    }
}
