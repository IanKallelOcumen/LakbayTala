using UnityEngine;

public class CheckpointBehavior : MonoBehaviour
{
    // This function runs when something enters the Green Box
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if it is the Player
        if (collision.CompareTag("Player"))
        {
            // 1. Find the object named "SpawnPoint" in the scene
            GameObject globalSpawnPoint = GameObject.Find("SpawnPoint");

            if (globalSpawnPoint != null)
            {
                // 2. Move the SpawnPoint to THIS checkpoint's location
                globalSpawnPoint.transform.position = transform.position;
                
                Debug.Log("SpawnPoint Moved! Game Saved.");
                
                // (Optional) Disable this checkpoint so you don't trigger it twice
                // GetComponent<Collider2D>().enabled = false; 
            }
            else
            {
                Debug.LogError("Could not find an object named 'SpawnPoint'!");
            }
        }
    }
}