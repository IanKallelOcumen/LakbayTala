using UnityEngine;

/// <summary>
/// Put on objects with tag "Trap" and a trigger Collider2D.
/// When the player enters the trigger, the player dies (works even if the player is kinematic).
/// GameController adds this at runtime to all "Trap" objects if missing.
/// </summary>
public class DeathTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (!other.CompareTag("Player")) return;
        var sc = other.GetComponent<SimpleCheckpoint>();
        if (sc != null)
            sc.RequestDeath();
    }
}
