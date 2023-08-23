using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace _Game
{
    public class CharacterSkillHandler : CharacterAbility
    {
        [Header("Skills")]
        /// the position from which projectiles will be spawned (can be safely left empty)
        public Transform ProjectileSpawn;
        public List<Skill> Skills;

        [Header("Buffering")]
        public bool BufferInput = true;
        public bool NewInputExtendsBuffer = true;
        public float MaxBufferDuration = .25f;

        protected List<JP_Input.Button> _buttons = new List<JP_Input.Button>();
        protected float _bufferTimer = 0f;
        protected bool _buffering = false;

        protected override void Initialization()
        {
            base.Initialization();
            Setup();
        }

        /// <summary>
        /// Used for extended flexibility.
        /// </summary>
        public virtual void Setup()
        {
            _character = GetComponent<Character>();
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            
            for (int i = 0; i < Skills.Count; i++)
            {
                _buttons.Add(InputManager.Instance.GetButtonFromID($"Skill_{i}"));
                Skills[i].Initialization(_character);
            }
        }

        public override void ProcessAbility()
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                Skill skill = Skills[i];

                HandleBuffer(skill);
            }
        }

        public override void LateProcessAbility()
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                Skill skill = Skills[i];

                skill.ProcessSkillState();
            }
        }

        /// <summary>
        /// Triggers a skill if the skill is idle and an input has been buffered
        /// </summary>
        protected virtual void HandleBuffer(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            // If we are currently buffering an input and if the skill is now idle
            if (_buffering && skill.SkillState.CurrentState == Skill.SkillStates.SkillIdle)
            {
                // and if our buffer is still valid, we trigger an attack
                if (Time.time < _bufferTimer)
                {
                    SkillStart(skill);
                }
                else
                {
                    _buffering = false;
                }
            }
        }

        protected override void HandleInput()
        {
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                Debug.Log("HandleInput: Condition is not normal. Skipping input processing.");
                return;
            }

            for (int i = 0; i < Skills.Count; i++)
            {
                Skill skill = Skills[i];
                JP_Input.Button button = _buttons[i];

                if (button.State.CurrentState == JP_Input.ButtonStates.ButtonDown
                || (skill.TriggerMode == Skill.TriggerModes.Auto && button.State.CurrentState == JP_Input.ButtonStates.ButtonPressed))
                {
                    SkillStart(skill);
                }

                if (button.State.CurrentState == JP_Input.ButtonStates.ButtonUp)
                {
                    SkillStop(skill);
                }

                if (skill.SkillState.CurrentState == Skill.SkillStates.SkillCooldown
                && button.State.CurrentState == JP_Input.ButtonStates.Off)
                {
                    skill.SkillInputStop();
                }
            }
        }

        public virtual void SkillStart(Skill skill)
        {
            // If the Shoot action is enabled in the permissions, we continue, if not we do nothing. If the player is dead we do nothing.
            if ((skill == null)
            || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                return;
            }

            // If we've decided to buffer input, and if the skill is in use right now
            if (BufferInput && (skill.SkillState.CurrentState != Skill.SkillStates.SkillIdle))
            {
                ExtendBuffer();
            }

            _movement.ChangeState(CharacterStates.MovementStates.Attacking);

            skill.SkillInputStart();
        }

        protected virtual void ExtendBuffer()
        {
            // If we're not already buffering, or if each new input extends the buffer, we turn our buffering state to true
            if (!_buffering || NewInputExtendsBuffer)
            {
                _buffering = true;
                _bufferTimer = Time.time + MaxBufferDuration;
            }
        }

        /// <summary>
        /// Stops the provided skill based on its current state and related conditions.
        /// </summary>
        /// <param name="skill">The skill to be stopped.</param>
        public virtual void SkillStop(Skill skill)
        {
            // Set character movement to idle
            _movement.ChangeState(CharacterStates.MovementStates.Idle);

            // If no skill is provided, exit the method
            if (skill == null) return;

            // Fetch the current state of the skill
            Skill.SkillStates skillState = skill.SkillState.CurrentState;

            // If the skill is already idle, exit the method
            if (skillState == Skill.SkillStates.SkillIdle) return;

            // If the skill is in delay before use and not interruptible, exit the method
            if (skillState == Skill.SkillStates.SkillDelayBeforeUse && !skill.DelayBeforeUseReleaseInterruption) return;

            // If the skill is currently being used, exit the method
            if (skillState == Skill.SkillStates.SkillUse) return;

            // Turn off the skill
            skill.TurnSkillOff();
        }

        protected override void OnHit(GameObject instigator)
        {
            base.OnHit(instigator);
            for (int i = 0; i < Skills.Count; i++)
            {
                Skill skill = Skills[i];
                skill.OnHit(instigator);
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            for (int i = 0; i < Skills.Count; i++)
            {
                Skill skill = Skills[i];
                SkillStop(skill);
            }
        }

        protected override void OnRespawn()
        {
            base.OnRespawn();
            Setup();
        }

        protected virtual void OnDrawGizmos()
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                Skill skill = Skills[i];
                skill.DrawGizmos();
            }
        }
    }
}
