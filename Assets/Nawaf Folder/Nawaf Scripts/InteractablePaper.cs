using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractablePaper : MonoBehaviour
{
    [Header("Paper Settings")]
    public string paperTitle = "Document";
    [TextArea(5, 10)]
    public string paperContent = "Enter your paper content here...";
    public Sprite paperBackgroundImage;
    
    [Header("UI References")]
    public GameObject paperUI;
    public TextMeshProUGUI paperTitleText;
    public TextMeshProUGUI paperContentText;
    public Image paperBackgroundImageComponent;
    public Button closePaperButton;
    
    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactionPrompt; // UI element showing "Press E to read"
    
    [Header("Player Reference")]
    public PlayerController playerController;
    
    private Transform player;
    private bool isPlayerNearby = false;
    private bool isPaperOpen = false;
    
    void Start()
    {
        // Find player if not assigned
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        if (playerController != null)
        {
            player = playerController.transform;
        }
        
        // Make sure paper UI is hidden at start
        if (paperUI != null)
        {
            paperUI.SetActive(false);
        }
        
        // Hide interaction prompt at start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Set up close button
        if (closePaperButton != null)
        {
            closePaperButton.onClick.AddListener(ClosePaper);
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= interactionRange)
        {
            if (!isPlayerNearby)
            {
                ShowInteractionPrompt();
                isPlayerNearby = true;
            }
            
            // Check for interaction input
            if (Input.GetKeyDown(interactKey) && !isPaperOpen)
            {
                OpenPaper();
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                HideInteractionPrompt();
                isPlayerNearby = false;
            }
        }
        
        // Close paper with ESC key
        if (isPaperOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePaper();
        }
    }
    
    void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }
    
    void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    public void OpenPaper()
    {
        if (isPaperOpen) return;
        
        isPaperOpen = true;
        
        // Show paper UI
        if (paperUI != null)
        {
            paperUI.SetActive(true);
        }
        
        // Set paper content
        if (paperTitleText != null)
        {
            paperTitleText.text = paperTitle;
        }
        
        if (paperContentText != null)
        {
            paperContentText.text = paperContent;
        }
        
        // Set background image
        if (paperBackgroundImageComponent != null && paperBackgroundImage != null)
        {
            paperBackgroundImageComponent.sprite = paperBackgroundImage;
        }
        
        // Fix button animations before pausing time
        FixButtonAnimations();
        
        // Pause game and show cursor
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Disable player movement
        if (playerController != null)
        {
            playerController.canMove = false;
        }
        
        // Hide interaction prompt
        HideInteractionPrompt();
    }
    
    public void ClosePaper()
    {
        if (!isPaperOpen) return;
        
        isPaperOpen = false;
        
        // Hide paper UI
        if (paperUI != null)
        {
            paperUI.SetActive(false);
        }
        
        // Resume game and hide cursor
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable player movement
        if (playerController != null)
        {
            playerController.canMove = true;
        }
        
        // Show interaction prompt again if player is still nearby
        if (isPlayerNearby)
        {
            ShowInteractionPrompt();
        }
    }
    
    private void FixButtonAnimations()
    {
        if (paperUI == null) return;
        
        // Get all buttons in the paper UI and their components
        Button[] buttons = paperUI.GetComponentsInChildren<Button>(true);
        
        foreach (Button button in buttons)
        {
            // Fix Animator components to use unscaled time
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
        
        // Get the canvas and refresh it
        Canvas paperCanvas = paperUI.GetComponentInParent<Canvas>();
        if (paperCanvas != null)
        {
            // Ensure canvas can handle UI interactions properly when time is paused
            GraphicRaycaster raycaster = paperCanvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = paperCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            
            // Force refresh the canvas
            paperCanvas.enabled = false;
            paperCanvas.enabled = true;
        }
    }
    
    // Continuously ensure button animations work while paper is open
    void LateUpdate()
    {
        if (isPaperOpen && paperUI != null && paperUI.activeInHierarchy)
        {
            // Continuously ensure all button animators use unscaled time
            Button[] buttons = paperUI.GetComponentsInChildren<Button>(true);
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
    
    // Visual debug in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
