namespace AutoMapper.Client.Commands
{
    using AutoMapper.Services;
    using Contracts;

    public class SetAddressCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public SetAddressCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            string address = args[1];

            this.employeeService.SetAddress(employeeId, address);

            string result = $"Successfully set address to employee {employeeId}!";

            return result;
        }
    }
}
