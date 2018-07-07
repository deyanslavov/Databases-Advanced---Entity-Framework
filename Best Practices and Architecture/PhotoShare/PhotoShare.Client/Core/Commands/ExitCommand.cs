namespace PhotoShare.Client.Core.Commands
{
    using System;
    using Contracts;

    public class ExitCommand : IExecutable
    {
        public string Execute(string[] args)
        {
            Environment.Exit(0);
            return "Bye-bye!";
        }
    }
}
