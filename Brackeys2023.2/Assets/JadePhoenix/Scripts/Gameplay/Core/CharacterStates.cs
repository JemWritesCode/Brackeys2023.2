using System;
using UnityEngine;

namespace JadePhoenix.Gameplay
{
    public class CharacterStates : MonoBehaviour
    {
        /// The possible character conditions
        public enum CharacterConditions
        {
            Normal,
            ControlledMovement,
            Paused,
            Dead
        }

        /// The possible Movement States the character can be in. These usually correspond to their own class, 
        /// but it's not mandatory
        [Flags]
        public enum MovementStates
        {
            Null            = 0,
            Idle            = 1 << 0,
            Walking         = 1 << 1,
            Dashing         = 1 << 2,
            Attacking       = 1 << 3,  
            Jumping         = 1 << 4,
            Falling         = 1 << 5,
        }
    }
}

