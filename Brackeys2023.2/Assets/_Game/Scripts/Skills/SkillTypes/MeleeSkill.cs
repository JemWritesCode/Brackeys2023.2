using JadePhoenix.Gameplay;
using JadePhoenix.Tools;
using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    [CreateAssetMenu(fileName = "New MeleeSkill", menuName = "Skills/MeleeSkill")]
    public class MeleeSkill : Skill
    {
        public enum MeleeDamageAreaShapes { Box, Sphere }

        [Header("DamageArea")]
        [Tooltip("The shape of the damage area.")]
        public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Box;

        [Tooltip("The dimensions of the damage area.")]
        public Vector3 AreaSize = new Vector3(1, 1);

        [Tooltip("Offset position of the damage area relative to the object.")]
        public Vector3 AreaOffset = new Vector3(1, 0);

        [Header("Damage Area Timing")]
        [Tooltip("The initial delay before the damage area becomes active.")]
        public float InitialDelay = 0f;

        [Tooltip("Base duration the damage area stays active.")]
        public float BaseActiveDuration = 2f;

        [Tooltip("Variable to modify or extend the active duration dynamically.")]
        public float ActiveDuration;

        [Header("Damage Caused")]
        [Tooltip("LayerMask determining which objects can be damaged.")]
        public LayerMask TargetLayerMask;

        [Tooltip("Amount of damage dealt by the area.")]
        public int DamageCaused = 10;

        [Tooltip("Style or type of knockback applied when damaged.")]
        public DamageOnTouch.KnockbackStyles Knockback;

        [Tooltip("Force applied for knockback. X for horizontal, Y for vertical.")]
        public Vector2 KnockbackForce = new Vector2(10, 2);

        [Tooltip("Duration for which the target remains invincible after being damaged.")]
        public float InvincibilityDuration = 0.5f;

        [Tooltip("Determines if the owner can be damaged by this damage area.")]
        public bool CanDamageOwner = false;

        protected Collider _damageAreaCollider;
        protected bool _attackInProgress = false;
        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;
        protected BoxCollider _boxCollider;
        protected SphereCollider _sphereCollider;
        protected Vector3 _gizmoOffset;
        protected DamageOnTouch _damageOnTouch;
        protected GameObject _damageArea;

        public override void Initialization(Character owner)
        {
            base.Initialization(owner);

            if (_damageArea == null)
            {
                CreateDamageArea();
                DisableDamageArea();
            }
            if (Owner != null)
            {
                _damageOnTouch.Owner = Owner.gameObject;
            }

            ActiveDuration = BaseActiveDuration;
        }

        protected virtual void CreateDamageArea()
        {
            _damageArea = new GameObject();
            _damageArea.name = $"{this.name}DamageArea";
            _damageArea.transform.SetPositionAndRotation(Owner.transform.position, Owner.transform.rotation);
            _damageArea.transform.SetParent(Owner.transform);
            _damageArea.layer = Owner.gameObject.layer;

            if (DamageAreaShape == MeleeDamageAreaShapes.Box)
            {
                _boxCollider = _damageArea.AddComponent<BoxCollider>();
                _boxCollider.center = AreaOffset;
                _boxCollider.size = AreaSize;
                _damageAreaCollider = _boxCollider;
                _damageAreaCollider.isTrigger = true;
            }

            if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
            {
                _sphereCollider = _damageArea.AddComponent<SphereCollider>();
                _sphereCollider.center = AreaOffset;
                _sphereCollider.radius = AreaSize.x / 2;
                _damageAreaCollider = _sphereCollider;
                _damageAreaCollider.isTrigger = true;
            }

            Rigidbody rigidbody = _damageArea.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();
            _damageOnTouch.SetGizmoSize(AreaSize);
            _damageOnTouch.SetGizmoOffset(AreaOffset);
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.DamageCaused = DamageCaused;
            _damageOnTouch.DamageCausedKnockbackType = Knockback;
            _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;

            if (!CanDamageOwner)
            {
                _damageOnTouch.IgnoreGameObject(Owner.gameObject);
            }
        }

        protected virtual void EnableDamageArea()
        {
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = true;
            }
        }

        protected virtual void DisableDamageArea()
        {
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = false;
            }
            else
            {
                Debug.LogError($"{this.GetType()}.DisableDamageArea: _damageAreaCollider not found.", Owner.gameObject);
            }
        }

        public override void SkillUse()
        {
            base.SkillUse();
            Owner.StartCoroutine(MeleeSkillAttackCoroutine());
        }

        protected virtual IEnumerator MeleeSkillAttackCoroutine()
        {
            if (_attackInProgress) { yield break; }

            _attackInProgress = true;

            yield return new WaitForSeconds(InitialDelay);
            EnableDamageArea();

            yield return new WaitForSeconds(ActiveDuration);
            DisableDamageArea();

            _attackInProgress = false;
        }

        public override void DrawGizmos()
        {
            if (Owner == null) return;

            // Store the original Gizmos matrix
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            // Set the Gizmos matrix to the Owner's transform matrix
            Gizmos.matrix = Matrix4x4.TRS(Owner.transform.position, Owner.transform.rotation, Vector3.one);

            switch (DamageAreaShape)
            {
                case MeleeDamageAreaShapes.Box:
                    Gizmos.DrawWireCube(AreaOffset, AreaSize);
                    break;

                case MeleeDamageAreaShapes.Sphere:
                    Gizmos.DrawWireSphere(AreaOffset, AreaSize.x / 2);
                    break;
            }

            // Restore the original Gizmos matrix
            Gizmos.matrix = oldGizmosMatrix;
        }
    }
}
