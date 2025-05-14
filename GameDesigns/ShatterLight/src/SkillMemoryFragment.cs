// SkillMemoryFragment.cs - Memories that unlock player abilities
using UnityEngine;

public class SkillMemoryFragment : MemoryFragment
{
    // Skill data
    public PlayerAbility UnlockedAbility { get; private set; }
    
    // Constructor
    public SkillMemoryFragment(MemoryFragmentData data) : base(data)
    {
        if (data is SkillMemoryFragmentData skillData)
        {
            UnlockedAbility = skillData.UnlockedAbility;
        }
    }
    
    // Override methods
    public override void OnCollect()
    {
        base.OnCollect();
        
        // Unlock ability
        if (UnlockedAbility != null)
        {
            Debug.Log($"Unlocked ability: {UnlockedAbility.AbilityName}");
            
            // Show ability unlock notification
            if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
            {
                GameManager.Instance.UIManager.ShowAbilityUnlockNotification(UnlockedAbility);
            }
        }
    }
    
    public override void ApplyEffects()
    {
        base.ApplyEffects();
        
        // Update player abilities
        if (GameManager.Instance != null && 
            GameManager.Instance.PlayerManager != null && 
            GameManager.Instance.PlayerManager.Combat != null)
        {
            GameManager.Instance.PlayerManager.Combat.UpdateAbilities();
        }
    }
}
