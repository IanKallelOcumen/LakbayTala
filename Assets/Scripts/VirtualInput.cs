using UnityEngine;

namespace Platformer.UI
{
    /// <summary>
    /// Static input from on-screen buttons (LeftBtn, RightBtn, JumpBtn).
    /// PlayerController reads this in addition to the Input System.
    /// </summary>
    public static class VirtualInput
    {
        static bool s_LeftPressed;
        static bool s_RightPressed;
        static bool s_JumpPressed;
        static bool s_JumpReleased;

        public static float MoveDirection
        {
            get
            {
                if (s_LeftPressed && !s_RightPressed) return -1f;
                if (s_RightPressed && !s_LeftPressed) return 1f;
                return 0f;
            }
        }

        public static void SetMoveLeft(bool pressed) => s_LeftPressed = pressed;
        public static void SetMoveRight(bool pressed) => s_RightPressed = pressed;
        public static void SetJump(bool pressed)
        {
            if (s_JumpPressed && !pressed) s_JumpReleased = true;
            s_JumpPressed = pressed;
        }

        /// <summary>Returns true once per press; call once per frame from PlayerController.</summary>
        public static bool ConsumeJump()
        {
            if (!s_JumpPressed) return false;
            s_JumpPressed = false;
            return true;
        }

        /// <summary>Returns true once per release (for variable-height jump).</summary>
        public static bool ConsumeJumpRelease()
        {
            if (!s_JumpReleased) return false;
            s_JumpReleased = false;
            return true;
        }
    }
}
