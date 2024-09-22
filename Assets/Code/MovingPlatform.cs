using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // First point of movement
    public Transform pointB; // Second point of movement
    public float speed = 2f; // Speed of the platform movement

    private Vector3 targetPosition;
    private Vector3 previousPosition;
    private Rigidbody2D playerRigidbody;


    void Start()
    {
        // Detach points from the platform at runtime
        if (pointA != null && pointA.parent == transform)
        {
            pointA.SetParent(null);
        }

        if (pointB != null && pointB.parent == transform)
        {
            pointB.SetParent(null);
        }

        // Start moving towards point B initially
        targetPosition = pointB.position;
        previousPosition = transform.position;
    }

    void Update()
    {
        // Move the platform towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Calculate the platform's movement
        Vector3 movement = transform.position - previousPosition;

        // Apply the platform's movement to the player, if the player is on the platform
        if (playerRigidbody != null)
        {
            playerRigidbody.transform.position += movement;
        }

        // Update the previous position
        previousPosition = transform.position;

        // Check if the platform has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Switch target position to the other point
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRigidbody = null;
        }
    }

    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
