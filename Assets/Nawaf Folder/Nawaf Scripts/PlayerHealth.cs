using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    
    [Header("Audio")]
    public AudioClip[] hitSounds;
    public AudioClip deathSound;
    private AudioSource audioSource;
    
    [Header("Animation")]
    public Animator playerAnimator;
    public string hitAnimationTrigger = "Hit";
    public float hitAnimationDuration = 0.5f;
    
    [Header("Death Settings")]
    public GameObject fadeout;
    public string deathSceneName;
    public float deathDelay = 2f;
    
    [Header("UI (Optional)")]
    public UnityEngine.UI.Text healthText;
    
    private bool isDead = false;
    private PlayerHitEffects hitEffects;
    
    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        
        // Add AudioSource if not present
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Get player animator if not assigned
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }
        
        // Get hit effects component
        hitEffects = GetComponent<PlayerHitEffects>();
        
        UpdateHealthUI();
    }
    
    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        // Play hit animation
        PlayHitAnimation();
        
        // Play hit sound
        PlayHitSound();
        
        // Update UI
        UpdateHealthUI();
        
        Debug.Log($"Player hit! Health: {currentHealth}/{maxHealth}");
        
        // Check if player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void PlayHitSound()
    {
        if (hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[randomIndex]);
        }
    }
    
    private void PlayHitAnimation()
    {
        // Try animator first
        if (playerAnimator != null && !string.IsNullOrEmpty(hitAnimationTrigger))
        {
            playerAnimator.SetTrigger(hitAnimationTrigger);
            Debug.Log("Hit animation triggered!");
        }
        
        // Play hit effects (screen shake, flash, etc.)
        if (hitEffects != null)
        {
            hitEffects.PlayHitEffect();
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log("Player died!");
        
        // Play death sound
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        // Activate fadeout only once
        if (fadeout != null && !fadeout.activeInHierarchy)
        {
            fadeout.SetActive(true);
        }
        
        // Disable player movement
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.canMove = false;
        }
        
        // Load death scene after delay (cancel any previous invoke)
        CancelInvoke("LoadDeathScene");
        Invoke("LoadDeathScene", deathDelay);
    }
    
    private void LoadDeathScene()
    {
        if (!string.IsNullOrEmpty(deathSceneName))
        {
            SceneManager.LoadScene(deathSceneName);
        }
        else
        {
            // Restart current scene if no death scene specified
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }
    
    public void RestoreHealth(int amount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }
    
    // Public properties
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;
}
