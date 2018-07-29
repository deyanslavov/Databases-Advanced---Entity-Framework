namespace TeamBuilder.App.Core.Commands
{
    using Contracts;
    using Utilities;

    public class ShowEventCommand : IExecutable
    {
        // •	ShowEvent <eventName>

        public string Execute(string[] args)
        {
            Checker.CheckLength(1, args);

            string eventName = args[0];

            string result = ContextHelper.ShowEvent(eventName);
            return result;
        }
    }
}
