using UnityEngine;
using System.Collections;

public class SleepPoint : MonoBehaviour
{
    // Sleep point settings
    [Header("Sleep Point Settings")]
    [SerializeField] private string sleepPointId;
    [SerializeField] private string sleepPointName = "Rest Point";
    [SerializeField] private bool isUnlocked = true;
    
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
        if (playerInRange && isUnlocked && !isActivating && 
            Input.GetKeyDown(interactKey) && 
            GameManager.Instance != null && 
            GameManager.Instance.CurrentGameState == GameState.WakingWorld)
        {
            ActivateSleepPoint();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            // Show interaction prompt if unlocked
            if (isUnlocked && interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                
                // Position above sleep point
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
    
    // Activate the sleep point to transition to dream world
    private void ActivateSleepPoint()
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
        StartCoroutine(TransitionToDreamWorld());
    }
    
    // Transition to dream world
    private IEnumerator TransitionToDreamWorld()
    {
        // Disable player controls
        if (GameManager.Instance != null && GameManager.Instance.PlayerManager != null)
        {
            GameManager.Instance.PlayerManager.DisablePlayer();
        }
        
        // Wait for effect
        yield return new WaitForSeconds(2f);
        
        // Transition to dream world
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TransitionToDreamWorld();
        }
        
        // Reset activation flag after transition
        isActivating = false;
    }
    
    // Unlock this sleep point
    public void Unlock()
    {
        isUnlocked = true;
        UpdateVisuals();
    }
    
    // Update visuals based on locked/unlocked state
    private void UpdateVisuals()
    {
        if (lockedVisual != null)
        {
            lockedVisual.SetActive(!isUnlocked);
        }
        
        if (unlockedVisual != null)
        {
            unlockedVisual.SetActive(isUnlocked);
        }
        
        if (visualIndicator != null)
        {
            visualIndicator.SetActive(isUnlocked);
        }
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
