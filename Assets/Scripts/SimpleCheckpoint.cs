using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Platformer.Model;
using Platformer.Core;
using Platformer.Gameplay;

/// <summary>
/// Handles checkpoint triggers (delegate to SpawnPoint via CheckpointBehavior on the checkpoint object),
/// trap death, and enemy collision (stomp vs side-hit). Respawns at SpawnPoint via Simulation.
/// Uses polling every frame so death works even when physics callbacks don't fire (e.g. kinematic player).
/// </summary>
public class SimpleCheckpoint : MonoBehaviour
{
    [Header("Respawn")]
    public float respawnDelay = 1.5f;
    public float bounceForce = 8f;

    [Header("Polling (fallback so death always works)")]
    public float killY = -50f;
    public float trapCheckRadius = 0.5f;
    public float enemyCheckRadius = 0.6f;

    bool isDead;
    static readonly List<Collider2D> s_overlapList = new List<Collider2D>();

    void FixedUpdate()
    {
        if (isDead) return;

        Vector2 pos = transform.position;

        // 1) Fall off the level
        if (pos.y < killY)
        {
            RequestDeath();
            return;
        }

        // 2) Standing in a trap?
        s_overlapList.Clear();
        Physics2D.OverlapCircle(pos, trapCheckRadius, ContactFilter2D.noFilter, s_overlapList);
        foreach (var c in s_overlapList)
        {
            if (c != null && c.CompareTag("Trap"))
            {
                RequestDeath();
                return;
            }
        }

        // 3) Touching an enemy from the side? (not stomping)
        s_overlapList.Clear();
        Physics2D.OverlapCircle(pos, enemyCheckRadius, ContactFilter2D.noFilter, s_overlapList);
        var myBounds = GetComponent<Collider2D>();
        float myBottom = myBounds != null ? myBounds.bounds.min.y : pos.y - 0.5f;
        foreach (var c in s_overlapList)
        {
            if (c == null || !c.CompareTag("Enemy")) continue;
            var b = c.bounds;
            // Stomp = our bottom is above their top. So if our bottom is NOT above their top, it's a side hit -> die
            if (myBottom <= b.max.y + 0.15f)
            {
                RequestDeath();
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || isDead) return;
        if (other.CompareTag("Trap"))
        {
            DieAndRescheduleRespawn();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.gameObject == null || isDead) return;
        if (!collision.gameObject.CompareTag("Enemy")) return;
        if (collision.contactCount == 0) return;

        Vector2 hitDirection = collision.GetContact(0).normal;
        if (hitDirection.y > 0.5f)
        {
            Destroy(collision.gameObject);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        }
        else
        {
            DieAndRescheduleRespawn();
        }
    }

    /// <summary>Call from trap/enemy scripts when they detect the player. Works even if the player's collider doesn't receive the callback (e.g. kinematic).</summary>
    public void RequestDeath()
    {
        DieAndRescheduleRespawn();
    }

    void DieAndRescheduleRespawn()
    {
        if (isDead) return;
        isDead = true;
        var model = Simulation.GetModel<PlatformerModel>();
        if (model != null && model.player != null)
        {
            model.player.controlEnabled = false;
            if (model.player.collider2d != null)
                model.player.collider2d.enabled = false;
        }
        if (TryGetComponent(out Animator anim) && anim != null)
            anim.SetBool("dead", true);
        if (model != null && model.spawnPoint != null)
            Simulation.Schedule<PlayerSpawn>(respawnDelay);
        else
            Invoke(nameof(ReloadScene), respawnDelay);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnEnable()
    {
        isDead = false;
    }

    public void SetAlive() { isDead = false; }

    public void UpdateLivesDisplay() { }
}