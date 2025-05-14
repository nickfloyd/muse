using UnityEngine;

[CreateAssetMenu(fileName = "CoreMemoryFragmentData", menuName = "Shatter Light/Core Memory Fragment Data", order = 2)]
public class CoreMemoryFragmentData : MemoryFragmentData
{
    // Story progression
    [Header("Story Properties")]
    public int StoryPhase = 1;
    
    // World changes triggered by this memory
    [Header("World Changes")]
    public WorldChangeData WorldChanges;
    
    // Flashback scene
    [Header("Flashback")]
    public string FlashbackSceneName;
    public Sprite FlashbackImage;
    
    // Override OnValidate to ensure type is set correctly
    private void OnValidate()
    {
        Type = MemoryType.Core;
    }
}
