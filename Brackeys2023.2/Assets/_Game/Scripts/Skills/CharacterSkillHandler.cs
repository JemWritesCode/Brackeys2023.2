using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class CharacterSkillHandler : CharacterAbility
    {
        [Header("Skills")]
        [Tooltip("The position from which projectiles will be spawned (can be safely left empty).")]
        public Transform ProjectileSpawn;

        [Tooltip("The list of skills for this character to use. Copies will be created at runtime for the purpose of modifications. DO NOT MODIFY THESE AT RUNTIME.")]
        public List<Skill> Skills;

        [Header("Charge")]
        [ReadOnly, Tooltip("The current charge of the player.")]
        public float Charge = 0f;

        [Tooltip("The maximum charge of the player.")]
        public float MaxCharge = 100f;

        [Tooltip("The amount of charge gained over time.")]
        public float ChargeTimeIncrement = 0f;

        [Tooltip("The amount of charge gained when the character deals damage.")]
        public float ChargeDamageIncrement = 0f;

        [Header("Damage Calculation")]
        public int BaseDamage = 10;

        [Tooltip("The current overall bonus percentage used in damage calculations.")]
        public float DamageBonusPercentage = 0f;

        [Header("Settings")]
        [Tooltip("A list of character states where input is permitted.")]
        public CharacterStates.MovementStates PermittedInputStates;

        public bool IsMaxCharge { get { return Charge >= MaxCharge; } }

        [SerializeField, ReadOnly]
        protected List<Skill> _currentActiveSkills = new List<Skill>();
        protected List<JP_Input.Button> _buttons = new List<JP_Input.Button>();
        protected bool _usingSkill;

        #region UNITY LIFECYCLE

        protected virtual void FixedUpdate()
        {
            for (int i = 0; i < _currentActiveSkills.Count; i++)
            {
                Skill skill = _currentActiveSkills[i];

                if (skill.UpdateMode != Skill.UpdateModes.FixedUpdate) continue;

                skill.ProcessSkillState();
            }
        }

        protected virtual void OnDrawGizmos()
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                Skill skill = Skills[i];
                skill.DrawGizmos(gameObject);
            }
        }

        #endregion

        protected override void Initialization()
        {
            base.Initialization();
            Setup();
        }

        public override void LateProcessAbility()
        {
            ModifyCharge(ChargeTimeIncrement * Time.deltaTime);

            for (int i = 0; i < _currentActiveSkills.Count; i++)
            {
                Skill skill = _currentActiveSkills[i];

                if (skill.UpdateMode != Skill.UpdateModes.Update) continue;

                skill.ProcessSkillState();
            }
        }

        /// <summary>
        /// Handles user input to determine when to start, stop, or modify character skills.
        /// </summary>
        protected override void HandleInput()
        {
            // Check if the character's condition is not normal (e.g. if the character is dead, stunned, etc.)
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                // Log a message indicating the character's condition is not normal and hence input will not be processed
                Debug.LogWarning($"{this.GetType()}.HandleInput: Condition is not normal. Skipping input processing.", gameObject);
                return;
            }

            // Do not allow inputs if player is not idle.
            if ((_movement.CurrentState & PermittedInputStates) != _movement.CurrentState)
            {
                //Debug.LogWarning($"{this.GetType()}.HandleInput: Movement State is not permitted. Skipping input processing.", gameObject);
                return;
            }

            // Iterate through each skill in the character's skill set
            for (int i = 0; i < _currentActiveSkills.Count; i++)
            {
                Skill skill = _currentActiveSkills[i]; // Get the current skill from the list
                JP_Input.Button button = _buttons[i]; // Get the corresponding button assigned to this skill

                // Check if the button was just pressed down or if the skill has an 'Auto' trigger mode 
                // and the button is currently being pressed
                if (button.State.CurrentState == JP_Input.ButtonStates.ButtonDown
                || (skill.TriggerMode == Skill.TriggerModes.Auto && button.State.CurrentState == JP_Input.ButtonStates.ButtonPressed))
                {
                    StartSkill(skill); // Start the skill as the condition met
                }

                // Check if the button was just released up
                if (button.State.CurrentState == JP_Input.ButtonStates.ButtonUp)
                {
                    StopSkill(skill); // Stop the skill as the button was released
                }

                // Check if the skill is currently in a cooldown state and the button is in an 'Off' state
                if (skill.SkillState.CurrentState == Skill.SkillStates.SkillCooldown
                && button.State.CurrentState == JP_Input.ButtonStates.Off)
                {
                    skill.SkillInputStop(); // Stop any ongoing input actions for this skill
                }
            }
        }

        protected override void OnHit(GameObject instigator)
        {
            base.OnHit(instigator);
            for (int i = 0; i < _currentActiveSkills.Count; i++)
            {
                Skill skill = _currentActiveSkills[i];
                skill.OnHit(instigator);
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            for (int i = 0; i < _currentActiveSkills.Count; i++)
            {
                Skill skill = _currentActiveSkills[i];
                StopSkill(skill);
            }
        }

        protected override void OnRespawn()
        {
            base.OnRespawn();
            Setup();
        }

        /// <summary>
        /// Used for extended flexibility.
        /// </summary>
        protected virtual void Setup()
        {
            _character = GetComponent<Character>();
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            
            for (int i = 0; i < Skills.Count; i++)
            {
                if (_inputManager != null)
                    _buttons.Add(_inputManager.GetButtonFromID($"Skill_{i}"));

                Skill skill = Instantiate(Skills[i]);
                _currentActiveSkills.Add(skill);
                skill.Initialization(_character, i);
                UIManager.Instance.SetSkillImage(i, skill.Icon);
            }

            InitializeAnimatorParameters();
        }

        #region PUBLIC METHODS

        public virtual void StartSkill(Skill skill)
        {
            // If the Shoot action is enabled in the permissions, we continue, if not we do nothing. If the player is dead we do nothing.
            if ((skill == null)
            || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                return;
            }

            skill.SkillInputStart();
        }

        /// <summary>
        /// Stops the provided skill based on its current state and related conditions.
        /// </summary>
        /// <param name="skill">The skill to be stopped.</param>
        public virtual void StopSkill(Skill skill)
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
            skill.SkillStop();
        }

        public virtual int GetDamageTotal(Skill skill)
        {
            return Mathf.RoundToInt(BaseDamage * (DamageBonusPercentage / 100 + skill.ImpactPercentageModifier / 100));
        }

        public virtual void ModifyCharge(float amount)
        {
            float newCharge = Mathf.Clamp(Charge + amount, 0, MaxCharge);

            Charge = newCharge;
        }

        /// <summary>
        /// Returns a list of Skill objects that match the type passed.
        /// Returns references to the instanced versions of the skills, so they can be safely modified.
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <returns>A list of matching skills.</returns>
        public virtual List<T> GetActiveSkillsOfType<T>() where T : Skill
        {
            List<T> activeSkillsOfType = new List<T>();

            foreach (Skill skill in _currentActiveSkills)
            {
                T specificSkill = skill as T;
                if (specificSkill != null)
                {
                    activeSkillsOfType.Add(specificSkill);
                }
            }

            return activeSkillsOfType;
        }

        /// <summary>
        /// Returns a list of Skill objects that match the SkillType enum value passed.
        /// Returns references to the instanced versions of the skills, so they can be safely modified.
        /// </summary>
        /// <param name="skillType">The SkillType to search for</param>
        /// <returns>A list of matching skills.</returns>
        public virtual List<Skill> GetActiveSkillsOfType(Skill.SkillTypes skillType)
        {
            List<Skill> activeSkillsOfType = new List<Skill>();

            foreach (Skill skill in _currentActiveSkills)
            {
                if (skill != null) continue;
                if (skill.SkillType != skillType) continue;

                activeSkillsOfType.Add(skill);
            }

            return activeSkillsOfType;
        }

        public virtual Skill GetActiveSkillByIndex(int index)
        {
            return _currentActiveSkills[index];
        }

        #endregion

        protected const string _skillAnimationParameterName = "skill_";
        protected const string _idleAnimationParameterName = "Idle";
        protected List<int> _skillAnimationParameter = new List<int>();
        protected int _idleAnimationParameter;

        protected override void InitializeAnimatorParameters()
        {
            _skillAnimationParameter = new List<int>();

            for (int i = 0; i < _currentActiveSkills.Count; i++)
            {
                Skill skill = _currentActiveSkills[i];

                //skill.InitializeAnimatorParameters();
                RegisterAnimatorParameter(_skillAnimationParameterName + skill.ID, AnimatorControllerParameterType.Bool, out int skillAnimationParameter);

                _skillAnimationParameter.Add(skillAnimationParameter);

                //Debug.Log($"{_skillAnimationParameterName + skill.ID} parameter = {_skillAnimationParameter}");
            }
            RegisterAnimatorParameter(_idleAnimationParameterName, AnimatorControllerParameterType.Bool, out _idleAnimationParameter);
        }

        public override void UpdateAnimator()
        {
            for (int i = 0; i < _currentActiveSkills.Count; i++)
            {
                Skill skill = _currentActiveSkills[i];

                //skill.UpdateAnimator(_animator, _movement.CurrentState);
                //Debug.Log("ID = "+ _skillAnimationParameterName + skill.ID + " || _movement.CurrentState == skill.ActiveState = " + (_movement.CurrentState == skill.ActiveState));
                AnimatorExtensions.UpdateAnimatorBool(_animator, _skillAnimationParameter[i], _movement.CurrentState == skill.ActiveState, _character.AnimatorParameters);
            }
            AnimatorExtensions.UpdateAnimatorBool(_animator, _idleAnimationParameter, _movement.CurrentState == CharacterStates.MovementStates.Idle, _character.AnimatorParameters);
        }
    }
}
