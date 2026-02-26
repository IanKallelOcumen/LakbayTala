using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Platformer.UI;
using UnityEngine.InputSystem;
using LakbayTala.Config;

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        public float maxSpeed = 7f;
        public float jumpTakeOffSpeed = 7f;

        [Header("Coyote time & jump buffer (forgiving controls)")]
        [Tooltip("Time after leaving ground during which jump is still allowed.")]
        public float coyoteTime = 0.12f;
        [Tooltip("Time before landing that a jump press is buffered and executed on land.")]
        public float jumpBufferTime = 0.15f;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        private InputAction m_MoveAction;
        private InputAction m_JumpAction;
        private float coyoteTimeLeft;
        private float jumpBufferLeft;

        public Bounds Bounds => collider2d != null ? collider2d.bounds : default;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                m_MoveAction = playerInput.actions["Move"];
                m_JumpAction = playerInput.actions["Jump"];
            }
            else
            {
                m_MoveAction = InputSystem.actions.FindAction("Player/Move");
                m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
            }
        }

        protected override void Start()
        {
            base.Start();
            if (MasterGameManager.Instance != null && MasterGameManager.Instance.Settings != null)
            {
                GameSettings s = MasterGameManager.Instance.Settings;
                maxSpeed = s.movementSpeed;
                jumpTakeOffSpeed = s.jumpForce;
            }
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                float keyboardMove = (m_MoveAction != null) ? m_MoveAction.ReadValue<Vector2>().x : 0f;
                float virtualMove = VirtualInput.MoveDirection;
                move.x = (Mathf.Abs(virtualMove) > 0.01f) ? virtualMove : keyboardMove;

                bool jumpPressed = (m_JumpAction != null && m_JumpAction.WasPressedThisFrame()) || VirtualInput.ConsumeJump();
                if (jumpPressed)
                {
                    if (jumpState == JumpState.Grounded || (jumpState == JumpState.InFlight && coyoteTimeLeft > 0f))
                        jumpState = JumpState.PrepareToJump;
                    else if (!IsGrounded)
                        jumpBufferLeft = jumpBufferTime;
                }
                if ((m_JumpAction != null && m_JumpAction.WasReleasedThisFrame()) || VirtualInput.ConsumeJumpRelease())
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }

            if (!IsGrounded)
            {
                coyoteTimeLeft -= Time.deltaTime;
                if (coyoteTimeLeft < 0f) coyoteTimeLeft = 0f;
                jumpBufferLeft -= Time.deltaTime;
                if (jumpBufferLeft < 0f) jumpBufferLeft = 0f;
            }
            else
            {
                if (jumpBufferLeft > 0f)
                {
                    jumpState = JumpState.PrepareToJump;
                    jumpBufferLeft = 0f;
                }
            }

            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                        coyoteTimeLeft = coyoteTime;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && (IsGrounded || coyoteTimeLeft > 0f))
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
                coyoteTimeLeft = 0f;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (spriteRenderer != null)
            {
                if (move.x > 0.01f)
                    spriteRenderer.flipX = false;
                else if (move.x < -0.01f)
                    spriteRenderer.flipX = true;
            }

            if(animator != null) {
                animator.SetBool("grounded", IsGrounded);
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            }

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }

        public void ApplyBounce(float force)
        {
            // This modifies the internal velocity of the controller
            // so gravity will still work correctly!
            velocity.y = force;
        }
    }
    
}