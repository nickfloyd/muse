// MemoryFragmentData.cs - ScriptableObject for storing memory fragment data
using UnityEngine;

[CreateAssetMenu(fileName = "NewMemoryData", menuName = "Shatter Light/Memory Data")]
public class MemoryFragmentData : ScriptableObject
{
    public string Id;
    public string Title;
    public string Description;
    public MemoryType Type;
    public Color FragmentColor = Color.white;
    public GameObject FragmentPrefab;
    public string[] DialogueLines;
    public AudioClip VoiceOver;
}

// CoreMemoryFragmentData.cs - Data for core memory fragments
[CreateAssetMenu(fileName = "NewCoreMemoryData", menuName = "Shatter Light/Core Memory Data")]
public class CoreMemoryFragmentData : MemoryFragmentData
{
    public int StoryPhase;
    public WorldChangeData WorldChanges;
    
    private void OnValidate()
    {
        Type = MemoryType.Core;
    }
}

// SkillMemoryFragmentData.cs - Data for skill memory fragments
[CreateAssetMenu(fileName = "NewSkillMemoryData", menuName = "Shatter Light/Skill Memory Data")]
public class SkillMemoryFragmentData : MemoryFragmentData
{
    public PlayerAbility UnlockedAbility;
    
    private void OnValidate()
    {
        Type = MemoryType.Skill;
    }
}

// PeripheralMemoryFragmentData.cs - Data for peripheral memory fragments
[CreateAssetMenu(fileName = "NewPeripheralMemoryData", menuName = "Shatter Light/Peripheral Memory Data")]
public class PeripheralMemoryFragmentData : MemoryFragmentData
{
    public string[] ConnectedMemoryIds;
    
    private void OnValidate()
    {
        Type = MemoryType.Peripheral;
    }
}

// WorldChangeData.cs - Data for world changes triggered by memories
[System.Serializable]
public class WorldChangeData
{
    public string[] EnabledGameObjectIds;
    public string[] DisabledGameObjectIds;
    public string[] UnlockedAreaIds;
    public string[] ChangedNPCIds;
    public string[] TriggeredEventIds;
}

// PlayerAbility.cs - Data for player abilities unlocked by memories
[System.Serializable]
public class PlayerAbility
{
    public string AbilityId;
    public string AbilityName;
    public string Description;
    public float MemoryEnergyCost = 20f;
    public float Cooldown = 5f;
    
    // Unlock requirements
    public string RequiredMemoryId;
    
    // Availability
    public bool AvailableInWakingWorld = false;
    
    // Visual representation
    public Sprite AbilityIcon;
    public GameObject VisualEffectPrefab;
    
    // Methods
    public bool CanUse(float currentMemoryEnergy)
    {
        return currentMemoryEnergy >= MemoryEnergyCost;
    }
    
    public void Use(PlayerCharacter player, Vector3 targetPosition)
    {
        // Implementation depends on ability type
        // This would be overridden in derived classes
        
        // Spawn visual effect
        if (VisualEffectPrefab != null && player != null)
        {
            Object.Instantiate(VisualEffectPrefab, targetPosition, Quaternion.identity);
        }
    }
    
    public void ApplyUpgrade(AbilityUpgrade upgrade)
    {
        if (upgrade == null)
            return;
            
        // Apply upgrade effects
        MemoryEnergyCost = Mathf.Max(1, MemoryEnergyCost - upgrade.EnergyCostReduction);
        Cooldown = Mathf.Max(0.1f, Cooldown - upgrade.CooldownReduction);
    }
}

// AbilityUpgrade.cs - Data for ability upgrades
[System.Serializable]
public class AbilityUpgrade
{
    public string UpgradeId;
    public string TargetAbilityId;
    public string UpgradeName;
    public string Description;
    public float EnergyCostReduction = 0f;
    public float CooldownReduction = 0f;
    public float DamageIncrease = 0f;
    public float RangeIncrease = 0f;
}

