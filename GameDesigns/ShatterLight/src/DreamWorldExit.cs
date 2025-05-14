using UnityEngine;
using System.Collections;

public class DreamWorldExit : MonoBehaviour
{
    // Exit settings
    [Header("Exit Settings")]
    [SerializeField] private string exitId;
    [SerializeField] private string exitName = "Memory Portal";
    [SerializeField] private bool requiresMemoryFragment = true;
    [SerializeField] private string requiredMemoryId;
    
    // Visual elements
    [Header("Visual Elements")]
    [SerializeField] private GameObject visualIndicator;
    [SerializeField] private GameObject lockedVisual;
    [SerializeField] private GameObject unlockedVisual;
    [SerializeField] private GameObject interactionPrompt;
    
    // Effects
    [Header("Effects")]
    [SerializeField] private ParticleSystem activationEffect;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private AudioSource audioSource;
    
    // Interaction
    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    // Private variables
    private bool playerInRange = false;
    private bool isActivating = false;
    
    private void Start()
    {
        // Initialize visuals
        UpdateVisuals();
        
        // Hide interaction prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Check for player interaction
        if (playerInRange && CanExit() && !isActivating && 
            Input.GetKeyDown(interactKey) && 
            GameManager.Instance != null && 
            GameManager.Instance.CurrentGameState == GameState.DreamWorld)
        {
            ActivateExit();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            // Show interaction prompt if can exit
            if (CanExit() && interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                
                // Position above exit point
                interactionPrompt.transform.position = transform.position + Vector3.up * 2f;
                
                // Make prompt face camera
                if (Camera.main != null)
                {
                    interactionPrompt.transform.LookAt(Camera.main.transform);
                    interactionPrompt.transform.Rotate(0, 180, 0); // Flip to face camera
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            // Hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
    
    // Check if player can exit
    private bool CanExit()
    {
        if (!requiresMemoryFragment)
        {
            return true;
        }
        
        // Check if player has the required memory
        if (GameManager.Instance != null && GameManager.Instance.MemoryManager != null)
        {
            return GameManager.Instance.MemoryManager.HasMemoryFragment(requiredMemoryId);
        }
        
        return false;
    }
    
    // Activate the exit to transition to waking world
    private void ActivateExit()
    {
        if (isActivating)
            return;
            
        isActivating = true;
        
        // Hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Play activation effect
        if (activationEffect != null)
        {
            activationEffect.Play();
        }
        
        // Play activation sound
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
        
        // Start transition sequence
        StartCoroutine(TransitionToWakingWorld());
    }
    
    // Transition to waking world
    private IEnumerator TransitionToWakingWorld()
    {
        // Disable player controls
        if (GameManager.Instance != null && GameManager.Instance.PlayerManager != null)
        {
            GameManager.Instance.PlayerManager.DisablePlayer();
        }
        
        // Wait for effect
        yield return new WaitForSeconds(2f);
        
        // Transition to waking world
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TransitionToWakingWorld();
        }
        
        // Reset activation flag after transition
        isActivating = false;
    }
    
    // Update visuals based on whether player can exit
    private void UpdateVisuals()
    {
        bool canExit = CanExit();
        
        if (lockedVisual != null)
        {
            lockedVisual.SetActive(!canExit);
        }
        
        if (unlockedVisual != null)
        {
            unlockedVisual.SetActive(canExit);
        }
        
        if (visualIndicator != null)
        {
            visualIndicator.SetActive(canExit);
        }
    }
    
    // Called when memory collection changes
    public void OnMemoryCollected()
    {
        UpdateVisuals();
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
