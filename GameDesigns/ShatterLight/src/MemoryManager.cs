// MemoryManager.cs - Handles memory fragment collection, storage, and effects
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MemoryType
{
    Core,
    Peripheral,
    Skill
}

public class MemoryManager : MonoBehaviour
{
    // Collection of all memory fragments
    private Dictionary<string, MemoryFragment> _allMemoryFragments = new Dictionary<string, MemoryFragment>();
    
    // Currently collected memories
    private List<MemoryFragment> _collectedMemories = new List<MemoryFragment>();
    
    // Memory journal visualization
    public MemoryJournal Journal { get; private set; }
    
    // Memory data
    [SerializeField] private MemoryFragmentData[] memoryData;
    
    // Events
    public delegate void MemoryCollectedHandler(MemoryFragment fragment);
    public event MemoryCollectedHandler OnMemoryCollected;
    
    private void Awake()
    {
        // Initialize memory journal
        Journal = new MemoryJournal();
        
        // Load memory data
        LoadMemoryData();
    }
    
    public void InitializeMemories()
    {
        // Clear collected memories
        _collectedMemories.Clear();
        
        // Reset memory journal
        Journal = new MemoryJournal();
        
        // Reload memory data if needed
        if (_allMemoryFragments.Count == 0)
        {
            LoadMemoryData();
        }
    }
    
    private void LoadMemoryData()
    {
        _allMemoryFragments.Clear();
        
        // Load from scriptable objects
        foreach (MemoryFragmentData data in memoryData)
        {
            MemoryFragment fragment = CreateMemoryFromData(data);
            if (fragment != null)
            {
                _allMemoryFragments.Add(fragment.Id, fragment);
            }
        }
        
        Debug.Log($"Loaded {_allMemoryFragments.Count} memory fragments");
    }
    
    private MemoryFragment CreateMemoryFromData(MemoryFragmentData data)
    {
        MemoryFragment fragment = null;
        
        switch (data.Type)
        {
            case MemoryType.Core:
                fragment = new CoreMemoryFragment(data);
                break;
                
            case MemoryType.Peripheral:
                fragment = new PeripheralMemoryFragment(data);
                break;
                
            case MemoryType.Skill:
                fragment = new SkillMemoryFragment(data);
                break;
        }
        
        return fragment;
    }
    
    // Methods for memory management
    public void CollectMemoryFragment(string fragmentId)
    {
        if (string.IsNullOrEmpty(fragmentId) || !_allMemoryFragments.ContainsKey(fragmentId))
        {
            Debug.LogWarning($"Memory fragment with ID {fragmentId} not found");
            return;
        }
        
        MemoryFragment fragment = _allMemoryFragments[fragmentId];
        
        if (fragment.IsCollected)
        {
            Debug.Log($"Memory fragment {fragmentId} already collected");
            return;
        }
        
        // Mark as collected
        fragment.OnCollect();
        
        // Add to collected list
        _collectedMemories.Add(fragment);
        
        // Add to journal
        Journal.AddMemory(fragment);
        
        // Create connections for peripheral memories
        if (fragment is PeripheralMemoryFragment peripheralMemory)
        {
            foreach (string connectedId in peripheralMemory.ConnectedMemoryIds)
            {
                if (_allMemoryFragments.ContainsKey(connectedId) && _allMemoryFragments[connectedId].IsCollected)
                {
                    Journal.CreateConnection(fragment.Id, connectedId);
                }
            }
        }
        
        // Apply memory effects
        ApplyMemoryEffects();
        
        // Trigger event
        OnMemoryCollected?.Invoke(fragment);
        
        // Update game progress
        GameManager.Instance.UpdateGameProgress();
        
        Debug.Log($"Collected memory fragment: {fragment.Title}");
    }
    
    public void ViewMemoryFragment(string fragmentId)
    {
        if (string.IsNullOrEmpty(fragmentId) || !_allMemoryFragments.ContainsKey(fragmentId))
        {
            Debug.LogWarning($"Memory fragment with ID {fragmentId} not found");
            return;
        }
        
        MemoryFragment fragment = _allMemoryFragments[fragmentId];
        
        if (!fragment.IsCollected)
        {
            Debug.LogWarning($"Cannot view uncollected memory fragment {fragmentId}");
            return;
        }
        
        // Trigger view event
        fragment.OnView();
        
        // Show memory flashback
        GameManager.Instance.ShowMemoryFlashback(fragmentId);
    }
    
    public bool HasMemoryFragment(string fragmentId)
    {
        if (string.IsNullOrEmpty(fragmentId) || !_allMemoryFragments.ContainsKey(fragmentId))
        {
            return false;
        }
        
        return _allMemoryFragments[fragmentId].IsCollected;
    }
    
    public List<MemoryFragment> GetCollectedMemoriesByType(MemoryType type)
    {
        List<MemoryFragment> result = new List<MemoryFragment>();
        
        foreach (MemoryFragment memory in _collectedMemories)
        {
            if (memory.Type == type)
            {
                result.Add(memory);
            }
        }
        
        return result;
    }
    
    public int GetTotalCoreMemoriesCount()
    {
        int count = 0;
        
        foreach (MemoryFragment memory in _allMemoryFragments.Values)
        {
            if (memory.Type == MemoryType.Core)
            {
                count++;
            }
        }
        
        return count;
    }
    
    public int GetCollectedCoreMemoriesCount()
    {
        int count = 0;
        
        foreach (MemoryFragment memory in _collectedMemories)
        {
            if (memory.Type == MemoryType.Core)
            {
                count++;
            }
        }
        
        return count;
    }
    
    // Memory effects on gameplay
    public void ApplyMemoryEffects()
    {
        // Apply effects to player
        if (GameManager.Instance.PlayerManager != null)
        {
            GameManager.Instance.PlayerManager.ApplyMemoryEffects(_collectedMemories);
        }
        
        // Apply effects to world
        if (GameManager.Instance.WorldManager != null)
        {
            GameManager.Instance.WorldManager.UpdateWorldBasedOnMemories(_collectedMemories);
        }
    }
    
    public MemoryFragment GetMemoryById(string fragmentId)
    {
        if (string.IsNullOrEmpty(fragmentId) || !_allMemoryFragments.ContainsKey(fragmentId))
        {
            return null;
        }
        
        return _allMemoryFragments[fragmentId];
    }
    
    public List<MemoryFragment> GetAllCollectedMemories()
    {
        return new List<MemoryFragment>(_collectedMemories);
    }
}
