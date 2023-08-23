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

        [Tooltip("Layers that can be targeted or affected by this skill.")]
        public LayerMask TargetLayers;

        [Tooltip("The skill's state machine.")]
        public StateMachine<SkillStates> SkillState;

        [Header("Use")]
        [Tooltip("The trigger mode of the skill (SemiAuto or Auto).")]
        public TriggerModes TriggerMode = TriggerModes.Auto;

        [Tooltip("The delay before the skill can be used for every activation.")]
        public float DelayBeforeUse = 0f;

        [Tooltip("Determines if the delay before use can be interrupted by releasing the button.")]
        public bool DelayBeforeUseReleaseInterruption = false;

        [Tooltip("Time (in seconds) required for the skill to be ready for use again after activation.")]
        public float Cooldown = 1f;

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

        [Tooltip("The character that owns this skill.")]
        public Character Owner { get; protected set; }

        [Tooltip("The skill's owner's CharacterSkillHandler component.")]
        public CharacterSkillHandler OwnerSkillHandler { get; set; }

        protected Timer _cooldownTimer;
        protected Timer _delayBeforeUseTimer;
        protected bool _triggerReleased = false;
        protected TopDownController _controller;
        protected CharacterMovement _characterMovement;

        /// <summary>
        /// Initializes the skill, setting up necessary properties and references.
        /// </summary>
        /// <param name="owner">The game object that will use or activate the skill.</param>
        public virtual void Initialization(Character owner)
        {
            SetOwner(owner);
            SkillState = new StateMachine<SkillStates>(Owner.gameObject, true);
            SkillState.ChangeState(SkillStates.SkillIdle);
        }

        public virtual void SetOwner(Character owner)
        {
            Owner = owner;
            if (Owner != null)
            {
                this.OwnerSkillHandler = Owner.GetAbility<CharacterSkillHandler>();
                _characterMovement = Owner.GetAbility<CharacterMovement>();
                _controller = Owner.TopDownController;
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
                _characterMovement.MovementForbidden = true;
            }

            if (DelayBeforeUse > 0)
            {
                _delayBeforeUseTimer = new Timer(DelayBeforeUse);
                _delayBeforeUseTimer.StartTimer();
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

            if (Cooldown > 0)
            {
                _cooldownTimer = new Timer(Cooldown, OnCooldownStart, OnCooldownEnd);
                _cooldownTimer.StartTimer();
                SkillState.ChangeState(SkillStates.SkillCooldown);
            }
            else
            {
                TurnSkillOff();
            }
        }

        protected virtual void CaseSkillCooldown()
        {
            _cooldownTimer.UpdateTimer();
            if (!_cooldownTimer.IsRunning)
            {
                if ((TriggerMode == TriggerModes.Auto) && !_triggerReleased)
                {
                    //Debug.Log($"{this.GetType()}.CaseSkillCooldown: Skill TriggerMode is Auto and TriggerReleased equals = {_triggerReleased}, ShootRequest called.", gameObject);
                    ActivateRequest();
                }
                else
                {
                    TurnSkillOff();
                }
            }
        }

        protected virtual void CaseSkillStop()
        {
            if (RestrictMovement)
            {
                _characterMovement.MovementForbidden = true;
            }

            SkillState.ChangeState(SkillStates.SkillIdle);
        }

        protected virtual void CaseSkillInterrupted()
        {
            TurnSkillOff();
            SkillState.ChangeState(SkillStates.SkillIdle);
        }

        #endregion

        #region PUBLIC METHODS

        public virtual void SkillInputStart()
        {
            if (SkillState.CurrentState == SkillStates.SkillIdle)
            {
                _triggerReleased = false;
                TurnSkillOn();
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
        public virtual void TurnSkillOn()
        {
            SkillState.ChangeState(SkillStates.SkillStart);
        }

        /// <summary>
        /// Turns the skill off, primarily ending its current state of operation.
        /// </summary>
        public virtual void TurnSkillOff()
        {
            if ((SkillState.CurrentState == SkillStates.SkillIdle || SkillState.CurrentState == SkillStates.SkillStop))
            {
                return;
            }
            _triggerReleased = true;
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

        #endregion
    }
}
