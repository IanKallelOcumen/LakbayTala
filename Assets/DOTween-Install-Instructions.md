# Installing DOTween

This project uses the **DG.Tweening** API (DOTween). Right now it runs with a **built-in compatibility layer** in `Assets/Scripts/Utils/SimpleTween.cs`, so the game works without DOTween installed.

If you want the **full DOTween** engine (more easing, better performance, extra features), install it as follows.

## Option 1: Official website (recommended)

1. Go to **https://dotween.demigiant.com/download.php**
2. Download the **DOTween** (free) `.unitypackage` for your Unity version.
3. In Unity: **Assets → Import Package → Custom Package…** and select the downloaded file.
4. After import, open **Tools → Demigiant → DOTween Utility Panel** and click **Setup DOTween…** to configure modules.
5. **Optional:** To avoid duplicate `DG.Tweening` code, you can disable or remove the built-in shim:
   - Rename `Assets/Scripts/Utils/SimpleTween.cs` to `SimpleTween.cs.disabled`, or
   - Move it out of the project.

## Option 2: Unity Asset Store

1. Open the **Asset Store** in Unity (Window → Asset Store) or go to https://assetstore.unity.com
2. Search for **DOTween**
3. Download and import into the project.
4. Run **Tools → Demigiant → DOTween Utility Panel → Setup DOTween…**
5. Optionally disable `Assets/Scripts/Utils/SimpleTween.cs` as in Option 1.

## After installing DOTween

- All existing UI and menu tweens will use the real DOTween.
- No code changes are required; the same `DG.Tweening` namespace and APIs are used.

## If you keep the built-in shim (no install)

- The project will keep using `SimpleTween.cs`, which provides a minimal DOTween-compatible API (`SetEase`, `IsActive`, `WaitForCompletion`, `Sequence`, etc.).
- This is enough for the current Brawl Stars–style UI animations.
