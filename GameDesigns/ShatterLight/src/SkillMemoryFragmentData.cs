using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillMemoryFragmentData", menuName = "Shatter Light/Skill Memory Fragment Data", order = 3)]
public class SkillMemoryFragmentData : MemoryFragmentData
{
    // Ability data
    [Header("Ability Properties")]
    public string AbilityId;
    public string AbilityName;
    [TextArea(2, 5)]
    public string AbilityDescription;
    public float EnergyCost = 20f;
    public float Cooldown = 5f;
    
    // Ability prefab and effects
    [Header("Ability Visuals")]
    public GameObject AbilityPrefab;
    public ParticleSystem AbilityEffect;
    public AudioClip AbilitySound;
    
    // Upgrade values
    [Header("Upgrades")]
    public float[] UpgradeValues = new float[] { 1.0f, 1.2f, 1.5f, 2.0f };
    
    // Dream world only flag
    [Header("Restrictions")]
    public bool DreamWorldOnly = false;
    
    // Override OnValidate to ensure type is set correctly
    private void OnValidate()
    {
        Type = MemoryType.Skill;
    }
}
