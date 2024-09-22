using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    [SerializeField] BasicEnemyHealth healthScr;
    public Transform player;
    [SerializeField] public Rigidbody2D rb;
    public float jumpHeight;
    public float jumpDelay;
    public bool isFlipped = false;
    public bool JumpPatternEnable;
    [SerializeField] Shower shower;
    [SerializeField] Animator animator;
    byte current_stage = 1;
    public int stage1Health, stage2Health, stage3Health;

    void Start()
    {
        player = FindObjectOfType<Player>().transform;

    }

    private void Update()
    {
        // Example: The boss could react to damage here

        if (healthScr.health < stage2Health && healthScr.health > stage3Health)
        {
            current_stage = 2;
            print("Stage 2 Enter");
        }
        else if (healthScr.health <= stage3Health)
        {
            current_stage = 3;
            print("Stage 3 Enter");
        }
        animator.SetInteger("Stage", current_stage);

        if (healthScr.health < 00)
        {
            animator.SetBool("BossDead", true);
        }
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }

        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void StartJumpPattern()
    {
        JumpPatternEnable = true;
        StartCoroutine(JumpPattern());
    }

    public void StopJumpPattern()
    {
        JumpPatternEnable = false;
    }

    public void Jump(float JumpForce)
    {
        rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
    }

    IEnumerator JumpPattern()
    {
        while (JumpPatternEnable)
        {
            yield return new WaitForSeconds(jumpDelay);
            Jump(jumpHeight);
        }
        yield return null;
    }

    public void DoRockShower()
    {
        shower.DoShower();
    }

    public void DoEnemyShower()
    {
        shower.DoEnemyShower();
    }

    // This method is called when the enemy takes damage
    public void OnDamageTaken(float damageAmount, Vector2 attackerPosition)
    {
        animator.SetInteger("Stage", current_stage);

    }

    public void BosaDead()
    {
        StartCoroutine(DeadBoss());
    }

    IEnumerator DeadBoss()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(3);
    }
}
