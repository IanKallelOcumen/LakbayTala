using UnityEngine;
using Platformer.Mechanics; // KinematicObject.Bounce

/// <summary>
/// Put on objects with tag "Enemy". When the player collides:
/// - Stomp (hit from above): enemy is destroyed, player bounces.
/// - Side hit: player dies.
/// GameController adds this at runtime to all "Enemy" objects if missing.
/// </summary>
public class EnemyDamagePlayer : MonoBehaviour
{
    public float stompBounceForce = 8f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.gameObject == null) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (collision.contactCount == 0) return;

        Vector2 normal = collision.GetContact(0).normal;
        // Normal points away from this enemy. If player hit from above (stomp), normal points up from enemy's top.
        if (normal.y > 0.5f)
        {
            // Stomp: bounce player and destroy self
            var ko = collision.gameObject.GetComponent<KinematicObject>();
            if (ko != null)
                ko.Bounce(stompBounceForce);
            Destroy(gameObject);
        }
        else
        {
            // Side hit: kill player
            var sc = collision.gameObject.GetComponent<SimpleCheckpoint>();
            if (sc != null)
                sc.RequestDeath();
        }
    }
}
