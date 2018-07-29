namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;
    using System.Globalization;

    public class CreateEventCommand : IExecutable
    {
        // •	CreateEvent <name> <description> <startDate hour> <endDate hour>

        public string Execute(string[] args)
        {
            Checker.CheckLength(6, args);

            User currentUser = AuthenticationManager.GetCurrentUser();

            string name = args[0];
            string description = args[1];
            string startDateString = $"{args[2]} {args[3]}";
            string endDateString = $"{args[4]} {args[5]}";

            bool startDateIsValid = DateTime.TryParseExact(startDateString, Constants.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);

            bool endDateIsValid = DateTime.TryParseExact(endDateString, Constants.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);

            if (!startDateIsValid || !endDateIsValid)
            {
                throw new ArgumentException(Constants.ErrorMessages.InvalidDateFormat);
            }

            if (startDate > endDate)
            {
                throw new ArgumentException(Constants.ErrorMessages.StartDateIsAfterEndDate);
            }

            Event @event = new Event()
            {
                Name = name,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                CreatorId = currentUser.Id,
            };

            ContextHelper.AddEntity<Event>(@event);

            return string.Format(Constants.SuccessMessages.SuccessfullyCreatedEvent, name);
        }
    }
}
