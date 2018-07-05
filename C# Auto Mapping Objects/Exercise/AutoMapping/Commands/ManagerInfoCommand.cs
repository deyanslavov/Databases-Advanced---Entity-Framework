namespace AutoMapper.Client.Commands
{
    using Contracts;
    using Services;

    public class ManagerInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public ManagerInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            string result = this.employeeService.GetManagerInfo(employeeId);

            return result;
        }
    }
}
