using Server.ArenaItems;

namespace Server
{
    // Abstract factory
    public interface IArenaObjectFactory
    {
        ICell CreateObject();
    }
}