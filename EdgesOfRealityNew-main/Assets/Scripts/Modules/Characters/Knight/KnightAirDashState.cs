using UnityEngine;

namespace Metroidvania.Characters.Knight
{
    public class KnightAirDashState : KnightStateBase
    {
        private float _elapsedTime;
        private float _lastExitTime;

        public bool isInCooldown => Time.time - _lastExitTime < character.data.airDashCooldown;

        public KnightAirDashState(KnightStateMachine machine) : base(machine) { }

public override bool CanEnter()
{
    // Allow air dash if not grounded or if character is in jump state (allow immediate dash after jump)
    bool isJumping = machine.currentState is KnightJumpState;
    bool canEnter = ( !character.collisionChecker.isGrounded || isJumping )
        && character.dashAction.WasPerformedThisFrame()
        && !isInCooldown
        && character.staminaAttribute.currentValue >= character.data.staminaConsumptionPerAirDash;

    Debug.Log($"KnightAirDashState.CanEnter: isGrounded={!character.collisionChecker.isGrounded}, isJumping={isJumping}, dashInput={character.dashAction.WasPerformedThisFrame()}, isInCooldown={!isInCooldown}, stamina={character.staminaAttribute.currentValue}, canEnter={canEnter}");

    return canEnter;
}

public override void Enter(KnightStateBase previousState)
{
    Debug.Log("KnightAirDashState.Enter called");

    _elapsedTime = 0;

    character.staminaAttribute.currentValue -= character.data.staminaConsumptionPerAirDash;
    if (character.staminaAttribute.currentValue < 0)
        character.staminaAttribute.currentValue = 0;

    character.SetColliderBounds(character.data.standColliderBounds);
    character.SwitchAnimation(KnightCharacterController.RollAnimHash, true);
    character.FlipFacingDirection(character.facingDirection);
    character.PlaySFX("Roll");
}

public override void Transition()
{
    // Prevent transitioning to other states until air dash duration completes
    if (_elapsedTime > character.data.airDashDuration)
    {
        machine.EnterState(machine.fallState);
    }
    // Else do nothing to keep air dash state active and animation playing
}

public override bool TryEnter()
{
    if (CanEnter())
    {
        machine.EnterState(this);
        // Set a flag in the character to block other animations
        character.isInAirDash = true;
        return true;
    }
    return false;
}

        public override void Exit()
        {
            _lastExitTime = Time.time;
            // Clear the flag when exiting air dash
            character.isInAirDash = false;
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;
        }

public override void PhysicsUpdate()
{
    float dashProgress = _elapsedTime / character.data.airDashDuration;
    float curveMultiplier = 1.0f; // Could add animation curve if desired
    Vector2 dashVelocity = new Vector2(character.data.airDashSpeed * curveMultiplier * character.facingDirection, 0.0f);
    character.rb.linearVelocity = dashVelocity;
}

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }
    }
}
