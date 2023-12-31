using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JadePhoenix.Gameplay
{
    public class DamageOnTouch : MonoBehaviour
    {
        public enum KnockbackStyles { NoKnockback, AddForce }
        public enum KnockbackDirections { BasedOnOwnerPosition, BasedOnSpeed }

        [Header("Targets")]
        public LayerMask TargetLayerMask;
        public bool PerfectImpact = false;

        [Header("DamageCaused")]
        public int BaseDamage = 10;
        public int DamageCaused;
        public KnockbackStyles DamageCausedKnockbackType = KnockbackStyles.AddForce;
        public KnockbackDirections DamageCausedKnockbackDirection;
        public Vector3 DamageCausedKnockbackForce = new Vector3(10, 2, 0);
        public float InvincibilityDuration = 0.5f;

        [Header("Damage Taken")]
        /// Parameters that determine damage taken when this object collides with a target
        public int DamageTakenEveryTime = 0;
        /// Damage taken when collision is damageable
        public int DamageTakenDamageable = 0;
        /// Damage taken when collision is nondamageable
        public int DamageTakenNonDamageable = 0;
        public KnockbackStyles DamageTakenKnockbackType = KnockbackStyles.NoKnockback;
        public KnockbackDirections DamageTakenKnockbackDirection;
        public Vector3 DamageTakenKnockbackForce = Vector3.zero;
        public float DamageTakenInvincibilityDuration = 0.5f;

        public GameObject Owner;
        public Character OwnerCharacter;
        public Action DamageDealt;

        protected Vector3 _lastPosition;
        protected Vector3 _velocity; 
        protected Vector3 _knockbackForce;
        protected float _startTime = 0f;
        protected Health _colliderHealth;
        protected TopDownController _topDownController;
        protected TopDownController _colliderTopDownController;
        protected Rigidbody _colliderRigidBody;
        protected Health _health;
        protected List<GameObject> _ignoredGameObjects = new List<GameObject>();
        protected Vector3 _collisionPoint;
        protected Vector3 _knockbackForceApplied;
        protected SphereCollider _circleCollider;
        protected BoxCollider _boxCollider;
        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;
        protected Vector3 _gizmoOffset;

        #region UNITY LIFECYCLE

        /// <summary>
        /// Initialization
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        protected virtual void Update()
        {
            ComputeVelocity();
        }

        /// <summary>
        /// OnEnable we set the start time to the current timestamp
        /// </summary>
        protected virtual void OnEnable()
        {
            _startTime = Time.time;
        }

        /// <summary>
        /// draws a cube or sphere around the damage area
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = _gizmosColor;

            if (_boxCollider != null)
            {
                if (_boxCollider.enabled)
                {
                    JP_Debug.DrawGizmoCube(this.transform, _boxCollider.center, _boxCollider.size, false);
                }
                else
                {
                    JP_Debug.DrawGizmoCube(this.transform, _boxCollider.center, _boxCollider.size, true);
                }
            }

            if (_circleCollider != null)
            {
                // Rotate the circle offset by the transform's rotation
                Vector3 rotatedOffset = this.transform.rotation * _circleCollider.center;

                if (_circleCollider.enabled)
                {
                    Gizmos.DrawSphere(this.transform.position + rotatedOffset, _circleCollider.radius * transform.localScale.x);
                }
                else
                {
                    Gizmos.DrawWireSphere(this.transform.position + rotatedOffset, _circleCollider.radius * transform.localScale.x);
                }
            }
        }

        #endregion

        protected virtual void Initialization()
        {
            if (Owner != null)
            {
                OwnerCharacter = Owner.GetComponent<Character>();
                _health = OwnerCharacter.Health;
                _topDownController = OwnerCharacter.TopDownController;
            }
            _ignoredGameObjects = new List<GameObject>();
            _health = GetComponent<Health>();
            _topDownController = GetComponent<TopDownController>();
            _boxCollider = GetComponent<BoxCollider>();
            _circleCollider = GetComponent<SphereCollider>();

            _gizmosColor = Color.red;
            _gizmosColor.a = 0.25f;

            DamageCaused = BaseDamage;
        }

        protected virtual void ComputeVelocity()
        {
            _velocity = (_lastPosition - transform.position) / Time.deltaTime;
            _lastPosition = transform.position;
        }

        /// <summary>
        /// When colliding, we apply damage
        /// </summary>
        protected virtual void Colliding(GameObject collision)
        {
            // We keep these if statements separate for ease of debugging.
            if (!this.isActiveAndEnabled)
            {
                //Debug.Log("Object is not active and enabled", gameObject);
                return;
            }

            // if the object we're colliding with is part of our ignore list, we do nothing and exit
            if (_ignoredGameObjects.Contains(collision))
            {
                //Debug.Log("Colliding object is part of the ignore list", gameObject);
                return;
            }

            // if what we're colliding with isn't part of the target layers, we do nothing and exit
            if (!JP_Layers.LayerInLayerMask(collision.layer, TargetLayerMask))
            {
                //Debug.Log("Colliding object is not part of the target layers", gameObject);
                return;
            }

            // if we're on our first frame, we don't apply damage
            if (Time.time == 0f)
            {
                //Debug.Log("First frame, not applying damage", gameObject);
                return;
            }

            //Debug.Log($"Error checks passed on {collision.name}, dealing damage", gameObject);

            _collisionPoint = this.transform.position;
            _colliderHealth = collision.gameObject.GetComponent<Health>();

            // if what we're colliding with is damageable
            if (_colliderHealth != null)
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                    OnCollideWithDamageable(collision);
                }
            }
            // if what we're colliding with can't be damaged
            else
            {
                OnCollideWithNonDamageable();
            }
        }

        /// <summary>
        /// Describes what happens when colliding with a damageable object
        /// </summary>
        protected virtual void OnCollideWithDamageable(GameObject collision)
        {
            // if what we're colliding with is a TopDownController, we apply a knockback force
            _colliderTopDownController = _colliderHealth.gameObject.GetComponent<TopDownController>();
            _colliderRigidBody = _colliderHealth.gameObject.GetComponent<Rigidbody>();

            if ((_colliderTopDownController != null) && (DamageCausedKnockbackForce != Vector3.zero) && (!_colliderHealth.Invulnerable) && (!_colliderHealth.ImmuneToKnockback))
            {
                _knockbackForce.x = DamageCausedKnockbackForce.x;
                _knockbackForce.y = DamageCausedKnockbackForce.y;

                if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnSpeed)
                {
                    Vector3 totalVelocity = (Vector3)_colliderTopDownController.Speed + _velocity;
                    _knockbackForce = Vector3.RotateTowards(DamageCausedKnockbackForce, totalVelocity.normalized, 10f, 0f);
                }
                if (DamageTakenKnockbackDirection == KnockbackDirections.BasedOnOwnerPosition)
                {
                    if (Owner.gameObject == null) { Owner = this.gameObject; }
                    Vector3 relativePosition = _colliderTopDownController.transform.position - Owner.transform.position;
                    _knockbackForce = Vector3.RotateTowards(DamageCausedKnockbackForce, relativePosition.normalized, 10f, 0f);
                }

                if (DamageCausedKnockbackType == KnockbackStyles.AddForce)
                {
                    _colliderTopDownController.Impact(_knockbackForce.normalized, _knockbackForce.magnitude);
                }
            }

            if (!_colliderHealth.Invulnerable)
            {
                if (SpawnManager.Instance != null)
                {
                    Vector3 collisionDirection = collision.transform.position - transform.position;
                    Vector3 spawnPosition = transform.position + collisionDirection.normalized * 1;
                    SpawnManager.Instance.SpawnAtPosition(spawnPosition);
                }
            }

            // we apply the damage to the thing we've collided with
            _colliderHealth.Damage(DamageCaused, gameObject, InvincibilityDuration);
            DamageDealt?.Invoke();

            if (DamageTakenEveryTime + DamageTakenDamageable > 0)
            {
                SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
            }
        }

        /// <summary>
        /// Describes what happens when colliding with a non damageable object
        /// </summary>
        protected virtual void OnCollideWithNonDamageable()
        {
            if (DamageTakenEveryTime + DamageTakenNonDamageable > 0)
            {
                SelfDamage(DamageTakenEveryTime + DamageTakenNonDamageable);
            }
        }

        /// <summary>
        /// Applies damage to itself
        /// </summary>
        protected virtual void SelfDamage(int damage)
        {
            if (_health != null)
            {
                _health.Damage(damage, gameObject, DamageTakenInvincibilityDuration);

                if ((_health.CurrentHealth <= 0) && PerfectImpact)
                {
                    this.transform.position = _collisionPoint;
                }
            }

            // if what we're colliding with is a TopDownController, we apply a knockback force
            if (_topDownController != null)
            {
                Vector2 totalVelocity = _colliderTopDownController.Speed + _velocity;
                Vector2 knockbackForce = Vector3.RotateTowards(DamageCausedKnockbackForce, totalVelocity.normalized, 10f, 0f);

                if (DamageTakenKnockbackType == KnockbackStyles.AddForce)
                {
                    _topDownController.AddForce(knockbackForce);
                }
            }
        }

        #region PUBLIC METHODS

        public void ClearIgnoreList()
        {
            _ignoredGameObjects.Clear();
        }

        public void IgnoreGameObject(GameObject newIgnoredGameObject)
        {
            _ignoredGameObjects.Add(newIgnoredGameObject);
        }

        public void StopIgnoringObject(GameObject ignoredGameObject)
        {
            _ignoredGameObjects.Remove(ignoredGameObject);
        }

        public virtual void SetGizmoSize(Vector3 newGizmoSize)
        {
            _boxCollider = GetComponent<BoxCollider>();
            _circleCollider = GetComponent<SphereCollider>();
            _gizmoSize = newGizmoSize;
        }

        public virtual void SetGizmoOffset(Vector3 newOffset)
        {
            _gizmoOffset = newOffset;
        }

        #endregion

        #region COLLISION METHODS

        /// <summary>
        /// When a collision with the player is triggered, we give damage to the player and knock it back
        /// </summary>
        /// <param name="collider">what's colliding with the object.</param>
        public virtual void OnTriggerStay(Collider collider)
        {
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger enter , we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>S
        public virtual void OnTriggerEnter(Collider collider)
        {
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger stay, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>
        public virtual void OnTriggerStay2D(Collider2D collider)
        {
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger enter, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>
        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
            Colliding(collider.gameObject);
        }

        #endregion
    }
}
