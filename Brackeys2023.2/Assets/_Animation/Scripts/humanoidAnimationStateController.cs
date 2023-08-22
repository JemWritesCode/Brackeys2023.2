using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public class humanoidAnimationStateController : MonoBehaviour
    {

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

            GetComponent<Animator>().SetFloat( "vel_fbw", vel_fbw );
            GetComponent<Animator>().SetFloat( "vel_lat", vel_lat );

        }
        
    }

}

