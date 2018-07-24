namespace FastFood.DataProcessor
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    using Data;
    using DataProcessor.Dto.Import;
    using FastFood.Models;
    using System.Xml.Serialization;
    using System.IO;
    using FastFood.Models.Enums;
    using System.Globalization;

    public static class Deserializer
	{
		private const string FailureMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

		public static string ImportEmployees(FastFoodDbContext context, string jsonString)
		{
            var deserializedEmployees = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);

            var sb = new StringBuilder();

            var validEmployees = new List<Employee>();
            
            foreach (var emp in deserializedEmployees)
            {
                if (!IsValid(emp))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                
                Position position = CreateOrFindPosition(context, emp.Position);

                Employee employee = new Employee()
                {
                    Name = emp.Name,
                    Age = emp.Age,
                    Position = position
                };

                validEmployees.Add(employee);

                sb.AppendLine(string.Format(SuccessMessage, emp.Name));
            }

            context.Employees.AddRange(validEmployees);
            context.SaveChanges();

            string result = sb.ToString();
            return result;
		}
        
        public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
            var deserializedItems = JsonConvert.DeserializeObject<ItemDto[]>(jsonString);

            var sb = new StringBuilder();

            var validItems = new List<Item>();

            foreach (var itemDto in deserializedItems)
            {
                bool itemAlreadyExists = validItems.Any(i => i.Name == itemDto.Name);

                if (!IsValid(itemDto) || itemAlreadyExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                Category category = CreateOrFindCategory(context, itemDto.Category);

                Item item = new Item
                {
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Category = category,
                };

                validItems.Add(item);

                sb.AppendLine(string.Format(SuccessMessage, item.Name));
            }

            context.Items.AddRange(validItems);
            context.SaveChanges();

            string result = sb.ToString();
            return result;
		}
        
        public static string ImportOrders(FastFoodDbContext context, string xmlString)
		{
            var serializer = new XmlSerializer(typeof(OrderDto[]), new XmlRootAttribute("Orders"));

            var deserializedOrders = (OrderDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var sb = new StringBuilder();

            var validOrders = new List<Order>();

            string dateFormat = @"dd/MM/yyyy HH:mm";

            var allItems = context.Items.ToArray();

            foreach (var orderDto in deserializedOrders)
            {
                bool employeeExists = context.Employees.Any(e => e.Name == orderDto.Employee);
                if (!employeeExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                bool orderItemsExists = orderDto.Items.All(oi => allItems.Any(i => i.Name == oi.Name));
                if (!orderItemsExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var type = (OrderType)Enum.Parse(typeof(OrderType), orderDto.Type);

                Employee employee = FindEmployee(context, orderDto.Employee);

                DateTime dateTime = DateTime.ParseExact(orderDto.DateTime, dateFormat, CultureInfo.InvariantCulture);

                List<OrderItem> orderItems = ParseOrderItems(context, orderDto);

                Order order = new Order
                {
                    Type = type,
                    Employee = employee,
                    DateTime = dateTime,
                    Customer = orderDto.Customer,
                    OrderItems = orderItems
                };

                validOrders.Add(order);

                sb.AppendLine($"Order for {orderDto.Customer} on {orderDto.DateTime} added");
            }

            context.Orders.AddRange(validOrders);
            context.SaveChanges();

            string result = sb.ToString();
            return result;
        }

        private static List<OrderItem> ParseOrderItems(FastFoodDbContext context, OrderDto orderDto)
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (var orderItemDto in orderDto.Items)
            {
                OrderItem orderItem = new OrderItem
                {
                    Quantity = orderItemDto.Quantity,
                    Item = FindItem(context, orderItemDto.Name)
                };

                orderItems.Add(orderItem);
            }

            return orderItems;
        }

        private static Item FindItem(FastFoodDbContext context, string name)
        {
            return context.Items.FirstOrDefault(i => i.Name == name);
        }

        private static Employee FindEmployee(FastFoodDbContext context, string employee)
        {
            return context.Employees.FirstOrDefault(e => e.Name == employee);
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }

        private static Position CreateOrFindPosition(FastFoodDbContext context, string positionName)
        {
            var position = FindPosition(context, positionName) ?? CreatePosition(context, positionName);

            return position;
        }

        private static Position CreatePosition(FastFoodDbContext context, string positionName)
        {
            Position position = new Position() { Name = positionName };

            context.Positions.Add(position);
            context.SaveChanges();

            return position;
        }

        private static Position FindPosition(FastFoodDbContext context, string positionName)
        {
            return context.Positions.SingleOrDefault(p => p.Name == positionName);
        }

        private static Category CreateOrFindCategory(FastFoodDbContext context, string categoryName)
        {
            Category category = FindCategory(context, categoryName) ?? CreateCategory(context, categoryName);

            return category;
        }

        private static Category CreateCategory(FastFoodDbContext context, string categoryName)
        {
            Category category = new Category() { Name = categoryName };

            context.Categories.Add(category);
            context.SaveChanges();

            return category;
        }

        private static Category FindCategory(FastFoodDbContext context, string categoryName)
        {
            return context.Categories.SingleOrDefault(p => p.Name == categoryName);
        }
    }
}