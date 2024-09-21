using UnityEngine;
using System.Collections;
public class BreakingPlatform : MonoBehaviour
{
    public float breakTime = 2f; // Time before the platform "breaks" and disables collision
    public bool toReenable = false; // If true, the platform will re-enable collision after a delay
    public float reenableTime = 5f; // Time after which the platform's collision is re-enabled
    public float brokenAlpha = 0.2f; // Alpha value when the platform is "broken"

    private Collider2D platformCollider;
    private Renderer platformRenderer;
    private Color originalColor;

    void Start()
    {
        // Get the platform's collider and renderer components
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<Renderer>();

        // Store the original color of the platform
        if (platformRenderer != null)
        {
            originalColor = platformRenderer.material.color;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Start the breaking process when the player steps on the platform
            StartCoroutine(BreakPlatform());
        }
    }

    private IEnumerator BreakPlatform()
    {
        // Wait for the specified break time
        yield return new WaitForSeconds(breakTime);

        // Reduce the platform's alpha and disable its collision
        if (platformRenderer != null)
        {
            Color brokenColor = originalColor;
            brokenColor.a = brokenAlpha;
            platformRenderer.material.color = brokenColor;
        }
        platformCollider.enabled = false;

        // Check if the platform should re-enable collision
        if (toReenable)
        {
            // Start the re-enable process
            yield return new WaitForSeconds(reenableTime);

            // Restore the platform's original color and re-enable its collision
            if (platformRenderer != null)
            {
                platformRenderer.material.color = originalColor;
            }
            platformCollider.enabled = true;
        }
    }
}
