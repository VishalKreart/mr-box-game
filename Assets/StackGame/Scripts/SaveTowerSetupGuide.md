# 🎯 Save Tower Feature - Setup Guide

## 📋 Overview
The Save Tower feature allows players to watch rewarded ads to rebalance their falling tower, providing a second chance before game over. This dramatically improves user engagement and ad revenue.

## 🛠️ Unity Setup Instructions

### Step 1: Add Scripts to Scene
1. **Add TowerStabilityMonitor**:
   - Create an empty GameObject named "TowerStabilityMonitor"
   - Add the `TowerStabilityMonitor.cs` script
   - Configure settings in inspector:
     - Danger Threshold: `1.5f`
     - Time Before Warning: `1.0f`
     - Show Debug Gizmos: `true` (for testing)

2. **Add TowerRebalancer**:
   - Create an empty GameObject named "TowerRebalancer"
   - Add the `TowerRebalancer.cs` script
   - Configure settings:
     - Centering Force: `2.0f`
     - Stabilization Duration: `3.0f`
     - Show Rebalance Effect: `true`

### Step 2: Create Save Tower UI
1. **Create UI Canvas** (if not exists):
   - Right-click in Hierarchy → UI → Canvas
   - Set Canvas Scaler to "Scale With Screen Size"

2. **Create Save Tower Panel**:
   - Right-click Canvas → UI → Panel
   - Name it "SaveTowerPanel"
   - Set anchor to center
   - Add background with warning colors

3. **Add UI Elements**:
   ```
   SaveTowerPanel/
   ├── WarningText (TextMeshPro)
   ├── TimerText (TextMeshPro)
   ├── StabilitySlider (Slider)
   ├── ButtonContainer/
   │   ├── WatchAdButton (Button)
   │   └── SkipButton (Button)
   └── WarningIcon (Image)
   ```

4. **Add SaveTowerUI Script**:
   - Add `SaveTowerUI.cs` to SaveTowerPanel
   - Assign all UI references in inspector
   - Configure warning messages and effects

### Step 3: Update Existing Components

1. **Replace FallDetector**:
   - The existing `FallDetector.cs` has been updated
   - Make sure "Enable Save Tower Feature" is checked
   - Set "Max Save Offers Per Game" to `1`

2. **Update GameManager** (if needed):
   - Ensure GameManager reference is assigned in FallDetector
   - No code changes required

### Step 4: Configure MonetizationManager
- The `ShowSaveTowerAd()` method has been added
- No additional setup required
- Ensure IronSource is properly configured

## 🎮 How It Works

### User Flow:
1. **Tower becomes unstable** → TowerStabilityMonitor detects
2. **Box starts falling** → FallDetector intercepts
3. **Save Tower popup appears** → Time slows to 0.3x
4. **User watches ad** → Tower rebalances with visual effects
5. **Game continues** → Player gets second chance

### Technical Flow:
```
TowerStabilityMonitor → Detects instability
         ↓
FallDetector → Pauses falling box
         ↓
SaveTowerUI → Shows popup with timer
         ↓
MonetizationManager → Shows rewarded ad
         ↓
TowerRebalancer → Applies physics correction
         ↓
Game continues normally
```

## ⚙️ Configuration Options

### TowerStabilityMonitor Settings:
- **Danger Threshold**: How far from center before unstable (1.5f recommended)
- **Time Before Warning**: Delay before showing warning (1.0f recommended)
- **Check Interval**: How often to check stability (0.1f recommended)
- **Minimum Boxes**: Don't check with fewer boxes (3 recommended)

### TowerRebalancer Settings:
- **Centering Force**: Strength of rebalancing (2.0f recommended)
- **Stabilization Duration**: How long effect lasts (3.0f recommended)
- **Reduced Gravity**: Temporary gravity during rebalance (-5.0f recommended)

### FallDetector Settings:
- **Enable Save Tower Feature**: Master toggle (true recommended)
- **Max Save Offers Per Game**: Limit offers per session (1 recommended)

## 🎨 UI Customization

### Visual Effects:
- Screen shake during warning
- Color transitions (red warning → green success)
- Stability slider showing tower health
- Countdown timer for urgency

### Customizable Elements:
- Warning messages array
- Warning colors and effects
- Shake intensity and duration
- Timer display format

## 🐛 Testing & Debugging

### Debug Features:
1. **TowerStabilityMonitor**: Enable "Show Debug Gizmos"
2. **Console Logs**: Watch for stability and rebalancing messages
3. **Visual Indicators**: Boxes change color during rebalancing

### Test Scenarios:
1. Build unstable tower → Should trigger warning
2. Watch ad → Tower should rebalance with effects
3. Skip offer → Should proceed to game over
4. Multiple saves → Should respect max offers limit

## 🚀 Performance Considerations

### Optimizations:
- Stability checking runs at 0.1s intervals (configurable)
- Physics modifications are temporary
- UI elements are pooled/reused
- Effects use coroutines for smooth performance

### Memory Management:
- Scripts automatically clean up coroutines
- UI elements are properly destroyed
- No memory leaks in physics modifications

## 📊 Analytics & Metrics

### Track These Events:
- `save_tower_offered` - When popup is shown
- `save_tower_ad_watched` - When ad is completed
- `save_tower_skipped` - When user skips
- `tower_rebalanced` - When rebalancing succeeds
- `boxes_affected` - Number of boxes rebalanced

### Revenue Impact:
- Premium ad placement at critical moment
- High completion rates due to user investment
- Increased session length and retention

## 🎯 Best Practices

1. **Limit Offers**: Don't overwhelm users (1 per game recommended)
2. **Clear Messaging**: Make value proposition obvious
3. **Smooth Effects**: Ensure rebalancing feels natural
4. **Fallback Handling**: Always have skip option available
5. **Performance**: Monitor frame rate during effects

## 🔧 Troubleshooting

### Common Issues:
- **No popup shows**: Check TowerStabilityMonitor is in scene
- **Ad doesn't play**: Verify IronSource setup and internet
- **Tower doesn't rebalance**: Ensure TowerRebalancer is configured
- **UI not responsive**: Check Canvas and EventSystem setup

### Debug Steps:
1. Enable debug gizmos in TowerStabilityMonitor
2. Check console for error messages
3. Verify all script references are assigned
4. Test with Unity Remote for mobile behavior

---

## 🎉 Congratulations!
Your Save Tower feature is now ready! This will significantly boost user engagement and ad revenue while providing a smooth, physics-based gameplay experience.
