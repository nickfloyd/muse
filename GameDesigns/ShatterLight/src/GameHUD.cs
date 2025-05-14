using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameHUD : MonoBehaviour
{
    // Health and Energy
    [Header("Player Stats")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider memoryEnergyBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Text energyText;
    
    // Ability Icons
    [Header("Abilities")]
    [SerializeField] private Transform abilityIconsContainer;
    [SerializeField] private GameObject abilityIconPrefab;
    [SerializeField] private Sprite defaultAbilityIcon;
    
    // Minimap
    [Header("Minimap")]
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private RectTransform playerMarker;
    [SerializeField] private Camera minimapCamera;
    
    // Interaction Prompts
    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Text interactionText;
    
    // Notifications
    [Header("Notifications")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Text notificationText;
    [SerializeField] private float notificationDuration = 3f;
    
    // Memory Detection
    [Header("Memory Detection")]
    [SerializeField] private GameObject memoryDetectionIndicator;
    [SerializeField] private Image detectionFillImage;
    
    // World State Indicator
    [Header("World State")]
    [SerializeField] private GameObject worldStateIndicator;
    [SerializeField] private Sprite wakingWorldIcon;
    [SerializeField] private Sprite dreamWorldIcon;
    [SerializeField] private Image worldStateImage;
    
    // Animation
    [Header("Animation")]
    [SerializeField] private Animator hudAnimator;
    
    // Private variables
    private Coroutine notificationCoroutine;
    private GameObject[] abilityIcons = new GameObject[4];
    
    private void Start()
    {
        // Initialize HUD
        UpdateHealthBar(100, 100);
        UpdateEnergyBar(100, 100);
        
        // Hide interaction prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Hide notification panel initially
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
        
        // Hide memory detection initially
        if (memoryDetectionIndicator != null)
        {
            memoryDetectionIndicator.SetActive(false);
        }
        
        // Set initial world state
        UpdateWorldStateIndicator(GameState.WakingWorld);
        
        // Create ability icons
        InitializeAbilityIcons();
    }
    
    private void Update()
    {
        // Update minimap player marker
        UpdateMinimapMarker();
    }
    
    // Initialize ability icons
    private void InitializeAbilityIcons()
    {
        if (abilityIconsContainer == null || abilityIconPrefab == null)
            return;
            
        // Clear existing icons
        foreach (Transform child in abilityIconsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create new icons
        for (int i = 0; i < 4; i++)
        {
            GameObject iconObj = Instantiate(abilityIconPrefab, abilityIconsContainer);
            abilityIcons[i] = iconObj;
            
            // Set default icon
            Image iconImage = iconObj.GetComponentInChildren<Image>();
            if (iconImage != null && defaultAbilityIcon != null)
            {
                iconImage.sprite = defaultAbilityIcon;
                iconImage.color = new Color(1, 1, 1, 0.5f); // Dimmed when no ability assigned
            }
            
            // Set key binding text
            Text keyText = iconObj.GetComponentInChildren<Text>();
            if (keyText != null)
            {
                keyText.text = (i + 1).ToString();
            }
        }
    }
    
    // Update health bar
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Round(currentHealth)}/{Mathf.Round(maxHealth)}";
        }
    }
    
    // Update energy bar
    public void UpdateEnergyBar(float currentEnergy, float maxEnergy)
    {
        if (memoryEnergyBar != null)
        {
            memoryEnergyBar.maxValue = maxEnergy;
            memoryEnergyBar.value = currentEnergy;
        }
        
        if (energyText != null)
        {
            energyText.text = $"{Mathf.Round(currentEnergy)}/{Mathf.Round(maxEnergy)}";
        }
    }
    
    // Update ability icons
    public void UpdateAbilityIcons(PlayerAbility[] abilities)
    {
        if (abilities == null)
            return;
            
        for (int i = 0; i < 4; i++)
        {
            if (i < abilities.Length && abilities[i] != null && abilityIcons[i] != null)
            {
                // Set ability icon
                Image iconImage = abilityIcons[i].GetComponentInChildren<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = abilities[i].Icon;
                    iconImage.color = Color.white; // Full opacity for assigned ability
                }
                
                // Set cooldown indicator
                Image cooldownImage = abilityIcons[i].transform.Find("Cooldown")?.GetComponent<Image>();
                if (cooldownImage != null)
                {
                    if (abilities[i].IsOnCooldown)
                    {
                        cooldownImage.gameObject.SetActive(true);
                        cooldownImage.fillAmount = abilities[i].CooldownRemaining / abilities[i].Cooldown;
                    }
                    else
                    {
                        cooldownImage.gameObject.SetActive(false);
                    }
                }
                
                // Set energy cost text
                Text costText = abilityIcons[i].transform.Find("Cost")?.GetComponent<Text>();
                if (costText != null)
                {
                    costText.text = abilities[i].EnergyCost.ToString();
                }
            }
            else if (abilityIcons[i] != null)
            {
                // No ability assigned to this slot
                Image iconImage = abilityIcons[i].GetComponentInChildren<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = defaultAbilityIcon;
                    iconImage.color = new Color(1, 1, 1, 0.5f); // Dimmed when no ability assigned
                }
                
                // Hide cooldown
                Image cooldownImage = abilityIcons[i].transform.Find("Cooldown")?.GetComponent<Image>();
                if (cooldownImage != null)
                {
                    cooldownImage.gameObject.SetActive(false);
                }
                
                // Clear cost text
                Text costText = abilityIcons[i].transform.Find("Cost")?.GetComponent<Text>();
                if (costText != null)
                {
                    costText.text = "";
                }
            }
        }
    }
    
    // Update minimap marker
    private void UpdateMinimapMarker()
    {
        if (minimapCamera == null || playerMarker == null)
            return;
            
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;
            
        // Convert player world position to minimap position
        Vector3 playerPos = player.transform.position;
        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(playerPos);
        
        // Update marker position
        playerMarker.anchorMin = new Vector2(viewportPos.x, viewportPos.y);
        playerMarker.anchorMax = new Vector2(viewportPos.x, viewportPos.y);
        
        // Update marker rotation to match player
        playerMarker.rotation = Quaternion.Euler(0, 0, -player.transform.eulerAngles.y);
    }
    
    // Show interaction prompt
    public void ShowInteractionPrompt(string text)
    {
        if (interactionPrompt == null)
            return;
            
        interactionPrompt.SetActive(true);
        
        if (interactionText != null)
        {
            interactionText.text = text;
        }
    }
    
    // Hide interaction prompt
    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    // Show notification
    public void ShowNotification(string message)
    {
        if (notificationPanel == null || notificationText == null)
            return;
            
        // Stop any existing notification
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        
        // Show notification
        notificationPanel.SetActive(true);
        notificationText.text = message;
        
        // Start auto-hide coroutine
        notificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
    }
    
    // Hide notification after delay
    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);
        
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
        
        notificationCoroutine = null;
    }
    
    // Update memory detection indicator
    public void UpdateMemoryDetection(float detectionStrength)
    {
        if (memoryDetectionIndicator == null || detectionFillImage == null)
            return;
            
        if (detectionStrength > 0)
        {
            memoryDetectionIndicator.SetActive(true);
            detectionFillImage.fillAmount = detectionStrength;
        }
        else
        {
            memoryDetectionIndicator.SetActive(false);
        }
    }
    
    // Update world state indicator
    public void UpdateWorldStateIndicator(GameState state)
    {
        if (worldStateIndicator == null || worldStateImage == null)
            return;
            
        switch (state)
        {
            case GameState.WakingWorld:
                worldStateImage.sprite = wakingWorldIcon;
                worldStateImage.color = new Color(0.8f, 0.9f, 1f);
                break;
                
            case GameState.DreamWorld:
                worldStateImage.sprite = dreamWorldIcon;
                worldStateImage.color = new Color(0.6f, 0.4f, 0.8f);
                break;
                
            default:
                worldStateIndicator.SetActive(false);
                return;
        }
        
        worldStateIndicator.SetActive(true);
    }
    
    // Show/hide HUD
    public void ShowHUD()
    {
        gameObject.SetActive(true);
        
        if (hudAnimator != null)
        {
            hudAnimator.SetTrigger("Show");
        }
    }
    
    public void HideHUD()
    {
        if (hudAnimator != null)
        {
            hudAnimator.SetTrigger("Hide");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
