namespace PhotoShare.Client.Core
{
    using System;
    using System.Linq;
    using Contracts;

    public class Engine
    {
        private readonly CommandDispatcher commandDispatcher;

        public Engine(CommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    Console.Write("Please enter a command: ");
                    string input = Console.ReadLine().Trim();

                    string[] data = input.Split(' ');

                    IExecutable command = this.commandDispatcher.DispatchCommand(data);

                    string result = command.Execute(data.Skip(1).ToArray());
                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
