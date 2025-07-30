# Animator Controller Setup Guide

## How to Assign Animator Controller in Inspector

### Step 1: Select Your Box Prefab
1. **In the Project window**, find your box prefab
2. **Double-click** to open it in the Inspector
3. **Look for the Animator component** (it should be there if you added it)

### Step 2: Assign the Controller
1. **In the Animator component**, you'll see a field called "Controller"
2. **Click the small circle icon** (Object Picker) next to "Controller"
3. **In the popup window**, search for "BoxAnimatorController"
4. **Click on "BoxAnimatorController"** to select it
5. **The Controller field should now show** "BoxAnimatorController"

### Alternative Method (Drag & Drop):
1. **In the Project window**, navigate to `Assets/StackGame/Animation/`
2. **Find "BoxAnimatorController"**
3. **Drag it directly** onto the "Controller" field in the Animator component

## Visual Guide

```
Animator Component in Inspector:
┌─────────────────────────────────┐
│ Animator                        │
├─────────────────────────────────┤
│ Controller: [BoxAnimatorController] ← Click here
│ Avatar: None                    │
│ Culling Mode: Always Animate    │
│ Update Mode: Normal             │
│ Apply Root Motion: ☐            │
│ Animate Physics: ☐              │
│ Custom Root Transform: ☐        │
│ Root Transform Position (Y):    │
│ Root Transform Rotation (Y):    │
│ Root Transform Position (XZ):   │
│ Root Transform Rotation (XZ):   │
│ Write Defaults: ☑               │
└─────────────────────────────────┘
```

## If You Don't See the Controller

### Option 1: Create the Controller
If the `BoxAnimatorController` doesn't exist:

1. **Right-click in Project window** → Create → Animator Controller
2. **Name it "BoxAnimatorController"**
3. **Double-click to open** the Animator window
4. **Set up the states** as described below

### Option 2: Use the Generated Controller
The `BoxAnimatorController.controller` file should already exist in:
```
Assets/StackGame/Animation/BoxAnimatorController.controller
```

## Setting Up Animator States

### If You Need to Create States:

1. **Open the Animator window** (Window → Animation → Animator)
2. **Select your BoxAnimatorController**
3. **Right-click in the Animator window** → Create State → Empty
4. **Create three states**:
   - `BoxSpawned`
   - `BoxFalling` 
   - `BoxSettled`

### Add Parameters:
1. **In the Parameters tab**, click the "+" button
2. **Add three Trigger parameters**:
   - `Spawned` (Type: Trigger)
   - `Falling` (Type: Trigger)
   - `Settled` (Type: Trigger)

### Add Transitions:
1. **Right-click on a state** → Make Transition
2. **Connect states** based on triggers
3. **Set transition conditions** to the appropriate triggers

## Complete Animator Setup

### States Configuration:
```
BoxSpawned State:
- Motion: None (or your spawned animation clip)
- Speed: 1
- Transitions: To BoxFalling, To BoxSettled

BoxFalling State:
- Motion: None (or your falling animation clip)
- Speed: 1.2
- Transitions: To BoxSettled

BoxSettled State:
- Motion: None (or your settled animation clip)
- Speed: 1
- Transitions: None (final state)
```

### Transitions Configuration:
```
Any State → BoxSpawned:
- Condition: Spawned trigger
- Has Exit Time: ☐
- Transition Duration: 0.25

Any State → BoxFalling:
- Condition: Falling trigger
- Has Exit Time: ☐
- Transition Duration: 0.25

Any State → BoxSettled:
- Condition: Settled trigger
- Has Exit Time: ☐
- Transition Duration: 0.25
```

## Testing the Setup

### In Play Mode:
1. **Enter Play mode**
2. **Spawn a box** - should trigger "Spawned" state
3. **Drop the box** - should trigger "Falling" state
4. **Box lands** - should trigger "Settled" state

### Debug Information:
1. **Open Animator window** while in Play mode
2. **Select a box** in the scene
3. **Watch the state changes** in real-time
4. **Check the Parameters tab** to see triggers being set

## Troubleshooting

### Controller Not Found:
1. **Check the file path**: `Assets/StackGame/Animation/BoxAnimatorController.controller`
2. **Refresh the Project window** (F5)
3. **Reimport the controller** (Right-click → Reimport)

### States Not Changing:
1. **Check the Animator window** in Play mode
2. **Verify triggers are being set** in the Parameters tab
3. **Check transition conditions** are correct
4. **Ensure BoxAnimationController script** is calling the right triggers

### Animation Not Playing:
1. **Check if animation clips** are assigned to states
2. **Verify the clips exist** and are properly imported
3. **Check the Animator's Update Mode** is set to "Normal"
4. **Ensure Write Defaults** is checked

## Quick Checklist

- [ ] Animator component added to box prefab
- [ ] BoxAnimatorController assigned to Controller field
- [ ] Three states created: BoxSpawned, BoxFalling, BoxSettled
- [ ] Three triggers created: Spawned, Falling, Settled
- [ ] Transitions set up from Any State to each state
- [ ] BoxAnimationController script calling the right triggers
- [ ] Tested in Play mode - states change correctly

## Alternative: Simple Setup

If you just want the basic functionality without complex animations:

1. **Assign the BoxAnimatorController** to the Animator component
2. **The controller already has** the basic states and transitions
3. **The BoxAnimationController script** will handle the rest automatically
4. **No additional setup needed** - it should work out of the box!

The Animator Controller is essential for managing the different animation states of your boxes. Once properly assigned, the facial expressions and animations will work automatically based on the box's state! 🎮✨ 