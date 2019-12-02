namespace Server
{
    public static class CommandChainExtensions
    {
        public static CommandChain UseStaticMessage(this CommandChain commandChain, string message)
            => commandChain.Use(new LoggerChain(message));
    }
}
