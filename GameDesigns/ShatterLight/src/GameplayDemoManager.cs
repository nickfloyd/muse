using UnityEngine;
using System.Collections;

public class GameplayDemoManager : MonoBehaviour
{
    // Demo settings
    [Header("Demo Settings")]
    [SerializeField] private bool autoStartDemo = true;
    [SerializeField] private float demoStartDelay = 2f;
    
    // Demo UI
    [Header("Demo UI")]
    [SerializeField] private GameObject demoInstructionsPanel;
    [SerializeField] private GameObject objectivePanel;
    [SerializeField] private UnityEngine.UI.Text objectiveText;
    
    // Demo objectives
    private enum DemoObjective
    {
        ExploreWakingWorld,
        FindSleepPoint,
        CollectMemoryFragments,
        DefeatEnemies,
        FindDreamWorldExit,
        CompleteDemo
    }
    
    private DemoObjective currentObjective;
    private int collectedMemoryCount = 0;
    private int defeatedEnemyCount = 0;
    
    private void Start()
    {
        // Initialize demo
        if (autoStartDemo)
        {
            StartCoroutine(StartDemoAfterDelay());
        }
        
        // Register for events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMemoryCollected += HandleMemoryCollected;
            GameManager.Instance.OnEnemyDefeated += HandleEnemyDefeated;
            GameManager.Instance.OnWorldChanged += HandleWorldChanged;
        }
    }
    
    private void OnDestroy()
    {
        // Unregister from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMemoryCollected -= HandleMemoryCollected;
            GameManager.Instance.OnEnemyDefeated -= HandleEnemyDefeated;
            GameManager.Instance.OnWorldChanged -= HandleWorldChanged;
        }
    }
    
    private IEnumerator StartDemoAfterDelay()
    {
        yield return new WaitForSeconds(demoStartDelay);
        
        // Show instructions
        if (demoInstructionsPanel != null)
        {
            demoInstructionsPanel.SetActive(true);
            
            // Wait for player to press any key
            yield return new WaitUntil(() => Input.anyKeyDown);
            
            demoInstructionsPanel.SetActive(false);
        }
        
        // Start first objective
        SetObjective(DemoObjective.ExploreWakingWorld);
    }
    
    // Set current objective
    private void SetObjective(DemoObjective objective)
    {
        currentObjective = objective;
        
        // Update objective UI
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(true);
        }
        
        if (objectiveText != null)
        {
            switch (objective)
            {
                case DemoObjective.ExploreWakingWorld:
                    objectiveText.text = "Objective: Explore the waking world and find memory fragments";
                    break;
                    
                case DemoObjective.FindSleepPoint:
                    objectiveText.text = "Objective: Find a sleep point to enter the dream world";
                    break;
                    
                case DemoObjective.CollectMemoryFragments:
                    objectiveText.text = "Objective: Collect 3 memory fragments in the dream world";
                    break;
                    
                case DemoObjective.DefeatEnemies:
                    objectiveText.text = "Objective: Defeat 2 dream world enemies";
                    break;
                    
                case DemoObjective.FindDreamWorldExit:
                    objectiveText.text = "Objective: Find a memory portal to return to the waking world";
                    break;
                    
                case DemoObjective.CompleteDemo:
                    objectiveText.text = "Demo Complete! Thank you for playing Shatter Light Chronicles";
                    break;
            }
        }
        
        // Show notification
        if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
        {
            GameManager.Instance.UIManager.ShowNotification("New Objective: " + objectiveText.text);
        }
    }
    
    // Event handlers
    private void HandleMemoryCollected(string memoryId)
    {
        // Check current objective
        if (currentObjective == DemoObjective.ExploreWakingWorld)
        {
            // After collecting a memory in waking world, guide to sleep point
            SetObjective(DemoObjective.FindSleepPoint);
        }
        else if (currentObjective == DemoObjective.CollectMemoryFragments)
        {
            collectedMemoryCount++;
            
            // Update objective text
            if (objectiveText != null)
            {
                objectiveText.text = $"Objective: Collect 3 memory fragments in the dream world ({collectedMemoryCount}/3)";
            }
            
            // Check if objective complete
            if (collectedMemoryCount >= 3)
            {
                SetObjective(DemoObjective.DefeatEnemies);
            }
        }
    }
    
    private void HandleEnemyDefeated()
    {
        // Only count if in the right objective
        if (currentObjective == DemoObjective.DefeatEnemies)
        {
            defeatedEnemyCount++;
            
            // Update objective text
            if (objectiveText != null)
            {
                objectiveText.text = $"Objective: Defeat 2 dream world enemies ({defeatedEnemyCount}/2)";
            }
            
            // Check if objective complete
            if (defeatedEnemyCount >= 2)
            {
                SetObjective(DemoObjective.FindDreamWorldExit);
            }
        }
    }
    
    private void HandleWorldChanged(GameState newState)
    {
        if (newState == GameState.DreamWorld && currentObjective == DemoObjective.FindSleepPoint)
        {
            // Player entered dream world
            SetObjective(DemoObjective.CollectMemoryFragments);
        }
        else if (newState == GameState.WakingWorld && currentObjective == DemoObjective.FindDreamWorldExit)
        {
            // Player returned to waking world
            SetObjective(DemoObjective.CompleteDemo);
            
            // Show completion panel
            StartCoroutine(ShowCompletionPanel());
        }
    }
    
    private IEnumerator ShowCompletionPanel()
    {
        yield return new WaitForSeconds(3f);
        
        // Show completion panel or return to main menu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowDemoCompletionScreen();
        }
    }
    
    // Public methods for manual demo control
    public void StartDemo()
    {
        StartCoroutine(StartDemoAfterDelay());
    }
    
    public void SkipToNextObjective()
    {
        switch (currentObjective)
        {
            case DemoObjective.ExploreWakingWorld:
                SetObjective(DemoObjective.FindSleepPoint);
                break;
                
            case DemoObjective.FindSleepPoint:
                SetObjective(DemoObjective.CollectMemoryFragments);
                break;
                
            case DemoObjective.CollectMemoryFragments:
                collectedMemoryCount = 3;
                SetObjective(DemoObjective.DefeatEnemies);
                break;
                
            case DemoObjective.DefeatEnemies:
                defeatedEnemyCount = 2;
                SetObjective(DemoObjective.FindDreamWorldExit);
                break;
                
            case DemoObjective.FindDreamWorldExit:
                SetObjective(DemoObjective.CompleteDemo);
                break;
        }
    }
}
