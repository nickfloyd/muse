using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // UI components
    [Header("Main UI Components")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject memoryJournalUI;
    [SerializeField] private GameObject dialogueSystem;
    
    [Header("HUD Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider memoryEnergyBar;
    [SerializeField] private Transform abilityIconsContainer;
    [SerializeField] private GameObject abilityIconPrefab;
    
    [Header("Memory Journal")]
    [SerializeField] private RectTransform journalContainer;
    [SerializeField] private Text memoryTitleText;
    [SerializeField] private Text memoryDescriptionText;
    [SerializeField] private Image memoryTypeIcon;
    
    [Header("Dialogue System")]
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Transform dialogueChoicesContainer;
    [SerializeField] private GameObject dialogueChoicePrefab;
    
    [Header("Notifications")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Text notificationText;
    [SerializeField] private float notificationDuration = 3f;
    
    [Header("Memory Flashback")]
    [SerializeField] private GameObject flashbackPanel;
    [SerializeField] private Text flashbackTitleText;
    [SerializeField] private Text flashbackContentText;
    [SerializeField] private Image flashbackImage;
    
    // Private variables
    private List<GameObject> abilityIcons = new List<GameObject>();
    private Coroutine notificationCoroutine;
    
    private void Awake()
    {
        // Hide all UI elements initially
        if (mainMenu != null) mainMenu.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (memoryJournalUI != null) memoryJournalUI.SetActive(false);
        if (dialogueSystem != null) dialogueSystem.SetActive(false);
        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (flashbackPanel != null) flashbackPanel.SetActive(false);
        
        // Show main menu at start
        if (mainMenu != null) mainMenu.SetActive(true);
    }
    
    // Main UI state methods
    public void ShowMainMenu()
    {
        if (mainMenu != null) mainMenu.SetActive(true);
        if (gameHUD != null) gameHUD.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (memoryJournalUI != null) memoryJournalUI.SetActive(false);
        if (dialogueSystem != null) dialogueSystem.SetActive(false);
        if (flashbackPanel != null) flashbackPanel.SetActive(false);
    }
    
    public void ShowGameHUD()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (memoryJournalUI != null) memoryJournalUI.SetActive(false);
        if (dialogueSystem != null) dialogueSystem.SetActive(false);
        if (flashbackPanel != null) flashbackPanel.SetActive(false);
    }
    
    public void ShowPauseMenu()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }
    
    public void HidePauseMenu()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }
    
    public void ShowMemoryJournal()
    {
        if (memoryJournalUI != null)
        {
            memoryJournalUI.SetActive(true);
            
            // Visualize journal
            if (GameManager.Instance != null && 
                GameManager.Instance.MemoryManager != null && 
                GameManager.Instance.MemoryManager.Journal != null &&
                journalContainer != null)
            {
                GameManager.Instance.MemoryManager.Journal.VisualizeJournal(journalContainer);
            }
        }
    }
    
    public void HideMemoryJournal()
    {
        if (memoryJournalUI != null) memoryJournalUI.SetActive(false);
    }
    
    // HUD update methods
    public void UpdateHUD()
    {
        UpdateHealthBar();
        UpdateMemoryEnergyUI(100, 100); // Default values, should be updated by PlayerManager
        UpdateAbilityUI(new List<string>()); // Empty list, should be updated by PlayerCombat
    }
    
    private void UpdateHealthBar()
    {
        if (healthBar != null && GameManager.Instance != null && GameManager.Instance.PlayerManager != null)
        {
            PlayerStats stats = GameManager.Instance.PlayerManager.Stats;
            if (stats != null)
            {
                healthBar.maxValue = stats.MaxHealth;
                healthBar.value = stats.CurrentHealth;
            }
        }
    }
    
    public void UpdateMemoryEnergyUI(float currentEnergy, float maxEnergy)
    {
        if (memoryEnergyBar != null)
        {
            memoryEnergyBar.maxValue = maxEnergy;
            memoryEnergyBar.value = currentEnergy;
        }
    }
    
    public void UpdateAbilityUI(List<string> abilityIds)
    {
        if (abilityIconsContainer == null) return;
        
        // Clear existing icons
        foreach (GameObject icon in abilityIcons)
        {
            Destroy(icon);
        }
        abilityIcons.Clear();
        
        // Create new icons
        if (GameManager.Instance != null && 
            GameManager.Instance.PlayerManager != null && 
            GameManager.Instance.PlayerManager.Combat != null)
        {
            foreach (string abilityId in abilityIds)
            {
                if (abilityIconPrefab != null)
                {
                    GameObject iconObj = Instantiate(abilityIconPrefab, abilityIconsContainer);
                    abilityIcons.Add(iconObj);
                    
                    // Set icon image and cooldown display
                    // This would need to be implemented based on your ability system
                }
            }
        }
    }
    
    // Dialogue system methods
    public void ShowDialogue(string text, string characterName, Sprite portrait)
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.SetActive(true);
            
            if (dialogueText != null) dialogueText.text = text;
            if (characterNameText != null) characterNameText.text = characterName;
            if (characterPortrait != null && portrait != null) characterPortrait.sprite = portrait;
        }
    }
    
    public void HideDialogue()
    {
        if (dialogueSystem != null) dialogueSystem.SetActive(false);
    }
    
    public void ShowDialogueChoices(string[] choices, System.Action<int> onChoiceSelected)
    {
        if (dialogueChoicesContainer == null || dialogueChoicePrefab == null) return;
        
        // Clear existing choices
        foreach (Transform child in dialogueChoicesContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create new choice buttons
        for (int i = 0; i < choices.Length; i++)
        {
            GameObject choiceObj = Instantiate(dialogueChoicePrefab, dialogueChoicesContainer);
            Button choiceButton = choiceObj.GetComponent<Button>();
            Text choiceText = choiceObj.GetComponentInChildren<Text>();
            
            if (choiceText != null) choiceText.text = choices[i];
            
            if (choiceButton != null)
            {
                int choiceIndex = i; // Capture for lambda
                choiceButton.onClick.AddListener(() => {
                    onChoiceSelected?.Invoke(choiceIndex);
                    HideDialogue();
                });
            }
        }
    }
    
    // Notification methods
    public void ShowNotification(string message)
    {
        if (notificationPanel == null || notificationText == null) return;
        
        // Stop any existing notification
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        
        // Show new notification
        notificationText.text = message;
        notificationPanel.SetActive(true);
        
        // Auto-hide after duration
        notificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
    }
    
    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);
        notificationPanel.SetActive(false);
        notificationCoroutine = null;
    }
    
    // Memory flashback methods
    public void DisplayMemoryFlashback(string memoryId)
    {
        if (flashbackPanel == null) return;
        
        MemoryFragment memory = null;
        if (GameManager.Instance != null && GameManager.Instance.MemoryManager != null)
        {
            memory = GameManager.Instance.MemoryManager.GetMemoryById(memoryId);
        }
        
        if (memory != null)
        {
            flashbackPanel.SetActive(true);
            
            if (flashbackTitleText != null) flashbackTitleText.text = memory.Title;
            if (flashbackContentText != null) flashbackContentText.text = memory.Description;
            
            // Set flashback image if available
            // This would need to be implemented based on your memory system
        }
    }
    
    public void HideMemoryFlashback()
    {
        if (flashbackPanel != null) flashbackPanel.SetActive(false);
    }
    
    // Special UI feedback methods
    public void ShowNotEnoughEnergyFeedback()
    {
        ShowNotification("Not enough memory energy!");
        
        // Could also flash the energy bar or play a sound
        if (memoryEnergyBar != null)
        {
            // Flash effect could be implemented here
        }
    }
    
    public void ShowAbilityUnlockNotification(PlayerAbility ability)
    {
        if (ability != null)
        {
            ShowNotification($"New ability unlocked: {ability.AbilityName}");
        }
    }
    
    public void ShowMemoryConnectionHints(List<string> connectedMemoryTitles)
    {
        if (connectedMemoryTitles != null && connectedMemoryTitles.Count > 0)
        {
            string message = "Connected memories: " + string.Join(", ", connectedMemoryTitles);
            ShowNotification(message);
        }
    }
    
    // Combat UI
    public void ShowCombatUI()
    {
        // Enable combat-specific UI elements
        // This would depend on your specific combat UI design
    }
    
    public void HideCombatUI()
    {
        // Disable combat-specific UI elements
        // This would depend on your specific combat UI design
    }
}
