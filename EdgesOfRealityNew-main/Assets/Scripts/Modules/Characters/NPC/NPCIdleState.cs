namespace Metroidvania.Characters.NPC
{
    public class NPCIdleState : NPCStateBase
    {
        public NPCIdleState(NPCStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.character.PlayGirlIdle();
        }

        public override void Update()
        {
            // Add logic to transition to other states if needed
        }
    }
}
