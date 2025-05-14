# Audio Design Document - Shatter Light Chronicles

## Music Design

### Main Theme
- **Style**: Orchestral with electronic elements
- **Mood**: Mysterious, emotional, with underlying tension
- **Key Elements**: Piano motif representing memory fragments, string sections for emotional depth, electronic pulses for sci-fi elements
- **Variations**: Different arrangements for menu, cutscenes, and credits
- **Technical Specs**: 
  - Stereo mix with potential for spatial audio
  - 48kHz, 24-bit audio quality
  - Dynamic range allowing for quiet introspective moments and dramatic crescendos

### Waking World Music
- **Style**: Ambient electronic with minimal melodic elements
- **Mood**: Subdued, slightly melancholic, with underlying tension
- **Key Locations**:
  - **Lumina City Streets**: Urban ambient with distant city sounds, muted synth pads
  - **Elian's Apartment**: Minimalist piano with subtle electronic textures
  - **Lumina Neuroscience HQ**: Clean, sterile ambient with rhythmic technological elements
  - **Maya's Clinic**: Warmer tones, gentle arpeggios, hopeful undertones
  - **The Undercity**: Gritty industrial sounds, distorted bass, uneasy atmosphere
- **Adaptive Elements**: Music subtly shifts based on:
  - Time of day
  - Story progression
  - Proximity to memory triggers
  - Character interactions

### Dream World Music
- **Style**: Ethereal, otherworldly, with more pronounced melodic elements
- **Mood**: Wonder mixed with danger, emotional depth, building intensity
- **Key Locations**:
  - **The Nexus**: Ethereal choir, floating arpeggios, spatial audio effects
  - **Memory Vaults**: Melodic themes unique to each memory type
  - **The Void Edges**: Dissonant, glitching audio, decaying reverb
  - **The Spire**: Majestic, mysterious, with the main theme motif woven throughout
  - **Nightmare Zones**: Distorted versions of other themes, horror elements
- **Adaptive Elements**: Music dynamically responds to:
  - Memory fragment proximity
  - Combat intensity
  - Environmental changes
  - Memory corruption levels

### Combat Music
- **Style**: Rhythmic electronic with orchestral elements
- **Mood**: Tense, energetic, building in intensity
- **Variations**:
  - **Standard Encounters**: Faster tempo, percussion-driven
  - **Memory Guardian Battles**: Unique themes for each guardian
  - **Final Confrontation**: Epic orchestral arrangement with full electronic elements
- **Adaptive Elements**:
  - Layered approach that builds as combat intensifies
  - Seamless transitions between exploration and combat
  - Victory and defeat variations

### Memory Flashback Music
- **Style**: Intimate, emotional, focused on piano and strings
- **Mood**: Nostalgic, revealing, emotionally resonant
- **Variations**: Different arrangements based on memory type:
  - **Core Memories**: Full orchestral arrangements with main theme motif
  - **Skill Memories**: More rhythmic, focused on achievement
  - **Peripheral Memories**: Intimate, personal, often solo instruments
- **Technical Elements**:
  - Reverb and delay effects creating dreamlike quality
  - Subtle audio processing to suggest memory distortion
  - Crossfading with environment themes

## Sound Effects Design

### Character Sound Effects

#### Elian (Protagonist)
- **Footsteps**: 
  - Waking World: Realistic with slight echo
  - Dream World: Subtle light particles sound with each step
- **Memory Abilities**:
  - Memory Projection: Whooshing light with crystalline impact
  - Cognitive Shield: Humming energy barrier formation
  - Neural Link: Electrical connection sound with whispered voices
- **Combat Sounds**:
  - Basic attacks: Light energy projection with impact
  - Dodging: Quick whoosh with light trail sound
  - Taking damage: Disrupted memory sound, pained reaction

#### Dr. Mercer (Antagonist)
- **Footsteps**:
  - Waking World: Authoritative, precise steps
  - Dream World: Subtle shadow movement, slightly distorted
