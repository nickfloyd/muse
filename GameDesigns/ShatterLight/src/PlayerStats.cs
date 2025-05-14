using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerStats
{
    // Base stats
    [Header("Base Stats")]
    public float BaseMaxHealth = 100f;
    public float BaseAttackDamage = 10f;
    public float BaseDefense = 5f;
    public float BaseMoveSpeed = 5f;
    public float BaseMemoryRegenRate = 5f;
    
    // Current values
    [Header("Current Stats")]
    public float CurrentHealth;
    public float MaxHealth;
    public float AttackDamageMultiplier = 1f;
    public float DefenseMultiplier = 1f;
    public float MoveSpeedMultiplier = 1f;
    public float MemoryRegenMultiplier = 1f;
    
    // Memory boosts
    private Dictionary<string, float> coreMemoryBoosts = new Dictionary<string, float>();
    private Dictionary<string, float> memoryRegenBoosts = new Dictionary<string, float>();
    
    // Dream world modifiers
    private bool dreamWorldModifiersActive = false;
    private float dreamWorldAttackBonus = 1.5f;
    private float dreamWorldDefenseBonus = 1.2f;
    
    // Constructor
    public PlayerStats()
    {
        ResetToDefault();
    }
    
    // Reset to default values
    public void ResetToDefault()
    {
        CurrentHealth = BaseMaxHealth;
        MaxHealth = BaseMaxHealth;
        AttackDamageMultiplier = 1f;
        DefenseMultiplier = 1f;
        MoveSpeedMultiplier = 1f;
        MemoryRegenMultiplier = 1f;
        
        coreMemoryBoosts.Clear();
        memoryRegenBoosts.Clear();
        dreamWorldModifiersActive = false;
    }
    
    // Reset to base values but keep memory boosts
    public void ResetToBase()
    {
        // Reset multipliers to 1
        AttackDamageMultiplier = 1f;
        DefenseMultiplier = 1f;
        MoveSpeedMultiplier = 1f;
        MemoryRegenMultiplier = 1f;
        
        // Apply core memory boosts
        foreach (float boost in coreMemoryBoosts.Values)
        {
            ApplyStatBoost(boost);
        }
        
        // Apply memory regen boosts
        foreach (float boost in memoryRegenBoosts.Values)
        {
            MemoryRegenMultiplier += boost;
        }
        
        // Apply dream world modifiers if active
        if (dreamWorldModifiersActive)
        {
            ApplyDreamWorldModifiers();
        }
        
        // Update max health
        MaxHealth = BaseMaxHealth * (1f + (AttackDamageMultiplier - 1f) / 2f);
        
        // Ensure current health doesn't exceed max
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
    
    // Apply memory boost
    public void ApplyMemoryBoost(CoreMemoryFragment memory)
    {
        if (memory == null) return;
        
        // Check if we already have this boost
        if (coreMemoryBoosts.ContainsKey(memory.Id))
        {
            return;
        }
        
        // Calculate boost based on story phase
        float boostAmount = 0.05f * memory.StoryPhase; // 5% per story phase
        
        // Add to boosts dictionary
        coreMemoryBoosts[memory.Id] = boostAmount;
        
        // Apply the boost
        ApplyStatBoost(boostAmount);
        
        // Update max health
        MaxHealth = BaseMaxHealth * (1f + (AttackDamageMultiplier - 1f) / 2f);
        
        // Heal player when collecting core memory
        CurrentHealth = MaxHealth;
    }
    
    // Apply stat boost
    private void ApplyStatBoost(float boostAmount)
    {
        AttackDamageMultiplier += boostAmount;
        DefenseMultiplier += boostAmount * 0.8f;
        MoveSpeedMultiplier += boostAmount * 0.5f;
    }
    
    // Add memory regen boost
    public void AddMemoryRegenBoost(string sourceId, float boostAmount)
    {
        if (memoryRegenBoosts.ContainsKey(sourceId))
        {
            memoryRegenBoosts[sourceId] = boostAmount;
        }
        else
        {
            memoryRegenBoosts.Add(sourceId, boostAmount);
        }
        
        // Recalculate regen multiplier
        MemoryRegenMultiplier = 1f;
        foreach (float boost in memoryRegenBoosts.Values)
        {
            MemoryRegenMultiplier += boost;
        }
    }
    
    // Add core memory boost directly
    public void AddCoreMemoryBoost(string memoryId, float boostAmount)
    {
        if (coreMemoryBoosts.ContainsKey(memoryId))
        {
            coreMemoryBoosts[memoryId] = boostAmount;
        }
        else
        {
            coreMemoryBoosts.Add(memoryId, boostAmount);
        }
        
        // Reset and reapply all boosts
        ResetToBase();
    }
    
    // Apply dream world modifiers
    public void ApplyDreamWorldModifiers()
    {
        if (!dreamWorldModifiersActive)
        {
            AttackDamageMultiplier *= dreamWorldAttackBonus;
            DefenseMultiplier *= dreamWorldDefenseBonus;
            dreamWorldModifiersActive = true;
            
            // Update max health
            MaxHealth = BaseMaxHealth * (1f + (AttackDamageMultiplier - 1f) / 2f);
        }
    }
    
    // Remove dream world modifiers
    public void RemoveDreamWorldModifiers()
    {
        if (dreamWorldModifiersActive)
        {
            AttackDamageMultiplier /= dreamWorldAttackBonus;
            DefenseMultiplier /= dreamWorldDefenseBonus;
            dreamWorldModifiersActive = false;
            
            // Update max health
            MaxHealth = BaseMaxHealth * (1f + (AttackDamageMultiplier - 1f) / 2f);
            
            // Ensure current health doesn't exceed max
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }
    }
    
    // Take damage
    public void TakeDamage(float damage)
    {
        // Apply defense
        float reducedDamage = damage / DefenseMultiplier;
        
        // Apply damage
        CurrentHealth -= reducedDamage;
        
        // Ensure health doesn't go below 0
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
    
    // Heal
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        
        // Ensure health doesn't exceed max
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
    
    // Check if player is dead
    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}
