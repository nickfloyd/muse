using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMemory : MonoBehaviour
{
    // Memory detection settings
    [Header("Memory Detection")]
    [SerializeField] private float memoryDetectionRadius = 5f;
    [SerializeField] private LayerMask memoryFragmentLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode resonanceKey = KeyCode.R;
    
    // Memory resonance settings
    [Header("Memory Resonance")]
    [SerializeField] private float resonanceCooldown = 10f;
    [SerializeField] private float resonanceDuration = 5f;
    [SerializeField] private GameObject resonanceEffectPrefab;
    
    // UI indicators
    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    
    // Private variables
    private float lastResonanceTime;
    private bool isResonanceActive;
    private GameObject currentResonanceEffect;
    private MemoryTrigger nearestMemoryTrigger;
    
    private void Update()
    {
        // Check if player is disabled
        if (GameManager.Instance != null && 
            GameManager.Instance.PlayerManager != null && 
            GameManager.Instance.PlayerManager.CurrentPlayerState == PlayerState.Disabled)
        {
            return;
        }
        
        // Find nearest memory trigger
        FindNearestMemoryTrigger();
        
        // Handle interaction
        if (nearestMemoryTrigger != null && Input.GetKeyDown(interactKey))
        {
            InteractWithMemory();
        }
        
        // Handle memory resonance
        if (Input.GetKeyDown(resonanceKey) && CanUseResonance())
        {
            ActivateMemoryResonance();
        }
        
        // Update resonance state
        UpdateResonanceState();
    }
    
    private void FindNearestMemoryTrigger()
    {
        // Reset nearest trigger
        nearestMemoryTrigger = null;
        
        // Hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Find all memory triggers in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, memoryDetectionRadius, memoryFragmentLayer);
        
        if (hitColliders.Length > 0)
        {
            // Find the closest one
            float closestDistance = float.MaxValue;
            
            foreach (Collider collider in hitColliders)
            {
                MemoryTrigger trigger = collider.GetComponent<MemoryTrigger>();
                
                if (trigger != null)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        nearestMemoryTrigger = trigger;
                    }
                }
            }
            
            // Show interaction prompt if we found a trigger
            if (nearestMemoryTrigger != null && interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                
                // Position the prompt above the trigger
                Vector3 promptPosition = nearestMemoryTrigger.transform.position + Vector3.up * 2f;
                interactionPrompt.transform.position = promptPosition;
                
                // Make prompt face camera
                if (Camera.main != null)
                {
                    interactionPrompt.transform.LookAt(Camera.main.transform);
                    interactionPrompt.transform.Rotate(0, 180, 0); // Flip to face camera
                }
            }
        }
    }
    
    private void InteractWithMemory()
    {
        if (nearestMemoryTrigger == null) return;
        
        string memoryId = nearestMemoryTrigger.MemoryId;
        
        if (string.IsNullOrEmpty(memoryId))
        {
            Debug.LogWarning("Memory trigger has no memory ID");
            return;
        }
        
        // Collect the memory
        if (GameManager.Instance != null && GameManager.Instance.MemoryManager != null)
        {
            GameManager.Instance.MemoryManager.CollectMemoryFragment(memoryId);
            
            // Destroy the trigger
            Destroy(nearestMemoryTrigger.gameObject);
            nearestMemoryTrigger = null;
            
            // Hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
    
    private bool CanUseResonance()
    {
        // Check cooldown
        if (Time.time < lastResonanceTime + resonanceCooldown)
        {
            float remainingCooldown = (lastResonanceTime + resonanceCooldown) - Time.time;
            
            // Show cooldown notification
            if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
            {
                GameManager.Instance.UIManager.ShowNotification($"Memory Resonance on cooldown: {remainingCooldown:F1}s");
            }
            
            return false;
        }
        
        return true;
    }
    
    private void ActivateMemoryResonance()
    {
        // Set cooldown
        lastResonanceTime = Time.time;
        isResonanceActive = true;
        
        // Create resonance effect
        if (resonanceEffectPrefab != null)
        {
            currentResonanceEffect = Instantiate(resonanceEffectPrefab, transform.position, Quaternion.identity);
            currentResonanceEffect.transform.SetParent(transform);
        }
        
        // Reveal hidden memory triggers
        RevealHiddenMemories();
        
        // Show notification
        if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
        {
            GameManager.Instance.UIManager.ShowNotification("Memory Resonance activated");
        }
        
        // Schedule deactivation
        StartCoroutine(DeactivateResonanceAfterDuration());
    }
    
    private void RevealHiddenMemories()
    {
        // Find all hidden memory triggers in a larger radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, memoryDetectionRadius * 2f, memoryFragmentLayer);
        
        int revealedCount = 0;
        
        foreach (Collider collider in hitColliders)
        {
            HiddenMemoryTrigger hiddenTrigger = collider.GetComponent<HiddenMemoryTrigger>();
            
            if (hiddenTrigger != null)
            {
                hiddenTrigger.Reveal();
                revealedCount++;
            }
        }
        
        if (revealedCount > 0)
        {
            // Show notification
            if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
            {
                GameManager.Instance.UIManager.ShowNotification($"Revealed {revealedCount} hidden memories");
            }
        }
    }
    
    private void UpdateResonanceState()
    {
        if (isResonanceActive)
        {
            // Update resonance effect position
            if (currentResonanceEffect != null)
            {
                currentResonanceEffect.transform.position = transform.position;
            }
            
            // Continuously reveal new hidden memories that might enter range
            RevealHiddenMemories();
        }
    }
    
    private IEnumerator DeactivateResonanceAfterDuration()
    {
        yield return new WaitForSeconds(resonanceDuration);
        
        // Deactivate resonance
        isResonanceActive = false;
        
        // Destroy effect
        if (currentResonanceEffect != null)
        {
            Destroy(currentResonanceEffect);
            currentResonanceEffect = null;
        }
        
        // Show notification
        if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
        {
            GameManager.Instance.UIManager.ShowNotification("Memory Resonance deactivated");
        }
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, memoryDetectionRadius);
    }
}
