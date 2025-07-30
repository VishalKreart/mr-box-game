# Troubleshooting Guide - Facial Expressions System

## Common Issues and Solutions

### Issue 1: "Blue Line" Instead of Box
**Problem**: Only a blue line appears when tapping, no box visible.

**Causes & Solutions**:

#### 1. Box Prefab Missing SpriteRenderer
```
Solution:
1. Select your box prefab
2. Check if it has a SpriteRenderer component
3. Verify the Sprite field has a sprite assigned
4. Make sure the sprite is visible (not transparent)
```

#### 2. Box Prefab Missing Main Sprite
```
Solution:
1. Open your box prefab
2. Add SpriteRenderer if missing
3. Assign the main box sprite to the SpriteRenderer
4. Check the Color is not transparent (should be white or colored)
```

#### 3. Box Scale is Too Small
```
Solution:
1. Select box prefab
2. Check Transform scale (should be around 1,1,1)
3. If scale is very small (like 0.1,0.1,0.1), increase it
4. Try scale of (1,1,1) or (2,2,2) for testing
```

#### 4. Box Position is Off-Screen
```
Solution:
1. Check BoxSpawner position in scene
2. Ensure spawner is visible in camera view
3. Adjust camera position if needed
4. Check if box spawns at correct position
```

### Issue 2: MissingReferenceException with Tears
**Problem**: Error about destroyed GameObject when tears fall.

**Solution**: ✅ **FIXED** - Added null checks in BoxFacialExpressions script.

### Issue 3: No Facial Expressions Showing
**Problem**: Box appears but no eyes/mouth visible.

**Causes & Solutions**:

#### 1. Missing Child Objects
```
Solution:
1. Check box prefab has "Eyes" and "Mouth" child objects
2. Each child should have SpriteRenderer component
3. Verify sprites are assigned to child SpriteRenderers
```

#### 2. Sprites Not Assigned
```
Solution:
1. Select box prefab
2. In BoxFacialExpressions component:
   - Check all sprite fields are assigned
   - ScaredEyes, ScreamingEyes, RelaxedEyes
   - ScaredMouth, ScreamingMouth, RelaxedMouth
```

#### 3. SpriteRenderers Not Referenced
```
Solution:
1. In BoxFacialExpressions component:
   - Drag Eyes child object's SpriteRenderer to "Eyes Renderer"
   - Drag Mouth child object's SpriteRenderer to "Mouth Renderer"
```

### Issue 4: Expressions Not Changing
**Problem**: Facial expressions don't change when box state changes.

**Causes & Solutions**:

#### 1. BoxState Component Missing
```
Solution:
1. Add BoxState component to box prefab
2. Verify BoxFacialExpressions script is present
3. Check BoxAnimationController is attached
```

#### 2. Animator Controller Not Assigned
```
Solution:
1. Select box prefab
2. In Animator component, assign BoxAnimatorController
3. Verify controller has proper states and transitions
```

#### 3. Scripts Not Calling Each Other
```
Solution:
1. Check BoxSpawner calls BoxAnimationController
2. Verify BoxAnimationController calls BoxFacialExpressions
3. Ensure all scripts are properly connected
```

### Issue 5: Tears Not Appearing
**Problem**: No tears fall when box is dropping.

**Causes & Solutions**:

#### 1. Tear Prefab Not Assigned
```
Solution:
1. In BoxFacialExpressions component
2. Drag TearDropPrefab to "Tear Prefab" field
3. Check "Use Tears On Falling" is enabled
```

#### 2. Tear Prefab Missing Components
```
Solution:
1. Check TearDropPrefab has:
   - SpriteRenderer with tear sprite
   - CircleCollider2D (IsTrigger = true)
   - TearDrop script
```

#### 3. Box Not in Falling State
```
Solution:
1. Check BoxState.state is "Falling"
2. Verify BoxSpawner properly sets state
3. Test in Play mode with debug logs
```

