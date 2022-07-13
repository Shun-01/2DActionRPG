using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private Animator animator;

    [SerializeField]
    private float invisibleTime, visibleTime;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    public void PlayFeedback()
    {
        StartCoroutine("FlashCoroutine");
    }
    private IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            animator.enabled = false;

            Color spriteColor = spriteRenderer.color;
            spriteColor.a = 0;
            spriteRenderer.color = spriteColor;

            yield return new WaitForSeconds(invisibleTime);

            animator.enabled = true;

            spriteColor.a = 1;
            spriteRenderer.color = spriteColor;

            yield return new WaitForSeconds(visibleTime);

        }

        yield break;
    }
}
