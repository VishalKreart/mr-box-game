# Quick Setup Guide - Facial Expressions

## 🚀 Fast Setup (5 Minutes)

### Step 1: Generate Sprites
1. **Create an empty GameObject** in your scene
2. **Add the `SpriteExpressionGenerator` script** to it
3. **Right-click the script** → "Generate All Expressions"
4. **Check the Console** - sprites will be saved to `Assets/StackGame/Sprites/Expressions/`

### Step 2: Set Up Box Prefabs
1. **Add child objects** to your box prefab:
   ```
   Box Prefab
   ├── Eyes (Child GameObject)
   │   └── SpriteRenderer
   └── Mouth (Child GameObject)
       └── SpriteRenderer
   ```

2. **Add components** to box prefab:
   - `BoxFacialExpressions` script
   - `BoxAnimationController` script
   - `Animator` component

### Step 3: Configure Facial Expressions
1. **Select your box prefab**
2. **In the BoxFacialExpressions component:**
   - Drag the generated sprites to their slots:
     - `scaredEyes` → ScaredEyes.png
     - `screamingEyes` → ScreamingEyes.png
     - `relaxedEyes` → RelaxedEyes.png
     - `scaredMouth` → ScaredMouth.png
     - `screamingMouth` → ScreamingMouth.png
     - `relaxedMouth` → RelaxedMouth.png
   - Drag the Eyes and Mouth SpriteRenderers to their slots

### Step 4: Create Tear Prefab
1. **Create a new GameObject**
2. **Add SpriteRenderer** with TearDrop.png
3. **Add CircleCollider2D** (IsTrigger = true)
4. **Add TearDrop script**
5. **Save as prefab**

## 🎨 Customizing Sprites

### Using the Generator:
```csharp
// In SpriteExpressionGenerator component:
spriteSize = 32;                    // Change sprite size
outlineColor = Color.black;         // Change outline color
fillColor = Color.white;            // Change fill color
backgroundColor = Color.clear;      // Change background
```

### Manual Sprite Creation:
1. **Use Piskel** (free online): https://www.piskelapp.com/
2. **Create 32x32px sprites**
3. **Follow the design guide** in `SpriteDesignGuide.md`
4. **Export as PNG** with transparency

## 🎯 Expression Examples

### Scared Expression (At Spawner):
- **Eyes**: Wide, worried with raised eyebrows
- **Mouth**: Small, trembling
- **Effect**: Gentle shaking

### Screaming Expression (While Falling):
- **Eyes**: Squinted or closed
- **Mouth**: Wide open, screaming
- **Effect**: Tears falling

### Relaxed Expression (When Settled):
- **Eyes**: Normal, happy
- **Mouth**: Happy smile
- **Effect**: No special effects

## 🔧 Troubleshooting

### Sprites Not Showing:
1. Check SpriteRenderer references
2. Verify sprites are assigned
3. Check sprite import settings:
   - Filter Mode: Point (no filter)
   - Compression: None
   - Max Size: 32

### Expressions Not Changing:
1. Verify BoxFacialExpressions component
2. Check BoxState component
3. Ensure sprites are assigned
4. Test in Play mode

### Performance Issues:
1. Reduce sprite size to 16x16px
2. Disable visual effects on mobile
3. Use sprite atlases
4. Minimize color palette

## 📱 Mobile Optimization

```csharp
// In BoxFacialExpressions component:
if (Application.platform == RuntimePlatform.Android || 
    Application.platform == RuntimePlatform.IPhonePlayer)
{
    useShakeOnScared = false;    // Disable shaking
    useTearsOnFalling = false;   // Disable tears
    useBlinking = false;         // Disable blinking
}
```

## 🎮 Testing

### In Unity Editor:
1. **Enter Play mode**
2. **Spawn boxes** - should show scared expression
3. **Drop boxes** - should show screaming expression
4. **Land boxes** - should show relaxed expression

### On Device:
1. **Build for Android/iOS**
2. **Test all expressions**
3. **Check performance**
4. **Adjust settings if needed**

## 🎨 Advanced Customization

### Different Expressions per Box Type:
```csharp
[Header("Box-Specific Expressions")]
public Sprite[] scaredEyesVariations;
public Sprite[] screamingEyesVariations;
public Sprite[] relaxedEyesVariations;
```

### Animation Overrides:
```csharp
// In BoxAnimationController:
public void OverrideExpression(ExpressionType type)
{
    BoxFacialExpressions facial = GetComponent<BoxFacialExpressions>();
    switch (type)
    {
        case ExpressionType.Scared:
            facial.ForceScaredExpression();
            break;
        case ExpressionType.Screaming:
            facial.ForceScreamingExpression();
            break;
        case ExpressionType.Relaxed:
            facial.ForceRelaxedExpression();
            break;
    }
}
```

## 📋 Checklist

- [ ] Generated all sprites
- [ ] Set up box prefab structure
- [ ] Added all components
- [ ] Assigned sprites to slots
- [ ] Created tear prefab
- [ ] Tested in Play mode
- [ ] Built and tested on device
- [ ] Optimized for mobile

Your boxes will now have expressive, toonish faces that change based on their situation! 😄🎮✨ 