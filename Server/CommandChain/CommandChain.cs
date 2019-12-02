using System;
using System.Threading.Tasks;

#nullable enable

namespace Server
{
    /// <summary>
    /// Abstract Handler in chain of responsibility pattern.
    /// </summary>
    public class CommandChain
    {
        // The next Handler in the chain
        private CommandChain? Next { get; set; }
        private Func<Func<Task>, Task> Action { get; }

        public CommandChain(Func<Func<Task>, Task> action)
        {
            Action = action;
        }

        /// <summary>
        /// Sets the Next piece of code to be execuded in the conveyor.
        /// </summary>
        public CommandChain Use(Func<Func<Task>, Task> action)
        {
            return Use(new CommandChain(action));
        }

        /// <summary>
        /// Sets the Next piece of code to be execuded in the conveyor.
        /// </summary>
        public CommandChain Use(CommandChain cmd)
        {
            var lastInChain = this;

            while (lastInChain.Next != null)
                lastInChain = lastInChain.Next;

            lastInChain.Next = cmd;
            return this;
        }

        public async Task ExecAsync()
        {
            await Action(async () => await ExecAsync(Next));
        }

        private async Task ExecAsync(CommandChain? next)
        {
            if (next == null)
                return;

            await next.Action(async () => await ExecAsync(next.Next));
        }
    }
}
