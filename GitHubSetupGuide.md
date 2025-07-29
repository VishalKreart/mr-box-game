# 🚀 GitHub Setup Guide for Mr. Box Project

## 📋 Prerequisites

- **Git** installed on your computer
- **GitHub account** (free at github.com)
- **Unity project** ready to commit

---

## 🎯 Step-by-Step Setup

### **Step 1: Install Git (if not already installed)**

#### **On Windows:**
1. Download from: https://git-scm.com/download/win
2. Install with default settings
3. Restart your computer

#### **On Mac:**
```bash
# Install via Homebrew
brew install git

# Or download from: https://git-scm.com/download/mac
```

#### **On Linux:**
```bash
sudo apt-get install git  # Ubuntu/Debian
sudo yum install git      # CentOS/RHEL
```

### **Step 2: Configure Git**

Open terminal/command prompt and run:
```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

### **Step 3: Create GitHub Repository**

1. **Go to GitHub.com** and sign in
2. **Click "New repository"** (green button)
3. **Fill in details**:
   - **Repository name**: `mr-box-game`
   - **Description**: `A fun 2D stacking game built with Unity`
   - **Visibility**: Public (or Private if you prefer)
   - **Don't initialize** with README (we'll do this locally)
4. **Click "Create repository"**

### **Step 4: Initialize Local Git Repository**

Open terminal/command prompt in your project folder:
```bash
# Navigate to your Unity project folder
cd "path/to/your/Mr. Box project"

# Initialize git repository
git init

# Add all files (except those in .gitignore)
git add .

# Make first commit
git commit -m "Initial commit: Mr. Box stacking game"

# Add GitHub as remote origin
git remote add origin https://github.com/yourusername/mr-box-game.git

# Push to GitHub
git branch -M main
git push -u origin main
```

---

## 🔄 Daily Workflow

### **Making Changes and Committing**

```bash
# Check what files have changed
git status

# Add specific files
git add Assets/StackGame/Scripts/NewScript.cs

# Or add all changes
git add .

# Commit with descriptive message
git commit -m "feat: add new power-up system"

# Push to GitHub
git push
```

### **Working from Different Computers**

#### **First time on new computer:**
```bash
# Clone the repository
git clone https://github.com/yourusername/mr-box-game.git

# Open in Unity
# Unity will automatically import the project
```

#### **Subsequent times:**
```bash
# Pull latest changes
git pull

# Make your changes
# Commit and push as usual
```

---

## 📁 What Gets Committed

### **✅ Included Files:**
- All your scripts (`.cs` files)
- Unity scenes (`.unity` files)
- Prefabs (`.prefab` files)
- Project settings
- Assets (sprites, sounds, etc.)
- README and documentation

### **❌ Excluded Files (via .gitignore):**
- `Library/` folder (Unity cache)
- `Temp/` folder (temporary files)
- `Logs/` folder (log files)
- `UserSettings/` folder (personal settings)
- Build files (`.apk`, `.app`, etc.)
- IDE files (`.vs/`, `.idea/`, etc.)

---

## 🛠️ Useful Git Commands

### **Basic Commands:**
```bash
git status          # Check what files have changed
git add .           # Stage all changes
git commit -m "message"  # Commit changes
git push            # Upload to GitHub
git pull            # Download from GitHub
git log             # View commit history
```

### **Advanced Commands:**
```bash
git branch          # List branches
git checkout -b feature-name  # Create new branch
git merge branch-name         # Merge branch
git stash           # Save changes temporarily
git stash pop       # Restore saved changes
```

---

## 🔧 Troubleshooting

### **Common Issues:**

#### **"Repository not found"**
- Check your GitHub username in the URL
- Make sure you have access to the repository

#### **"Permission denied"**
- Set up SSH keys or use GitHub CLI
- Or use HTTPS with personal access token

#### **"Large file" error**
- Unity generates large files sometimes
- Check `.gitignore` is working properly
- Use Git LFS for large assets if needed

#### **"Merge conflicts"**
- Git will show conflict markers in files
- Edit files to resolve conflicts
- Commit the resolved files

---

## 📱 Working from Mobile/Tablet

### **GitHub Mobile App:**
1. **Download GitHub app** from App Store/Play Store
2. **Sign in** with your GitHub account
3. **View repository** and files
4. **Edit README** and other text files
5. **Create issues** and pull requests

### **GitHub Web Interface:**
1. **Go to github.com** on any device
2. **Navigate to your repository**
3. **Edit files** directly in browser
4. **Commit changes** through web interface

---

## 🔒 Security Best Practices

### **Personal Access Tokens:**
1. **Go to GitHub Settings** → **Developer settings**
2. **Personal access tokens** → **Tokens (classic)**
3. **Generate new token**
4. **Select scopes**: `repo`, `workflow`
5. **Copy token** and use as password

### **SSH Keys (Recommended):**
```bash
# Generate SSH key
ssh-keygen -t ed25519 -C "your.email@example.com"

# Add to GitHub
# Copy public key to GitHub Settings → SSH and GPG keys
```

---

## 📊 GitHub Features for Your Project

### **Issues:**
- **Bug reports**
- **Feature requests**
- **Task tracking**

### **Projects:**
- **Kanban board** for development
- **Track progress** on features
- **Organize tasks**

### **Actions:**
- **Automated builds**
- **Testing**
- **Deployment**

### **Wiki:**
- **Documentation**
- **Tutorials**
- **Development guides**

---

## 🎯 Next Steps

1. **Set up GitHub repository** (follow steps above)
2. **Create first commit** with your current project
3. **Set up branch protection** (optional)
4. **Create issues** for future features
5. **Invite collaborators** (if working with others)

---

## 📞 Need Help?

- **GitHub Help**: https://help.github.com/
- **Git Documentation**: https://git-scm.com/doc
- **Unity Git Integration**: Unity's built-in Git support
- **Community**: Stack Overflow, Unity Forums

---

**Happy Coding! 🚀🎮** 