using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WorldChangeData
{
    // Game objects to enable/disable
    [Header("Game Object Changes")]
    public string[] EnabledGameObjectIds;
    public string[] DisabledGameObjectIds;
    
    // Areas to unlock
    [Header("Area Changes")]
    public string[] UnlockedAreaIds;
    
    // NPC changes
    [Header("NPC Changes")]
    public string[] ChangedNPCIds;
    
    // Events to trigger
    [Header("Events")]
    public string[] TriggeredEventIds;
    
    // Environment changes
    [Header("Environment Changes")]
    public bool ChangeLighting = false;
    public Color NewAmbientColor = Color.white;
    public float NewFogDensity = 0.01f;
    
    // Audio changes
    [Header("Audio Changes")]
    public bool ChangeMusic = false;
    public AudioClip NewMusicTrack;
    public float MusicTransitionDuration = 2.0f;
}
