using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public static Vector3 lastCheckpointPos;
    public static bool hasCheckpoint = false; 

    IEnumerator Start()
    {
        if (hasCheckpoint)
        {
            Debug.Log("Teleporting to Checkpoint...");
            
            // Wait for Unity to finish loading
            yield return new WaitForSeconds(0.1f);

            // Force position
            transform.position = lastCheckpointPos;
            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.Sleep(); // Stop physics briefly
                yield return null;
                rb.WakeUp();
            }
        }
    }

    // LISTENER 1: For Ghosts (Checkpoints)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            lastCheckpointPos = collision.transform.position;
            hasCheckpoint = true;
            Debug.Log("Checkpoint Saved!");
        }
    }

    // LISTENER 2: For Solids (Enemies)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // We use 'collision.gameObject' here because it's a physical hit
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player Hit Enemy (Solid)!");
        }
    }
}