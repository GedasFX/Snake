using System.Threading.Tasks;

namespace Server
{
    public class ArenaAdapter : Arena
    {
        private readonly ArenaAdaptee _adaptee = new ArenaAdaptee();

        public override Task StartAsync() => base.StartAsync();

        private class ArenaAdaptee
        {

        }
    }
}