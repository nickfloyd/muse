using UnityEngine;
using System.Collections;

public class MemoryTrigger : MonoBehaviour
{
    // Memory properties
    [Header("Memory Properties")]
    [SerializeField] private string memoryId;
    [SerializeField] private MemoryType memoryType = MemoryType.Peripheral;
    
    // Visual effects
    [Header("Visual Effects")]
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private Light memoryLight;
    [SerializeField] private float pulsateSpeed = 1f;
    [SerializeField] private float pulsateIntensity = 0.5f;
    
    // Audio
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip detectionSound;
    [SerializeField] private AudioClip collectionSound;
    
    // Interaction
    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private GameObject interactionPrompt;
    
    // Properties
    public string MemoryId => memoryId;
    public MemoryType MemoryType => memoryType;
    
    // Private variables
    private float baseIntensity;
    private Color baseColor;
    private bool playerInRange = false;
    private Transform playerTransform;
    
    private void Start()
    {
        // Initialize effects
        if (memoryLight != null)
        {
            baseIntensity = memoryLight.intensity;
            baseColor = memoryLight.color;
            
            // Set color based on memory type
            switch (memoryType)
            {
                case MemoryType.Core:
                    memoryLight.color = Color.yellow;
                    break;
                case MemoryType.Skill:
                    memoryLight.color = Color.cyan;
                    break;
                case MemoryType.Peripheral:
                    memoryLight.color = new Color(0.8f, 0.3f, 0.8f); // Purple
                    break;
            }
        }
        
        // Hide interaction prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Start pulsating effect
        StartCoroutine(PulsateEffect());
    }
    
    private void Update()
    {
        // Check if player is in range
        if (playerInRange && playerTransform != null)
        {
            // Update interaction prompt position
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                
                // Position above memory
                Vector3 promptPosition = transform.position + Vector3.up * 1.5f;
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
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;
            
            // Play detection sound
            if (audioSource != null && detectionSound != null)
            {
                audioSource.PlayOneShot(detectionSound);
            }
            
            // Show interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
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
    
    // Collect the memory
    public void CollectMemory()
    {
        // Play collection sound
        if (audioSource != null && collectionSound != null)
        {
            audioSource.PlayOneShot(collectionSound);
        }
        
        // Collect the memory in the memory manager
        if (GameManager.Instance != null && GameManager.Instance.MemoryManager != null)
        {
            GameManager.Instance.MemoryManager.CollectMemoryFragment(memoryId);
        }
        
        // Disable interaction
        playerInRange = false;
        
        // Hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Play collection effect
        StartCoroutine(CollectionEffect());
    }
    
    // Pulsating light effect
    private IEnumerator PulsateEffect()
    {
        float time = 0f;
        
        while (true)
        {
            if (memoryLight != null)
            {
                // Pulsate intensity
                float intensityOffset = Mathf.Sin(time * pulsateSpeed) * pulsateIntensity;
                memoryLight.intensity = baseIntensity + intensityOffset;
                
                // Slightly vary color
                float colorVariation = Mathf.Sin(time * pulsateSpeed * 0.5f) * 0.2f + 0.8f;
                memoryLight.color = baseColor * colorVariation;
            }
            
            time += Time.deltaTime;
            yield return null;
        }
    }
    
    // Collection effect
    private IEnumerator CollectionEffect()
    {
        // Increase light intensity
        if (memoryLight != null)
        {
            float targetIntensity = baseIntensity * 3f;
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                memoryLight.intensity = Mathf.Lerp(baseIntensity, targetIntensity, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        // Disable glow effect
        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
        
        // Disable light
        if (memoryLight != null)
        {
            memoryLight.enabled = false;
        }
        
        // Destroy this object after a delay
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
