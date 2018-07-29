namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using TeamBuilder.App.Utilities;

    public class ExitCommand : IExecutable
    {
        public string Execute(string[] args)
        {
            Checker.CheckLength(0, args);
            Environment.Exit(0);
            return null;
        }
    }
}
