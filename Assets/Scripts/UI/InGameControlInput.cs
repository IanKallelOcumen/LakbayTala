using UnityEngine;
using UnityEngine.EventSystems;

namespace Platformer.UI
{
    /// <summary>
    /// Attach to the same GameObject as MainUIController (e.g. UI Canvas).
    /// Finds LeftBtn, RightBtn, JumpBtn under this transform and wires them to VirtualInput
    /// so the player moves when you tap the on-screen controls.
    /// </summary>
    public class InGameControlInput : MonoBehaviour
    {
        GameObject leftBtn;
        GameObject rightBtn;
        GameObject jumpBtn;

        static GameObject FindInChildren(Transform root, string name)
        {
            if (root == null) return null;
            if (root.name == name) return root.gameObject;
            for (int i = 0; i < root.childCount; i++)
            {
                var found = FindInChildren(root.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        void Awake()
        {
            leftBtn = FindInChildren(transform, "LeftBtn");
            rightBtn = FindInChildren(transform, "RightBtn");
            jumpBtn = FindInChildren(transform, "JumpBtn");

            WireButton(leftBtn, () => VirtualInput.SetMoveLeft(true), () => VirtualInput.SetMoveLeft(false));
            WireButton(rightBtn, () => VirtualInput.SetMoveRight(true), () => VirtualInput.SetMoveRight(false));
            WireButton(jumpBtn, () => VirtualInput.SetJump(true), () => VirtualInput.SetJump(false)); // release sets jump released for variable-height jump
        }

        void OnDisable()
        {
            VirtualInput.SetMoveLeft(false);
            VirtualInput.SetMoveRight(false);
            VirtualInput.SetJump(false);
        }

        static void WireButton(GameObject go, System.Action onDown, System.Action onUp)
        {
            if (go == null) return;
            var et = go.GetComponent<EventTrigger>();
            if (et == null) et = go.AddComponent<EventTrigger>();

            AddTrigger(et, EventTriggerType.PointerDown, _ => onDown());
            AddTrigger(et, EventTriggerType.PointerUp, _ => onUp());
            AddTrigger(et, EventTriggerType.PointerExit, _ => onUp());
        }

        static void AddTrigger(EventTrigger et, EventTriggerType type, System.Action<BaseEventData> callback)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(data => callback(data));
            et.triggers.Add(entry);
        }
    }
}
