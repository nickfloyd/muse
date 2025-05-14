// CoreMemoryFragment.cs - Essential story memories
using UnityEngine;

public class CoreMemoryFragment : MemoryFragment
{
    // Story progression
    public int StoryPhase { get; private set; }
    
    // World changes triggered by this memory
    public WorldChangeData WorldChanges { get; private set; }
    
    // Constructor
    public CoreMemoryFragment(MemoryFragmentData data) : base(data)
    {
        if (data is CoreMemoryFragmentData coreData)
        {
            StoryPhase = coreData.StoryPhase;
            WorldChanges = coreData.WorldChanges;
        }
    }
    
    // Override methods
    public override void OnCollect()
    {
        base.OnCollect();
        
        // Trigger story progression
        if (GameManager.Instance != null && GameManager.Instance.StoryManager != null)
        {
            GameManager.Instance.StoryManager.AdvanceStory(StoryPhase);
        }
        
        // Apply world changes
        if (GameManager.Instance != null && GameManager.Instance.WorldManager != null && WorldChanges != null)
        {
            GameManager.Instance.WorldManager.ApplyWorldChanges(WorldChanges);
        }
    }
    
    public override void ApplyEffects()
    {
        base.ApplyEffects();
        
        // Core memories provide stat boosts to the player
        if (GameManager.Instance != null && GameManager.Instance.PlayerManager != null)
        {
            // Apply stat boost based on story phase
            // Higher phase memories provide better boosts
            float boostAmount = 0.05f * StoryPhase; // 5% boost per story phase
            
            PlayerStats stats = GameManager.Instance.PlayerManager.Stats;
            if (stats != null)
            {
                stats.AddCoreMemoryBoost(Id, boostAmount);
            }
        }
    }
}
