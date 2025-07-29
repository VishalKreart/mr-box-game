# 📦 Box Creation Guide

## 🎯 How to Create Different Box Dimensions

### **Method 1: Create Multiple Box Prefabs (Recommended)**

#### **Step 1: Create Base Box Prefab**
1. **Create a new GameObject** in your scene
2. **Add components**:
   - `SpriteRenderer` (with your box sprite)
   - `Rigidbody2D` (for physics)
   - `BoxCollider2D` (for collision)
   - `BoxState` script
   - Tag it as "Box"

#### **Step 2: Create Different Sizes**

**📏 Normal Box (1x1)**
- Scale: (1, 1, 1)
- Width: 1 unit
- Height: 1 unit
- Color: White
- Points: 1

**📏 Wide Box (1.5x1)**
- Scale: (1.5, 1, 1)
- Width: 1.5 units
- Height: 1 unit
- Color: Blue
- Points: 2 (harder to place, more points)

**📏 Tall Box (1x1.5)**
- Scale: (1, 1.5, 1)
- Width: 1 unit
- Height: 1.5 units
- Color: Green
- Points: 2

**📏 Small Box (0.7x0.7)**
- Scale: (0.7, 0.7, 1)
- Width: 0.7 units
- Height: 0.7 units
- Color: Yellow
- Points: 3 (very hard to place, high reward)

**📏 Large Box (1.2x1.2)**
- Scale: (1.2, 1.2, 1)
- Width: 1.2 units
- Height: 1.2 units
- Color: Red
- Points: 1 (easier to place, normal points)

#### **Step 3: Create Prefabs**
1. **Drag each box** to your Prefabs folder
2. **Name them** clearly: "Box_Normal", "Box_Wide", "Box_Tall", etc.

---

## **🎮 Setting Up BoxVariations System**

### **Step 1: Add BoxVariations Script**
1. **Create empty GameObject** named "BoxVariations"
2. **Add BoxVariations script**
3. **Set up variations** in Inspector:

```
Box Variations:
├── Element 0:
│   ├── Box Name: "Normal"
│   ├── Box Prefab: Box_Normal
│   ├── Width: 1.0
│   ├── Height: 1.0
│   ├── Spawn Weight: 5.0
│   ├── Box Color: White
│   └── Point Value: 1
├── Element 1:
│   ├── Box Name: "Wide"
│   ├── Box Prefab: Box_Wide
│   ├── Width: 1.5
│   ├── Height: 1.0
│   ├── Spawn Weight: 2.0
│   ├── Box Color: Blue
│   └── Point Value: 2
├── Element 2:
│   ├── Box Name: "Tall"
│   ├── Box Prefab: Box_Tall
│   ├── Width: 1.0
│   ├── Height: 1.5
│   ├── Spawn Weight: 2.0
│   ├── Box Color: Green
│   └── Point Value: 2
├── Element 3:
│   ├── Box Name: "Small"
│   ├── Box Prefab: Box_Small
│   ├── Width: 0.7
│   ├── Height: 0.7
│   ├── Spawn Weight: 1.0
│   ├── Box Color: Yellow
│   └── Point Value: 3
└── Element 4:
    ├── Box Name: "Large"
    ├── Box Prefab: Box_Large
    ├── Width: 1.2
    ├── Height: 1.2
    ├── Spawn Weight: 3.0
    ├── Box Color: Red
    └── Point Value: 1
```

### **Step 2: Update BoxSpawner**
1. **Select BoxSpawner GameObject**
2. **Drag BoxVariations** to the "Box Variations" field
3. **Enable** "Use Difficulty Progression"
4. **Enable** "Use Weighted Spawning"

---

## **🎯 Advanced Box Types**

### **Special Boxes (Optional)**

**💎 Golden Box**
- Scale: (1, 1, 1)
- Color: Gold/Yellow
- Points: 5
- Spawn Weight: 0.5 (rare)
- Effect: Double points when stacked

**💥 Explosive Box**
- Scale: (1, 1, 1)
- Color: Orange/Red
- Points: 2
- Spawn Weight: 1.0
- Effect: Removes nearby boxes when dropped

**🔄 Rotating Box**
- Scale: (1, 1, 1)
- Color: Purple
- Points: 3
- Spawn Weight: 1.5
- Effect: Rotates while falling

---

## **⚙️ Difficulty Progression**

### **Early Game (Score 0-5)**
- **70%** Normal boxes
- **20%** Wide boxes
- **10%** Large boxes

### **Mid Game (Score 5-15)**
- **50%** Normal boxes
- **25%** Wide boxes
- **15%** Tall boxes
- **10%** Small boxes

### **Late Game (Score 15+)**
- **30%** Normal boxes
- **25%** Wide boxes
- **20%** Tall boxes
- **15%** Small boxes
- **10%** Special boxes

---

## **🎨 Visual Tips**

### **Colors for Different Boxes**
- **Normal**: White/Gray
- **Wide**: Blue
- **Tall**: Green
- **Small**: Yellow
- **Large**: Red
- **Golden**: Gold/Yellow
- **Explosive**: Orange/Red
- **Rotating**: Purple

### **Sprite Variations**
- **Different patterns** for each box type
- **Borders** to make them distinct
- **Icons** on special boxes
- **Gradients** for visual appeal

---

## **🔧 Testing Your Boxes**

### **Quick Test Setup**
1. **Create test scene** with just boxes
2. **Spawn different types** manually
3. **Test physics** and collision
4. **Verify scoring** works correctly
5. **Check difficulty progression**

### **Balance Testing**
- **Easy boxes** should be common early
- **Hard boxes** should be rare but rewarding
- **Progression** should feel natural
- **No impossible situations**

---

## **📊 Box Statistics Example**

| Box Type | Width | Height | Points | Weight | Difficulty |
|----------|-------|--------|--------|--------|------------|
| Normal   | 1.0   | 1.0    | 1      | 5.0    | Easy       |
| Wide     | 1.5   | 1.0    | 2      | 2.0    | Medium     |
| Tall     | 1.0   | 1.5    | 2      | 2.0    | Medium     |
| Small    | 0.7   | 0.7    | 3      | 1.0    | Hard       |
| Large    | 1.2   | 1.2    | 1      | 3.0    | Easy       |

This system gives you complete control over box variety, difficulty, and scoring! 🎮 