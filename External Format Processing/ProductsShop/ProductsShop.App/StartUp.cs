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
            ExportUsersAndProductsInXml();
        }

        private static void ExportCategoriesByProductsInXml()
        {
            using (var db = new ProductsShopContext())
            {
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

                xDoc.Save("categories-by-products.xml");
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

                xDoc.Save("users-and-products.xml");
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

                xDocument.Save("users-sold-products.xml");
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

                xDocument.Save("products-in-range.xml");
            }
        }

        #region Import XML

        private static void ImportCategoriesXml()
        {
            XDocument xDoc = XDocument.Load("categories.xml");
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
            XDocument xDoc = XDocument.Load("users.xml");

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
            XDocument xDoc = XDocument.Load("products.xml");

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

        #region Export JSON 

        // Export JSON - Query 4

        private static void SerializeUsersAndProducts()
        {
            using (var db = new ProductsShopContext())
            {
                var users = db.Users
                    .Include(u => u.ProductsSold)
                    .Where(u => u.ProductsSold.Count > 0)
                    .ToArray();

                var usersOutput = new
                {
                    usersCount = users.Length,
                    users = users.OrderByDescending(u => u.ProductsSold.Count).Select(u => new
                    {
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        age = u.Age,
                        soldProducts = new
                        {
                            count = u.ProductsSold.Count,
                            products = u.ProductsSold.Select(ps => new
                            {
                                name = ps.Name,
                                price = ps.Price
                            })
                        }
                    })
                };


                var usersJson = JsonConvert.SerializeObject(usersOutput, Formatting.Indented);

                File.WriteAllText("users-and-products.json", usersJson);
            }
        }

        // Export JSON - Query 3

        private static void SerializeCategoriesByProductsCount()
        {
            using (var db = new ProductsShopContext())
            {
                var categories = db.Categories
                    .Include(c => c.CategoryProducts)
                    .ThenInclude(cp => cp.Product)
                    .Select(c => new
                    {
                        category = c.Name,
                        productsCount = c.CategoryProducts.Count,
                        averagePrice = c.CategoryProducts.Average(p => p.Product.Price),
                        totalRevenue = c.CategoryProducts.Sum(p => p.Product.Price),
                    })
                    .OrderBy(c => c.category)
                    .ToArray();

                var categoriesJson = JsonConvert.SerializeObject(categories, Formatting.Indented);

                File.WriteAllText("categories-by-products.json", categoriesJson);
            }
        }

        // Export JSON - Query 2

        private static void SerializeUsersSoldProducts()
        {
            using (var db = new ProductsShopContext())
            {
                var usersWithProducts = db.Users
                    .Include(u => u.ProductsSold)
                    .ThenInclude(p => p.Buyer)
                    .Where(u => u.ProductsSold.Count > 0)
                    .Select(a => new
                    {
                        firstName = a.FirstName,
                        lastName = a.LastName,
                        soldProducts = a.ProductsSold.Where(p => p.Buyer != null).Select(s => new
                        {
                            name = s.Name,
                            price = s.Price,
                            buyerFirstName = s.Buyer.FirstName,
                            buyerLastName = s.Buyer.LastName
                        })
                    })
                    .OrderBy(u => u.lastName)
                    .ThenBy(u => u.firstName)
                    .ToArray();

                var usersJson = JsonConvert.SerializeObject(usersWithProducts, Formatting.Indented);

                File.WriteAllText("users-sold-products.json", usersJson);
            }
        }

        // Export JSON - Query 1

        private static void SerializeProductsInPriceRange(int start = 500, int end = 1000)
        {
            using (var db = new ProductsShopContext())
            {
                var products = db.Products
                    .Include(p => p.Seller)
                    .Where(p => p.Price >= start && p.Price <= end)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                    })
                    .OrderBy(a => a.price)
                    .ToArray();

                var jsonProducts = JsonConvert.SerializeObject(products, Formatting.Indented);

                File.WriteAllText("products-in-range.json", jsonProducts);
            }
        }
        #endregion

        #region Import JSON

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

        private static void ImportCategories()
        {
            var categoriesAsString = File.ReadAllText("categories.json");

            var categories = JsonConvert.DeserializeObject<Category[]>(categoriesAsString);

            using (var db = new ProductsShopContext())
            {
                db.Categories.AddRange(categories);

                db.SaveChanges();
            }
        }

        private static void ImportProducts()
        {
            var productsAsString = File.ReadAllText("products.json");

            var products = JsonConvert.DeserializeObject<Product[]>(productsAsString);

            using (var db = new ProductsShopContext())
            {
                var users = db.Users.ToArray();

                int counter = 1;
                Random r = new Random();

                foreach (var p in products)
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

                db.Products.AddRange(products);
                db.SaveChanges();
            }
        }

        private static void ImportUsers()
        {
            var usersAsString = File.ReadAllText("users.json");

            var users = JsonConvert.DeserializeObject<User[]>(usersAsString);

            using (var db = new ProductsShopContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();
            }
        }
        #endregion
    }
}
