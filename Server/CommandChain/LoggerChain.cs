namespace Server
{
    /// <summary>
    /// Adds a message logger into the execution chain.
    /// </summary>
    public class LoggerChain : CommandChain
    {
        public LoggerChain(string message)
            : base(async next =>
        {
            Logger.Instance.LogMessage(message);
            await next();
        })
        { }
    }
}
