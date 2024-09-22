using UnityEngine;

public class Buttons : MonoBehaviour
{
    public GameObject player;  // Reference to the player game object (if needed)
    public bool isTriggered = false;

    // Events to trigger
    public UnityEngine.Events.UnityEvent onButtonPressed;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !isTriggered)  // Check if the player touches the button
        {
            isTriggered = true;
            onButtonPressed.Invoke();  // Invoke the events linked to this button
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isTriggered = false;  // Reset trigger state when the player exits the button
        }
    }
}
