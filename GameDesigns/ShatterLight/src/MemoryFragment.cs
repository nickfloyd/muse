// MemoryFragment.cs - Base class for all memory fragments
using UnityEngine;
using System.Collections;

public abstract class MemoryFragment
{
    // Basic properties
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public MemoryType Type { get; private set; }
    public bool IsCollected { get; private set; }
    
    // Visual representation
    public Color FragmentColor { get; private set; }
    public GameObject FragmentPrefab { get; private set; }
    
    // Narrative content
    public string[] DialogueLines { get; private set; }
    public AudioClip VoiceOver { get; private set; }
    
    // Constructor
    public MemoryFragment(MemoryFragmentData data)
    {
        Id = data.Id;
        Title = data.Title;
        Description = data.Description;
        Type = data.Type;
        IsCollected = false;
        FragmentColor = data.FragmentColor;
        FragmentPrefab = data.FragmentPrefab;
        DialogueLines = data.DialogueLines;
        VoiceOver = data.VoiceOver;
    }
    
    // Methods
    public virtual void OnCollect()
    {
        IsCollected = true;
        Debug.Log($"Memory fragment collected: {Title}");
    }
    
    public virtual void OnView()
    {
        Debug.Log($"Memory fragment viewed: {Title}");
    }
    
    public virtual void ApplyEffects()
    {
        // Base implementation does nothing
        // Override in derived classes
    }
}
