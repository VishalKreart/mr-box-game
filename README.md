# 🎮 Mr. Box - Stacking Game

A fun and addictive 2D mobile stacking game built with Unity where players drop boxes to build the tallest tower possible!

## 🎯 Game Overview

**Mr. Box** is a challenging stacking game where players:
- Drag boxes left and right to position them
- Drop boxes to stack them on top of each other
- Try to build the highest tower without boxes falling off
- Experience increasing difficulty with different box types
- Compete for high scores

## 🚀 Features

### **Core Gameplay**
- ✅ **Drag & Drop Mechanics** - Drag anywhere on screen to move boxes
- ✅ **Smooth Camera Movement** - Camera follows the stack intelligently
- ✅ **Multiple Box Types** - Different sizes and colors for variety
- ✅ **Scoring System** - Points for each successfully stacked box
- ✅ **High Score Tracking** - Persistent high scores

### **Visual Enhancements**
- ✅ **Box Borders** - Clear visual separation between boxes
- ✅ **Color-coded Boxes** - Different colors for different box types
- ✅ **Smooth Animations** - Professional visual effects
- ✅ **Camera Zoom** - Zoom out on game over to show entire tower

### **User Experience**
- ✅ **Tutorial System** - First-time user guidance
- ✅ **Main Menu** - Professional game flow
- ✅ **Settings Panel** - Tutorial reset functionality
- ✅ **Mobile Optimized** - Touch controls and responsive design

## 🛠️ Technical Details

### **Unity Version**
- Unity 2022.3 LTS or later
- 2D URP (Universal Render Pipeline)

### **Platforms**
- Android
- iOS
- Windows/Mac/Linux (for development)

### **Key Scripts**
- `BoxSpawner.cs` - Manages box spawning and game flow
- `CameraStackFollow.cs` - Intelligent camera movement
- `DragAnywhereToMoveBox.cs` - Touch/mouse input handling
- `BoxVariations.cs` - Box type management system
- `SimpleBoxBorder.cs` - Visual enhancement system
- `ScoreManager.cs` - Score tracking and persistence
- `TutorialManager.cs` - Tutorial system
- `MainMenuManager.cs` - Menu system

## 📦 Box Types

| Box Type | Size | Color | Points | Difficulty |
|----------|------|-------|--------|------------|
| Normal   | 1x1  | White | 1      | Easy       |
| Wide     | 1.5x1| Blue  | 2      | Medium     |
| Tall     | 1x1.5| Green | 2      | Medium     |
| Small    | 0.7x0.7| Yellow | 3   | Hard       |
| Large    | 1.2x1.2| Red | 1    | Easy       |

## 🎮 Controls

### **Desktop**
- **Mouse Drag** - Move box left/right
- **Mouse Release** - Drop box

### **Mobile**
- **Touch & Drag** - Move box left/right
- **Touch Release** - Drop box

## 🚀 Getting Started

### **Prerequisites**
- Unity 2022.3 LTS or later
- Git

### **Installation**
1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/mr-box-game.git
   cd mr-box-game
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Click "Open" → Select the project folder
   - Wait for Unity to import the project

3. **Open the Main Scene**
   - Navigate to `Assets/StackGame/Scenes/`
   - Open `MainScene.unity`

4. **Play the Game**
   - Click the Play button in Unity
   - Or build for your target platform

## 📁 Project Structure

```
Assets/
├── StackGame/
│   ├── Scripts/           # C# scripts
│   ├── Scenes/           # Unity scenes
│   ├── Prefabs/          # Game object prefabs
│   └── Animation/        # Animation files
├── ProjectSettings/      # Unity project settings
└── Packages/            # Unity packages
```

## 🔧 Development Setup

### **Setting Up Box Variations**
1. Create different box prefabs with varying sizes
2. Add `SimpleBoxBorder` script to each prefab
3. Configure `BoxVariations` component with box types
4. Set colors in SpriteRenderer components

### **Adding New Features**
1. Create new scripts in `Assets/StackGame/Scripts/`
2. Follow the existing naming conventions
3. Update this README with new features
4. Test thoroughly before committing

## 🎨 Customization

### **Adding New Box Types**
1. Create new box prefab
2. Add to `BoxVariations` array
3. Configure size, color, and point value
4. Set spawn weight for frequency

### **Modifying Visual Style**
1. Update colors in `SimpleBoxBorder` components
2. Adjust border width and shadow settings
3. Modify camera settings in `CameraStackFollow`
4. Update UI elements in scenes

## 📱 Building for Mobile

### **Android**
1. **File → Build Settings**
2. **Switch Platform** to Android
3. **Player Settings** → Configure app details
4. **Build** → Select output location

### **iOS**
1. **File → Build Settings**
2. **Switch Platform** to iOS
3. **Player Settings** → Configure app details
4. **Build** → Select output location

## 🤝 Contributing

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Make your changes**
4. **Test thoroughly**
5. **Commit your changes**
   ```bash
   git commit -m 'Add amazing feature'
   ```
6. **Push to the branch**
   ```bash
   git push origin feature/amazing-feature
   ```
7. **Open a Pull Request**

## 📝 Commit Guidelines

Use clear, descriptive commit messages:
- `feat: add new box type`
- `fix: resolve camera movement issue`
- `docs: update README`
- `style: improve code formatting`
- `refactor: simplify box spawning logic`

## 🐛 Known Issues

- None currently reported

## 🚀 Future Features

- [ ] Power-ups and special boxes
- [ ] Multiple game modes
- [ ] Sound effects and music
- [ ] Particle effects
- [ ] Achievement system
- [ ] Leaderboards
- [ ] Multiplayer mode

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- Email: your.email@example.com

## 🙏 Acknowledgments

- Unity Technologies for the amazing game engine
- The Unity community for helpful resources
- All playtesters who provided feedback

---

**Happy Stacking! 🎮📦** 