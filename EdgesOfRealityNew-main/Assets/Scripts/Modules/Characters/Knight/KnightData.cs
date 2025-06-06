using Metroidvania.Events;
using System;
using UnityEngine;

namespace Metroidvania.Characters.Knight
{
    [CreateAssetMenu(fileName = "KnightData", menuName = "Scriptables/Characters/Knight Data")]
    public class KnightData : ScriptableObject
    {
        [Serializable]
        public class Attack
        {
#if UNITY_EDITOR
            public bool drawGizmos;
#endif

            [Space]
            public float duration;

            public float horizontalMoveOffset;

            [Space]
            public float triggerTime;

            public float attackEndOffset;

            public Rect triggerCollider;

            [Space]
            public int damage;
            public float force;
        }

        [Serializable]
        public class ColliderBounds
        {
#if UNITY_EDITOR
            public bool drawGizmos;
#endif
            public Rect bounds;
        }

        [Header("Properties")]
        public CharacterAttributeData<float> lifeAttributeData;

        // Stamina attribute data added
        public CharacterAttributeData<float> staminaAttributeData;

        [Header("Stamina")]
        public float staminaRecoveryRate = 5f; // stamina points per second
        public float staminaConsumptionPerRoll = 20f;
        public float staminaConsumptionPerSlide = 15f;

        [Header("Events")]
        public ObjectEventChannel onDieChannel;
        public CharacterHurtEventChannel onHurtChannel;

        [Header("Ground Check")]
        public LayerMask groundLayer;

        [Header("Movement")]
        public float moveSpeed;
        public Rigidbody2D.SlideMovement slideMovement;
        public float airMoveSpeed;

        [Header("Jump")]
        public float jumpHeight;
        public float jumpFallMultiplier;
        public float jumpLowMultiplier;
        public float jumpCoyoteTime;

        [Header("Fall")]
        public float fallParticlesDistance;

        [Header("Crouch")]
        public float crouchWalkSpeed;
        public float crouchTransitionTime;

        [Header("Slide")]
        public float slideDuration;
        public float slideSpeed;
        public float slideCooldown;
        public AnimationCurve slideMoveCurve;
        public float slideTransitionTime;

        [Header("Roll")]
        public float rollDuration;
        public float rollSpeed;
        public float rollCooldown;
        public AnimationCurve rollHorizontalMoveCurve;

        [Header("Air Dash")]
        public float airDashDuration = 0.3f;
        public float airDashSpeed = 15f;
        public float airDashCooldown = 1.0f;
        public float staminaConsumptionPerAirDash = 20f;

        [Header("Attacks")]
        public LayerMask hittableLayer;
        public float attackComboMaxDelay;
        public Attack firstAttack;
        public Attack secondAttack;
        public Attack crouchAttack;

        [Header("Wall Abilities")]
        public float wallSlideSpeed;
        public Vector2 wallJumpForce;
        public float wallJumpDuration;

        [Header("Hurt")]
        public float hurtTime;

        [Header("Fake Walk")]
        public float fakeWalkOnSceneTransitionTime;

        [Header("Invincibility")]
        public float invincibilityAlphaChange;
        public float invincibilityFadeSpeed;
        public float defaultInvincibilityTime;

        [Header("Colliders")]
        public ColliderBounds standColliderBounds;
        public ColliderBounds crouchColliderBounds;
        public Rect crouchHeadRect;
    }
}
