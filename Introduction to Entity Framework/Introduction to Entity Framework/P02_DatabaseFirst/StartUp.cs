namespace P02_DatabaseFirst
{
    using Microsoft.EntityFrameworkCore;
    using P02_DatabaseFirst.Data;
    using P02_DatabaseFirst.Data.Models;
    using System;
    using System.Linq;

    public class StartUp
    {
        private readonly static SoftUniContext db = new SoftUniContext();

        // 14.	Delete Project by Id 

        public static void Main()
        {
            var project = db.Projects.Find(2);

            var employeesProjectsIds = db.EmployeesProjects
                .Where(p => p.Project == project)
                .ToArray();

            db.EmployeesProjects.RemoveRange(employeesProjectsIds);
            db.Projects.Remove(project);
            db.SaveChanges();

            var projectsToPrint = db.Projects
                .Take(10)
                .ToArray();

            PrintProjects(projectsToPrint);
        }

        private static void PrintProjects(Project[] projectsToPrint)
        {
            foreach (var project in projectsToPrint)
            {
                Console.WriteLine(project.Name);
            }
        }

        //------------------//

        // 13.	Find Employees by First Name Starting With Sa

        //public static void Main()
        //{
        //    var employees = db.Employees
        //        .Where(e => e.FirstName.StartsWith("Sa"))
        //        .OrderBy(e => e.FirstName)
        //        .ThenBy(e => e.LastName)
        //        .ToArray();

        //    PrintEmployees(employees);
        //}

        //private static void PrintEmployees(Employee[] employees)
        //{
        //    foreach (var employee in employees)
        //    {
        //        Console.WriteLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:F2})");
        //    }
        //}


        //---------------------//

        // 12.	Increase Salaries 

        //public static void Main()
        //{
        //    var employees = db.Employees
        //        .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" ||
        //            e.Department.Name == "Marketing" || e.Department.Name == "Information Services")
        //        .OrderBy(e => e.FirstName)
        //        .ThenBy(e => e.LastName)
        //        .ToArray();

        //    IncreaseSalaryAndPrintEmployeeInfo(employees);
        //    db.SaveChanges();
        //}

        //private static void IncreaseSalaryAndPrintEmployeeInfo(Employee[] employees)
        //{
        //    foreach (var employee in employees)
        //    {
        //        employee.Salary += employee.Salary * 0.12M;

        //        Console.WriteLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:F2})");
        //    }
        //}

        //------------------//

        // 11.	Find Latest 10 Projects 

        //public static void Main()
        //{
        //    var projects = db.Projects
        //        .OrderByDescending(p => p.StartDate)
        //        .Take(10)
        //        .OrderBy(p => p.Name)
        //        .ToArray();

        //    PrintProjects(projects);
        //}

        //private static void PrintProjects(Project[] projects)
        //{
        //    foreach (var project in projects)
        //    {
        //        Console.WriteLine(project.Name);
        //        Console.WriteLine(project.Description);
        //        Console.WriteLine(project.StartDate);
        //    }
        //}

        //----------------------//

        // 10.	Departments with More Than 5 Employees 

        //public static void Main()
        //{
        //    var departmentsWithEmployees = db.Departments
        //        .Include(d => d.Employees)
        //        .Where(d => d.Employees.Count > 5)
        //        .OrderBy(d => d.Employees.Count)
        //        .ThenBy(d => d.Name)
        //        .ToArray();

        //    foreach (var department in departmentsWithEmployees)
        //    {
        //        Console.WriteLine($"{department.Name} - {department.Manager.FirstName} {department.Manager.LastName}");

        //        foreach (var employee in department.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
        //        {
        //            Console.WriteLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
        //        }

        //        Console.WriteLine($"----------");
        //    }
        //}

        //------------------------------------//

        // 09. Employee 147

        //public static void Main()
        //{
        //    using (db)
        //    {
        //        var employees = db.Employees
        //            .Include(e => e.EmployeesProjects)
        //            .ThenInclude(e => e.Project)
        //            .ToList();

        //        var employee = employees.SingleOrDefault(e => e.EmployeeId == 147);

        //        Console.WriteLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

        //        foreach (var project in employee.EmployeesProjects.OrderBy(p => p.Project.Name))
        //        {
        //            Console.WriteLine(project.Project.Name);
        //        }
        //    }
        //}

        //08. Addresses by Town 

        //public static void Main()
        //{
        //    using (db)
        //    {
        //        var addressesWithEmployees = db.Addresses
        //            .Include(a => a.Town)
        //            .Select(a => new
        //            {
        //                a.AddressText,
        //                a.Town.Name,
        //                a.Employees
        //            })
        //            .OrderByDescending(x => x.Employees.Count)
        //            .ThenBy(x => x.Name)
        //            .ThenBy(x => x.AddressText)
        //            .Take(10)
        //            .ToArray();

        //        foreach (var address in addressesWithEmployees)
        //        {
        //            Console.WriteLine($"{address.AddressText}, {address.Name} - {address.Employees.Count} employees");
        //        }
        //    }
        //}

        //------------------------------//

        // 07. Employees and Projects 

        //public static void Main()
        //{
        //    using (db)
        //    {
        //        var emloyeesWithProjects = db.Employees
        //            .Include(e => e.EmployeesProjects)
        //            .ThenInclude(e => e.Project)
        //            .Where(p => p.EmployeesProjects
        //            .Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
        //            .Take(30)
        //            .ToArray();

        //        string format = "M/d/yyyy h:mm:ss tt";

        //        foreach (var employee in emloyeesWithProjects)
        //        {
        //            var managerId = employee.ManagerId;
        //            var manager = db.Employees.Find(managerId);

        //            Console.WriteLine($"{employee.FirstName} {employee.LastName} - Manager: {manager.FirstName} {manager.LastName}");

        //            foreach (var project in employee.EmployeesProjects)
        //            {
        //                var endDate = project.Project.EndDate != null ? string.Format(project.Project.EndDate.ToString(), format) : "not finished";

        //                Console.WriteLine($"--{project.Project.Name} - {string.Format(project.Project.StartDate.ToString(), format)} - {endDate}");
        //            }
        //        }
        //    }
        //}

        //---------------------------------------------//

        // 06.	Adding a New Address and Updating Employee 

        //public static void Main()
        //{
        //    using (db)
        //    {
        //        var address = new Address()
        //        {
        //            AddressText = "Vitoshka 15",
        //            TownId = 4
        //        };

        //        var employee = db.Employees.SingleOrDefault(e => e.LastName == "Nakov");
        //        employee.Address = address;

        //        db.SaveChanges();

        //        var addresses = db.Employees.OrderByDescending(e => e.AddressId).Take(10).Select(e => e.Address.AddressText).ToArray();

        //        foreach (var addressText in addresses)
        //        {
        //            Console.WriteLine(addressText);
        //        }
        //    }
        //}

        //---------------------------------------------//

        // 05. Employees from Research and Development 

        //public static void Main()
        //{
        //    using (db)
        //    {
        //        var employees = db.Employees
        //            .Where(e => e.Department.Name == "Research and Development")
        //            .OrderBy(e => e.Salary)
        //            .ThenByDescending(e => e.FirstName)
        //            .Select(e => new
        //            {
        //                e.FirstName,
        //                e.LastName,
        //                e.Department.Name,
        //                e.Salary
        //            })
        //            .ToArray();

        //        foreach (var employee in employees)
        //        {
        //            Console.WriteLine($"{employee.FirstName} {employee.LastName} from {employee.Name} - ${employee.Salary:f2}");
        //        }

        //    }
        //}

        //---------------------------------------------//

        // 04. Employees with Salary Over 50 000 

        //public static void Main()
        //{
        //    var employees = db.Employees.Where(e => e.Salary > 50000).ToArray();

        //    foreach (var employee in employees.OrderBy(e => e.FirstName))
        //    {
        //        Console.WriteLine(employee.FirstName);
        //    }
        //}

        //---------------------------------------------//

        // 03. Employees Full Information

        //public static void Main()
        //{

        //    var employees = db.Employees.ToArray();

        //    foreach (var employee in employees.OrderBy(e => e.EmployeeId))
        //    {
        //        Console.WriteLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
        //    }
        //}
    }
}
