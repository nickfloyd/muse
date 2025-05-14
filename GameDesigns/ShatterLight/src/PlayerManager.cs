// PlayerManager.cs - Manages the player character and its states
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState
{
    Normal,
    InCombat,
    Transitioning,
    InMemoryFlashback,
    Disabled
}

public class PlayerManager : MonoBehaviour
{
    // Player state
    public PlayerState CurrentPlayerState { get; private set; }
    
    // Player components
    public PlayerMovement Movement { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public PlayerMemory Memory { get; private set; }
    
    // Player stats
    public PlayerStats Stats { get; private set; }
    
    // References
    [SerializeField] private GameObject wakingWorldPlayerPrefab;
    [SerializeField] private GameObject dreamWorldPlayerPrefab;
    private GameObject currentPlayerInstance;
    
    // Dream state tracking
    private bool isInDreamWorld = false;
    
    private void Awake()
    {
        // Initialize player components
        InitializeComponents();
        
        // Set initial state
        CurrentPlayerState = PlayerState.Normal;
    }
    
    private void InitializeComponents()
    {
        // Create stats
        Stats = new PlayerStats();
        
        // Initialize with waking world player
        if (currentPlayerInstance == null && wakingWorldPlayerPrefab != null)
        {
            currentPlayerInstance = Instantiate(wakingWorldPlayerPrefab);
            
            // Get components
            Movement = currentPlayerInstance.GetComponent<PlayerMovement>();
            Combat = currentPlayerInstance.GetComponent<PlayerCombat>();
            Memory = currentPlayerInstance.GetComponent<PlayerMemory>();
        }
    }
    
    public void InitializePlayer()
    {
        // Reset player stats
        Stats.ResetToDefault();
        
        // Reset player state
        CurrentPlayerState = PlayerState.Normal;
        isInDreamWorld = false;
        
        // Update player appearance
        UpdatePlayerAppearance(isInDreamWorld);
    }
    
    // Methods for player control
    public void UpdatePlayerAppearance(bool dreamWorld)
    {
        if (dreamWorld == isInDreamWorld && currentPlayerInstance != null)
        {
            return; // No change needed
        }
        
        // Store current position and rotation
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        
        if (currentPlayerInstance != null)
        {
            position = currentPlayerInstance.transform.position;
            rotation = currentPlayerInstance.transform.rotation;
            Destroy(currentPlayerInstance);
        }
        
        // Create new instance based on world state
        if (dreamWorld)
        {
            currentPlayerInstance = Instantiate(dreamWorldPlayerPrefab, position, rotation);
        }
        else
        {
            currentPlayerInstance = Instantiate(wakingWorldPlayerPrefab, position, rotation);
        }
        
        // Update component references
        Movement = currentPlayerInstance.GetComponent<PlayerMovement>();
        Combat = currentPlayerInstance.GetComponent<PlayerCombat>();
        Memory = currentPlayerInstance.GetComponent<PlayerMemory>();
        
        // Update tracking variable
        isInDreamWorld = dreamWorld;
    }
    
    public void ApplyMemoryEffects(List<MemoryFragment> activeMemories)
    {
        if (activeMemories == null || Stats == null)
        {
            return;
        }
        
        // Reset stats to base values
        Stats.ResetToBase();
        
        // Apply effects from each memory
        foreach (MemoryFragment memory in activeMemories)
        {
            if (memory is SkillMemoryFragment skillMemory)
            {
                // Apply skill memory effects
                skillMemory.ApplyEffects();
                
                // Update available abilities
                if (Combat != null)
                {
                    Combat.UpdateAbilities();
                }
            }
            else if (memory is CoreMemoryFragment coreMemory)
            {
                // Apply core memory stat boosts
                Stats.ApplyMemoryBoost(coreMemory);
            }
        }
        
        // Update player visuals based on memories
        UpdateVisualEffects(activeMemories);
    }
    
    private void UpdateVisualEffects(List<MemoryFragment> activeMemories)
    {
        if (currentPlayerInstance == null)
        {
            return;
        }
        
        // Get visual effect controller
        PlayerVisualEffects visualEffects = currentPlayerInstance.GetComponent<PlayerVisualEffects>();
        
        if (visualEffects != null)
        {
            visualEffects.UpdateEffects(activeMemories, isInDreamWorld);
        }
    }
    
    public void EnterDreamState()
    {
        if (!isInDreamWorld)
        {
            // Change player appearance
            UpdatePlayerAppearance(true);
            
            // Apply dream world stats
            Stats.ApplyDreamWorldModifiers();
            
            // Enable dream abilities
            if (Combat != null)
            {
                Combat.EnableDreamAbilities();
            }
            
            // Update player state
            CurrentPlayerState = PlayerState.Normal;
        }
    }
    
    public void ExitDreamState()
    {
        if (isInDreamWorld)
        {
            // Change player appearance
            UpdatePlayerAppearance(false);
            
            // Restore waking world stats
            Stats.RemoveDreamWorldModifiers();
            
            // Disable dream abilities
            if (Combat != null)
            {
                Combat.DisableDreamAbilities();
            }
            
            // Update player state
            CurrentPlayerState = PlayerState.Normal;
        }
    }
    
    public void EnterCombat()
    {
        if (CurrentPlayerState != PlayerState.InCombat)
        {
            CurrentPlayerState = PlayerState.InCombat;
            
            // Enable combat mode
            if (Combat != null)
            {
                Combat.EnableCombatMode();
            }
        }
    }
    
    public void ExitCombat()
    {
        if (CurrentPlayerState == PlayerState.InCombat)
        {
            CurrentPlayerState = PlayerState.Normal;
            
            // Disable combat mode
            if (Combat != null)
            {
                Combat.DisableCombatMode();
            }
        }
    }
    
    public void DisablePlayer()
    {
        CurrentPlayerState = PlayerState.Disabled;
        
        if (Movement != null)
        {
            Movement.enabled = false;
        }
        
        if (Combat != null)
        {
            Combat.enabled = false;
        }
    }
    
    public void EnablePlayer()
    {
        CurrentPlayerState = PlayerState.Normal;
        
        if (Movement != null)
        {
            Movement.enabled = true;
        }
        
        if (Combat != null)
        {
            Combat.enabled = true;
        }
    }
}
