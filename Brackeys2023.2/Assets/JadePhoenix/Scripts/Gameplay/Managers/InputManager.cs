using JadePhoenix.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    /// <summary>
    /// Handles the inputs given by the player, processing button states and movement.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        #region Fields and Properties

        [Header("Status")]
        public bool InputDetectionActive = true;

        [Header("Player Binding")]
        public string PlayerID = "Player";
        public enum ControlProfiles { Gamepad, Keyboard }

        [Header("Movement Settings")]
        public bool SmoothMovement = true;
        public Vector2 Threshold = new Vector2(0.1f, 0.4f);

        public JP_Input.Button PauseButton { get; protected set; }
        public JP_Input.Button ShootButton { get; protected set; }
        public JP_Input.Button DashButton { get; protected set; }
        public JP_Input.Button Skill_0Button { get; protected set; }
        public JP_Input.Button Skill_1Button { get; protected set; }
        public JP_Input.Button Skill_2Button { get; protected set; }
        public JP_Input.Button Skill_3Button { get; protected set; }

        public Vector2 PrimaryMovement { get { return _primaryMovement; } }

        public List<JP_Input.Button> ButtonList;
        protected Vector2 _primaryMovement = Vector2.zero;
        protected string _axisHorizontal;
        protected string _axisVertical;

        #endregion

        #region Unity Lifecycle Methods

        protected virtual void Start()
        {
            InitializeButtons();
            InitializeAxis();
        }

        protected virtual void Update()
        {
            if (InputDetectionActive)
            {
                SetMovement();
                GetInputButtons();
            }
        }

        protected virtual void LateUpdate()
        {
            ProcessButtonStates();
        }

        #endregion

        #region Initialization Methods

        protected virtual void InitializeButtons()
        {
            ButtonList = new List<JP_Input.Button>
            {
                //(PauseButton = new JP_Input.Button(PlayerID, "Pause", PauseButtonDown, PauseButtonPressed, PauseButtonUp)),
                (Skill_0Button = new JP_Input.Button(PlayerID, "Skill_0", Skill_0ButtonDown, Skill_0ButtonPressed, Skill_0ButtonUp)),
                (Skill_1Button = new JP_Input.Button(PlayerID, "Skill_1", Skill_1ButtonDown, Skill_1ButtonPressed, Skill_1ButtonUp)),
                (Skill_2Button = new JP_Input.Button(PlayerID, "Skill_2", Skill_2ButtonDown, Skill_2ButtonPressed, Skill_2ButtonUp)),
                (Skill_3Button = new JP_Input.Button(PlayerID, "Skill_3", Skill_3ButtonDown, Skill_3ButtonPressed, Skill_3ButtonUp)),
                //(DashButton = new JP_Input.Button(PlayerID, "Dash", DashButtonDown, DashButtonPressed, DashButtonUp)),
            };
        }

        protected virtual void InitializeAxis()
        {
            _axisHorizontal = $"{PlayerID}_Horizontal";
            _axisVertical = $"{PlayerID}_Vertical";
        }

        #endregion

        #region Movement and Button Input Methods

        public virtual void SetMovement()
        {
            if (InputDetectionActive)
            {
                if (SmoothMovement)
                {
                    _primaryMovement.x = Input.GetAxis(_axisHorizontal);
                    _primaryMovement.y = Input.GetAxis(_axisVertical);
                }
                else
                {
                    _primaryMovement.x = Input.GetAxisRaw(_axisHorizontal);
                    _primaryMovement.y = Input.GetAxisRaw(_axisVertical);
                }
            }
        }

        protected virtual void GetInputButtons()
        {
            foreach (JP_Input.Button button in ButtonList)
            {
                if (Input.GetButtonDown(button.ButtonID))
                {
                    button.TriggerButtonDown();
                }
                if (Input.GetButton(button.ButtonID))
                {
                    button.TriggerButtonPressed();
                }
                if (Input.GetButtonUp(button.ButtonID))
                {
                    button.TriggerButtonUp();
                }
            }
        }

        #endregion

        #region Button State Management Methods

        public virtual void ProcessButtonStates()
        {
            foreach (JP_Input.Button button in ButtonList)
            {
                if (button.State.CurrentState == JP_Input.ButtonStates.ButtonDown)
                {
                    button.State.ChangeState(JP_Input.ButtonStates.ButtonPressed);
                }
                if (button.State.CurrentState == JP_Input.ButtonStates.ButtonUp)
                {
                    button.State.ChangeState(JP_Input.ButtonStates.Off);
                }
            }
        }

        #endregion

        #region Public Methods

        public virtual JP_Input.Button GetButtonFromID(string buttonID)
        {
            JP_Input.Button targetButton = null;

            if (ButtonList == null)
            {
                Debug.Log($"{this.GetType()}.GetButtonFromID: ButtonList not found.", gameObject);
                return null;
            }

            foreach (JP_Input.Button button in ButtonList)
            {
                if (button.ButtonID == $"{PlayerID}_{buttonID}")
                {
                    targetButton = button;
                }
            }

            if (targetButton == null)
            {
                Debug.LogWarning($"{this.GetType()}.GetButtonFromID: [{PlayerID}_{buttonID}] not found.", gameObject);
            }

            return targetButton;
        }

        #endregion

        #region Button Event Methods

        public virtual void PauseButtonDown() { PauseButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
        public virtual void PauseButtonPressed() { PauseButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
        public virtual void PauseButtonUp() { PauseButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

        public virtual void Skill_0ButtonDown() { Skill_0Button.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
        public virtual void Skill_0ButtonPressed() { Skill_0Button.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
        public virtual void Skill_0ButtonUp() { Skill_0Button.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

        public virtual void Skill_1ButtonDown() { Skill_1Button.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
        public virtual void Skill_1ButtonPressed() { Skill_1Button.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
        public virtual void Skill_1ButtonUp() { Skill_1Button.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

        public virtual void Skill_2ButtonDown() { Skill_2Button.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
        public virtual void Skill_2ButtonPressed() { Skill_2Button.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
        public virtual void Skill_2ButtonUp() { Skill_2Button.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

        public virtual void Skill_3ButtonDown() { Skill_3Button.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
        public virtual void Skill_3ButtonPressed() { Skill_3Button.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
        public virtual void Skill_3ButtonUp() { Skill_3Button.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

        public virtual void DashButtonDown() { DashButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
        public virtual void DashButtonPressed() { DashButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
        public virtual void DashButtonUp() { DashButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

        #endregion
    }
}

