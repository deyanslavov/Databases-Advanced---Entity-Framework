namespace AutoMapper.Client.Commands
{
    using AutoMapper.Client.Contracts;
    using System;

    public class ExitCommand : ICommand
    {
        public string Execute(params string[] args)
        {
            System.Console.WriteLine("Good Bye!");
            Environment.Exit(0);
            return string.Empty;
        }
    }
}
