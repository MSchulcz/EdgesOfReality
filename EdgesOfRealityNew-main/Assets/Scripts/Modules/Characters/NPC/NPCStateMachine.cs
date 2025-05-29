namespace Metroidvania.Characters.NPC
{
    public class NPCStateMachine
    {
        public NPCCharacterController character { get; private set; }
        private NPCStateBase currentState;

        public NPCStateMachine(NPCCharacterController character)
        {
            this.character = character;
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void ChangeState(NPCStateBase newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }
    }
}
