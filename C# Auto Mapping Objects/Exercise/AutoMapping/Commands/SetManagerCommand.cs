namespace AutoMapper.Client.Commands
{
    using AutoMapper.Services;
    using Contracts;

    public class SetManagerCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public SetManagerCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            int managerId = int.Parse(args[1]);

            string result = this.employeeService.SetManager(employeeId, managerId);

            return result;
        }
    }
}
