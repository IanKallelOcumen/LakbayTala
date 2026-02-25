using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Platformer.Mechanics;

public class SimpleCheckpoint : MonoBehaviour
{
    // ===========================================
    // STATIC VARIABLES (Functional Checkpoint Memory)
    // ===========================================
    public static Vector3 savedPosition;
    public static bool hasCheckpoint = false;
    
    [Header("Manager Settings")]
    public float respawnDelay = 1.5f; 
    public float bounceForce = 8f; 
    
    private bool isStompInvulnerable = false; 

    // We remove all complex Coroutine references, keeping the script clean.
    
    IEnumerator Start()
    {
        // Teleport logic (Original stable version)
        if (hasCheckpoint)
        {
            yield return new WaitForSeconds(0.1f); 
            transform.position = savedPosition;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
            }
        }
    }

    // ------------------------------------------
    // 1. COLLISION & TRIGGER DETECTION
    // ------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Checkpoint Save Logic
        if (other.CompareTag("Checkpoint"))
        {
            savedPosition = other.transform.position;
            hasCheckpoint = true;
            Debug.Log("Progress Saved!");
        }
        else if (other.CompareTag("Trap")) 
        {
            // Simple Death Logic (Reloads the scene)
            StartCoroutine(SimpleDieAndReload());
        }
    }

    // ------------------------------------------
    // 2. STOMP / DEATH LOGIC (The last working code logic)
    // ------------------------------------------
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitDirection = collision.contacts[0].normal;

            if (hitDirection.y > 0.5f) // Stomp Logic
            {
                // This is the code that caused the constant jump bug. We remove it for stability.
                // However, for the revert, we return to the state where the code just tried to destroy.
                Destroy(collision.gameObject);
                
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce); 
                }
            }
            else
            {
                // Side Hit Logic: Player dies
                StartCoroutine(SimpleDieAndReload());
            }
        }
    }
    
    // ------------------------------------------
    // 3. SUPPORT & ROUTINES
    // ------------------------------------------
    
    // The essential die function: just reloads the scene
    IEnumerator SimpleDieAndReload()
    {
        // This is the simplest death, which assumes PlayerController is managing control
        Debug.Log("Player Died! Reloading scene...");
        yield return new WaitForSeconds(respawnDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // We keep these methods empty to prevent compile errors in other scripts that might still call them
    public void UpdateLivesDisplay() {}
}