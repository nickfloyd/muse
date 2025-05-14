// MemoryJournal.cs - Visualizes collected memories and their connections
using UnityEngine;
using System.Collections.Generic;

public class MemoryJournal
{
    // Journal data
    private Dictionary<string, MemoryNode> _memoryNodes = new Dictionary<string, MemoryNode>();
    private List<MemoryConnection> _memoryConnections = new List<MemoryConnection>();
    
    // Visual settings
    private float nodeSpacing = 150f;
    private float connectionWidth = 3f;
    
    // Methods
    public void AddMemory(MemoryFragment fragment)
    {
        if (fragment == null || _memoryNodes.ContainsKey(fragment.Id))
        {
            return;
        }
        
        // Create new node
        MemoryNode node = new MemoryNode(fragment);
        
        // Position node based on type and existing nodes
        PositionNewNode(node);
        
        // Add to dictionary
        _memoryNodes.Add(fragment.Id, node);
        
        Debug.Log($"Added memory to journal: {fragment.Title}");
    }
    
    private void PositionNewNode(MemoryNode node)
    {
        // Position based on memory type
        Vector2 basePosition = Vector2.zero;
        
        switch (node.Fragment.Type)
        {
            case MemoryType.Core:
                // Core memories in the center
                basePosition = new Vector2(0, 0);
                break;
                
            case MemoryType.Skill:
                // Skill memories on the right
                basePosition = new Vector2(nodeSpacing, 0);
                break;
                
            case MemoryType.Peripheral:
                // Peripheral memories on the left
                basePosition = new Vector2(-nodeSpacing, 0);
                break;
        }
        
        // Adjust position based on existing nodes of same type
        int count = 0;
        foreach (MemoryNode existingNode in _memoryNodes.Values)
        {
            if (existingNode.Fragment.Type == node.Fragment.Type)
            {
                count++;
            }
        }
        
        // Spiral layout
        float angle = count * 0.5f * Mathf.PI;
        float radius = nodeSpacing * (1 + count / 8);
        
        Vector2 offset = new Vector2(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius
        );
        
        node.Position = basePosition + offset;
    }
    
    public void CreateConnection(string sourceId, string targetId)
    {
        if (!_memoryNodes.ContainsKey(sourceId) || !_memoryNodes.ContainsKey(targetId))
        {
            Debug.LogWarning($"Cannot create connection: one or both nodes not found ({sourceId}, {targetId})");
            return;
        }
        
        // Check if connection already exists
        foreach (MemoryConnection connection in _memoryConnections)
        {
            if ((connection.SourceId == sourceId && connection.TargetId == targetId) ||
                (connection.SourceId == targetId && connection.TargetId == sourceId))
            {
                return; // Connection already exists
            }
        }
        
        // Create new connection
        MemoryConnection connection = new MemoryConnection(sourceId, targetId);
        _memoryConnections.Add(connection);
        
        Debug.Log($"Created memory connection: {sourceId} -> {targetId}");
    }
    
    public List<MemoryFragment> GetConnectedMemories(string fragmentId)
    {
        List<MemoryFragment> connectedMemories = new List<MemoryFragment>();
        
        if (!_memoryNodes.ContainsKey(fragmentId))
        {
            return connectedMemories;
        }
        
        foreach (MemoryConnection connection in _memoryConnections)
        {
            string connectedId = null;
            
            if (connection.SourceId == fragmentId)
            {
                connectedId = connection.TargetId;
            }
            else if (connection.TargetId == fragmentId)
            {
                connectedId = connection.SourceId;
            }
            
            if (connectedId != null && _memoryNodes.ContainsKey(connectedId))
            {
                connectedMemories.Add(_memoryNodes[connectedId].Fragment);
            }
        }
        
        return connectedMemories;
    }
    
