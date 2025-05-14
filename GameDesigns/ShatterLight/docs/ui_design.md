# UI Design Document - Shatter Light Chronicles

## General UI Philosophy

The UI for Shatter Light Chronicles follows a dual design approach that reflects the game's two distinct worlds. All interfaces are designed to be intuitive, immersive, and reinforce the memory-based narrative themes.

### Design Principles
- **Duality**: Clear visual distinction between waking world and dream world interfaces
- **Memory Integration**: UI elements that visualize the memory collection and connection system
- **Minimalism**: Clean, unobtrusive design that doesn't detract from gameplay
- **Responsiveness**: Subtle animations and feedback for all interactions
- **Accessibility**: Readable text, colorblind options, and scalable elements

## Main Menu

### Visual Style
- Animated background showing shifting memories and dream/waking world transitions
- Main title "Shatter Light Chronicles" with glowing memory fragment effect
- Semi-transparent panels with subtle light borders

### Elements
- **Title**: Large, centered at top with memory light effects
- **Menu Options**:
  - New Game
  - Continue
  - Memory Journal (grayed out until first memory collected)
  - Options
  - Credits
  - Exit
- **Visual Feature**: Rotating 3D memory fragment that changes color based on menu selection
- **Background**: Slowly shifting between waking world city skyline and dream world abstract landscape

### Animations
- Smooth transitions between menu screens with light trail effects
- Menu options glow and expand slightly on hover
- Selection confirmed with light pulse effect
- Background subtly responds to menu navigation

## HUD (Heads-Up Display)

### Waking World HUD
- **Style**: Minimalist, resembling augmented reality interface
- **Color Scheme**: Cool blues and whites on dark transparent backgrounds
- **Elements**:
  - Health: Brain wave pattern in top left corner
  - Memory Energy: Small, subtle meter only visible when abilities are available
  - Objective Marker: Neural connection icon with distance indicator
  - Interaction Prompts: Simple white text with blue highlight
  - Mini-map: Optional, appears as neural network in corner

### Dream World HUD
- **Style**: Ethereal, light-based with flowing elements
- **Color Scheme**: Vibrant blues and cyans with purple accents
- **Elements**:
  - Health: Pulsing light orb in top left
  - Memory Energy: Prominent flowing energy bar
  - Ability Icons: Crystal-shaped icons with cooldown indicators
  - Enemy Indicators: Directional markers showing threat locations
  - Memory Fragment Detector: Pulsing indicator that intensifies near fragments

### Combat HUD Additions
- Target lock indicator with enemy health display
- Combo counter with visual feedback
- Damage numbers with color coding (player damage in red, enemy in white)
- Ability cooldown indicators with circular progress

## Memory Journal Interface

### Visual Style
- 3D interactive visualization resembling a constellation or neural network
- Nodes representing memory fragments connected by light beams
- Color-coded by memory type (Core: gold, Skill: cyan, Peripheral: violet)

### Main View Elements
- **Memory Web**: Central interactive visualization of all collected memories
- **Filter Options**: Buttons to show/hide memory types
- **Search Function**: Find specific memories by keyword
- **Detail Panel**: Right side panel showing selected memory information
- **Connection Strength**: Visualized by thickness of connecting lines

### Memory Detail View
- **Title and Type**: Prominently displayed at top
- **Description**: Scrollable text area with memory content
- **Connected Memories**: Visual links to related fragments
- **Replay Option**: Button to re-experience memory flashback
- **Effects Display**: Information on gameplay effects this memory provides

### Interactions
- Zoom in/out of memory web with scroll wheel
- Drag to rotate and reposition the web
- Click on nodes to select and view details
- Double-click to trigger memory flashback
- Highlight connections by hovering over nodes

## Ability Selection Interface

### Waking World Version
- **Style**: Simple radial menu with limited options
- **Activation**: Hold right bumper/key to display
- **Selection**: Use directional input to highlight, release to select
- **Visual Feedback**: Subtle highlight effect on selection

### Dream World Version
- **Style**: Expanded radial menu with memory light effects
- **Activation**: Hold right bumper/key to display, slows game time
- **Selection**: Use directional input to highlight, release to select
- **Visual Feedback**: Energy flow animation on selection
- **Additional Info**: Energy cost and cooldown displayed on hover

## Dialogue System

### Visual Style
- Character portraits with subtle animations
- Text appears in stylized dialogue boxes
- Important keywords highlighted with memory light effect

### Elements
- **Character Portrait**: Left side, changes expression based on dialogue
- **Name Plate**: Below portrait with character name
- **Dialogue Box**: Semi-transparent with subtle border
- **Response Options**: When available, appear as branching paths
- **Memory Impact**: Icon indicating when dialogue affects memory system

### Memory-Influenced Dialogue
- Dialogue options that require specific memories are marked with memory icon
- Previously unavailable options appear with "newly remembered" effect
- Character reactions change based on memory status

## Pause Menu

### Visual Style
- Game world freezes and desaturates in background
- Menu panels appear with smooth transition
- Memory journal visualization visible in background

### Elements
- **Resume**: Return to gameplay
- **Memory Journal**: Open full journal interface
- **Map**: Area map with discovered locations and objectives
- **Inventory**: Any collectible items or notes
- **Options**: Game settings
- **Quit**: Return to main menu

## Options Menu

### Categories
- **Gameplay**: Difficulty, tutorial toggles, auto-save frequency
- **Controls**: Button mapping, sensitivity settings
- **Display**: Resolution, quality settings, brightness
- **Audio**: Volume sliders for music, SFX, dialogue
- **Accessibility**: Text size, colorblind modes, control simplification

### Visual Style
- Clean, organized layout with categories as tabs
- Interactive previews where applicable
- Changes apply immediately with reset option

## Loading Screens

### Visual Style
- Memory fragment visualization that builds as loading progresses
- Tips and lore snippets appear as "memory echoes"

### Elements
- **Progress Indicator**: Memory fragment that fills with light
- **Game Tips**: Contextual gameplay advice
- **Lore Snippets**: Short texts expanding world background
- **Controls Reminder**: Basic control scheme displayed subtly

## Notification System

### Memory Collection
- **Animation**: Fragment flies to screen corner with light trail
- **Sound**: Distinctive chime with variation by memory type
- **Text**: Brief description appears temporarily
- **Journal Indicator**: Small icon pulses to indicate journal update

### Ability Unlock
- **Animation**: Ability icon materializes with energy effect
- **Sound**: Powerful activation sound
- **Text**: "New Ability Unlocked" with name and brief description
- **Tutorial Prompt**: Optional quick tutorial for new ability

### Objective Updates
- **Animation**: Neural pulse effect across screen
- **Sound**: Subtle notification tone
- **Text**: Brief objective description
- **Marker Update**: Map and HUD markers update simultaneously

## Technical Implementation Notes

### Responsive Design
- UI scales appropriately for different resolutions
- Text size adjustable for accessibility
- Controller and keyboard/mouse layouts with appropriate prompts

### Performance Considerations
- Particle effects for UI limited on lower-end hardware
- Simplified animations option for performance mode
- Memory journal complexity scales based on system capabilities

### Development Guidelines
- UI elements built in modular components
- Consistent naming convention for all UI assets
- Animation curves standardized for consistent feel
- Color palette strictly enforced for visual consistency
