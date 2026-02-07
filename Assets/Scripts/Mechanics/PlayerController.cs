<<<<<<< HEAD
=======
﻿using System.Collections;
using System.Collections.Generic;
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.InputSystem;
<<<<<<< HEAD
using LakbayTala.Config;
=======
>>>>>>> cc13d476401456216a06540c93347c14c71e8946

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

<<<<<<< HEAD
        public float maxSpeed = 7f;
        public float jumpTakeOffSpeed = 7f;

        [Header("Coyote time & jump buffer (forgiving controls)")]
        [Tooltip("Time after leaving ground during which jump is still allowed.")]
        public float coyoteTime = 0.12f;
        [Tooltip("Time before landing that a jump press is buffered and executed on land.")]
        public float jumpBufferTime = 0.15f;
=======
        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 7;
>>>>>>> cc13d476401456216a06540c93347c14c71e8946

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
<<<<<<< HEAD
=======
        // public Health health; // REMOVED
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        private InputAction m_MoveAction;
        private InputAction m_JumpAction;
<<<<<<< HEAD
        private float coyoteTimeLeft;
        private float jumpBufferLeft;
=======
>>>>>>> cc13d476401456216a06540c93347c14c71e8946

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
<<<<<<< HEAD
=======
            // health = GetComponent<Health>(); // REMOVED
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

<<<<<<< HEAD
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

        void Start()
        {
            if (MasterGameManager.Instance != null && MasterGameManager.Instance.Settings != null)
            {
                GameSettings s = MasterGameManager.Instance.Settings;
                maxSpeed = s.movementSpeed;
                jumpTakeOffSpeed = s.jumpForce;
=======
            // Input System setup
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null) {
                m_MoveAction = playerInput.actions["Move"];
                m_JumpAction = playerInput.actions["Jump"];
            } else {
                 // Fallback if PlayerInput component is missing, looks for global asset
                 m_MoveAction = InputSystem.actions.FindAction("Player/Move");
                 m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
            }
        }

        protected override void Update()
        {
            if (controlEnabled && m_MoveAction != null)
            {
                move.x = m_MoveAction.ReadValue<Vector2>().x;
<<<<<<< HEAD

                bool jumpPressed = m_JumpAction != null && m_JumpAction.WasPressedThisFrame();
                if (jumpPressed)
                {
                    if (jumpState == JumpState.Grounded || (jumpState == JumpState.InFlight && coyoteTimeLeft > 0f))
                        jumpState = JumpState.PrepareToJump;
                    else if (!IsGrounded)
                        jumpBufferLeft = jumpBufferTime;
                }
                if (m_JumpAction != null && m_JumpAction.WasReleasedThisFrame())
=======
                
                if (jumpState == JumpState.Grounded && m_JumpAction.WasPressedThisFrame())
                    jumpState = JumpState.PrepareToJump;
                else if (m_JumpAction.WasReleasedThisFrame())
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
<<<<<<< HEAD

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

=======
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
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
<<<<<<< HEAD
                        coyoteTimeLeft = coyoteTime;
=======
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
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
<<<<<<< HEAD

        protected override void ComputeVelocity()
        {
            if (jump && (IsGrounded || coyoteTimeLeft > 0f))
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
                coyoteTimeLeft = 0f;
=======
        

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

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
<<<<<<< HEAD

=======
        // Add this to the bottom of PlayerController.cs
>>>>>>> cc13d476401456216a06540c93347c14c71e8946
        public void ApplyBounce(float force)
        {
            // This modifies the internal velocity of the controller
            // so gravity will still work correctly!
            velocity.y = force;
        }
    }
    
}