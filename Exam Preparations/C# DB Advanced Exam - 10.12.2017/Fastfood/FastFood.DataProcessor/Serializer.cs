namespace FastFood.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    using FastFood.Data;
    using FastFood.DataProcessor.Dto.Export;
    using FastFood.Models.Enums;

    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var type = Enum.Parse<OrderType>(orderType);

            var employeeOrders = context.Employees
                .Include(e => e.Orders)
                .ThenInclude(o => o.OrderItems)
                .Where(e => e.Name == employeeName)
                .Select(e => new EmployeeDto
                {
                    Name = e.Name,
                    Orders = e.Orders.Where(o => o.Type == type)
                                    .Select(o => new OrderDto
                                    {
                                        Customer = o.Customer,
                                        Items = o.OrderItems.Select(oi => new ItemDto
                                        {
                                            Name = oi.Item.Name,
                                            Price = oi.Item.Price,
                                            Quantity = oi.Quantity
                                        })
                                        .ToArray(),
                                        TotalPrice = o.OrderItems.Sum(a => a.Quantity * a.Item.Price),
                                    })
                                    .OrderByDescending(o => o.TotalPrice)
                                    .ThenByDescending(o => o.Items.Count)
                                    .ToArray(),
                })
                .Select(e => new EmployeeDto
                {
                    Name = e.Name,
                    Orders = e.Orders,
                    TotalMade = e.Orders.Sum(o => o.TotalPrice),
                })
                .SingleOrDefault();

            var json = JsonConvert.SerializeObject(employeeOrders, Newtonsoft.Json.Formatting.None);
            return json;
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            string[] categoriesArray = categoriesString.Split(',');

            var categories = context.Categories
                .Include(c => c.Items)
                .ThenInclude(i => i.OrderItems)
                .Where(c => categoriesArray.Any(a => a == c.Name))
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    MostPopularItem = c.Items.Select(i => new ItemCategoryDto
                    {
                        Name = i.Name,
                        TotalMade = i.Price * i.OrderItems.Sum(oi => oi.Quantity),
                        TimesSold = i.OrderItems.Sum(oi => oi.Quantity),
                    })
                    .OrderByDescending(i => i.TotalMade)
                    .First()
                })
                .OrderByDescending(c => c.MostPopularItem.TotalMade)
                .ThenByDescending(c => c.MostPopularItem.TimesSold)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("Categories"));

            serializer.Serialize(new StringWriter(sb), categories, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var xml = sb.ToString();
            return xml;
        }
    }
}