- **Memory Abilities**:
  - Memory Extraction: Vacuum-like pull with crystalline breaking
  - Nightmare Manifestation: Deep rumbling with distorted voices
  - Shadow Veil: Whooshing darkness with muffled environment
- **Combat Sounds**:
  - Shadow attacks: Slicing darkness with bass impact
  - Teleport: Quick implosion followed by reformation
  - Barrier: Dark energy solidifying

### Environment Sound Effects

#### Waking World
- **Lumina City**: Urban ambience, traffic, distant conversations, technology beeps
- **Lumina Neuroscience HQ**: Clean, sterile atmosphere, subtle machinery, computer terminals
- **Elian's Apartment**: Quiet room tone, occasional water pipes, distant city
- **Memory Gardens**: Water features, subtle wind, faint whispers near sculptures
- **The Undercity**: Industrial machinery, dripping water, electrical hums, distant voices

#### Dream World
- **General Atmosphere**: Ethereal ambience, distant memory echoes, reality distortion
- **The Nexus**: Humming energy, memory fragments tinkling like glass, spatial anomalies
- **Memory Vaults**: Resonant space, whispered memories, energy flows
- **The Void Edges**: Wind rushing into nothingness, crumbling reality, distortion effects
- **Nightmare Zones**: Distorted heartbeat, corrupted memories, threatening presence

### Memory Fragment Interaction
- **Detection**: Subtle high-pitched ringing that increases with proximity
- **Collection**: Crystalline chime with energy absorption whoosh
- **Viewing**: Whooshing transition with reverberating memory voices
- **Connection Formation**: Electrical arcing with harmonic resonance
- **Corruption**: Distorted version of collection sound with dark undertones

### UI Sound Effects
- **Menu Navigation**: Subtle light interface sounds
- **Memory Journal**: Pages turning, light connections forming, memory echoes
- **Ability Selection**: Energy charging and readying sounds
- **Notifications**: Gentle chimes for discoveries, alerts for dangers
- **Health/Energy**: Subtle heartbeat for low health, energy fluctuation for memory power

## Voice Acting Direction

### General Direction
- Realistic performances with subtle emotional depth
- Dream world scenes have slight reverb/processing
- Memory flashbacks have nostalgic, distant quality
- Voice distortion increases with memory corruption

### Key Characters

#### Elian Voss
- **Waking World**: Slightly hesitant, searching for words occasionally
- **Dream World**: More confident, resonant
- **Character Arc**: Voice becomes more assured as memories return
- **Key Emotional Moments**: Confusion, determination, revelation, connection

#### Dr. Silas Mercer
- **Public Persona**: Charismatic, authoritative, measured
- **True Nature**: Cold, calculating, intense
- **Dream World**: Echoing, slightly distorted
- **Key Emotional Moments**: Controlled anger, scientific fascination, revelation of true motives

#### Maya Chen
- **General Tone**: Compassionate but guarded
- **Technical Discussions**: Precise, knowledgeable
- **Key Emotional Moments**: Concern, revelation of secrets, determination

#### The Architect
- **Voice Quality**: Ethereal, multiple voices layered
- **Delivery**: Enigmatic, speaking in metaphors and memory fragments
- **Evolution**: Voice becomes more singular and familiar as truth is revealed

#### Echo
- **Voice Quality**: Small, bright, slightly digital
- **Delivery**: Quick, sometimes fragmented sentences
- **Personality**: Curious, occasionally humorous, innocent

## Implementation Notes

### Adaptive Audio System
- Dynamic mixing based on gameplay state
- Layered stems for interactive music transitions
- RTPC (Real-Time Parameter Control) for environmental effects
- Spatial audio implementation for immersive experience

### Memory-Based Audio Features
- Audio distortion increases with memory corruption
- Flashback audio uses filters to suggest time period
- Voice processing changes based on memory clarity
- Ambient sounds include subliminal memory echoes

### Technical Specifications
- 48kHz sample rate for all audio
- 24-bit depth for music and key sound effects
- Stereo implementation with spatial audio options
- Dynamic range compression options for different playback environments
- Adaptive mixing for different audio setups (headphones, speakers, surround)
