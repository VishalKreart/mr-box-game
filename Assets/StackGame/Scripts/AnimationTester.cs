using UnityEngine;

public class AnimationTester : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Test animations with keyboard
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("ShowDrag");
            Debug.Log("Playing HandDrag animation");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("ShowTap");
            Debug.Log("Playing HandTap animation");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetTrigger("Hide");
            Debug.Log("Playing HandHide animation");
        }
    }
} 