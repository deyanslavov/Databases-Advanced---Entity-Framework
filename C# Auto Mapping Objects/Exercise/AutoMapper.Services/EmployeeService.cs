namespace AutoMapper.Services
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using AutoMapper.Data;
    using AutoMapper.ModelDtos;
    using AutoMapper.Models;
    using AutoMapper.QueryableExtensions;

    public class EmployeeService
    {
        private readonly EmployeeDbContext db;

        public EmployeeService(EmployeeDbContext db)
        {
            this.db = db;
        }

        public void AddEmployee(string firstName, string lastName, decimal salary)
        {
            Employee employee = new Employee()
            {
                FirstName = firstName,
                LastName = lastName,
                Salary = salary,
            };

            this.db.Employees.Add(employee);

            this.db.SaveChanges();
        }

        public void SetBirthday(int employeeId, string date)
        {
            Employee employee = this.db.Employees.Find(employeeId);

            if (employee == null)
            {
                throw new ArgumentException("Employee with the given Id does not exists!");
            }

            DateTime birthday = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            employee.Birthday = birthday;

            this.db.SaveChanges();
        }

        public void SetAddress(int employeeId, string address)
        {
            Employee employee = this.db.Employees.Find(employeeId);

            if (employee == null)
            {
                throw new ArgumentException("Employee with the given Id does not exists!");
            }

            employee.Address = address;

            this.db.SaveChanges();
        }

        public string GetEmployeeInfo(int employeeId)
        {
            Employee employee = this.db.Employees.Find(employeeId);

            if (employee == null)
            {
                throw new ArgumentException("Employee with the given Id does not exists!");
            }

            EmployeeDto employeeDto = Mapper.Map<EmployeeDto>(employee);

            string result = $"ID: {employeeId} - {employeeDto.FirstName} {employeeDto.LastName} - ${employeeDto.Salary:F2}";

            return result;
        }

        public string GetEmployeePersonalInfo(int employeeId)
        {
            Employee employee = this.db.Employees.Find(employeeId);

            if (employee == null)
            {
                throw new ArgumentException("Employee with the given Id does not exists!");
            }

            string birthday = employee.Birthday.ToString() ?? @"N/A";
            string address = employee.Address ?? @"N/A";

            string result = $"ID: {employeeId} - {employee.FirstName} {employee.LastName} - ${employee.Salary:f2}" +
                            Environment.NewLine +
                            $"Birthday: {birthday}" +
                            Environment.NewLine +
                            $"Address: {address}";

            return result;
        }

        public string SetManager(int employeeId, int managerId)
        {
            Employee employee = this.db.Employees.Find(employeeId);
            Employee manager = this.db.Employees.Find(managerId);

            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {employeeId} does not exist!");
            }

            if (manager == null)
            {
                throw new ArgumentException($"Manager with ID {managerId} does not exist!");
            }

            employee.ManagerId = managerId;

            this.db.SaveChanges();

            string result = $"Successfully set employee with ID {employeeId} to have manager {manager.FirstName} {manager.LastName}!";

            return result;
        }

        public string GetManagerInfo(int employeeId)
        {
            Employee manager = this.db.Employees.Find(employeeId);

            if (manager == null)
            {
                throw new ArgumentException($"Manager with ID {employeeId} does not exist!");
            }

            ManagerDto managerDto = Mapper.Map<ManagerDto>(manager);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{managerDto.FirstName} {managerDto.LastName} | Employees: {managerDto.EmployeesCount}");

            foreach (var employee in managerDto.Employees)
            {
                sb.AppendLine($"    - {employee.FirstName} {employee.LastName} - ${employee.Salary:f2}");
            }

            string result = sb.ToString().Trim();
            return result;
        }

        public string ListEmployeesOlderThan(int age)
        {
            var employees = this.db.Employees
                .Where(e => DateTime.Now.Year - e.Birthday.Value.Year > age)
                .ProjectTo<EmployeeWithManagerDto>()
                .OrderByDescending(e => e.Salary)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                string manager = e.Manager == null ? "No manager" : e.Manager.FirstName;

                sb.AppendLine($"{e.FirstName} {e.LastName} - ${e.Salary:f2} - Manager: {manager}");
            }

            string result = sb.ToString().Trim();
            return result;
        }
    }
}
