namespace Metroidvania.Characters.NPC
{
    public abstract class NPCStateBase
    {
        protected NPCStateMachine stateMachine;

        public NPCStateBase(NPCStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}
