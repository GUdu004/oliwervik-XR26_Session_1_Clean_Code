# Input Not Working - Troubleshooting Guide

## 🚨 Common Input Issues & Solutions

### **Most Likely Causes:**

## 1. **Missing Components** ⚠️
**Problem**: PlayerController can't find required input components
**Solution**: 
```
1. Select Player GameObject
2. Add PlayerSetupHelper script
3. Right-click → "Setup Player Components"
4. Verify all components are added
```

## 2. **Input Disabled State** ⚠️
**Problem**: Input is disabled in PlayerInputHandler or PlayerController
**Check**:
- PlayerInputHandler: `enableInput = true`
- PlayerInputHandler: `InputEnabled = true`
- PlayerController: `enableInput = true`
- PlayerController: `enableMovement = true`

## 3. **Cursor Lock Issues** ⚠️
**Problem**: Mouse input not working due to cursor state
**Solution**:
```csharp
// In PlayerController Inspector:
lockCursor = true
hideCursor = true
```

## 4. **Game Focus Issues** ⚠️
**Problem**: Unity game window doesn't have focus
**Solution**: Click on Game window to ensure focus

## 5. **Component Dependencies Missing** ⚠️
**Problem**: PlayerController dependencies not resolved
**Required Components on Player GameObject**:
- ✅ PlayerController
- ✅ PlayerInputHandler
- ✅ PlayerMovement
- ✅ HealthComponent
- ✅ ScoreComponent
- ✅ CollisionHandler
- ✅ Rigidbody
- ✅ Collider

---

## 🔧 **Quick Diagnostic Steps**

### **Step 1: Add Input Debugger**
1. **Add InputDebugger script** to any GameObject in scene
2. **Run the game**
3. **Check Console** for input detection messages
4. **Look at Game window** for on-screen input display

### **Step 2: Check Console Messages**
Look for these messages:
- ✅ `"PlayerInputHandler initialized"`
- ✅ `"PlayerController initialized successfully"`
- ❌ `"PlayerController: IInputHandler component not found!"`

### **Step 3: Test Raw Input**
In Console, you should see:
```
[RAW INPUT] Movement: W=True A=False S=False D=False, Actions: Space=True...
```

### **Step 4: Component Verification**
In InputDebugger on-screen display:
- ✅ `PlayerController: ✅`
- ✅ `InputHandler: ✅`
- ✅ `Input Enabled: True`

---

## 🛠️ **Manual Fixes**

### **Fix 1: Reset Component Setup**
```csharp
// On Player GameObject, remove all scripts and re-add:
1. Remove PlayerController, PlayerInputHandler
2. Add PlayerSetupHelper
3. Use "Setup Player Components" context menu
4. Remove PlayerSetupHelper when done
```

### **Fix 2: Force Enable Input**
```csharp
// In InputDebugger context menu:
Right-click InputDebugger → "Force Enable All Input"
```

### **Fix 3: Manual Component Check**
```csharp
// Verify in Player GameObject Inspector:
PlayerInputHandler:
  ✅ Enable Input = true
  ✅ Jump Key = Space
  ✅ Restart Key = R
  ✅ Pause Key = Escape
  ✅ Interact Key = E

PlayerController:
  ✅ Enable Input = true
  ✅ Enable Movement = true
  ✅ Lock Cursor = true
```

### **Fix 4: Input Manager Settings**
```
1. Edit → Project Settings → Input Manager
2. Verify these axes exist:
   - Horizontal (A/D keys)
   - Vertical (W/S keys)  
   - Mouse X
   - Mouse Y
```

---

## 🎯 **Expected Input Behavior**

### **Movement (WASD)**:
- **W**: Move forward
- **A**: Move left  
- **S**: Move backward
- **D**: Move right

### **Mouse**:
- **Mouse Movement**: Rotate player view
- **Cursor**: Should be locked and hidden

### **Actions**:
- **Space**: Jump
- **R**: Restart game
- **Escape**: Pause game
- **E**: Interact

---

## 🚨 **Emergency Solutions**

### **Solution A: Use Original Player.cs**
If the new modular system isn't working:
1. **Disable PlayerController** component
2. **Add original Player.cs** script
3. **Configure references** in Inspector
4. **Add GameManager reference**

### **Solution B: Component Reset**
```csharp
// Complete reset procedure:
1. Remove ALL scripts from Player GameObject
2. Add only: Rigidbody, CapsuleCollider
3. Add PlayerSetupHelper
4. Use "Setup Player Components"
5. Test with InputDebugger
```

### **Solution C: New GameObject**
```csharp
// Start fresh:
1. Create new GameObject named "Player"
2. Add PlayerSetupHelper script
3. Use "Setup Player Components" 
4. Copy over any custom settings
5. Delete old player GameObject
```

---

## 📊 **Debugging Checklist**

**Before asking for help, verify:**

- [ ] All required components present on Player GameObject
- [ ] InputDebugger shows input detection in Console
- [ ] Game window has focus (click on it)
- [ ] No compilation errors in Console
- [ ] PlayerController shows "initialized successfully" message
- [ ] InputHandler shows "Input Enabled: True" in debugger
- [ ] Raw input shows key presses in debugger output

**If all above are ✅ but input still not working:**
- Check Unity Input Manager settings
- Restart Unity Editor
- Check for conflicting input scripts
- Verify Unity version compatibility

---

## 💡 **Pro Tips**

1. **Always use InputDebugger** when having input issues
2. **Check Console messages** for component initialization
3. **Use context menu tests** for quick verification
4. **PlayerSetupHelper** solves 90% of component issues
5. **Focus Game window** before testing input

The modular input system is more robust than the old monolithic approach, but requires proper component setup. Once configured correctly, it provides much better separation of concerns and is easier to debug and extend!
