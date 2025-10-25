using UnityEngine;

public class RandomBlink : MonoBehaviour
{
    public Animator animator;
    [Tooltip("Min seconds between blinks")]
    public float minDelay = 2f;
    [Tooltip("Max seconds between blinks")]
    public float maxDelay = 10f;

    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        StartCoroutine(BlinkLoop());
    }

    System.Collections.IEnumerator BlinkLoop()
    {
        while (true)
        {
            float wait = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(wait);
            animator.SetTrigger("blink");
        }
    }
}

