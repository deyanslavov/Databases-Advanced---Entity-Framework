namespace AutoMapper.Client.Commands
{
    using AutoMapper.Services;
    using Contracts;

    public class AddEmployeeCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public AddEmployeeCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            string firstName = args[0];
            string lastName = args[1];
            decimal salary = decimal.Parse(args[2]);

            this.employeeService.AddEmployee(firstName, lastName, salary);

            string result = $"Successfully added user {firstName} {lastName}!";

            return result;
        }
    }
}
