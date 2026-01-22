using UnityEngine;

public class HandController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Start with hand hidden
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void ShowHand()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    public void HideHand()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void PlayDragAnimation()
    {
        ShowHand();
        animator.SetTrigger("ShowDrag");
    }

    public void PlayTapAnimation()
    {
        ShowHand();
        //animator.SetTrigger("ShowTap");
    }

    public void PlayHideAnimation()
    {
        animator.SetTrigger("Hide");
        // Hide the sprite after animation completes
        Invoke("HideHand", 0.5f);
    }
} 