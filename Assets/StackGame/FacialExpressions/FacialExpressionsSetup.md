# Box Facial Expressions Setup Guide

## Overview
This system adds toonish facial expressions to boxes with eyes and mouth that change based on the box's state:
- **Scared** - When at spawner (afraid of height)
- **Screaming** - When falling
- **Relaxed** - When settled

## Setup Instructions

### 1. Create Box Prefab Structure
```
Box Prefab
├── Main Box Sprite
├── Eyes (Child GameObject)
│   └── SpriteRenderer (for eyes)
└── Mouth (Child GameObject)
    └── SpriteRenderer (for mouth)
```

### 2. Add Components to Box Prefabs
Add these components to each box prefab:
- `BoxFacialExpressions` script
- `BoxAnimationController` script
- `Animator` component

### 3. Create Facial Expression Sprites

#### Eye Expressions (32x32px recommended)
- **Scared Eyes**: Wide, worried eyes with raised eyebrows
- **Screaming Eyes**: Squinted or closed eyes
- **Relaxed Eyes**: Normal, happy eyes

#### Mouth Expressions (32x32px recommended)
- **Scared Mouth**: Small, worried mouth or open in fear
- **Screaming Mouth**: Wide open mouth (screaming)
- **Relaxed Mouth**: Happy smile or neutral mouth

### 4. Configure BoxFacialExpressions
Set these parameters in the inspector:

```csharp
[Header("Facial Components")]
eyesRenderer = [Drag Eyes SpriteRenderer]
mouthRenderer = [Drag Mouth SpriteRenderer]

[Header("Eye Expressions")]
scaredEyes = [Drag Scared Eyes Sprite]
screamingEyes = [Drag Screaming Eyes Sprite]
relaxedEyes = [Drag Relaxed Eyes Sprite]

[Header("Mouth Expressions")]
scaredMouth = [Drag Scared Mouth Sprite]
screamingMouth = [Drag Screaming Mouth Sprite]
relaxedMouth = [Drag Relaxed Mouth Sprite]

[Header("Animation Settings")]
expressionChangeSpeed = 0.2f
useBlinking = true
blinkInterval = 2f
blinkDuration = 0.1f

[Header("Visual Effects")]
useShakeOnScared = true
useTearsOnFalling = true
shakeIntensity = 0.02f
tearPrefab = [Drag Tear Prefab]
```

### 5. Create Tear Prefab
Create a tear drop prefab:
```
Tear Prefab
├── SpriteRenderer (tear sprite)
├── CircleCollider2D (IsTrigger = true)
└── TearDrop script
```

### 6. Sprite Design Guidelines

#### Scared Expression
- **Eyes**: Wide, worried, looking down
- **Mouth**: Small, trembling, or open in fear
- **Effect**: Gentle shaking

#### Screaming Expression
- **Eyes**: Squinted or closed
- **Mouth**: Wide open, screaming
- **Effect**: Tears falling

#### Relaxed Expression
- **Eyes**: Normal, happy, content
- **Mouth**: Smile or neutral
- **Effect**: No special effects

## Integration

The system automatically integrates with:
- `BoxAnimationController` - Triggers expressions
- `BoxState` - Monitors state changes
- `BoxSpawner` - Initializes expressions

## Visual Effects

### Scared Effects
- **Shaking**: Eyes and mouth shake gently
- **Blinking**: Eyes blink occasionally
- **Looking down**: Eyes appear worried

### Screaming Effects
- **Tears**: Tears fall from eyes
- **Wide mouth**: Screaming expression
- **Squinted eyes**: Fear expression

### Relaxed Effects
- **No effects**: Calm, settled expression
- **Happy face**: Content expression

## Customization

### Different Expressions per Box Type
```csharp
[Header("Box-Specific Expressions")]
public Sprite[] scaredEyesVariations;
public Sprite[] screamingEyesVariations;
public Sprite[] relaxedEyesVariations;
```

### Performance Optimization
```csharp
// Disable effects on mobile for better performance
if (Application.platform == RuntimePlatform.Android || 
    Application.platform == RuntimePlatform.IPhonePlayer)
{
    useShakeOnScared = false;
    useTearsOnFalling = false;
    useBlinking = false;
}
```

## Sprite Creation Tips

### Eye Sprites
- Keep consistent size (32x32px)
- Use simple, clear expressions
- Ensure good contrast
- Test at small scale

### Mouth Sprites
- Match eye style
- Keep simple shapes
- Ensure readability
- Use consistent positioning

### Tear Sprites
- Simple teardrop shape
- Semi-transparent
- Small size (16x16px)
- Blue or clear color

## Troubleshooting

### Expressions Not Changing
1. Check if BoxFacialExpressions component is added
2. Verify SpriteRenderer references
3. Ensure sprites are assigned
4. Check BoxState component

### Performance Issues
1. Reduce shake intensity
2. Disable tears on mobile
3. Use simpler sprites
4. Reduce blink frequency

### Visual Glitches
1. Check sprite pivot points
2. Verify positioning
3. Ensure proper layering
4. Test at different scales

## Example Sprite Specifications

### Scared Eyes
- Size: 32x32px
- Style: Wide, worried
- Color: Black or dark
- Position: Upper part of box

### Screaming Mouth
- Size: 32x32px
- Style: Wide open
- Color: Black or dark
- Position: Lower part of box

### Tear Drop
- Size: 16x16px
- Style: Simple teardrop
- Color: Light blue, semi-transparent
- Position: Falls from eyes 