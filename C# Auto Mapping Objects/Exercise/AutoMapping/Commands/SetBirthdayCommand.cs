namespace AutoMapper.Client.Commands
{
    using AutoMapper.Services;
    using Contracts;

    public class SetBirthdayCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public SetBirthdayCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            string date = args[1];

            this.employeeService.SetBirthday(employeeId, date);

            string result = $"Successfully set birthday to employee {employeeId}!";

            return result;
        }
    }
}
