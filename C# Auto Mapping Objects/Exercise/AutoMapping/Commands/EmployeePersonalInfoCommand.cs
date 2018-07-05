namespace AutoMapper.Client.Commands
{
    using AutoMapper.Services;
    using Contracts;

    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public EmployeePersonalInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            string result = this.employeeService.GetEmployeePersonalInfo(employeeId);

            return result;
        }
    }
}
