using UnityEngine;
using System.Collections;

public class HiddenMemoryTrigger : MemoryTrigger
{
    // Hidden memory properties
    [Header("Hidden Memory Properties")]
    [SerializeField] private bool requiresResonance = true;
    [SerializeField] private bool requiresSpecificMemory = false;
    [SerializeField] private string requiredMemoryId;
    
    // Visual effects
    [Header("Hidden Visual Effects")]
    [SerializeField] private Material hiddenMaterial;
    [SerializeField] private Material revealedMaterial;
    [SerializeField] private GameObject revealEffect;
    [SerializeField] private float revealDuration = 1.0f;
    
    // State
    private bool isRevealed = false;
    private Renderer memoryRenderer;
    
    private void Awake()
    {
        // Get renderer
        memoryRenderer = GetComponentInChildren<Renderer>();
        
        // Set initial state
        if (memoryRenderer != null && hiddenMaterial != null)
        {
            memoryRenderer.material = hiddenMaterial;
        }
        
        // Hide any effects initially
        if (revealEffect != null)
        {
            revealEffect.SetActive(false);
        }
        
        // Disable collider initially if requires resonance
        if (requiresResonance)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
    
    // Check if this memory can be revealed
    public bool CanReveal()
    {
        if (isRevealed)
        {
            return false; // Already revealed
        }
        
        if (requiresSpecificMemory)
        {
            // Check if player has the required memory
            if (GameManager.Instance != null && GameManager.Instance.MemoryManager != null)
            {
                return GameManager.Instance.MemoryManager.HasMemoryFragment(requiredMemoryId);
            }
            return false;
        }
        
        return true;
    }
    
    // Reveal the hidden memory
    public void Reveal()
    {
        if (isRevealed || !CanReveal())
        {
            return;
        }
        
        // Start reveal effect
        StartCoroutine(RevealEffect());
    }
    
    // Reveal effect coroutine
    private IEnumerator RevealEffect()
    {
        isRevealed = true;
        
        // Enable collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
        
        // Show reveal effect
        if (revealEffect != null)
        {
            revealEffect.SetActive(true);
        }
        
        // Transition material
        if (memoryRenderer != null && hiddenMaterial != null && revealedMaterial != null)
        {
            float elapsed = 0f;
            
            // Create instance materials to avoid modifying the originals
            Material hiddenInstance = new Material(hiddenMaterial);
            Material revealedInstance = new Material(revealedMaterial);
            memoryRenderer.material = hiddenInstance;
            
            while (elapsed < revealDuration)
            {
                float t = elapsed / revealDuration;
                
                // Lerp material properties
                memoryRenderer.material.Lerp(hiddenInstance, revealedInstance, t);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Set final material
            memoryRenderer.material = revealedMaterial;
        }
        
        // Disable reveal effect after a delay
        if (revealEffect != null)
        {
            yield return new WaitForSeconds(2f);
            revealEffect.SetActive(false);
        }
    }
}
