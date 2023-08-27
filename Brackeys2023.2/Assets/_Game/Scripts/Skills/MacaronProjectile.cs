using _Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class MacaronProjectile : Projectile
    {
        public float LowestPosition = 0;

        public override void Movement()
        {
            if (transform.position.y <= LowestPosition)
            {
                transform.position = new Vector3(transform.position.x, LowestPosition, transform.position.z);
                return;
            }
            base.Movement();
        }
    }
}
