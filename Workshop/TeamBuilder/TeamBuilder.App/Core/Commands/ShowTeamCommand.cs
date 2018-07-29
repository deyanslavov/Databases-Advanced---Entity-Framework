namespace TeamBuilder.App.Core.Commands
{
    using Contracts;
    using Utilities;

    public class ShowTeamCommand : IExecutable
    {
        // •	ShowTeam <teamName>

        public string Execute(string[] args)
        {
            Checker.CheckLength(1, args);

            string teamName = args[0];

            string result =  ContextHelper.ShowTeam(teamName);
            return result;
        }
    }
}
