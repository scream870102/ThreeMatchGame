namespace TmUnity.Node
{
    enum NodeType
    {
        NORMAL,
        CHARGE,
        ENERGY,
        DEFENSE,
        CHEST,
    }
    enum Direction
    {
        LEFT,
        UP,
        RIGHT,
        DOWN,
        NONE,
    }
    enum ChestType
    {
        HP_RECOVER,
        ENERGY_UP,
        ATK_UP,
        DEF_UP,
        CHARGE_COUNT_PLUS,
    }



}
namespace TmUnity
{
    enum GameState
    {
        WAIT,
        ACTION,
        ANIMATE,
        ENEMY,
    }
}