using Metroidvania.Animations;
using Metroidvania.Characters;
using Metroidvania.Entities;
using Metroidvania.SceneManagement;
using UnityEngine;

namespace Metroidvania.Characters.NPC
{
    public class NPCCharacterController : CharacterBase
    {
        [SerializeField] private GameObject m_gfxGameObject;

        private SpriteSheetAnimator _animator;

        // Define animation hashes
        public static readonly int IdleAnimHash = Animator.StringToHash("Idle");
        public static readonly int WalkAnimHash = Animator.StringToHash("Walk");
        public static readonly int AttackAnimHash = Animator.StringToHash("Attack");
        public static readonly int GirlIdleAnimHash = Animator.StringToHash("girl_idle");

        private int currentAnimationHash;

        private void Awake()
        {
            if (m_gfxGameObject == null)
            {
                Debug.LogError("Gfx GameObject is not assigned in NPCCharacterController.");
                return;
            }

            _animator = m_gfxGameObject.GetComponent<SpriteSheetAnimator>();
            if (_animator == null)
            {
                Debug.LogError("SpriteSheetAnimator component not found on Gfx GameObject.");
            }

            facingDirection = 1; // Default facing right
        }

        private NPCStateMachine stateMachine;

        private void Start()
        {
            stateMachine = new NPCStateMachine(this);
            stateMachine.ChangeState(new NPCIdleState(stateMachine));
        }

        private void Update()
        {
            stateMachine?.Update();
        }

        public void SwitchAnimation(int animationHash, bool force = false)
        {
            if (!force && currentAnimationHash == animationHash)
                return;

            if (_animator != null)
            {
                Debug.Log($"SwitchAnimation called with animationHash: {animationHash}");
                _animator.SetSheet(animationHash);
                currentAnimationHash = animationHash;
            }
            else
            {
                Debug.LogWarning("SwitchAnimation called but _animator is null");
            }
        }

        // Example method to set idle animation
        public void PlayIdle()
        {
            SwitchAnimation(IdleAnimHash);
        }

        // Example method to set walk animation
        public void PlayWalk()
        {
            SwitchAnimation(WalkAnimHash);
        }

        // Example method to set attack animation
        public void PlayAttack()
        {
            SwitchAnimation(AttackAnimHash);
        }

        // Example method to set girl_idle animation
        public void PlayGirlIdle()
        {
            SwitchAnimation(GirlIdleAnimHash);
        }

        public override void OnTakeHit(EntityHitData hitData)
        {
            // Implement NPC hit reaction if needed
        }

        public override void OnSceneTransition(SceneLoader.SceneTransitionData transitionData)
        {
            // Implement scene transition logic if needed
        }

        public override void BeforeUnload(SceneLoader.SceneUnloadData unloadData)
        {
            // Implement unload logic if needed
        }
    }
}