    public void VisualizeJournal(RectTransform journalContainer)
    {
        if (journalContainer == null)
        {
            Debug.LogError("Journal container is null");
            return;
        }
        
        // Clear existing visualization
        foreach (Transform child in journalContainer)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        // Create connection lines first (so they appear behind nodes)
        foreach (MemoryConnection connection in _memoryConnections)
        {
            if (!_memoryNodes.ContainsKey(connection.SourceId) || !_memoryNodes.ContainsKey(connection.TargetId))
            {
                continue;
            }
            
            MemoryNode sourceNode = _memoryNodes[connection.SourceId];
            MemoryNode targetNode = _memoryNodes[connection.TargetId];
            
            // Create line
            GameObject lineObj = new GameObject("Connection");
            lineObj.transform.SetParent(journalContainer, false);
            
            RectTransform rectTransform = lineObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.Lerp(sourceNode.Position, targetNode.Position, 0.5f);
            
            // Calculate line properties
            Vector2 direction = targetNode.Position - sourceNode.Position;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            rectTransform.sizeDelta = new Vector2(distance, connectionWidth);
            rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            
            // Add line renderer
            UnityEngine.UI.Image lineImage = lineObj.AddComponent<UnityEngine.UI.Image>();
            
            // Set color based on connection type
            Color connectionColor = Color.Lerp(sourceNode.Fragment.FragmentColor, targetNode.Fragment.FragmentColor, 0.5f);
            connectionColor.a = 0.6f; // Semi-transparent
            lineImage.color = connectionColor;
        }
        
        // Create nodes
        foreach (MemoryNode node in _memoryNodes.Values)
        {
            // Create node object
            GameObject nodeObj = new GameObject("Memory_" + node.Fragment.Id);
            nodeObj.transform.SetParent(journalContainer, false);
            
            RectTransform rectTransform = nodeObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = node.Position;
            rectTransform.sizeDelta = new Vector2(100, 100); // Node size
            
            // Add visual components
            UnityEngine.UI.Image nodeImage = nodeObj.AddComponent<UnityEngine.UI.Image>();
            nodeImage.color = node.Fragment.FragmentColor;
            
            // Make circular
            nodeImage.sprite = Resources.Load<Sprite>("UI/CircleSprite");
            
            // Add button component for interaction
            UnityEngine.UI.Button button = nodeObj.AddComponent<UnityEngine.UI.Button>();
            
            // Set up button click
            button.onClick.AddListener(() => {
                // View memory when clicked
                if (GameManager.Instance != null && GameManager.Instance.MemoryManager != null)
                {
                    GameManager.Instance.MemoryManager.ViewMemoryFragment(node.Fragment.Id);
                }
            });
            
            // Add title text
            GameObject textObj = new GameObject("Title");
            textObj.transform.SetParent(nodeObj.transform, false);
            
            RectTransform textRectTransform = textObj.AddComponent<RectTransform>();
            textRectTransform.anchoredPosition = Vector2.zero;
            textRectTransform.sizeDelta = new Vector2(90, 90);
            
            UnityEngine.UI.Text titleText = textObj.AddComponent<UnityEngine.UI.Text>();
            titleText.text = node.Fragment.Title;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontSize = 14;
            titleText.color = Color.white;
            
            // Add glow effect for emphasis
            if (node.Fragment.Type == MemoryType.Core)
            {
                // Add outline effect for core memories
                UnityEngine.UI.Outline outline = textObj.AddComponent<UnityEngine.UI.Outline>();
                outline.effectColor = Color.white;
                outline.effectDistance = new Vector2(2, 2);
            }
        }
    }
    
    // Helper classes
    public class MemoryNode
    {
        public MemoryFragment Fragment { get; private set; }
        public Vector2 Position { get; set; }
        
        public MemoryNode(MemoryFragment fragment)
        {
            Fragment = fragment;
            Position = Vector2.zero;
        }
    }
    
    public class MemoryConnection
    {
        public string SourceId { get; private set; }
        public string TargetId { get; private set; }
        
        public MemoryConnection(string sourceId, string targetId)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }
    }
}
