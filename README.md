# LakbayTala

Exploration/travel-themed 2D game prototype — menu, levels, lore, and quiz systems.

---

## Open on another PC (clone and play)

1. **Clone the repo** (or copy the project folder).
2. **Install Unity 6** (2023 LTS or Unity 6000.x). This project uses **Unity 6000.3.6f1**; similar 6.x versions usually work.
3. **Open the project** in Unity: *Add* → open the folder that contains `Assets`, `ProjectSettings`, and `Packages`.
4. Wait for Unity to import and compile (first open creates `Library`, `Temp`, etc. — these are not in the repo and are machine-specific).
5. **Build Settings:** *File → Build Settings*. Add **MenuScene** and **GameScene** (or your menu/game scenes) to *Scenes In Build*, with the menu at index 0.
6. Press **Play** from the menu scene.

No need to commit or copy `Library`, `Temp`, `Obj`, or `UserSettings` — the `.gitignore` excludes them so the project stays portable; Unity regenerates them on each machine.

---

## Quick setup (menu + game)

- **Menu scene:** Add **UIManager** and **LakbayTalaGameMenuController** to your Canvas. Drag your buttons (Play, Leaderboard, Settings, etc.) and panels into the controller in the Inspector. See **Game Scene Name** on the controller for the scene loaded when pressing Play.
- **Game scene:** Add **LoreManager**, **LoreCardController**, **QuizController** and their UI panels if you use lore/quiz.
- **Detailed steps:** [Assets/Scripts/UI/SETUP_STEP_BY_STEP.md](Assets/Scripts/UI/SETUP_STEP_BY_STEP.md) and [Assets/Scripts/PROJECT_OVERVIEW.md](Assets/Scripts/PROJECT_OVERVIEW.md).

---

## Repo layout

| Folder / file        | Purpose |
|----------------------|--------|
| `Assets/`            | Scenes, scripts, art, prefabs, UI |
| `Packages/`           | Package manifest (Unity restores packages from here) |
| `ProjectSettings/`   | Unity project and editor settings |

---

## Status

- **Version:** v1 (work in progress)
- **Focus:** Menu and game scenes, lore, quiz, leaderboard, configurable mechanics

---

## License and contact

- **License:** MIT — see [LICENSE](LICENSE).
- **Contributing:** See CONTRIBUTING.md for branch naming and PR guidelines.
- **Contact:** IanKallelOcumen
