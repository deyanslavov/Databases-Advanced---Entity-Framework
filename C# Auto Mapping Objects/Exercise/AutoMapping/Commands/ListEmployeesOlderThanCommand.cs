namespace AutoMapper.Client.Commands
{
    using Contracts;
    using Services;

    public class ListEmployeesOlderThanCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public ListEmployeesOlderThanCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int age = int.Parse(args[0]);

            string result = this.employeeService.ListEmployeesOlderThan(age);
            return result;
        }
    }
}
