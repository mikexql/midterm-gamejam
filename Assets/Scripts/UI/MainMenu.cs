using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Refs")]
    public AudioSource musicSource;          // assign your menu music source
    public Image blackOverlay;          // optional black Image with CanvasGroup (start alpha=0)
    public Button startButton;               // optional: to prevent double clicks

    [Header("Timings")]
    public float audioFadeDuration = 1.0f;
    public float imageFadeDuration = 0.5f;

    public void LoadSceneWithFade(string sceneName)
    {
        if (startButton) startButton.interactable = false;
        StartCoroutine(FadeAndLoad(sceneName));
    }

     IEnumerator FadeAndLoad(string sceneName)
    {
        float t = 0f;
        float startVol = musicSource ? musicSource.volume : 0f;
        Color startColor = blackOverlay.color;

        // fade both image and audio
        while (t < Mathf.Max(audioFadeDuration, imageFadeDuration))
        {
            t += Time.unscaledDeltaTime;
            float ka = audioFadeDuration  > 0 ? Mathf.Clamp01(t / audioFadeDuration)  : 1f;
            float ki = imageFadeDuration > 0 ? Mathf.Clamp01(t / imageFadeDuration) : 1f;

            if (musicSource)
                musicSource.volume = Mathf.Lerp(startVol, 0f, ka);

            if (blackOverlay)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(0f, 1f, ki);
                blackOverlay.color = c;
            }

            yield return null;
        }

        if (musicSource) { musicSource.volume = 0f; musicSource.Stop(); }

        // Load next scene
        SceneManager.LoadScene(sceneName);
    }
}