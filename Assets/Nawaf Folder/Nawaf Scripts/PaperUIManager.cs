using UnityEngine;
using UnityEngine.UI;

public class PaperUIManager : MonoBehaviour
{
    [Header("Paper UI Components")]
    public Canvas paperCanvas;
    public GameObject paperPanel;
    public Image paperBackground;
    public Text paperTitle;
    public Text paperContent;
    public Button closeButton;
    public ScrollRect contentScrollRect;
    
    [Header("Animation Settings")]
    public float fadeInDuration = 0.3f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private CanvasGroup canvasGroup;
    private bool isAnimating = false;
    
    void Awake()
    {
        // Get or add CanvasGroup for fading
        canvasGroup = paperCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = paperCanvas.gameObject.AddComponent<CanvasGroup>();
        }
        
        // Set up canvas to work with unscaled time
        if (paperCanvas != null)
        {
            // Make sure canvas renders on top
            paperCanvas.sortingOrder = 100;
            paperCanvas.overrideSorting = true;
        }
        
        // Set up scroll rect for mouse wheel support
        if (contentScrollRect != null)
        {
            contentScrollRect.scrollSensitivity = 20f;
        }
    }
    
    void Start()
    {
        // Hide at start
        if (paperPanel != null)
        {
            paperPanel.SetActive(false);
        }
        
        canvasGroup.alpha = 0f;
    }
    
    void Update()
    {
        // Handle mouse wheel scrolling when paper is open
        if (paperPanel != null && paperPanel.activeInHierarchy && contentScrollRect != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                contentScrollRect.verticalNormalizedPosition += scroll * 0.1f;
                contentScrollRect.verticalNormalizedPosition = Mathf.Clamp01(contentScrollRect.verticalNormalizedPosition);
            }
        }
    }
    
    public void ShowPaper(string title, string content, Sprite backgroundSprite)
    {
        if (isAnimating) return;
        
        // Set content
        if (paperTitle != null)
        {
            paperTitle.text = title;
        }
        
        if (paperContent != null)
        {
            paperContent.text = content;
        }
        
        if (paperBackground != null && backgroundSprite != null)
        {
            paperBackground.sprite = backgroundSprite;
        }
        
        // Reset scroll position
        if (contentScrollRect != null)
        {
            contentScrollRect.verticalNormalizedPosition = 1f;
        }
        
        // Show panel
        if (paperPanel != null)
        {
            paperPanel.SetActive(true);
        }
        
        // Fade in
        StartCoroutine(FadeIn());
    }
    
    public void HidePaper()
    {
        if (isAnimating) return;
        
        StartCoroutine(FadeOut());
    }
    
    private System.Collections.IEnumerator FadeIn()
    {
        isAnimating = true;
        float timer = 0f;
        
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            float normalizedTime = timer / fadeInDuration;
            canvasGroup.alpha = fadeInCurve.Evaluate(normalizedTime);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        isAnimating = false;
    }
    
    private System.Collections.IEnumerator FadeOut()
    {
        isAnimating = true;
        float timer = 0f;
        
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            float normalizedTime = timer / fadeInDuration;
            canvasGroup.alpha = 1f - fadeInCurve.Evaluate(normalizedTime);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        
        // Hide panel
        if (paperPanel != null)
        {
            paperPanel.SetActive(false);
        }
        
        isAnimating = false;
    }
}
