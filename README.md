# KitchenMastersDemo
Bigger Games Case Study

This is a demo version of **Kitchen Master**, made with **Unity 6.0 (6000.0.58f2) (Android)**.  
The main scene is called `Game` and it is located in `Assets/Scenes`.

---

## 🎮 Game Overview

The game has **three difficulty levels**: Easy, Medium, and Hard.  
Each difficulty level has its **own set of procedural generation rules**, stored inside a **Scriptable Object** called  
`ProceduralGenerationInfosSO`, located in `Assets/Resources/Datas`.

### 📋 These rules include:
- `Difficulty Type`
- `Usable Grid Sizes` → Grid size can vary depending on difficulty
- `Special Item Spawn Possibility`
- `Block Spawn Possibility`
- `Hybrid Block Spawn Possibility`
- `Min Block Spacing`
- `Min Hybrid Block Spacing`

When a level is generated, these rules are used to **create the grid procedurally**.

---

## 🍬 Game Items

In the game, we have:
- 🟦 **4 Candy Types**
- 🧱 **2 Block Types** → Cannot move or match, but take damage from nearby explosions
- 🟢 **1 Hybrid Block** → Can move but cannot match
- 💣 **2 Special Items:** Bomb and Rocket → Each has a unique explosion area and can combo together

All item prefabs are located in:  
`Assets/Game/Prefabs`

---

## 🕹️ Gameplay

When the game starts:
1. A **random difficulty** is selected.
2. A **level grid** is generated using the procedural rules.
3. **Candies are placed using pattern-based logic** to ensure the level is playable right from the start.

The game continues **endlessly**, allowing players to keep playing without interruption.

---

## 🧾 Extra Info

- Every generated level is **printed as JSON** in the Unity console.  
- The selected difficulty level is also **logged to the console**.

---


> ⚠️This project was created as part of a case study for Bigger Games.

