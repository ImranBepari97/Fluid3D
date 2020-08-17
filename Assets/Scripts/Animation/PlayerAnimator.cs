using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    Animator animator;
    GlobalPlayerController gpc;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        gpc = transform.parent.GetComponent<GlobalPlayerController>();
        animator = GetComponent<Animator>();
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isGrounded", gpc.isGrounded);

        Vector3 horVel = rb.velocity;
        horVel.y = 0;

        animator.SetFloat("horizontalVelocity", horVel.magnitude);
        animator.SetFloat("verticalVelocity", rb.velocity.y);
    }
}