// PlayerStats.cs - Player character statistics
[System.Serializable]
public class PlayerStats
{
    // Base stats
    public float BaseHealth = 100f;
    public float BaseMemoryEnergy = 100f;
    public float BaseDamage = 10f;
    public float BaseDefense = 5f;
    public float BaseMoveSpeed = 5f;
    
    // Current stats (with modifiers)
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }
    public float MaxMemoryEnergy { get; private set; }
    public float Damage { get; private set; }
    public float Defense { get; private set; }
    public float MoveSpeed { get; private set; }
    
    // Memory boosts
    private Dictionary<string, float> coreMemoryBoosts = new Dictionary<string, float>();
    
    // Dream world modifiers
    private bool dreamWorldModifiersActive = false;
    private float dreamWorldDamageMultiplier = 1.5f;
    private float dreamWorldDefenseMultiplier = 1.2f;
    
    // Constructor
    public PlayerStats()
    {
        ResetToDefault();
    }
    
    // Reset to starting values
    public void ResetToDefault()
    {
        CurrentHealth = BaseHealth;
        MaxHealth = BaseHealth;
        MaxMemoryEnergy = BaseMemoryEnergy;
        Damage = BaseDamage;
        Defense = BaseDefense;
        MoveSpeed = BaseMoveSpeed;
        
        coreMemoryBoosts.Clear();
        dreamWorldModifiersActive = false;
    }
    
    // Reset to base values but keep memory boosts
    public void ResetToBase()
    {
        MaxHealth = BaseHealth;
        MaxMemoryEnergy = BaseMemoryEnergy;
        Damage = BaseDamage;
        Defense = BaseDefense;
        MoveSpeed = BaseMoveSpeed;
        
        // Apply memory boosts
        ApplyMemoryBoosts();
        
        // Apply dream world modifiers if active
        if (dreamWorldModifiersActive)
        {
            ApplyDreamWorldModifiers();
        }
        
        // Ensure current health doesn't exceed max
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
    }
    
    // Add core memory boost
    public void AddCoreMemoryBoost(string memoryId, float boostAmount)
    {
        coreMemoryBoosts[memoryId] = boostAmount;
        ResetToBase(); // Recalculate stats
    }
    
    // Apply memory boosts to stats
    private void ApplyMemoryBoosts()
    {
        float totalBoost = 0f;
        
        foreach (float boost in coreMemoryBoosts.Values)
        {
            totalBoost += boost;
        }
        
        // Apply total boost to all stats
        MaxHealth *= (1f + totalBoost);
        MaxMemoryEnergy *= (1f + totalBoost);
        Damage *= (1f + totalBoost);
        Defense *= (1f + totalBoost);
        MoveSpeed *= (1f + totalBoost * 0.5f); // Less boost to move speed
    }
    
    // Apply memory boost from a specific core memory
    public void ApplyMemoryBoost(CoreMemoryFragment memory)
    {
        if (memory == null)
            return;
            
        AddCoreMemoryBoost(memory.Id, 0.05f * memory.StoryPhase);
    }
    
    // Apply dream world modifiers
    public void ApplyDreamWorldModifiers()
    {
        if (!dreamWorldModifiersActive)
        {
            Damage *= dreamWorldDamageMultiplier;
            Defense *= dreamWorldDefenseMultiplier;
            dreamWorldModifiersActive = true;
        }
    }
    
    // Remove dream world modifiers
    public void RemoveDreamWorldModifiers()
    {
        if (dreamWorldModifiersActive)
        {
            Damage /= dreamWorldDamageMultiplier;
            Defense /= dreamWorldDefenseMultiplier;
            dreamWorldModifiersActive = false;
        }
    }
    
    // Take damage
    public void TakeDamage(float amount)
    {
        float actualDamage = Mathf.Max(1, amount - Defense * 0.5f);
        CurrentHealth -= actualDamage;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
    }
    
    // Heal
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
    }
    
    // Check if dead
    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}
