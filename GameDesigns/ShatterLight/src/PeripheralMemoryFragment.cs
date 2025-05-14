// PeripheralMemoryFragment.cs - Optional backstory memories
using UnityEngine;
using System.Collections.Generic;

public class PeripheralMemoryFragment : MemoryFragment
{
    // Connections to other memories
    public string[] ConnectedMemoryIds { get; private set; }
    
    // Constructor
    public PeripheralMemoryFragment(MemoryFragmentData data) : base(data)
    {
        if (data is PeripheralMemoryFragmentData peripheralData)
        {
            ConnectedMemoryIds = peripheralData.ConnectedMemoryIds;
        }
    }
    
    // Override methods
    public override void OnCollect()
    {
        base.OnCollect();
        
        // Create connections in memory journal
        if (GameManager.Instance != null && 
            GameManager.Instance.MemoryManager != null && 
            GameManager.Instance.MemoryManager.Journal != null)
        {
            foreach (string connectedId in ConnectedMemoryIds)
            {
                // Only create connection if the other memory is already collected
                if (GameManager.Instance.MemoryManager.HasMemoryFragment(connectedId))
                {
                    GameManager.Instance.MemoryManager.Journal.CreateConnection(Id, connectedId);
                }
            }
        }
    }
    
    public override void OnView()
    {
        base.OnView();
        
        // Peripheral memories might reveal hints about connected memories
        if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
        {
            List<string> uncollectedConnections = new List<string>();
            
            foreach (string connectedId in ConnectedMemoryIds)
            {
                if (!GameManager.Instance.MemoryManager.HasMemoryFragment(connectedId))
                {
                    MemoryFragment fragment = GameManager.Instance.MemoryManager.GetMemoryById(connectedId);
                    if (fragment != null)
                    {
                        uncollectedConnections.Add(fragment.Title);
                    }
                }
            }
            
            if (uncollectedConnections.Count > 0)
            {
                GameManager.Instance.UIManager.ShowMemoryConnectionHints(uncollectedConnections);
            }
        }
    }
}
