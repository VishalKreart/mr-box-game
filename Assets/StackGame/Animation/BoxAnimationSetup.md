# Box Animation System Setup Guide

## Overview
This system provides different animations for boxes in three states:
1. **Spawned** - When box appears at spawner
2. **Falling** - When box is dropping
3. **Settled** - When box lands and settles

## Setup Instructions

### 1. Add Components to Box Prefabs
For each box prefab, add these components:
- `BoxAnimationController` script
- `Animator` component
- Assign the `BoxAnimatorController` to the Animator

### 2. Create Animation Clips
Create three animation clips for each box type:

#### Spawned Animation
- **Duration**: 0.5 seconds
- **Effects**: Scale pulse, fade in, or bounce
- **Trigger**: "Spawned"

#### Falling Animation
- **Duration**: Continuous
- **Effects**: Rotation, slight wobble, or glow
- **Trigger**: "Falling"
- **Speed**: 1.2x (faster than normal)

#### Settled Animation
- **Duration**: 0.3 seconds
- **Effects**: Shake, settle bounce, or sparkle
- **Trigger**: "Settled"

### 3. Configure BoxAnimationController
Set these parameters in the inspector:

```csharp
[Header("Animation States")]
spawnedAnimationTrigger = "Spawned"
fallingAnimationTrigger = "Falling"
settledAnimationTrigger = "Settled"

[Header("Animation Settings")]
spawnAnimationDuration = 0.5f
fallAnimationSpeed = 1.2f
settleAnimationDuration = 0.3f

[Header("Visual Effects")]
usePulseOnSpawn = true
useShakeOnSettle = true
pulseIntensity = 0.1f
shakeIntensity = 0.05f
```

### 4. Animation Examples

#### Spawned Animation Example:
```csharp
// Scale from 0 to 1 with bounce
transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
// Add bounce effect
float bounce = Mathf.Sin(progress * Mathf.PI * 2) * 0.1f;
transform.localScale += Vector3.one * bounce;
```

#### Falling Animation Example:
```csharp
// Gentle rotation
transform.Rotate(0, 0, 45 * Time.deltaTime);
// Slight wobble
float wobble = Mathf.Sin(Time.time * 3f) * 0.02f;
transform.position += Vector3.right * wobble;
```

#### Settled Animation Example:
```csharp
// Shake effect
float shakeX = Mathf.Sin(progress * Mathf.PI * 8) * intensity;
float shakeY = Mathf.Cos(progress * Mathf.PI * 6) * intensity;
transform.position = originalPos + new Vector3(shakeX, shakeY, 0);
```

## Integration

The system automatically integrates with:
- `BoxSpawner` - Triggers spawned animation
- `BoxState` - Monitors state changes
- `BoxLandingDetector` - Triggers settled animation

## Customization

### Different Animations per Box Type
```csharp
public class BoxAnimationController : MonoBehaviour
{
    [Header("Box-Specific Animations")]
    public AnimationClip spawnedClip;
    public AnimationClip fallingClip;
    public AnimationClip settledClip;
    
    void Start()
    {
        // Assign clips based on box type
        Animator animator = GetComponent<Animator>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        overrideController["Spawned"] = spawnedClip;
        overrideController["Falling"] = fallingClip;
        overrideController["Settled"] = settledClip;
        animator.runtimeAnimatorController = overrideController;
    }
}
```

### Performance Optimization
```csharp
// Disable animations on mobile for better performance
if (Application.platform == RuntimePlatform.Android || 
    Application.platform == RuntimePlatform.IPhonePlayer)
{
    usePulseOnSpawn = false;
    useShakeOnSettle = false;
}
```

## Troubleshooting

### Animation Not Playing
1. Check if Animator component is assigned
2. Verify trigger names match exactly
3. Ensure animation clips are assigned to states

### Performance Issues
1. Reduce animation complexity
2. Disable visual effects on mobile
3. Use simpler animations for background boxes

### Animation Conflicts
1. Reset triggers before setting new ones
2. Use different animation layers
3. Check for multiple animation controllers 