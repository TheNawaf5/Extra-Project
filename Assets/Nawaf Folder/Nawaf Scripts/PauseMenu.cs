using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuUI;
    public GameObject pausePanel;
    
    [Header("Player Reference")]
    public PlayerController playerController;
    
    [Header("UI Animation Fix")]
    public Canvas pauseCanvas; // Reference to the pause menu canvas
    
    private bool isPaused = false;
    
    void Start()
    {
        // Make sure pause menu is hidden at start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        
        // Set up canvas to use unscaled time for UI animations
        SetupCanvasForUnscaledTime();
    }

    void Update()
    {
        // Check for pause input (ESC key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        
        // Show pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        
        // Fix animations before pausing time
        FixButtonAnimations();
        
        // Stop time
        Time.timeScale = 0f;
        
        // Show and unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Disable player movement if reference exists
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        
        // Hide pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        
        // Resume time
        Time.timeScale = 1f;
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable player movement if reference exists
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
    
    public void RestartLevel()
    {
        // Resume time before restarting
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void LoadMainMenu()
    {
        // Resume time before loading main menu
        Time.timeScale = 1f;
        
        // Load main menu scene (change "MainMenu" to your actual main menu scene name)
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame()
    {
        // Resume time before quitting
        Time.timeScale = 1f;
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Property to check if game is paused (useful for other scripts)
    public bool IsPaused
    {
        get { return isPaused; }
    }
    
    private void SetupCanvasForUnscaledTime()
    {
        // If pauseCanvas is not assigned, try to find it automatically
        if (pauseCanvas == null && pauseMenuUI != null)
        {
            pauseCanvas = pauseMenuUI.GetComponentInParent<Canvas>();
        }
        
        // Set canvas to use unscaled time
        if (pauseCanvas != null)
        {
            // Try to get GraphicRaycaster and set it up for unscaled time
            GraphicRaycaster raycaster = pauseCanvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = pauseCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
    
    private void FixButtonAnimations()
    {
        if (pauseMenuUI == null) return;
        
        // Get all buttons and their components
        Button[] buttons = pauseMenuUI.GetComponentsInChildren<Button>(true);
        
        foreach (Button button in buttons)
        {
            // Fix Animator components
            Animator buttonAnimator = button.GetComponent<Animator>();
            if (buttonAnimator != null)
            {
                buttonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            
            // Fix any child animators too
            Animator[] childAnimators = button.GetComponentsInChildren<Animator>(true);
            foreach (Animator animator in childAnimators)
            {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            
            // Ensure button is interactable
            button.interactable = true;
            
            // Force refresh button state
            button.OnPointerExit(null);
        }
        
        // Force refresh the entire canvas
        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = false;
            pauseCanvas.enabled = true;
        }
    }
    
    // Call this method to refresh button animations if needed
    public void RefreshButtonAnimations()
    {
        FixButtonAnimations();
    }
    
    // Alternative method if animations still don't work
    void LateUpdate()
    {
        // Only run this when paused and menu is active
        if (isPaused && pauseMenuUI != null && pauseMenuUI.activeInHierarchy)
        {
            // Continuously ensure animators use unscaled time
            Button[] buttons = pauseMenuUI.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                Animator animator = button.GetComponent<Animator>();
                if (animator != null && animator.updateMode != AnimatorUpdateMode.UnscaledTime)
                {
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }
        }
    }
}
