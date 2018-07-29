namespace ProductsShop.App
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            ExportCategoriesByProductsInXml();
            ExportProductsInRangeInXml();
            ExportUsersAndProductsInXml();
            ExportUsersSoldProductsInXml();
        }

        #region Export XML

        private static void ExportCategoriesByProductsInXml()
        {
            using (var db = new ProductsShopContext())
            {
                var a = db.Categories.Include(c => c.CategoryProducts).ToList();

                var categories = db.Categories
                    .Include(c => c.CategoryProducts)
                    .ThenInclude(cp => cp.Product)
                    .Select(c => new
                    {
                        c.Name,
                        ProductsCount = c.CategoryProducts.Count,
                        AveragePrice = c.CategoryProducts.Average(p => p.Product.Price),
                        TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price),
                    })
                    .OrderByDescending(c => c.ProductsCount)
                    .ToArray();

                XDocument xDoc = new XDocument(new XElement("categories"));

                foreach (var c in categories)
                {
                    var categoryInfo = new XElement("category",
                                        new XAttribute("name", c.Name),
                                        new XElement("products-count", c.ProductsCount),
                                        new XElement("average-price", c.AveragePrice),
                                        new XElement("total-revenue", c.TotalRevenue));

                    xDoc.Root.Add(categoryInfo);
                }

                xDoc.Save("DataExport/categories-by-products.xml");
            }
        }

        private static void ExportUsersAndProductsInXml()
        {
            using (var db = new ProductsShopContext())
            {
                var users = db.Users
                    .Include(u => u.ProductsSold)
                    .Where(u => u.ProductsSold.Count > 0)
                    .Select(u => new
                    {
                        u.FirstName,
                        u.LastName,
                        u.Age,
                        ProductsSold = u.ProductsSold.Select(p => new
                        {
                            p.Name,
                            p.Price
                        })
                    })
                    .OrderByDescending(u => u.ProductsSold.Count())
                    .ToArray();

                XDocument xDoc = new XDocument(new XElement("users", new XAttribute("count", users.Length)));

                foreach (var u in users)
                {
                    XElement userInfo = new XElement("user",
                                        new XAttribute("first-name", u.FirstName == null ? "" : u.FirstName), 
                                        new XAttribute("last-Name", u.LastName),
                                        new XAttribute("age", u.Age == null ? -1 : u.Age),
                                        new XElement("sold-products", new XAttribute("count" ,u.ProductsSold.Count())));

                    foreach (var p in u.ProductsSold)
                    {
                        XElement productInfo = new XElement("product",
                                                new XAttribute("price", p.Price),
                                                new XAttribute("name", p.Name));

                        userInfo.Element("sold-products").Add(productInfo);
                    }

                    xDoc.Root.Add(userInfo);
                }

                xDoc.Save("DataExport/users-and-products.xml");
            }
        }

        private static void ExportUsersSoldProductsInXml()
        {
            using (var db = new ProductsShopContext())
            {
                var users = db.Users
                    .Include(u => u.ProductsSold)
                    .Where(u => u.ProductsSold.Count > 0)
                    .Select(u => new
                    {
                        u.FirstName,
                        u.LastName,
                        ProductsSold = u.ProductsSold.ToList()
                    })
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToArray();

                XDocument xDocument = new XDocument(new XElement("users"));

                foreach (var u in users)
                {
                    var userInfo = new XElement("user", new XAttribute("last-name", u.LastName),
                                                            new XAttribute("first-name", u.FirstName != null ? u.FirstName : ""),
                                                            new XElement("sold-products"));

                    foreach (var p in u.ProductsSold)
                    {
                        var product = new XElement("products",
                                                    new XElement("name", p.Name),
                                                    new XElement("price", p.Price));

                        userInfo.Element("sold-products").Add(product);
                    }

                    xDocument.Root.Add(userInfo);
                }

                xDocument.Save("DataExport/users-sold-products.xml");
            }
        }

        private static void ExportProductsInRangeInXml(int start = 1000, int end = 2000)
        {
            using (var db = new ProductsShopContext())
            {
                var products = db.Products
                    .Include(p => p.Buyer)
                    .Where(p => p.Price >= start && p.Price <= end)
                    .OrderBy(p => p.Price)
                    .ToArray();

                XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                xDocument.Add(new XElement("products"));

                foreach (var p in products)
                {
                    string name = p.Name;
                    string price = p.Price.ToString();
                    User buyer = p.Buyer ?? null;

                    if (p.Buyer == null)
                    {
                        xDocument.Root.Add(new XElement("product", new XAttribute("price", price), new XAttribute("name", name)));
                    }
                    else
                    {
                        xDocument.Root.Add(new XElement("product", new XAttribute("buyer", $"{buyer?.FirstName} {buyer?.LastName}"), new XAttribute("price", price), new XAttribute("name", name)));
                    }
                }

                xDocument.Save("DataExport/products-in-range.xml");
            }
        }

        #endregion

        #region Import XML

        private static void ImportCategoriesXml()
        {
            XDocument xDoc = XDocument.Load("DataToImport/categories.xml");
            var categoriesXml = xDoc.Root.Elements();

            var categories = new List<Category>();

            foreach (var c in categoriesXml)
            {
                string name = c.Element("name").Value;

                Category category = new Category()
                {
                    Name = name,
                };

                categories.Add(category);
            }

            using (var db = new ProductsShopContext())
            {
                db.Categories.AddRange(categories);

                db.SaveChanges();
            }
        }

        private static void ImportUsersXml()
        {
            XDocument xDoc = XDocument.Load("DataToImport/users.xml");

            var users = xDoc.Root.Elements();

            var usersDb = new List<User>();

            foreach (var u in users)
            {
                string lastName = u.Attribute("lastName").Value;
                string firstName = u.Attribute("firstName")?.Value;
                string age = u.Attribute("age")?.Value;

                User user = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                };

                if (age == null)
                {
                    user.Age = null;
                }
                else
                {
                    user.Age = int.Parse(age);
                }

                usersDb.Add(user);
            }

            using (var db = new ProductsShopContext())
            {
                db.Users.AddRange(usersDb);
                db.SaveChanges();
            }
        }

        private static void ImportProductsXml()
        {
            XDocument xDoc = XDocument.Load("DataToImport/products.xml");

            var products = xDoc.Root.Elements();

            var productsDb = new List<Product>();

            foreach (var p in products)
            {
                string name = p.Element("name").Value;
                decimal price = decimal.Parse(p.Element("price").Value);

                Product product = new Product()
                {
                    Name = name,
                    Price = price,
                };

                productsDb.Add(product);
            }

            using (var db = new ProductsShopContext())
            {
                var users = db.Users.ToArray();

                int counter = 1;
                Random r = new Random();

                foreach (var p in productsDb)
                {
                    if (counter % 3 != 0)
                    {
                        p.Buyer = users[r.Next(0, 56)];
                        p.Seller = users[r.Next(0, 56)];
                    }
                    else
                    {
                        p.Seller = users[r.Next(0, 56)];
                        p.Buyer = null;
                    }
                    counter++;
                }

                db.Products.AddRange(productsDb);
                db.SaveChanges();
            }
        }

        #endregion

        private static void ImportCategoriesProducts()
        {
            using (var db = new ProductsShopContext())
            {

                var products = db.Products
                            .ToArray();

                var categories = db.Categories.ToArray();

                var categoriesProducts = new List<CategoryProduct>();

                Random r = new Random();

                foreach (var p in products)
                {
                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                        CategoryId = categories[r.Next(0, categories.Length)].Id,
                        ProductId = p.Id,
                    };

                    categoriesProducts.Add(categoryProduct);
                }

                db.CategoriesProducts.AddRange(categoriesProducts);

                db.SaveChanges();
            }
        }
            }
}
