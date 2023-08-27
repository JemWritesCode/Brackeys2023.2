using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public abstract class Skill : ScriptableObject
    {
        public enum TriggerModes { SemiAuto, Auto }
        public enum UpdateModes { Update, FixedUpdate }
        public enum ChargeConsumptionModes { None, Partial, Full }

        public enum SkillTypes
        {
            Basic,
            Mobility,
            Special,
            Ultimate,
        }

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
        [Tooltip("Unique identifier for the skill. Used for animations.")]
        public string ID;

        [Tooltip("Display name of the skill.")]
        public string Name;

        [Tooltip("Detailed description of what the skill does.")]
        public string Description;

        [Tooltip("Visual representation of the skill.")]
        public Sprite Icon;

        [Tooltip("The type of Skill, used for skill searching.")]
        public SkillTypes SkillType = SkillTypes.Basic;

        [Tooltip("The skill's state machine.")]
        public StateMachine<SkillStates> SkillState;

        [Tooltip("Percentage modifier of the skill. Used for damage calculations where 1 = 1%.")]
        public float ImpactPercentageModifier = 100f;

        [Tooltip("The update mode of the skill (Update or FixedUpdate).")]
        public UpdateModes UpdateMode = UpdateModes.Update;

        [Header("Use")]
        [Tooltip("What state this skill should put the state machine in when activated.")]
        public CharacterStates.MovementStates ActiveState = CharacterStates.MovementStates.Attacking;

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

        [Header("Charge")]
        [Tooltip("The mode of charge consumption for the skill.")]
        public ChargeConsumptionModes ChargeConsumption = ChargeConsumptionModes.None;

        [Tooltip("The amount of charge consumed on use. Only used if ChargeConsumption is set to Partial.")]
        public float PartialChargeConsumeAmount = 25f;

        [Tooltip("Determines if the skill provides charge to the SkillHandler.")]
        public bool ProvidesCharge = false;

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
        public Character Owner;

        [Tooltip("The total cooldown of the skill (in seconds) including modifiers.")]
        public float TotalCooldown { get { return Cooldown * _cooldownPercentageModifier; } }

        protected Timer _cooldownTimer;
        protected Timer _delayBeforeUseTimer;
        protected Timer _skillDurationTimer;
        protected bool _triggerReleased = false;
        protected TopDownController _controller;
        protected CharacterSkillHandler _skillHandler;
        protected CharacterMovement _movement;
        protected CharacterAiming _aiming;
        protected int _skillIndex;
        protected float _cooldownPercentageModifier = 1f;

        protected const string _skillAnimationParameterName = "skill_";
        protected const string _idleAnimationParameterName = "Idle";
        protected int _skillAnimationParameter;
        protected int _idleAnimationParameter;

        /// <summary>
        /// Initializes the skill, setting up necessary properties and references.
        /// </summary>
        /// <param name="owner">The game object that will use or activate the skill.</param>
        public virtual void Initialization(Character owner, int index)
        {
            SetOwner(owner);
            SkillState = new StateMachine<SkillStates>(owner.gameObject, true);
            SkillState.ChangeState(SkillStates.SkillIdle);

            _cooldownTimer = new Timer(TotalCooldown, OnCooldownStart, OnCooldownEnd, OnCooldownUpdate);
            _delayBeforeUseTimer = new Timer(DelayBeforeUse);
            _skillDurationTimer = new Timer(SkillDuration);

            _skillIndex = index;
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
        protected virtual void OnCooldownStart()
        {
            if (Owner.PlayerID == "Player")
            {
                UIManager.Instance.SkillCooldownSetFill(_skillIndex, 1);
            }
        }

        /// <summary>
        /// Is called when the skill's cooldown ends.
        /// </summary>
        protected virtual void OnCooldownEnd()
        {
            if (Owner.PlayerID == "Player")
            {
                UIManager.Instance.SkillCooldownSetFill(_skillIndex, 0);
            }
        }

        /// <summary>
        /// Is called when the skill's cooldown updates.
        /// </summary>
        protected virtual void OnCooldownUpdate()
        {
            if (Owner.PlayerID == "Player")
            {
                UIManager.Instance.SkillCooldownSetFill(_skillIndex, _cooldownTimer.GetNormalisedTime());
            }
        }

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

        protected virtual void CaseSkillIdle()
        {
            if (ChargeConsumption != ChargeConsumptionModes.None && Owner.PlayerID == "Player")
            {
                UIManager.Instance.SkillCooldownSetFill(_skillIndex, 1f - Mathf.Clamp01(_skillHandler.Charge / _skillHandler.MaxCharge));
            }
        }

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
                    _delayBeforeUseTimer.Duration = DelayBeforeUse;
                    _delayBeforeUseTimer.StartTimer(true, true);
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

            if (TotalCooldown > 0)
            {
                if (!_cooldownTimer.IsRunning)
                {
                    _cooldownTimer.Duration = TotalCooldown;
                    _cooldownTimer.StartTimer(true, true);
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

        protected virtual void ConsumeCharge()
        {
            switch (ChargeConsumption)
            {
                case ChargeConsumptionModes.None:
                    break;

                case ChargeConsumptionModes.Partial:
                    _skillHandler.ModifyCharge(-PartialChargeConsumeAmount);
                    break;

                case ChargeConsumptionModes.Full:
                    _skillHandler.ModifyCharge(-_skillHandler.MaxCharge);
                    break;
            }
        }

        protected virtual void ProvideCharge() { }

        #region PUBLIC METHODS

        public virtual bool CanCastSkill()
        {
            bool canCast = true;

            if (SkillState.CurrentState != SkillStates.SkillIdle)
            {
                canCast = false;
            }

            switch (ChargeConsumption)
            {
                case ChargeConsumptionModes.None: 
                    break;

                case ChargeConsumptionModes.Partial:
                    if (_skillHandler.Charge < PartialChargeConsumeAmount)
                    {
                        canCast = false;
                    }
                    break;

                case ChargeConsumptionModes.Full:
                    if (_skillHandler.Charge < _skillHandler.MaxCharge)
                    {
                        canCast = false;
                    }
                    break;
            }

            //Debug.Log($"CanCast = {canCast}");
            return canCast;
        }

        public virtual void SkillInputStart()
        {
            if (CanCastSkill())
            {
                _triggerReleased = false;
                ConsumeCharge();
                SkillStart();
            }
        }

        public virtual void SkillInputStop()
        {
            _triggerReleased = true;
        }

        /// <summary>
        /// Handle what happens when the skill starts
        /// </summary>
        public virtual void SkillStart()
        {
            if (Owner.PlayerID == "Player")
            {
                UIManager.Instance.SkillCooldownSetFill(_skillIndex, 1);
            }

            _skillHandler.SetMovementState(ActiveState);
            SkillState.ChangeState(SkillStates.SkillStart);
        }

        /// <summary>
        /// Makes a request for the skill to shoot.
        /// </summary>
        public virtual void ActivateRequest()
        {
            _skillDurationTimer.Duration = SkillDuration;
            _skillDurationTimer.StartTimer(true, true);

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

        /// <summary>
        /// Modifies the Cooldown of the skill.
        /// </summary>
        /// <param name="amount">A number to change the modifier by. 1 = 1%.</param>
        public virtual void ModifyCooldown(float amount)
        {
            float newModifier = _cooldownPercentageModifier + (amount / 100);

            if (newModifier < 0)
            {
                Debug.LogWarning("Skill Cooldown modifier trying to go below 0, will cause inconsistent modifications.", Owner.gameObject);
                newModifier = 0;
            }

            _cooldownPercentageModifier = newModifier;
        }

        public virtual void OnHit(GameObject instigator)
        {
            if (GettingHitInterruptsSkill)
            {
                Interrupt();
            }
        }

        public virtual void DrawGizmos(GameObject owner) { }

        public virtual void InitializeAnimatorParameters()
        {
            _skillHandler.RegisterAnimatorParameter(_skillAnimationParameterName + ID, AnimatorControllerParameterType.Bool, out _skillAnimationParameter);
            _skillHandler.RegisterAnimatorParameter(_idleAnimationParameterName, AnimatorControllerParameterType.Bool, out _idleAnimationParameter);
        }

        public virtual void UpdateAnimator(Animator animator, CharacterStates.MovementStates currentState)
        {
            AnimatorExtensions.UpdateAnimatorBool(animator, _skillAnimationParameter, currentState == ActiveState, Owner.AnimatorParameters);
            AnimatorExtensions.UpdateAnimatorBool(animator, _idleAnimationParameter, currentState == CharacterStates.MovementStates.Idle, Owner.AnimatorParameters);
        }

        #endregion
    }
}
