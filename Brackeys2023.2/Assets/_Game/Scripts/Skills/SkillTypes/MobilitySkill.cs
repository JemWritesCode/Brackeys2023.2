using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    [CreateAssetMenu(fileName = "New MobilitySkill", menuName = "Skills/Mobility")]
    public class MobilitySkill : Skill
    {
        public enum DashModes { Fixed, MainMovement, MousePosition }

        [Header("MobilitySkill")]
        public DashModes DashMode = DashModes.MainMovement;
        public Vector3 DashDirection = Vector3.forward;
        public float DashDistance = 6f;
        public AnimationCurve DashCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        protected Vector3 _dashOrigin;
        protected Vector3 _dashDestination;
        protected Vector3 _newPosition;

        /// <summary>
        /// Handle what happens when the skill starts
        /// </summary>
        public override void SkillStart()
        {
            _skillHandler.SetMovementState(ActiveState);
            SkillState.ChangeState(SkillStates.SkillStart);

            _dashOrigin = Owner.transform.position;

            switch (DashMode)
            {
                case DashModes.MainMovement:
                    _dashDestination = Owner.transform.position + _controller.CurrentDirection.normalized * DashDistance;
                    _dashDestination.y = 0f;
                    break;

                case DashModes.Fixed:
                    _dashDestination = Owner.transform.position + DashDirection.normalized * DashDistance;
                    break;

                case DashModes.MousePosition:
                    Camera mainCamera = Camera.main;
                    Vector3 inputPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    inputPosition.y = Owner.transform.position.y;
                    _dashDestination = Owner.transform.position + (inputPosition - Owner.transform.position).normalized * DashDistance;
                    break;
            }
        }

        public override void SkillUse()
        {
            _newPosition = Vector3.Lerp(_dashOrigin, _dashDestination, DashCurve.Evaluate(_skillDurationTimer.ElapsedTime / SkillDuration));
            _controller.MovePosition(_newPosition);
        }

        public override void SkillStop()
        {
            base.SkillStop();
            _controller.Rigidbody.velocity = Vector2.zero;
        }
    }
}
