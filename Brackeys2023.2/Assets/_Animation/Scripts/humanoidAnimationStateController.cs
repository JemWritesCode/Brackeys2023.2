using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public class humanoidAnimationStateController : MonoBehaviour
    {

        // Internal class parameters
        private Animator animator;
        private int velFbwHash;
        private int velLatHash;
        private int takeItemHash;
        private int meleePunchHash;
        private int meleeRollHash;
        private int meleePunishWindupHash;
        private int meleePunishHitHash;
        private int meleeSlamHash;

        void Start()
        {

            animator = GetComponent<Animator>();
            velFbwHash = Animator.StringToHash("vel_fbw");
            velLatHash = Animator.StringToHash("vel_lat");
            takeItemHash = Animator.StringToHash("action_take");
            meleePunchHash = Animator.StringToHash("melee_punch");
            meleeRollHash = Animator.StringToHash("melee_roll");
            meleePunishWindupHash = Animator.StringToHash("melee_punish_windup");
            meleePunishHitHash = Animator.StringToHash("melee_punish_hit");
            meleeSlamHash = Animator.StringToHash("melee_slam");

        }

        /* animateMotion
         * It indicates the State Controller the motion animation
         * to perform according to two input parameters:
         * - vel_fbw: Forward/backward velocity.
         * - vel_lat: Lateral velocity.
         * For both input parameters, the values work this way:
         * - 0.0 = Idle
         * - 1.0 = Walk forwards/right
         * - 2.0 = Run forwards/right
         * - -1.0 = Walk backwards/left
         * - -2.0 = Run backwards/left
        */
        public void animateMotion( float vel_fbw, float vel_lat)
        {

            animator.SetFloat( velFbwHash, vel_fbw );
            animator.SetFloat( velLatHash, vel_lat );

        }

        public void animateActionTakeItem()
        {
            animator.ResetTrigger( takeItemHash );
            animator.SetTrigger( takeItemHash );
        }

        public void animateMeleePunch()
        {
            animator.ResetTrigger( meleePunchHash );
            animator.SetTrigger( meleePunchHash );
        }

        public void animateMeleeRoll()
        {
            animator.ResetTrigger( meleeRollHash );
            animator.SetTrigger( meleeRollHash );
        }

        public void animateMeleeStartPunish()
        {
            animator.ResetTrigger( meleePunishWindupHash );
            animator.SetTrigger( meleePunishWindupHash );
        }

        public void animateMeleeInterruptPunish()
        {
            animator.ResetTrigger( meleePunishHitHash );
            animator.SetTrigger( meleePunishHitHash );
        }

        public void animateMeleeSlam()
        {
            animator.ResetTrigger( meleeSlamHash );
            animator.SetTrigger( meleeSlamHash );
        }
        
    }

}