## Debug Steps

### Step 1: Check Box Prefab Structure
```
Box Prefab should have:
├── Main Box (with SpriteRenderer)
├── Eyes (Child with SpriteRenderer)
└── Mouth (Child with SpriteRenderer)
```

### Step 2: Check Components
```
Box Prefab should have:
├── SpriteRenderer (main box sprite)
├── Rigidbody2D
├── BoxCollider2D
├── BoxState
├── BoxAnimationController
├── BoxFacialExpressions
└── Animator
```

### Step 3: Check Script References
```
BoxFacialExpressions should have:
├── Eyes Renderer: [Drag Eyes SpriteRenderer]
├── Mouth Renderer: [Drag Mouth SpriteRenderer]
├── Scared Eyes: [Drag ScaredEyes.png]
├── Screaming Eyes: [Drag ScreamingEyes.png]
├── Relaxed Eyes: [Drag RelaxedEyes.png]
├── Scared Mouth: [Drag ScaredMouth.png]
├── Screaming Mouth: [Drag ScreamingMouth.png]
├── Relaxed Mouth: [Drag RelaxedMouth.png]
└── Tear Prefab: [Drag TearDropPrefab]
```

### Step 4: Test in Play Mode
```
1. Enter Play mode
2. Check Console for errors
3. Spawn a box - should see scared expression
4. Drop box - should see screaming + tears
5. Box lands - should see relaxed expression
```

## Quick Fix Checklist

### For "Blue Line" Issue:
- [ ] Box prefab has SpriteRenderer with sprite assigned
- [ ] Box scale is not too small (1,1,1 or larger)
- [ ] Box spawner is in visible area
- [ ] Camera can see the spawn area

### For MissingReferenceException:
- [ ] ✅ **FIXED** - Updated BoxFacialExpressions script
- [ ] Tear prefab has all required components
- [ ] Tear prefab is properly assigned

### For No Expressions:
- [ ] Box prefab has Eyes and Mouth child objects
- [ ] Child objects have SpriteRenderers
- [ ] All sprites are assigned in BoxFacialExpressions
- [ ] SpriteRenderers are referenced in BoxFacialExpressions

### For Expressions Not Changing:
- [ ] BoxState component is present
- [ ] Animator Controller is assigned
- [ ] All scripts are properly connected
- [ ] Test in Play mode

## Performance Issues

### Mobile Optimization:
```csharp
// In BoxFacialExpressions
if (Application.platform == RuntimePlatform.Android || 
    Application.platform == RuntimePlatform.IPhonePlayer)
{
    useShakeOnScared = false;
    useTearsOnFalling = false;
    useBlinking = false;
}
```

### Reduce Sprite Sizes:
- Use 16x16px sprites instead of 32x32px
- Reduce tear spawn frequency
- Simplify animations

## Common Error Messages

### "MissingReferenceException":
- **Cause**: Trying to access destroyed GameObject
- **Solution**: ✅ **FIXED** - Added null checks

### "NullReferenceException":
- **Cause**: Component not assigned
- **Solution**: Check all inspector references

### "Sprite not found":
- **Cause**: Sprite not imported properly
- **Solution**: Reimport sprite or regenerate

### "Animator Controller not found":
- **Cause**: Controller not assigned
- **Solution**: Assign BoxAnimatorController to Animator component

## Testing Procedure

### 1. Basic Test:
```
1. Enter Play mode
2. Tap screen to spawn box
3. Should see box with scared expression
4. Tap to drop box
5. Should see screaming expression + tears
6. Box lands with relaxed expression
```

### 2. Debug Test:
```
1. Open Console window
2. Look for error messages
3. Check if scripts are calling each other
4. Verify state changes are happening
```

### 3. Performance Test:
```
1. Test on target device
2. Check frame rate
3. Reduce effects if needed
4. Optimize for mobile
```

This troubleshooting guide should help you resolve most issues with the facial expressions system! 🎮✨ 