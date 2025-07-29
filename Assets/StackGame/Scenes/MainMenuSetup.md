# Main Menu Scene Setup Guide

## 🎮 Creating the Main Menu Scene

### Step 1: Create New Scene
1. **File → New Scene** (or Ctrl+N)
2. **Save as "MainMenu"** in `Assets/StackGame/Scenes/`
3. **Delete the default Main Camera** (we'll use UI only)

### Step 2: Setup Canvas
1. **Right-click in Hierarchy → UI → Canvas**
2. **Set Canvas Scaler** to "Scale With Screen Size"
3. **Reference Resolution**: 1920x1080
4. **Match**: 0.5 (both width and height)

### Step 3: Create UI Elements

#### Title Section:
```
Canvas
├── Title Panel
│   ├── Game Title (TextMeshPro)
│   │   └── Text: "MR. BOX"
│   │   └── Font Size: 72
│   │   └── Color: White
│   └── Subtitle (TextMeshPro)
│       └── Text: "Stack & Survive"
│       └── Font Size: 24
│       └── Color: Light Gray
```

#### Button Section:
```
Canvas
├── Button Panel
│   ├── Play Button
│   │   └── Text: "PLAY"
│   │   └── Font Size: 36
│   │   └── Color: White
│   ├── Settings Button
│   │   └── Text: "SETTINGS"
│   │   └── Font Size: 24
│   │   └── Color: White
│   └── Quit Button
│       └── Text: "QUIT"
│       └── Font Size: 24
│       └── Color: White
```

#### High Score Section:
```
Canvas
├── High Score Panel
│   └── High Score Text (TextMeshPro)
│       └── Text: "High Score: 0"
│       └── Font Size: 18
│       └── Color: Yellow
```

#### Settings Panel (Initially Hidden):
```
Canvas
├── Settings Panel (GameObject - initially inactive)
│   ├── Background (Image - semi-transparent black)
│   ├── Settings Title (TextMeshPro)
│   │   └── Text: "SETTINGS"
│   │   └── Font Size: 48
│   ├── Reset Tutorial Button
│   │   └── Text: "Reset Tutorial"
│   │   └── Font Size: 24
│   └── Close Button
│       └── Text: "X"
│       └── Font Size: 36
```

### Step 4: Add Scripts
1. **Add MainMenuManager script** to an empty GameObject
2. **Connect all UI elements** to the script fields:
   - Play Button → Play Button field
   - Settings Button → Settings Button field
   - Quit Button → Quit Button field
   - High Score Text → High Score Text field
   - Settings Panel → Settings Panel field
   - Close Settings Button → Close Settings Button field
   - Reset Tutorial Button → Reset Tutorial Button field

### Step 5: Build Settings
1. **File → Build Settings**
2. **Add scenes in this order**:
   - MainMenu (index 0)
   - MainScene (index 1)
3. **Set MainMenu as the first scene**

### Step 6: Update Game Over Panel
In your MainScene, update the Game Over Panel to include:
- **Return to Menu Button** (connect to GameManager.ReturnToMainMenu)

## 🎯 Layout Suggestions

### Vertical Layout:
- **Top**: Title (20% of screen)
- **Middle**: Buttons (60% of screen)
- **Bottom**: High Score (20% of screen)

### Button Spacing:
- **Play Button**: Large, prominent
- **Settings Button**: Medium size
- **Quit Button**: Medium size
- **Spacing**: 20-30 pixels between buttons

### Colors:
- **Background**: Dark blue/black gradient
- **Buttons**: Blue with white text
- **Title**: White with shadow
- **High Score**: Yellow/gold

## 🔧 Testing
1. **Play from MainMenu scene** → Should show menu
2. **Click Play** → Should load MainScene
3. **Game Over** → Should show return to menu option
4. **Return to Menu** → Should show updated high score

## 🎨 Optional Enhancements
- **Background image** of stacked boxes
- **Particle effects** behind title
- **Button hover animations**
- **Background music**
- **Sound effects** for button clicks 