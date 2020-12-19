namespace TmUnity.Node
{
    enum NodeType
    {
        ATTACK,
        MANA,
        ENERGY,
        DEFENSE,
        CHEST,
    }

    enum ChestType
    {
        HP_RECOVER,
        ENERGY_UP,
        ATK_UP,
        DEF_UP,
    }

}
namespace TmUnity
{
    enum GameState
    {
        START,
        END,
        WAIT,
        ACTION,
        ANIMATE,
        ENEMY,
    }

    enum VFXType
    {
        ELIMINATE,
        HEAL,
        BUFF,
    }
}