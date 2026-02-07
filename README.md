# LakbayTala

LakbayTala — v1 unupdate

Short description:
An exploration/travel-themed game prototype. This V1 (unupdate) focuses on revising core mechanics and adding more configurable settings rather than large feature additions. The goal of this iteration is to make gameplay systems clearer and easier to tweak.

Status:
- Version: v1 (unupdate)
- State: Work-in-progress / Alpha
- Main focus: mechanics refinement and settings overhaul

Core features:
- Map exploration and location discovery
- Basic player progression and stats
- Configurable settings for gameplay balance (difficulty, encounter rates, resource spawn, etc.)
- Modular systems intended for easy iteration

What changed in this v1 unupdate:
- Reworked core mechanics to centralize setting-driven behavior
- Emphasis on making parameters adjustable via configuration files or in-game menus
- Cleaned up code paths for movement, encounters, and resource handling
- Kept existing mechanics but prepared them for future revisions

Repository snapshot
- Unity project layout (Assets/, Packages/, ProjectSettings/)
- Input System actions: Assets/InputSystem_Actions.inputactions
- Example scripts: Assets/Scripts/PlayerController.cs, GameManager.cs, EncounterSystem.cs
- Settings JSON: Assets/Resources/config/settings.json

Scripts and mechanics (implementation details)

1) Assets/Scripts/GameManager.cs
- Purpose: Central singleton that loads and exposes runtime settings for mechanics and gameplay tuning.
- Key responsibilities:
  - Provide a globally-accessible Instance (singleton).
  - Load Settings from Resources/config/settings.json at Awake() and populate a Settings object.
  - Persist across scenes via DontDestroyOnLoad.
- Settings structure (GameManager.Settings):
  - string version
  - float movementSpeed
  - float jumpForce
  - float encounterRate
  - float resourceSpawnRate
  - string difficulty
  - int seed
- Usage:
  - Add an empty GameObject named "GameManager" to the initial scene and attach the GameManager component (or make a prefab).
  - The GameManager will automatically attempt to load Assets/Resources/config/settings.json. If missing, defaults are used and a log message is printed.
- Notes:
  - For deterministic behaviour across runs, call Random.InitState(GameManager.Instance.settings.seed) at startup (recommended where world generation or spawn order matters).

2) Assets/Scripts/PlayerController.cs
- Purpose: Basic player movement controller (2D Rigidbody) exposing tunable parameters for rapid iteration.
- Public tunables (serialized fields):
  - float movementSpeed (default 4f)
  - float jumpForce (default 6f)
- Behavior:
  - Reads horizontal input via Input.GetAxisRaw("Horizontal") and applies velocity in FixedUpdate.
  - Jump is invoked via Input.GetButtonDown("Jump") — currently does a direct velocity set for the y-axis.
- Integration notes:
  - Ensure player GameObject has a Rigidbody2D component.
  - The script uses Unity’s legacy Input API by default. The repository includes a new Input System asset (InputSystem_Actions.inputactions). To use the new Input System, either:
    - Rebind the legacy axes/buttons to your input devices in Project Settings -> Input Manager, or
    - Convert PlayerController to read input from a generated InputActions C# class (I can add this if you want).
- Recommended improvements to add for a platformer feel:
  - Ground check (Physics2D.OverlapCircle or collision-based) to prevent double jumps.
  - Coyote time and jump buffering to make jumping more forgiving.
  - Acceleration and deceleration (instead of immediate velocity) for smoother movement.

3) Assets/Scripts/EncounterSystem.cs
- Purpose: Example encounter-spawning trigger system that periodically samples chance and fires encounters.
- Public tunables:
  - float encounterRate (chance per second; default loaded from GameManager.settings.encounterRate)
- Behavior:
  - Accumulates time and once per second samples Random.value < encounterRate to decide whether to trigger an encounter.
  - TriggerEncounter() is a placeholder that logs to the console; replace with your spawn/encounter logic.
- Usage:
  - Add the EncounterSystem component to the GameManager object or another persistent manager object in the scene.
  - Tune encounterRate in Assets/Resources/config/settings.json or at runtime via GameManager.Instance.settings.

4) Assets/Resources/config/settings.json
- Purpose: Central JSON-based configuration for rapid tuning without recompiling code.
- Location: Assets/Resources/config/settings.json — loaded by GameManager via Resources.Load at runtime.
- Default keys and example values:
  {
    "version": "v1-unupdate",
    "movementSpeed": 4.0,
    "jumpForce": 6.0,
    "encounterRate": 0.05,
    "resourceSpawnRate": 0.02,
    "difficulty": "normal",
    "seed": 0
  }
- Notes on editing:
  - Edit the JSON file in the project and reimport (or edit via editor tools if added later).
  - If you prefer an editor-first approach, convert this to a ScriptableObject so designers can tweak via the Inspector.

Quick integration guide (how to run):
1. Open the project in Unity.
2. Confirm the settings JSON is located at Assets/Resources/config/settings.json.
3. Add a GameObject named "GameManager" in your first scene and attach the GameManager component.
4. Add EncounterSystem to the GameManager or another manager object.
5. Attach PlayerController to your player GameObject and ensure a Rigidbody2D is present.
6. Play the scene and observe console logs for encounter triggers. Move and try jumping to verify PlayerController.

Tuning tips and recommended defaults
- Movement
  - movementSpeed: 3.0 - 6.0 (try 4.0 as a baseline)
  - jumpForce: 5.0 - 8.0 (adjust to match gravity and desired jump height)
- Encounters
  - encounterRate: 0.01 - 0.2 (represents chance per second to start an encounter; 0.05 is moderate)
- Resources
  - resourceSpawnRate: Adjust spawn rates in EncounterSystem or a ResourceSpawner that consumes this value.
- Difficulty
  - difficulty: "easy" | "normal" | "hard" — use this to drive multipliers in spawn logic and encounter composition.

Troubleshooting
- Scripts do not compile: Ensure you opened the project in a compatible Unity version and allowed the editor to recompile. Check the Unity Console for compile errors.
- Settings not loaded: Confirm the file path is exactly Assets/Resources/config/settings.json and the JSON is valid. GameManager logs a message if the file is missing or fails to parse.
- Jumping while airborne: Add a ground check to PlayerController before allowing jump logic.
- Input mismatch: If you're using the new Input System package, the legacy Input.GetAxis/GetButton calls will not work until you enable the "Both" or "Input Manager" compatibility mode or switch the script to use the generated InputActions.

Next improvements (recommended priorities)
- Replace the Resources-based settings load with a ScriptableObject settings asset and an Editor window for runtime tuning.
- Add ground detection, coyote time, and jump buffering to PlayerController for better platformer feel.
- Implement a proper encounter/spawn manager that uses pooled prefabs and UI hooks.
- Add a simple save/load system for player progress and settings.
- Convert PlayerController input to the new Input System (generate a C# wrapper from InputSystem_Actions.inputactions).

Contributing
See CONTRIBUTING.md for branch naming, PR guidelines, and playtest reporting.

License: MIT — see LICENSE file for details.

Contact: Maintainer: IanKallelOcumen