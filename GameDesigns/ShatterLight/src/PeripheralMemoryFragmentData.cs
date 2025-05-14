using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PeripheralMemoryFragmentData", menuName = "Shatter Light/Peripheral Memory Fragment Data", order = 4)]
public class PeripheralMemoryFragmentData : MemoryFragmentData
{
    // Connected memories
    [Header("Memory Connections")]
    public string[] ConnectedMemoryIds;
    
    // Narrative importance
    [Header("Narrative Properties")]
    [Range(1, 5)]
    public int NarrativeImportance = 1;
    
    // Dialogue options unlocked by this memory
    [Header("Dialogue")]
    public string[] UnlockedDialogueIds;
    
    // Additional lore
    [Header("Additional Lore")]
    [TextArea(5, 10)]
    public string ExtendedLore;
    
    // Character relationship
    [Header("Character Relationship")]
    public string RelatedCharacterId;
    public string RelationshipDescription;
    
    // Override OnValidate to ensure type is set correctly
    private void OnValidate()
    {
        Type = MemoryType.Peripheral;
    }
}
