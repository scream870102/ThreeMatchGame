namespace TmUnity{
    interface IState{
        void Init();
        void Tick();
        void End();
    }
}
