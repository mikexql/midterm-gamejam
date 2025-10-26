using UnityEngine;

public class PortraitSpeaker : MonoBehaviour
{
    public Animator animator;  // Animator controlling blink/talk frames

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    public void SetTalking(bool talking)
    {
        if (animator)
            animator.SetBool("isTalking", talking);
    }
}
