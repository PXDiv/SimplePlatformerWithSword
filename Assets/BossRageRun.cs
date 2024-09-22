using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRageRun : StateMachineBehaviour
{
    public float speed = 10f;
    private Transform player;
    private Rigidbody2D rb;
    //public float attackRange;

    private Boss boss;
    public float edgeCheckDistance = 1f; // Distance to check for the edge
    private int dir = 1; // 1 for right, -1 for left

    int SlamTimes;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = FindObjectOfType<Player>().transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        rb.isKinematic = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = new Vector2(speed * dir, 0);

        // Check for edge using a raycast
        Vector2 rayOrigin = rb.position + Vector2.right * dir * edgeCheckDistance;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 1f);

        // If there's no ground detected, reverse direction
        if (hit.collider == null)
        {
            animator.SetInteger("RageTimes", animator.GetInteger("RageTimes") + 1);
            dir *= -1; // Change direction
            boss.LookAtPlayer(); // Flip the boss's sprite (assuming you have a Flip method)
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = Vector2.zero; // Stop movement on exit
        animator.SetInteger("RageTimes", 0);
        animator.SetInteger("TotalRageTimes", animator.GetInteger("TotalRageTimes") + 1);

    }
}
