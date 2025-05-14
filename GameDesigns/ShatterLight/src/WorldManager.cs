using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    // World states
    public enum WorldState
    {
        WakingWorld,
        DreamWorld,
        Transitioning
    }
    
    public WorldState CurrentWorldState { get; private set; }
    
    // World components
    [Header("World Settings")]
    [SerializeField] private GameObject wakingWorldEnvironmentPrefab;
    [SerializeField] private GameObject dreamWorldEnvironmentPrefab;
    
    [Header("Memory Guardian Settings")]
    [SerializeField] private GameObject[] memoryGuardianPrefabs;
    [SerializeField] private Transform[] guardianSpawnPoints;
    
    // Active environment
    private GameObject currentEnvironment;
    
    // Memory effects on world
    private Dictionary<string, GameObject> memoryTriggers = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> memoryBarriers = new Dictionary<string, GameObject>();
    
    private void Awake()
    {
        // Set initial state
        CurrentWorldState = WorldState.WakingWorld;
    }
    
    private void Start()
    {
        // Initialize world based on current scene
        Scene currentScene = SceneManager.GetActiveScene();
        
        if (currentScene.name == "WakingWorld")
        {
            LoadWakingWorldScene();
        }
        else if (currentScene.name == "DreamWorld")
        {
            LoadDreamWorldScene();
        }
    }
    
    // Methods for world management
    public void LoadWakingWorldScene()
    {
        // Clean up existing environment
        if (currentEnvironment != null)
        {
            Destroy(currentEnvironment);
        }
        
        // Instantiate waking world environment
        if (wakingWorldEnvironmentPrefab != null)
        {
            currentEnvironment = Instantiate(wakingWorldEnvironmentPrefab);
        }
        
        // Update state
        CurrentWorldState = WorldState.WakingWorld;
        
        // Initialize memory triggers in this world
        InitializeMemoryTriggers();
    }
    
    public void LoadDreamWorldScene()
    {
        // Clean up existing environment
        if (currentEnvironment != null)
        {
            Destroy(currentEnvironment);
        }
        
        // Instantiate dream world environment
        if (dreamWorldEnvironmentPrefab != null)
        {
            currentEnvironment = Instantiate(dreamWorldEnvironmentPrefab);
        }
        
        // Update state
        CurrentWorldState = WorldState.DreamWorld;
        
        // Initialize memory triggers in this world
        InitializeMemoryTriggers();
    }
    
    public void LoadMemoryFlashbackScene(string memoryId)
    {
        // This would load a specific memory flashback scene
        // For the prototype, we'll just use a generic scene
        
        // Update state
        CurrentWorldState = WorldState.Transitioning;
    }
    
    private void InitializeMemoryTriggers()
    {
        // Find all memory triggers in the current scene
        MemoryTrigger[] triggers = FindObjectsOfType<MemoryTrigger>();
        
        memoryTriggers.Clear();
        
        foreach (MemoryTrigger trigger in triggers)
        {
            if (!string.IsNullOrEmpty(trigger.MemoryId))
            {
                memoryTriggers[trigger.MemoryId] = trigger.gameObject;
            }
        }
        
        // Find all memory barriers
        MemoryBarrier[] barriers = FindObjectsOfType<MemoryBarrier>();
        
        memoryBarriers.Clear();
        
        foreach (MemoryBarrier barrier in barriers)
        {
            if (!string.IsNullOrEmpty(barrier.RequiredMemoryId))
            {
                memoryBarriers[barrier.RequiredMemoryId] = barrier.gameObject;
            }
        }
        
        // Apply collected memories to update world state
        if (GameManager.Instance != null && GameManager.Instance.MemoryManager != null)
        {
            List<MemoryFragment> collectedMemories = GameManager.Instance.MemoryManager.GetAllCollectedMemories();
            UpdateWorldBasedOnMemories(collectedMemories);
        }
    }
    
    public void UpdateWorldBasedOnMemories(List<MemoryFragment> activeMemories)
    {
        if (activeMemories == null)
            return;
            
        // Update memory barriers
        foreach (MemoryFragment memory in activeMemories)
        {
            // Remove barriers for collected memories
            if (memoryBarriers.ContainsKey(memory.Id))
            {
                GameObject barrier = memoryBarriers[memory.Id];
                if (barrier != null)
                {
                    barrier.SetActive(false);
                }
            }
            
            // Disable triggers for already collected memories
            if (memoryTriggers.ContainsKey(memory.Id))
            {
                GameObject trigger = memoryTriggers[memory.Id];
                if (trigger != null)
                {
                    trigger.SetActive(false);
                }
            }
            
            // Apply specific world changes for core memories
            if (memory is CoreMemoryFragment coreMemory)
            {
                ApplyWorldChanges(coreMemory);
            }
        }
    }
    
    private void ApplyWorldChanges(CoreMemoryFragment coreMemory)
    {
        if (coreMemory == null || coreMemory.WorldChanges == null)
            return;
            
        // Enable/disable game objects
        foreach (string objectId in coreMemory.WorldChanges.EnabledGameObjectIds)
        {
            GameObject obj = GameObject.Find(objectId);
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        
        foreach (string objectId in coreMemory.WorldChanges.DisabledGameObjectIds)
        {
            GameObject obj = GameObject.Find(objectId);
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        
        // Unlock areas
        foreach (string areaId in coreMemory.WorldChanges.UnlockedAreaIds)
        {
            // This would depend on your area system
            // For now, we'll just look for objects with this name
            GameObject area = GameObject.Find(areaId);
            if (area != null)
            {
                // Enable the area
                area.SetActive(true);
                
                // Disable any barriers
                Transform barrier = area.transform.Find("Barrier");
                if (barrier != null)
                {
                    barrier.gameObject.SetActive(false);
                }
            }
        }
        
        // Update NPCs
        foreach (string npcId in coreMemory.WorldChanges.ChangedNPCIds)
        {
            GameObject npc = GameObject.Find(npcId);
            if (npc != null)
            {
                NPC npcComponent = npc.GetComponent<NPC>();
                if (npcComponent != null)
                {
                    npcComponent.UpdateBasedOnMemory(coreMemory.Id);
                }
            }
        }
        
        // Trigger events
        foreach (string eventId in coreMemory.WorldChanges.TriggeredEventIds)
        {
            // This would depend on your event system
            // For now, we'll just broadcast a message
            BroadcastMessage("OnMemoryEvent", eventId, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    public void SpawnMemoryGuardian(string guardianId)
    {
        if (memoryGuardianPrefabs == null || memoryGuardianPrefabs.Length == 0)
            return;
            
        // Find the guardian prefab
        GameObject guardianPrefab = null;
        
        foreach (GameObject prefab in memoryGuardianPrefabs)
        {
            MemoryGuardian guardian = prefab.GetComponent<MemoryGuardian>();
            if (guardian != null && guardian.GuardianId == guardianId)
            {
                guardianPrefab = prefab;
                break;
            }
        }
        
        if (guardianPrefab == null)
        {
            Debug.LogWarning($"Guardian prefab with ID {guardianId} not found");
            return;
        }
        
        // Find a spawn point
        Transform spawnPoint = null;
        
        if (guardianSpawnPoints != null && guardianSpawnPoints.Length > 0)
        {
            // For simplicity, just use the first spawn point
            // In a full implementation, you'd select an appropriate spawn point
            spawnPoint = guardianSpawnPoints[0];
        }
        
        if (spawnPoint == null)
        {
            // Use player position as fallback
            if (GameManager.Instance != null && 
                GameManager.Instance.PlayerManager != null &&
                GameManager.Instance.PlayerManager.Movement != null)
            {
                spawnPoint = GameManager.Instance.PlayerManager.Movement.transform;
            }
            else
            {
                Debug.LogWarning("No spawn point available for guardian");
                return;
            }
        }
        
        // Spawn the guardian
        Vector3 spawnPosition = spawnPoint.position + new Vector3(0, 0, 5); // Offset to avoid spawning on top of player
        Instantiate(guardianPrefab, spawnPosition, spawnPoint.rotation);
        
        // Notify player to enter combat
        if (GameManager.Instance != null && GameManager.Instance.PlayerManager != null)
        {
            GameManager.Instance.PlayerManager.EnterCombat();
        }
    }
}
