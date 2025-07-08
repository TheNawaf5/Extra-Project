using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{
    [Header("Death Settings")]
    public string nextSceneName; // Name of the next scene to load
    public float delay = 0.5f; // Delay in seconds before loading the next scene
    public GameObject fadeout;
    
    [Header("Death Type")]
    public bool instantKill = true; // If false, will deal damage instead
    public int damageAmount = 1; // Damage to deal if not instant kill
    
    private bool playerInsideTrigger = false;
    private PlayerHealth playerHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = true;
            
            // Get player health component
            if (playerHealth == null)
            {
                playerHealth = other.GetComponent<PlayerHealth>();
            }
            
            if (instantKill)
            {
                // Instant kill (like falling into pit, lava, etc.)
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    // Force kill player by dealing maximum damage
                    playerHealth.TakeDamage(playerHealth.MaxHealth);
                }
                else if (playerHealth == null)
                {
                    // Fallback if no PlayerHealth component
                    fadeout.SetActive(true);
                    Invoke("LoadNextScene", delay);
                }
            }
            else
            {
                // Deal damage instead of instant kill
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damageAmount);
                    Debug.Log($"Player took {damageAmount} damage from hazard!");
                }
            }
        }
    }


    private void LoadNextScene()
    {
        if (playerInsideTrigger)
        {
            // Load the next scene by name
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
