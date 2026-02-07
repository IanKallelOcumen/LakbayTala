using UnityEngine;

public class SimpleEnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float patrolDistance = 2f;
    
    private float startX;
    private int direction = 1;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        // Move enemy
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);

        // Check limits
        if (transform.position.x > startX + patrolDistance)
        {
            direction = -1; // Go Left
            // Flip sprite if needed
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (transform.position.x < startX - patrolDistance)
        {
            direction = 1; // Go Right
             GetComponent<SpriteRenderer>().flipX = false;
        }
    }
}
