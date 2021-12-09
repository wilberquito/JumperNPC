using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mathf.Abs(rb2d.velocity.x) <= Mathf.Epsilon)
        {
            animator.SetBool("WALK", false);
            return;
        }

        animator.SetBool("WALK", true);
    }

}
