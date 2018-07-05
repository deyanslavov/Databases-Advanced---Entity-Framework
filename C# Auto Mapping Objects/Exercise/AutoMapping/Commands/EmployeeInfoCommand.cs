namespace AutoMapper.Client.Commands
{
    using AutoMapper.Services;
    using Contracts;

    public class EmployeeInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public EmployeeInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            string result = this.employeeService.GetEmployeeInfo(employeeId);

            return result;
        }
    }
}
