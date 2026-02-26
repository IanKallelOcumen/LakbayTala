# LakbayTala Canvas-Based UI System Architecture

## Overview

The UI system is built around **three primary interface components** with modular, self-contained canvas components, clear layering, and centralized state.

| Component | Purpose | Layer | Key Features |
|-----------|---------|--------|--------------|
| **Main Game Interface** | In-game HUD, minimap, controls | HUD (200) | Tala count, lore triggers, responsive layout |
| **Lore Cards System** | Interactive card-based stories & world-building | Lore (400) | Reveal animations, categories, unlock state |
| **Game Menu Scene** | Main menu, leaderboard, stats, achievements | Menu (300) | Sortable leaderboard, stats, achievements, navigation |

---

## 1. Canvas Layering & Z-Index Management

Layers are managed by `UIManager` with dedicated `Canvas` per layer; each uses `overrideSorting = true` and a fixed `sortingOrder` to prevent rendering conflicts:

| Layer | Sort Order | Contents |
|-------|------------|----------|
| Background | 0 | Full-screen backgrounds, parallax |
| Gameplay | 100 | World-space UI, interactive game elements |
| HUD | 200 | Health, Tala count, minimap, quick actions |
| Menu | 300 | Main menu, leaderboard, stats, achievements, settings |
| Lore | 400 | Lore card panels, story overlays |
| Popup | 500 | Dialogs, confirmations |
| Overlay | 600 | Loading, fade, blocking overlays |

- **Z-index within a layer:** Use sibling order and/or child `Canvas.sortingOrder` for same-layer stacking.
- **Responsive design:** All panels use `RectTransform` anchors and optional `BrawlStarsResponsiveLayout` / safe-area for multiple resolutions and aspect ratios.

---

## 2. Main Game Interface (HUD)

- **Location:** Panels registered as `UIPanelType.HUD`; reparented to `UIManager.hudLayer`.
- **Components:** `MainUIController`, `TalaCountDisplay`, `InGameControlInput`, optional minimap.
- **Integration:** Listens to `UIStateManager` for game state; updates via events (no full refresh).
- **Responsive:** Anchored to safe area; scales for different devices.

---

## 3. Lore Cards System

- **Data:** `LoreData` (ScriptableObject): id, category, title, short/full description, illustration, unlock.
- **Display:** `LoreCardUI` (extends `UIPanel`): card-based layout with image, title, category, description, close button.
- **Behavior:**
  - **Reveal animations:** Scale/fade reveal with `AnimationCurve`; optional particle system on reveal.
  - **Categories:** Character biographies, world-building, location lore (Laguna theme).
  - **Unlock:** Driven by `LoreManager`; unlocked cards are available in the lore panel/grid.
- **Visual:** Optional shader-based transitions (mask/reveal) or particle effects; all driven by `LoreCardUI` and optional effects components.

---

## 4. Game Menu Scene Canvas

- **Entry:** `GameMenuController` wires Play, Leaderboard, Stats, Achievements, Settings, Quit to `UIManager.ShowPanel(type)`.
- **Leaderboard (`LakbayTalaLeaderboardPanel`):**
  - **Sortable columns:** Score, time, level, cultural knowledge, Tala collected (via `LeaderboardSortController` or panel sort API).
  - **Real-time updates:** `LeaderboardService` + refresh; UI binds to service events for incremental updates without full page reload.
  - **Cultural theme:** Rank titles (Diwata, Tikbalang, etc.), Baybayin, Filipino translations.
- **Stats:** `StatsPanel` – player statistics tracking.
- **Achievements:** `AchievementsPanel` – progress and unlock status indicators.
- **Navigation:** Buttons and back-stack via `UIManager.panelStack`; transitions use `UIPanelTransition` for consistent animations.

---

## 5. API & Event Integration

- **Leaderboard:** `LeaderboardService` (and Firebase/backend when configured); panels subscribe to data/refresh events.
- **Central state:** `UIStateManager` holds current screen, modal state, and theme; panels read state and subscribe to change events.
- **Event handling:** Centralized UI events (e.g. `UIStateManager.OnPanelShown`, `LeaderboardService.OnLeaderboardUpdated`) to keep UI consistent and avoid tight coupling.

---

## 6. Transitions & Animations

- **Panel transitions:** `UIPanelTransition` provides fade/scale/slide with easing (AnimationCurve or SimpleTween when available).
- **Lore card reveal:** Scale + fade in `LoreCardUI`; optional particles/shaders.
- **Consistency:** All panels use `UIPanel.Show/Hide` with configurable duration and curves.

---

## 7. Memory & Performance

- **Object pooling:** `UIObjectPool<T>` for frequently created/destroyed elements (e.g. leaderboard rows, lore card instances).
- **Texture atlases:** Shared atlases for icons/buttons to reduce draw calls (configure in project/assets).
- **GC prevention:** Reuse lists, avoid per-frame allocations in hot paths; pool returns objects instead of Destroy.

---

## 8. Cross-Platform & Accessibility

- **Responsive:** Safe areas and anchor presets; optional layout components for different aspect ratios.
- **Haptic/visual feedback:** `UIFeedbackHelper` – haptic on supported platforms; visual (scale/highlight) on button press and hover.
- **Error handling:** `UIErrorHandler` – try/catch around UI operations, fallback rendering, and optional logging.

---

## 9. Save/Load & Persistence

- **Bridge:** `UISaveLoadBridge` – interface to game save/load (local or cloud); persists UI preferences, last menu tab, and unlocked lore IDs.
- **Integration:** Menu and lore systems read/write through this bridge so one system handles storage.

---

## 10. File Map

| File | Role |
|------|------|
| `Core/UIManager.cs` | Layer creation, panel registration, show/hide by type |
| `Core/UIPanel.cs` | Base panel with fade transition, events |
| `Core/UICanvasLayers.cs` | Layer sort-order constants |
| `Core/UIStateManager.cs` | Central UI state and events |
| `Core/UIPanelTransition.cs` | Reusable transition helpers |
| `Core/UIObjectPool.cs` | Generic UI element pool |
| `Core/UISaveLoadBridge.cs` | Persistence for preferences/unlocks |
| `Core/UIErrorHandler.cs` | Error handling and fallbacks |
| `Core/UIFeedbackHelper.cs` | Haptic and visual feedback |
| `Lore/LoreCardUI.cs` | Lore card display and reveal |
| `Menu/GameMenuController.cs` | Menu scene wiring |
| `LakbayTalaLeaderboardPanel.cs` | Leaderboard with sort/stats |
| `Leaderboard/LeaderboardService.cs` | Data and API for leaderboard |

---

## UXML/USS (UI Toolkit)

Where UI Toolkit is used (future or partial adoption), use UXML for structure and USS for theming so visual style stays consistent. Canvas-based UI (uGUI) remains the primary implementation; UXML/USS can be introduced for specific screens (e.g. settings, accessibility) without replacing the canvas stack.
