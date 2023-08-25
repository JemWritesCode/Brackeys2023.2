using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace _Game
{
    public abstract class Skill : ScriptableObject
    {
        public enum TriggerModes { SemiAuto, Auto }
        public enum UpdateModes { Update, FixedUpdate }

        public enum SkillStates
        {
            SkillIdle,
            SkillStart,
            SkillDelayBeforeUse,
            SkillUse,
            SkillCooldown,
            SkillStop,
            SkillInterrupted,
        }

        [Header("Skill Data")]
        [Tooltip("Unique identifier for the skill.")]
        public string ID;

        [Tooltip("Display name of the skill.")]
        public string Name;

        [Tooltip("Detailed description of what the skill does.")]
        public string Description;

        [Tooltip("Visual representation of the skill.")]
        public Sprite Icon;

        [Tooltip("The skill's state machine.")]
        public StateMachine<SkillStates> SkillState;

        [Tooltip("Percentage modifier of the skill. Used for damage calculations and can be safely left 0.")]
        public float PercentageModifier = 0f;

        [Tooltip("The update mode of the skill (Update or FixedUpdate).")]
        public UpdateModes UpdateMode = UpdateModes.Update;

        [Header("Use")]
        [Tooltip("The trigger mode of the skill (SemiAuto or Auto).")]
        public TriggerModes TriggerMode = TriggerModes.Auto;

        [Tooltip("The delay (in seconds) before the skill can be used for every activation.")]
        public float DelayBeforeUse = 0f;

        [Tooltip("Determines if the delay before use can be interrupted by releasing the button.")]
        public bool DelayBeforeUseReleaseInterruption = false;

        [Tooltip("Time (in seconds) required for the skill to be ready for use again after activation.")]
        public float Cooldown = 1f;

        [Tooltip("Time (in seconds) that the skill takes to finish. Will usually be the length of the animation.")]
        public float SkillDuration = 0f;

        [Header("Settings")]
        [Tooltip("Whether this skill's button can be held to repeatedly cast.")]
        public bool ContinuousPress = true;

        [Tooltip("Whether or not this skill is interrupted by the owner being hit.")]
        public bool GettingHitInterruptsSkill = false;

        [Tooltip("The force of the weapon's recoil.")]
        public float RecoilForce = 0f;

        [Tooltip("Determines if the skill can be interrupted.")]
        public bool Interruptible = false;

        [Tooltip("Determines if the skill restricts the owner's movement.")]
        public bool RestrictMovement = false;

        [Tooltip("Determines if the skill restricts the player's rotation.")]
        public bool RestrictRotation = false;

        [Tooltip("The character that owns this skill.")]
        public Character Owner { get; protected set; }

        protected Timer _cooldownTimer;
        protected Timer _delayBeforeUseTimer;
        protected Timer _skillDurationTimer;
        protected bool _triggerReleased = false;
        protected TopDownController _controller;
        protected CharacterSkillHandler _skillHandler;
        protected CharacterMovement _movement;
        protected CharacterAiming _aiming;

        /// <summary>
        /// Initializes the skill, setting up necessary properties and references.
        /// </summary>
        /// <param name="owner">The game object that will use or activate the skill.</param>
        public virtual void Initialization(Character owner)
        {
            SetOwner(owner);
            SkillState = new StateMachine<SkillStates>(owner.gameObject, true);
            SkillState.ChangeState(SkillStates.SkillIdle);

            _cooldownTimer = new Timer(Cooldown, OnCooldownStart, OnCooldownEnd);
            _delayBeforeUseTimer = new Timer(DelayBeforeUse);
        }

        public virtual void SetOwner(Character owner)
        {
            Owner = owner;
            if (Owner != null)
            {
                _controller = Owner.TopDownController;
                _skillHandler = Owner.GetAbility<CharacterSkillHandler>();
                _movement = Owner.GetAbility<CharacterMovement>();
                _aiming = Owner.GetAbility<CharacterAiming>();
            }
        }

        /// <summary>
        /// Is called when the skill's cooldown starts.
        /// </summary>
        public virtual void OnCooldownStart() { }

        /// <summary>
        /// Is called when the skill's cooldown ends.
        /// </summary>
        public virtual void OnCooldownEnd() { }

        #region Skill STATE MACHINE CASE METHODS

        public virtual void ProcessSkillState()
        {
            if (SkillState == null) { return; }

            switch (SkillState.CurrentState)
            {
                case SkillStates.SkillIdle:
                    CaseSkillIdle();
                    break;
                case SkillStates.SkillDelayBeforeUse:
                    CaseSkillDelayBeforeUse();
                    break;
                case SkillStates.SkillStart:
                    CaseSkillStart();
                    break;
                case SkillStates.SkillUse:
                    CaseSkillUse();
                    break;
                case SkillStates.SkillCooldown:
                    CaseSkillCooldown();
                    break;
                case SkillStates.SkillStop:
                    CaseSkillStop();
                    break;
                case SkillStates.SkillInterrupted:
                    CaseSkillInterrupted();
                    break;
            }
        }

        protected virtual void CaseSkillIdle() { }

        protected virtual void CaseSkillStart()
        {
            if (RestrictMovement)
            {
                _movement.MovementForbidden = true;
            }

            if (RestrictRotation)
            {
                _aiming.RotationForbidden = true;
            }

            if (DelayBeforeUse > 0)
            {
                if (!_delayBeforeUseTimer.IsRunning)
                {
                    _delayBeforeUseTimer = new Timer(DelayBeforeUse);
                    _delayBeforeUseTimer.StartTimer();
                }
                SkillState.ChangeState(SkillStates.SkillDelayBeforeUse);
            }
            else
            {
                ActivateRequest();
            }
        }

        protected virtual void CaseSkillDelayBeforeUse()
        {
            _delayBeforeUseTimer.UpdateTimer();

            if (!_delayBeforeUseTimer.IsRunning)
            {
                ActivateRequest();
            }
        }

        protected virtual void CaseSkillUse()
        {
            SkillUse();

            _skillDurationTimer.UpdateTimer();

            if (!_skillDurationTimer.IsRunning)
            {
                SkillStop();
            }
        }

        protected virtual void CaseSkillStop()
        {
            if (RestrictMovement)
            {
                _movement.MovementForbidden = false;
            }

            if (RestrictRotation)
            {
                _aiming.RotationForbidden = false;
            }

            if (Cooldown > 0)
            {
                if (!_cooldownTimer.IsRunning)
                {
                    _cooldownTimer = new Timer(Cooldown, OnCooldownStart, OnCooldownEnd);
                    _cooldownTimer.StartTimer();
                }
                SkillState.ChangeState(SkillStates.SkillCooldown);
            }
            else
            {
                SkillState.ChangeState(SkillStates.SkillIdle);
            }
        }

        protected virtual void CaseSkillCooldown()
        {
            _cooldownTimer.UpdateTimer();
            if (!_cooldownTimer.IsRunning)
            {
                if ((TriggerMode == TriggerModes.Auto) && !_triggerReleased)
                {
                    ActivateRequest();
                }
                else
                {
                    SkillState.ChangeState(SkillStates.SkillIdle);
                }
            }
        }

        protected virtual void CaseSkillInterrupted()
        {
            SkillStop();
            SkillState.ChangeState(SkillStates.SkillIdle);
        }

        #endregion

        #region PUBLIC METHODS

        public virtual void SkillInputStart()
        {
            if (SkillState.CurrentState == SkillStates.SkillIdle)
            {
                _triggerReleased = false;
                SkillStart();
            }
        }

        public virtual void SkillInputStop()
        {
            _triggerReleased = true;
        }

        /// <summary>
        /// Makes a request for the skill to shoot.
        /// </summary>
        public virtual void ActivateRequest()
        {
            _skillDurationTimer = new Timer(SkillDuration);
            _skillDurationTimer.StartTimer();
            SkillState.ChangeState(SkillStates.SkillUse);
        }

        /// <summary>
        /// Defines the behavior when the skill is used (like triggering sfx, vfx, recoil).
        /// </summary>
        public virtual void SkillUse()
        {
            if (RecoilForce != 0f && _controller != null && Owner != null)
            {
                _controller.Impact(-Owner.transform.forward, RecoilForce);
            }
        }

        /// <summary>
        /// Handle what happens when the skill starts
        /// </summary>
        public virtual void SkillStart()
        {
            _skillHandler.SetMovementState(CharacterStates.MovementStates.Attacking);
            SkillState.ChangeState(SkillStates.SkillStart);
        }

        /// <summary>
        /// Turns the skill off, primarily ending its current state of operation.
        /// </summary>
        public virtual void SkillStop()
        {
            if ((SkillState.CurrentState == SkillStates.SkillIdle || SkillState.CurrentState == SkillStates.SkillStop))
            {
                return;
            }
            _triggerReleased = true;
            _skillHandler.SetMovementState(CharacterStates.MovementStates.Idle);
            SkillState.ChangeState(SkillStates.SkillStop);
        }

        /// <summary>
        /// Interrupts the skill's current operation if it's interruptible.
        /// </summary>
        public virtual void Interrupt()
        {
            if (Interruptible)
            {
                SkillState.ChangeState(SkillStates.SkillInterrupted);
            }
        }

        public virtual void OnHit(GameObject instigator)
        {
            if (GettingHitInterruptsSkill)
            {
                Interrupt();
            }
        }

        public virtual void DrawGizmos() { }

        public virtual void InitializeAnimatorParameters() { }

        public virtual void UpdateAnimator(Animator animator, CharacterStates.MovementStates currentState) { }

        #endregion
    }
}
