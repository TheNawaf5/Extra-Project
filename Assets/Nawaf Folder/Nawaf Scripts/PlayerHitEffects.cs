using UnityEngine;
using System.Collections;

public class PlayerHitEffects : MonoBehaviour
{
    [Header("Visual Effects")]
    public bool enableScreenShake = true;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.3f;
    
    [Header("Screen Flash")]
    public bool enableScreenFlash = true;
    public Color flashColor = Color.red;
    public float flashDuration = 0.2f;
    public UnityEngine.UI.Image flashImage; // Drag a full-screen UI Image here
    
    [Header("Camera Effects")]
    public Camera playerCamera;
    
    private Vector3 originalCameraPosition;
    private PlayerHealth playerHealth;
    
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        if (playerCamera != null)
        {
            originalCameraPosition = playerCamera.transform.localPosition;
        }
        
        // Set up flash image
        if (flashImage != null)
        {
            Color flashCol = flashImage.color;
            flashCol.a = 0f;
            flashImage.color = flashCol;
        }
    }
    
    void Update()
    {
        // Listen for player taking damage
        if (playerHealth != null)
        {
            // You can call this from PlayerHealth.TakeDamage() instead
        }
    }
    
    public void PlayHitEffect()
    {
        if (enableScreenShake)
        {
            StartCoroutine(ScreenShake());
        }
        
        if (enableScreenFlash)
        {
            StartCoroutine(ScreenFlash());
        }
    }
    
    private IEnumerator ScreenShake()
    {
        if (playerCamera == null) yield break;
        
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            
            playerCamera.transform.localPosition = originalCameraPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Reset camera position
        playerCamera.transform.localPosition = originalCameraPosition;
    }
    
    private IEnumerator ScreenFlash()
    {
        if (flashImage == null) yield break;
        
        // Flash in
        float elapsed = 0f;
        Color startColor = flashImage.color;
        Color targetColor = flashColor;
        targetColor.a = 0.3f; // Semi-transparent
        
        while (elapsed < flashDuration / 2)
        {
            float alpha = Mathf.Lerp(0f, targetColor.a, elapsed / (flashDuration / 2));
            Color currentColor = targetColor;
            currentColor.a = alpha;
            flashImage.color = currentColor;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Flash out
        elapsed = 0f;
        while (elapsed < flashDuration / 2)
        {
            float alpha = Mathf.Lerp(targetColor.a, 0f, elapsed / (flashDuration / 2));
            Color currentColor = targetColor;
            currentColor.a = alpha;
            flashImage.color = currentColor;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Reset
        Color resetColor = flashImage.color;
        resetColor.a = 0f;
        flashImage.color = resetColor;
    }
}
